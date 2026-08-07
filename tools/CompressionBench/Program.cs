using System.Diagnostics;
using System.Globalization;
using System.Text;
using Astronomy.XISF;
using Astronomy.XISF.Compression;
using ZstdSharp;

namespace CompressionBench;

/// <summary>
/// Benchmarks candidate XISF block codecs against real library files: decode each sampled file's
/// block to raw bytes (AL verified reader), recompress with every candidate, record size and
/// encode/decode wall time. Stratified deterministic sampling so runs are reproducible.
/// </summary>
internal static class Program
{
    private sealed record Candidate(string Name, Func<byte[], int, (byte[] Compressed, TimeSpan Enc, TimeSpan Dec)> Run);

    private sealed record FileResult(
        string Path, string Stratum, string Camera, string SampleFormat, int Width, int Height,
        string OnDiskCodec, long RawBytes, Dictionary<string, (long Size, TimeSpan Enc, TimeSpan Dec)> ByCandidate);

    private static async Task<int> Main(string[] args)
    {
        string? root = null;
        string outDir = ".";
        int perStratum = 12;
        int seed = 42;
        int[] zstdLevels = { 3, 9, 15, 19, 22 };

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--root": root = args[++i]; break;
                case "--out": outDir = args[++i]; break;
                case "--per-stratum": perStratum = int.Parse(args[++i], CultureInfo.InvariantCulture); break;
                case "--seed": seed = int.Parse(args[++i], CultureInfo.InvariantCulture); break;
                case "--zstd-levels":
                    zstdLevels = args[++i].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                          .Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    break;
                default:
                    Console.Error.WriteLine($"Unknown argument '{args[i]}'.");
                    return 2;
            }
        }

        if (root is null || !Directory.Exists(root))
        {
            Console.Error.WriteLine("Usage: CompressionBench --root <library-dir> [--out <dir>] [--per-stratum N] [--seed N] [--zstd-levels 3,9,15,19,22]");
            return 2;
        }

        Console.WriteLine($"Scanning {root} ...");
        var byStratum = ScanAndStratify(root);
        foreach (var (stratum, files) in byStratum.OrderBy(kv => kv.Key))
            Console.WriteLine($"  {stratum}: {files.Count} files");

        var sample = SampleDeterministic(byStratum, perStratum, seed);
        Console.WriteLine($"Sampled {sample.Count} files ({perStratum}/stratum, seed {seed}).");

        var candidates = BuildCandidates(zstdLevels);
        var results = new List<FileResult>();
        var failures = new List<string>();

        foreach (var (path, stratum) in sample)
        {
            Console.WriteLine($"[{results.Count + failures.Count + 1}/{sample.Count}] {Path.GetFileName(path)}");
            try
            {
                results.Add(await BenchOneAsync(path, stratum, candidates));
            }
            catch (Exception ex)
            {
                failures.Add($"{path}: {ex.Message}");
                Console.Error.WriteLine($"  FAILED: {ex.Message}");
            }
        }

        if (results.Count == 0)
        {
            Console.Error.WriteLine("No files benchmarked successfully.");
            return 1;
        }

        Directory.CreateDirectory(outDir);
        string csvPath = Path.Combine(outDir, "compression-bench-results.csv");
        string mdPath = Path.Combine(outDir, "compression-bench-summary.md");
        WriteCsv(csvPath, results, candidates);
        string summary = BuildSummary(results, candidates, root, perStratum, seed, failures);
        File.WriteAllText(mdPath, summary);
        Console.WriteLine();
        Console.WriteLine(summary);
        Console.WriteLine($"Per-file rows: {csvPath}");
        Console.WriteLine($"Summary:       {mdPath}");
        return 0;
    }

    // Filename-heuristic classification (canonical XFM names). Header truth (sample format, camera
    // keyword) is recorded per file during the run, so a misclassified stray stays visible in the CSV.
    private static Dictionary<string, List<string>> ScanAndStratify(string root)
    {
        var strata = new Dictionary<string, List<string>>();
        foreach (string path in Directory.EnumerateFiles(root, "*.xisf", SearchOption.AllDirectories))
        {
            if (new FileInfo(path).Length < 1_000_000) continue; // stubs/thumbnails: not representative

            string name = Path.GetFileName(path);
            string stratum;
            if (name.Contains("master", StringComparison.OrdinalIgnoreCase))
            {
                stratum = name.Contains("dark", StringComparison.OrdinalIgnoreCase) ? "master-dark"
                        : name.Contains("flat", StringComparison.OrdinalIgnoreCase) ? "master-flat"
                        : name.Contains("bias", StringComparison.OrdinalIgnoreCase) ? "master-bias"
                        : "master-other";
            }
            else if (name.Contains(" L-", StringComparison.Ordinal))
            {
                stratum = "light-sub";
            }
            else
            {
                stratum = "other-sub";
            }

            (strata.TryGetValue(stratum, out var list) ? list : strata[stratum] = new List<string>()).Add(path);
        }
        return strata;
    }

    private static List<(string Path, string Stratum)> SampleDeterministic(
        Dictionary<string, List<string>> byStratum, int perStratum, int seed)
    {
        var sample = new List<(string, string)>();
        foreach (var (stratum, files) in byStratum.OrderBy(kv => kv.Key))
        {
            files.Sort(StringComparer.Ordinal); // enumeration order is not stable across runs; sort first
            var rng = new Random(seed ^ StringComparer.Ordinal.GetHashCode(stratum));
            sample.AddRange(files.OrderBy(_ => rng.Next()).Take(perStratum).Select(f => (f, stratum)));
        }
        return sample;
    }

    private static List<Candidate> BuildCandidates(int[] zstdLevels)
    {
        var candidates = new List<Candidate>
        {
            // AL's Compress = what XFM/consumers can write today. Shuffle is applied internally.
            new("zlib+sh(max)", (raw, itemSize) => RunAl(raw, itemSize, BlockCodec.Zlib)),
            new("lz4hc+sh",     (raw, itemSize) => RunAl(raw, itemSize, BlockCodec.Lz4Hc)),
            new("zstd+sh(1)",   (raw, itemSize) => RunAl(raw, itemSize, BlockCodec.Zstd)),
        };
        foreach (int level in zstdLevels)
            candidates.Add(new($"zstd+sh({level})", (raw, itemSize) => RunZstdDirect(raw, itemSize, level)));
        return candidates;
    }

    private static (byte[], TimeSpan, TimeSpan) RunAl(byte[] raw, int itemSize, BlockCodec codec)
    {
        var sw = Stopwatch.StartNew();
        BlockCompressionResult result = XisfBlockCompression.Compress(raw, itemSize, codec);
        TimeSpan enc = sw.Elapsed;

        sw.Restart();
        byte[] roundTrip = XisfBlockCompression.Decompress(result.CompressedBytes, result.Info);
        TimeSpan dec = sw.Elapsed;

        if (roundTrip.LongLength != raw.LongLength)
            throw new InvalidDataException($"{codec} round-trip length mismatch.");
        return (result.CompressedBytes, enc, dec);
    }

    // Same shuffle AL applies, then ZstdSharp at the requested level — byte-identical to what an
    // AL Compress(level:) overload would produce (same package, same version).
    private static (byte[], TimeSpan, TimeSpan) RunZstdDirect(byte[] raw, int itemSize, int level)
    {
        var sw = Stopwatch.StartNew();
        byte[] shuffled = itemSize > 1 ? XisfBlockCompression.Shuffle(raw, itemSize) : raw;
        using var compressor = new Compressor(level);
        byte[] compressed = compressor.Wrap(shuffled).ToArray();
        TimeSpan enc = sw.Elapsed;

        sw.Restart();
        using var decompressor = new Decompressor();
        byte[] unwrapped = decompressor.Unwrap(compressed).ToArray();
        byte[] roundTrip = itemSize > 1 ? XisfBlockCompression.Unshuffle(unwrapped, itemSize) : unwrapped;
        TimeSpan dec = sw.Elapsed;

        if (roundTrip.LongLength != raw.LongLength)
            throw new InvalidDataException($"zstd({level}) round-trip length mismatch.");
        return (compressed, enc, dec);
    }

    private static async Task<FileResult> BenchOneAsync(string path, string stratum, List<Candidate> candidates)
    {
        XisfImageData image = await XisfImageReader.ReadImageAsync(path);
        XisfHeader header = await XisfHeaderReader.ReadAsync(path);
        string camera = header.Raw("INSTRUME") ?? "?";

        var byCandidate = new Dictionary<string, (long, TimeSpan, TimeSpan)>();
        foreach (Candidate candidate in candidates)
        {
            var (compressed, enc, dec) = candidate.Run(image.Pixels, image.BytesPerSample);
            byCandidate[candidate.Name] = (compressed.LongLength, enc, dec);
        }

        return new FileResult(path, stratum, camera, image.SampleFormat, image.Width, image.Height,
            image.Compression.CodecName, image.Pixels.LongLength, byCandidate);
    }

    private static void WriteCsv(string path, List<FileResult> results, List<Candidate> candidates)
    {
        var sb = new StringBuilder();
        sb.Append("file,stratum,camera,sampleFormat,width,height,onDiskCodec,rawBytes");
        foreach (Candidate c in candidates)
            sb.Append($",{c.Name}.bytes,{c.Name}.encMs,{c.Name}.decMs");
        sb.AppendLine();

        foreach (FileResult r in results)
        {
            sb.Append(string.Join(',',
                Quote(Path.GetFileName(r.Path)), r.Stratum, Quote(r.Camera), r.SampleFormat,
                r.Width.ToString(CultureInfo.InvariantCulture), r.Height.ToString(CultureInfo.InvariantCulture),
                r.OnDiskCodec, r.RawBytes.ToString(CultureInfo.InvariantCulture)));
            foreach (Candidate c in candidates)
            {
                var (size, enc, dec) = r.ByCandidate[c.Name];
                sb.Append(CultureInfo.InvariantCulture, $",{size},{enc.TotalMilliseconds:F1},{dec.TotalMilliseconds:F1}");
            }
            sb.AppendLine();
        }
        File.WriteAllText(path, sb.ToString());

        static string Quote(string s) => s.Contains(',') ? $"\"{s}\"" : s;
    }

    private static string BuildSummary(List<FileResult> results, List<Candidate> candidates,
        string root, int perStratum, int seed, List<string> failures)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# XISF block-codec benchmark");
        sb.AppendLine();
        sb.AppendLine(CultureInfo.InvariantCulture,
            $"Root `{root}` · {results.Count} files · {perStratum}/stratum · seed {seed} · single-threaded wall time.");
        sb.AppendLine("Ratio = compressed/raw (lower is better). Δ vs zlib = size change vs XFM's current `zlib+sh(max)` write.");
        sb.AppendLine();

        foreach (var group in results.GroupBy(r => r.Stratum).OrderBy(g => g.Key)
                                     .Append(GroupAll(results)))
        {
            long raw = group.Sum(r => r.RawBytes);
            long zlibTotal = group.Sum(r => r.ByCandidate["zlib+sh(max)"].Size);
            sb.AppendLine(CultureInfo.InvariantCulture,
                $"## {group.Key} — {group.Count()} files, {raw / 1e6:F0} MB raw");
            sb.AppendLine();
            sb.AppendLine("| candidate | ratio | Δ vs zlib | enc MB/s | dec MB/s |");
            sb.AppendLine("|---|---|---|---|---|");
            foreach (Candidate c in candidates)
            {
                long size = group.Sum(r => r.ByCandidate[c.Name].Size);
                double encSec = group.Sum(r => r.ByCandidate[c.Name].Enc.TotalSeconds);
                double decSec = group.Sum(r => r.ByCandidate[c.Name].Dec.TotalSeconds);
                sb.AppendLine(CultureInfo.InvariantCulture,
                    $"| {c.Name} | {(double)size / raw:F4} | {(double)(size - zlibTotal) / zlibTotal * 100:+0.0;-0.0}% " +
                    $"| {raw / 1e6 / encSec:F0} | {raw / 1e6 / decSec:F0} |");
            }
            sb.AppendLine();
        }

        if (failures.Count > 0)
        {
            sb.AppendLine($"## Failures ({failures.Count})");
            failures.ForEach(f => sb.AppendLine($"- {f}"));
        }
        return sb.ToString();

        static IGrouping<string, FileResult> GroupAll(List<FileResult> results) =>
            results.GroupBy(_ => "ALL (sampled)").Single();
    }
}

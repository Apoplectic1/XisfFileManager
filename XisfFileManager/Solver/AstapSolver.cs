using System.Diagnostics;
using System.Globalization;
using Astronomy.Core.Astrometry;
using Astronomy.Diagnostics;
using Astronomy.XISF;
using XisfFileManager.Configuration;

namespace XisfFileManager.Solver
{
    /// <summary>One frame's plate-solve outcome. On failure only <see cref="ErrorText"/> is meaningful.</summary>
    public sealed class SolveResult
    {
        public bool Success { get; init; }

        /// <summary>Solved RA of the frame centre (degrees, J2000) — ASTAP's reference pixel is the centre.</summary>
        public double RaDegrees { get; init; }

        /// <summary>Solved Dec of the frame centre (degrees, J2000).</summary>
        public double DecDegrees { get; init; }

        /// <summary>Measured position angle (degrees North-toward-East) after the ASTAP convention bridge.</summary>
        public double PositionAngleDegrees { get; init; }

        /// <summary>Image parity after the ASTAP bridge's inversion.</summary>
        public bool Flipped { get; init; }

        /// <summary>Raw WCS values exactly as ASTAP's .ini reported them (stamped verbatim).</summary>
        public string Crval1 { get; init; } = string.Empty;
        public string Crval2 { get; init; } = string.Empty;
        public string Crpix1 { get; init; } = string.Empty;
        public string Crpix2 { get; init; } = string.Empty;
        public string Cd1_1 { get; init; } = string.Empty;
        public string Cd1_2 { get; init; } = string.Empty;
        public string Cd2_1 { get; init; } = string.Empty;
        public string Cd2_2 { get; init; } = string.Empty;
        public string Crota1 { get; init; } = string.Empty;
        public string Crota2 { get; init; } = string.Empty;

        /// <summary>Why the solve failed (ASTAP ERROR text, exit-code description, or our own message).</summary>
        public string ErrorText { get; init; } = string.Empty;
    }

    /// <summary>
    /// Local ASTAP plate solving for the read pass: hand an uncompressed XISF to astap_cli directly,
    /// or decode a compressed one (shared library) into a minimal temporary FITS first. All solver
    /// inputs/outputs live in the temp directory (-o redirect) — the image library never gains solver
    /// files. UI-free; MainForm owns the checkbox gate and failure presentation.
    /// </summary>
    public static class AstapSolver
    {
        private const int SolveTimeoutMs = 60_000;
        private const string HintedRadiusDeg = "10";

        /// <summary>True when the ASTAP CLI executable exists at the configured path.</summary>
        public static bool IsInstalled => File.Exists(XisfConstants.AstapCliPath);

        /// <summary>
        /// Solves one XISF file. Never throws for solve-level failures (clouds, few stars, bad frame) —
        /// those return <c>Success=false</c> with the reason; the caller reports and continues.
        /// </summary>
        public static async Task<SolveResult> SolveAsync(string filePath, bool isCompressed, CancellationToken ct = default)
        {
            string tempBase = Path.Combine(Path.GetTempPath(), "xfm-astap-" + Guid.NewGuid().ToString("N"));
            string name = Path.GetFileName(filePath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            SolveResult result;
            try
            {
                Log.Diag("SOLVER", $"start {filePath} compressed={isCompressed}");

                // Hints from the header (shared library read — identical for compressed/uncompressed).
                XisfHeader header = await XisfHeaderReader.ReadAsync(filePath, ct);

                string solveInput;
                if (isCompressed)
                {
                    XisfImageData image = await XisfImageReader.ReadImageAsync(filePath, ct);
                    if (image.SampleFormat != "UInt16" || image.Channels != 1)
                    {
                        return LogOutcome(Fail($"unsupported solve input ({image.SampleFormat}, {image.Channels} channel(s)) — UInt16 mono only"), name, stopwatch);
                    }
                    solveInput = tempBase + ".fit";
                    WriteMinimalFits(solveInput, image);
                    Log.Diag("SOLVER", $"temp FITS {image.Width}x{image.Height} -> {solveInput}");
                }
                else
                {
                    solveInput = filePath; // astap_cli reads uncompressed XISF natively (read-only)
                }

                string args = BuildArguments(solveInput, tempBase, header);
                Log.Diag("SOLVER", $"args: {args}");
                (int exitCode, bool timedOut) = await RunAstapAsync(args, ct);
                Log.Diag("SOLVER", $"exit={exitCode} timedOut={timedOut}");
                if (timedOut)
                {
                    return LogOutcome(Fail($"solver timed out after {SolveTimeoutMs / 1000} s"), name, stopwatch);
                }

                result = ParseIni(tempBase + ".ini", exitCode);
            }
            catch (InvalidDataException ex)
            {
                // Malformed/corrupt XISF surfaced by the library's fail-fast read.
                result = Fail(ex.Message);
            }
            finally
            {
                foreach (string ext in new[] { ".fit", ".ini", ".wcs", ".log" })
                {
                    try { File.Delete(tempBase + ext); } catch { /* best-effort temp cleanup */ }
                }
            }

            return LogOutcome(result, name, stopwatch);
        }

        private static SolveResult LogOutcome(SolveResult result, string name, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            if (result.Success)
            {
                Log.Info(string.Create(CultureInfo.InvariantCulture,
                    $"ASTAP solved {name}: PA={result.PositionAngleDegrees:0.00} RA={result.RaDegrees:0.0000} DEC={result.DecDegrees:0.0000} flipped={result.Flipped} ({stopwatch.ElapsedMilliseconds} ms)"));
            }
            else
            {
                Log.Error($"ASTAP failed {name}: {result.ErrorText} ({stopwatch.ElapsedMilliseconds} ms)");
            }
            return result;
        }

        private static SolveResult Fail(string reason) => new() { Success = false, ErrorText = reason };

        private static string BuildArguments(string solveInput, string tempBase, XisfHeader header)
        {
            List<string> args = new()
            {
                $"-f \"{solveInput}\"",
                $"-o \"{tempBase}\"",   // .ini/.wcs land in temp, never beside a library image
                "-z 0",                 // auto downsample
            };

            if (header.RaDegrees is double ra && header.DecDegrees is double dec)
            {
                args.Add($"-ra {(ra / 15.0).ToString("0.######", CultureInfo.InvariantCulture)}");
                args.Add($"-spd {(dec + 90.0).ToString("0.######", CultureInfo.InvariantCulture)}");
                args.Add($"-r {HintedRadiusDeg}");
            }
            else
            {
                args.Add("-r 180"); // blind
            }

            if (header.FieldHeightDeg is double fov)
            {
                args.Add($"-fov {fov.ToString("0.######", CultureInfo.InvariantCulture)}");
            }
            else
            {
                args.Add("-fov 0"); // auto
            }

            return string.Join(" ", args);
        }

        private static async Task<(int ExitCode, bool TimedOut)> RunAstapAsync(string arguments, CancellationToken ct)
        {
            using Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = XisfConstants.AstapCliPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();

            using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(SolveTimeoutMs);
            try
            {
                await process.WaitForExitAsync(timeout.Token);
                return (process.ExitCode, false);
            }
            catch (OperationCanceledException)
            {
                try { process.Kill(entireProcessTree: true); } catch { /* already gone */ }
                ct.ThrowIfCancellationRequested();
                return (-1, true);
            }
        }

        private static SolveResult ParseIni(string iniPath, int exitCode)
        {
            if (!File.Exists(iniPath))
            {
                return Fail($"solver produced no result file ({DescribeExitCode(exitCode)})");
            }

            if (Log.IsDiagEnabled("SOLVER"))
            {
                Log.Diag("SOLVER", "ini: " + string.Join(" | ", File.ReadLines(iniPath)));
            }

            Dictionary<string, string> ini = File.ReadLines(iniPath)
                .Where(line => line.Contains('='))
                .Select(line => line.Split('=', 2))
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

            if (!ini.TryGetValue("PLTSOLVD", out string? solved) || solved != "T")
            {
                ini.TryGetValue("ERROR", out string? error);
                return Fail(string.IsNullOrWhiteSpace(error) ? DescribeExitCode(exitCode) : error);
            }

            // The ASTAP convention bridge (NINA ASTAPSolver): generic WCS math from the CD matrix,
            // then the solver-specific 180-degree offset and parity inversion.
            WcsOrientation wcs = WcsOrientation.FromCdMatrix(
                ParseDouble(ini, "CD1_1"), ParseDouble(ini, "CD1_2"),
                ParseDouble(ini, "CD2_1"), ParseDouble(ini, "CD2_2"));
            double positionAngle = (((360.0 - (wcs.RotationDegrees - 180.0)) % 360.0) + 360.0) % 360.0;

            return new SolveResult
            {
                Success = true,
                RaDegrees = ParseDouble(ini, "CRVAL1"),
                DecDegrees = ParseDouble(ini, "CRVAL2"),
                PositionAngleDegrees = positionAngle,
                Flipped = !wcs.Flipped,
                Crval1 = ini["CRVAL1"],
                Crval2 = ini["CRVAL2"],
                Crpix1 = ini["CRPIX1"],
                Crpix2 = ini["CRPIX2"],
                Cd1_1 = ini["CD1_1"],
                Cd1_2 = ini["CD1_2"],
                Cd2_1 = ini["CD2_1"],
                Cd2_2 = ini["CD2_2"],
                Crota1 = ini.GetValueOrDefault("CROTA1", string.Empty),
                Crota2 = ini.GetValueOrDefault("CROTA2", string.Empty),
            };
        }

        private static double ParseDouble(Dictionary<string, string> ini, string key)
        {
            if (!ini.TryGetValue(key, out string? raw)
                || !double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                throw new InvalidDataException($"ASTAP result is missing or has a non-numeric '{key}'.");
            }
            return value;
        }

        private static string DescribeExitCode(int exitCode) => exitCode switch
        {
            0 => "solver reported success but no solution was found",
            1 => "no solution",
            2 => "not enough stars detected",
            16 => "error reading image file",
            32 => "no star database found",
            33 => "error reading star database",
            34 => "error updating input file",
            _ => $"solver exit code {exitCode}",
        };

        /// <summary>
        /// Minimal mono FITS for the solver: SIMPLE/BITPIX=16/NAXIS=2 + BZERO=32768, big-endian
        /// signed data (value − 32768), padded to 2880-byte blocks. No keywords — hints ride the CLI.
        /// </summary>
        private static void WriteMinimalFits(string path, XisfImageData image)
        {
            const int block = 2880;
            List<string> cards = new()
            {
                FitsCard("SIMPLE", "T"),
                FitsCard("BITPIX", "16"),
                FitsCard("NAXIS", "2"),
                FitsCard("NAXIS1", image.Width.ToString(CultureInfo.InvariantCulture)),
                FitsCard("NAXIS2", image.Height.ToString(CultureInfo.InvariantCulture)),
                FitsCard("BZERO", "32768"),
                FitsCard("BSCALE", "1"),
                "END".PadRight(80),
            };

            using FileStream fs = new(path, FileMode.Create, FileAccess.Write);

            byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(string.Concat(cards));
            fs.Write(headerBytes, 0, headerBytes.Length);
            WritePadding(fs, block - (headerBytes.Length % block));

            // UInt16 little-endian pixels -> big-endian signed shorts offset by BZERO.
            byte[] pixels = image.Pixels;
            byte[] data = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i += 2)
            {
                int value = pixels[i] | (pixels[i + 1] << 8);
                short stored = (short)(value - 32768);
                data[i] = (byte)((stored >> 8) & 0xFF);
                data[i + 1] = (byte)(stored & 0xFF);
            }
            fs.Write(data, 0, data.Length);
            int remainder = data.Length % block;
            if (remainder != 0) WritePadding(fs, block - remainder);
        }

        private static string FitsCard(string keyword, string value) =>
            (keyword.PadRight(8) + "= " + value.PadLeft(20)).PadRight(80);

        private static void WritePadding(FileStream fs, int count)
        {
            if (count <= 0) return;
            byte[] pad = new byte[count];
            fs.Write(pad, 0, count);
        }
    }
}

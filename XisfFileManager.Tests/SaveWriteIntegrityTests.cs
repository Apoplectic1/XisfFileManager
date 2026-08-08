using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Astronomy.XISF;
using Astronomy.XISF.Compression;
using XisfFileManager.Files;
using XisfFileManager.Globals;
using Xunit;

namespace XisfFileManager.Tests;

/// <summary>
/// Behavior tests for the save path's structural-integrity contract
/// (openspec spec: save-write-integrity). Every scenario drives the real
/// <see cref="XisfFileUpdate.UpdateFileAsync"/> against a scratch copy of the TestData fixture,
/// compressed through the real AL rewriter so the fixture matches library reality (zstd+sh + SHA-1).
/// </summary>
public sealed class SaveWriteIntegrityTests : IDisposable
{
    private static readonly string FixturePath =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Unit16_0s_200x200.xisf");

    private readonly string mScratchDir;

    static SaveWriteIntegrityTests()
    {
        Astronomy.Diagnostics.Log.Init(new Astronomy.Diagnostics.AppLogIdentity(
            "XisfFileManager.Tests", "tests.log", "XFM_DIAG", Astronomy.Diagnostics.DiagDefault.None));
    }

    public SaveWriteIntegrityTests()
    {
        mScratchDir = Path.Combine(Path.GetTempPath(), "xfm-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(mScratchDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(mScratchDir, recursive: true); } catch { /* scratch cleanup is best-effort */ }
    }

    // ****************************************************************************************************

    [Fact]
    public async Task Save_aborts_and_leaves_file_untouched_when_declared_offset_is_corrupt()
    {
        string path = await CreateZstdFixtureAsync("corrupt-offset.xisf");
        ShiftDeclaredOffsetInPlace(path, delta: 64);   // real block no longer at the declared offset
        XisfFile file = LoadLikeBrowse(path);
        string before = Sha256(path);

        XisfFileUpdate updater = new();
        bool ok = await updater.UpdateFileAsync(file, path);

        Assert.False(ok);
        Assert.Equal(eUpdateOutcome.Failed, updater.LastUpdateOutcome);
        Assert.Equal(before, Sha256(path));
    }

    [Fact]
    public async Task Save_aborts_and_leaves_file_untouched_when_cached_geometry_is_stale()
    {
        string path = await CreateZstdFixtureAsync("stale-cache.xisf");
        XisfFile file = LoadLikeBrowse(path);
        file.TargetAttachmentStart -= 100;   // the historic double-save failure shape
        string before = Sha256(path);

        XisfFileUpdate updater = new();
        bool ok = await updater.UpdateFileAsync(file, path);

        Assert.False(ok);
        Assert.Equal(eUpdateOutcome.Failed, updater.LastUpdateOutcome);
        Assert.Equal(before, Sha256(path));
    }

    [Fact]
    public async Task Force_save_twice_produces_a_verified_file_via_refreshed_geometry()
    {
        string path = await CreateZstdFixtureAsync("double-save.xisf");
        XisfFile file = LoadLikeBrowse(path);
        file.AddKeyword("OBJECT", "GateTest", "grows the header between writes");

        XisfFileUpdate updater = new();
        Assert.True(await updater.UpdateFileAsync(file, path));
        Assert.Equal(eUpdateOutcome.Written, updater.LastUpdateOutcome);

        // Pre-fix, this second write copied the block from the stale offset and corrupted the file.
        Assert.True(await updater.UpdateFileAsync(file, path));
        Assert.Equal(eUpdateOutcome.Written, updater.LastUpdateOutcome);

        XisfChecksumResult verification = await XisfChecksumVerifier.VerifyAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(XisfChecksumVerdict.Verified, verification.Verdict);
        AssertGeometryConsistent(path);
    }

    [Fact]
    public async Task Update_new_save_skips_when_keywords_already_match_the_file()
    {
        string path = await CreateZstdFixtureAsync("skip-unchanged.xisf");
        XisfFile file = LoadLikeBrowse(path);
        file.KeywordUpdateMode = eKeywordUpdateMode.UPDATE_NEW;
        file.AddKeyword("OBJECT", "SkipTest", "differs from the keyword-less fixture");

        XisfFileUpdate updater = new();
        Assert.True(await updater.UpdateFileAsync(file, path));
        Assert.Equal(eUpdateOutcome.Written, updater.LastUpdateOutcome);

        string before = Sha256(path);
        Assert.True(await updater.UpdateFileAsync(file, path));
        Assert.Equal(eUpdateOutcome.Skipped, updater.LastUpdateOutcome);
        Assert.Equal(before, Sha256(path));
    }

    // ****************************************************************************************************

    /// <summary>Copies the uncompressed TestData fixture and re-stores it as zstd+sh + SHA-1 via the AL rewriter.</summary>
    private async Task<string> CreateZstdFixtureAsync(string name)
    {
        string path = Path.Combine(mScratchDir, name);
        File.Copy(FixturePath, path);
        await XisfBlockRewriter.RewriteAsync(path, path, BlockCodec.Zstd,
            XisfFileManager.Configuration.XisfConstants.CompressionZstdLevel, TestContext.Current.CancellationToken);
        return path;
    }

    /// <summary>Browse-equivalent init: geometry from the declared header, FORCE so the keyword skip can't hide a scenario.</summary>
    private static XisfFile LoadLikeBrowse(string path)
    {
        string head = Encoding.UTF8.GetString(File.ReadAllBytes(path), 0, Math.Min((int)new FileInfo(path).Length, 30000));
        Match location = Regex.Match(head, "location=\"attachment:(\\d+):(\\d+)\"");
        Assert.True(location.Success);
        return new XisfFile
        {
            FilePath = path,
            KeywordUpdateMode = eKeywordUpdateMode.FORCE,
            TargetAttachmentStart = int.Parse(location.Groups[1].Value),
            TargetAttachmentLength = int.Parse(location.Groups[2].Value),
            XmlVersionText = Regex.Match(head, @"<\?xml.*?\?>", RegexOptions.Singleline).Value,
            XmlCommentText = Regex.Match(head, @"<!--.*?-->", RegexOptions.Singleline).Value,
        };
    }

    /// <summary>Rewrites the declared attachment offset to point away from the real block, digit-count preserved.</summary>
    private static void ShiftDeclaredOffsetInPlace(string path, int delta)
    {
        byte[] data = File.ReadAllBytes(path);
        string head = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 30000));
        Match m = Regex.Match(head, "location=\"attachment:(\\d+):(\\d+)\"");
        Assert.True(m.Success);
        string oldOffset = m.Groups[1].Value;
        string newOffset = (int.Parse(oldOffset) + delta).ToString();
        Assert.Equal(oldOffset.Length, newOffset.Length);   // header length must not change
        byte[] patch = Encoding.UTF8.GetBytes(newOffset);
        Array.Copy(patch, 0, data, m.Groups[1].Index, patch.Length);
        File.WriteAllBytes(path, data);
    }

    private static void AssertGeometryConsistent(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        string head = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 30000));
        Match m = Regex.Match(head, "location=\"attachment:(\\d+):(\\d+)\"");
        Assert.True(m.Success);
        Assert.Equal(data.Length, int.Parse(m.Groups[1].Value) + int.Parse(m.Groups[2].Value));
    }

    private static string Sha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}

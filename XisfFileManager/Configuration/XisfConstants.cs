namespace XisfFileManager.Configuration
{
    public static class XisfConstants
    {
        public const int SignatureSize = 16;
        public const int MaxFileReadBytes = 1_000_000_000;  // 1 GB

        // Image-block compression written by XFM (codec: Astronomy.XISF.Compression, sibling Library repo).
        // zstd+sh level 19, chosen by the 2026-08-07 library benchmark (docs/2026-08-07-compression-benchmark.md):
        // -11% vs zlib-SmallestSize on light subs; higher levels add nothing, lower levels lose the win.
        // The shuffle variant is selected by AL from item size (1-byte samples write plain "zstd").
        // Requires readers with zstd support: NINA 3.x, PixInsight >= 1.8.9-2, AL. Level never affects
        // readability - it is encoder effort only.
        public const int CompressionZstdLevel = 19;

        // Checksum written alongside compression, computed over the stored (compressed) bytes.
        public const string ChecksumAlgorithm = "sha-1";

        // Local ASTAP plate solver (astap-plate-solve, 2026-08-06). The GUI binary astap.exe, driven
        // headless (NINA's pattern) - NOT astap_cli.exe, whose loader has no XISF support at all
        // (its extension dispatch handles FITS/TIFF/PNM/PNG only; .xisf fails as "Error reading image
        // file"; verified against unit_command_line_general.pas + empirically 2026-08-07). Only
        // astap.exe carries unit_xisf.pas (uncompressed-XISF reader). Constant, not a setting:
        // promote to Properties.Settings only if a second machine ever needs a different path.
        public const string AstapPath = @"C:\Program Files\astap\astap.exe";
    }
}

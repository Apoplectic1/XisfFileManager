namespace XisfFileManager.Configuration
{
    public static class XisfConstants
    {
        public const int SignatureSize = 16;
        public const int MaxFileReadBytes = 1_000_000_000;  // 1 GB

        // Image-block compression written by XFM (codec: Astronomy.XISF.Compression, sibling Library repo).
        // "zlib+sh" = zlib with byte-shuffle, the PixInsight/NINA default for multi-byte samples.
        // Plain "zlib" is used only as a fallback for 1-byte samples (no shuffle benefit).
        // Level maps to System.IO.Compression.CompressionLevel.SmallestSize (≈ zlib level 9 ≈ PixInsight "level 100").
        public const string CompressionCodec = "zlib+sh";
        public const string CompressionCodecZlib = "zlib";

        // Checksum written alongside compression, computed over the stored (compressed) bytes.
        public const string ChecksumAlgorithm = "sha-1";
    }
}

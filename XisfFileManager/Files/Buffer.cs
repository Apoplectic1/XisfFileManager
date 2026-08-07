using XisfFileManager.Globals;

namespace XisfFileManager
{
    public class Buffer
    {
        public eBufferData Type { get; set; }
        public string AsciiData { get; set; } = string.Empty;
        public int BinaryDataStart { get; set; }
        public int BinaryByteLength { get; set; }
        public byte[] BinaryData { get; set; } = Array.Empty<byte>();
    }
}

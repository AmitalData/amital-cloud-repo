namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IByteCompressorUtil
    {
        byte[] Compress(byte[] buffer);
        string CompressText(string text);
        byte[] Decompress(byte[] gzBuffer);
        string DeCompressText(string compressedText);
    }

}

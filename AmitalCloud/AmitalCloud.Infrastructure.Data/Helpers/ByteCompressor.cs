using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.IO;
using System.IO.Compression;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class ByteCompressorUtil : IByteCompressorUtil
    {
        public byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(buffer, 0, buffer.Length);
            zip.Close();
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;
        }

        public string CompressText(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) { return null; }
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(text);
            var compressBytes = Compress(buffer);
            return Convert.ToBase64String(compressBytes);
        }

        public string DeCompressText(string compressedText)
        {
            if (String.IsNullOrWhiteSpace(compressedText)) { return null; }
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            var deCompressBytes = Decompress(gzBuffer);
            return System.Text.Encoding.Unicode.GetString(deCompressBytes, 0, deCompressBytes.Length);
        }


        public byte[] Decompress(byte[] gzBuffer)
        {
            MemoryStream ms = new MemoryStream();
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

            byte[] buffer = new byte[msgLength];

            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            zip.Read(buffer, 0, buffer.Length);

            return buffer;
        }



    }
    public class ByteCompressor
    {
        public static byte[] Compress(byte[] buffer)
        {
            var obj = new ByteCompressorUtil();
            return obj.Compress(buffer); ;
        }

        public static byte[] Decompress(byte[] gzBuffer)
        {
            var obj = new ByteCompressorUtil();
            return obj.Decompress(gzBuffer); ;
        }
    }
}
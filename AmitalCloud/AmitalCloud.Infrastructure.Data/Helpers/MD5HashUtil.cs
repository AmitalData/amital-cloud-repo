using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.Helpers
{

    public class MD5HashUtil
    {
        public static string GetMD5Hash(byte[] Data)
        {


            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(Data)
                //    File.OpenRead(FilePath)
                )
                {
                    return GetByteArray(md5.ComputeHash(stream));
                }
            }



        }
        static string GetByteArray(byte[] array)
        {
            string hash = null;
            int i;
            for (i = 0; i < array.Length; i++)
            {
                hash = hash + String.Format("{0:X2}", array[i]);
                //if ((i % 4) == 3) hash = hash + " ";
            }
            return hash;
        }


        public static string GenerateHashForString(string entityString)
        {
            if (String.IsNullOrEmpty(entityString))
            {
                return "NULL";
            }

            MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] stringBytes = Encoding.UTF8.GetBytes(entityString);
            byte[] computedHash = cryptoServiceProvider.ComputeHash(stringBytes);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var h in computedHash)
            {
                stringBuilder.Append(h.ToString("x2").ToLower());
            }

            return stringBuilder.ToString();
        }
    }
}
public static class StreamExtensions
{
    public static IEnumerable<byte[]> GetByteChunks(this Stream stream, int length)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        var buffer = new byte[length];
        int count;
        while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            var result = new byte[count];
            Array.Copy(buffer, 0, result, 0, count);
            yield return result;
        }
    }
}
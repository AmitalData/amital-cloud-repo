using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class AesFunction
    {
        public byte[] DecryptData(byte[] data, int tenant, string aesKey = null)
        {
            if (data != null && data.Length > 0)
            {

                try
                {

                    string plaintext = null;
                    byte[] result = null;

                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = GetAesKey(tenant, aesKey);

                        byte[] IV = new byte[aesAlg.BlockSize / 8];
                        byte[] cipherText = new byte[data.Length - IV.Length];

                        Array.Copy(data, IV, IV.Length);
                        Array.Copy(data, IV.Length, cipherText, 0, cipherText.Length);

                        aesAlg.IV = IV;

                        aesAlg.Mode = CipherMode.CBC;


                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                        using (var msDecrypt = new MemoryStream(cipherText))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                    }

                    result = System.Convert.FromBase64String(plaintext);
                    return result;
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }

            }
            else return data;
        }

        public byte[] EncryptData(byte[] data, int tenant, string aesKey = null)
        {
            if (data != null && data.Length > 0)
            {
                try
                {
                    string dataAsString = Convert.ToBase64String(data);
                    byte[] encrypted;
                    byte[] IV;

                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = GetAesKey(tenant, aesKey);
                        aesAlg.GenerateIV();
                        IV = aesAlg.IV;
                        aesAlg.Mode = CipherMode.CBC;
                        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (var swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    swEncrypt.Write(dataAsString);
                                }
                                encrypted = msEncrypt.ToArray();
                            }
                        }
                    }

                    var combinedIvCt = new byte[IV.Length + encrypted.Length];
                    Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
                    Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);


                    return combinedIvCt;
                }
                catch (Exception ex)
                {
                    //string message = "In Valid Aes key";
                    //if (ex.Message == "Aes key is Empty")
                    //{
                    //    message = ex.Message;
                    //}

                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return data;
            }
        }


        private byte[] GetAesKey(int tenant, string aesKey = null)
        {
            if (string.IsNullOrEmpty(aesKey))
            {
                Tenant currentTenant = new Repository<Tenant>(tenant).GetSingle(new TenantKeys<string>() { Id = tenant });    ///.GetSingleTenantByIdAndTenant(tenant, true);
                if (currentTenant != null) aesKey = currentTenant.StorageEncryptionKey;
            }

            if (!string.IsNullOrEmpty(aesKey))
            {
                if (aesKey.Length < 32) throw new Exception("In Valid Aes key");
                else return Encoding.UTF8.GetBytes(aesKey);
            }
            else
            {
                throw new Exception("Aes key is Empty");
            }
        }

        private Random random = new Random();
        public string GenerateAesKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 32)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

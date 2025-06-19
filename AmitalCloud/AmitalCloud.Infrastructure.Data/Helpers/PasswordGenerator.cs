using System;
using System.Security.Cryptography;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class PasswordGenerator
    {
        /// <summary>
        /// Generates a password with the given character length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate(int length)
        {
            /* Alternative method
            Random random = new Random();
            return MD5Hash(random.Next(0,1000).ToString());
            */
            if (length < 5)
            {
                length = 5;
            }
            Random random = new Random();
            string password = MD5Hash(random.Next().ToString()).Substring(0, 8);
            string newPass = "";
            // Uppercase at random
            random = new Random();
            for (int i = 0; i < password.Length; i++)
            {
                if (random.Next(0, 2) == 1)
                    newPass += password.Substring(i, 1).ToUpper();
                else
                    newPass += password.Substring(i, 1);
            }
            return newPass.ToLower();
        }
        private static string MD5Hash(string data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
        public static string GetHashedPassword(string email, string password)
        {
            email = email.ToLower();
            byte[] byteRepresentation = UnicodeEncoding.UTF8.GetBytes(password + email);
            byte[] hashedTextInBytes = null;
            MD5CryptoServiceProvider myMd5 = new MD5CryptoServiceProvider();
            hashedTextInBytes = myMd5.ComputeHash(byteRepresentation);
            string hashedText = Convert.ToBase64String(hashedTextInBytes);
            return hashedText;
        }
        public static string GetBCryptHashedPassword(string email, string password)
        {
            string hashedPassword = "";
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                email = email.ToLower() + password;
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(email);
            }
            return hashedPassword;
        }
        public static bool VerifyBCryptHashedPassword(string email, string password, string hashedPassword)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(hashedPassword))
            {
                string submittedPassword = email.ToLower() + password;
                try
                {
                    isValid = BCrypt.Net.BCrypt.Verify(submittedPassword, hashedPassword);
                }
                catch (Exception ex)
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public static string GetOldHashedPassword(string password)
        {
            byte[] byteRepresentation = UnicodeEncoding.UTF8.GetBytes(password);
            byte[] hashedTextInBytes = null;
            MD5CryptoServiceProvider myMd5 = new MD5CryptoServiceProvider();
            hashedTextInBytes = myMd5.ComputeHash(byteRepresentation);
            string hashedText = Convert.ToBase64String(hashedTextInBytes);
            return hashedText;
        }
    }
}
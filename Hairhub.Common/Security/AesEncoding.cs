using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Common.Security
{
    public class AesEncoding
    {
        private static readonly string encryptionKey = "hairhub";

        private static byte[] GetAesKey(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);
                Array.Resize(ref hash, 32); // Ensure the key is 32 bytes long
                return hash;
            }
        }

        public static string EncryptAES(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetAesKey(encryptionKey);
                aes.IV = new byte[16]; // Default IV to 0

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    byte[] encryptedBytes = ms.ToArray();
                    return Convert.ToBase64String(encryptedBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
                }
            }
        }

        public static string DecryptAES(string cipherText)
        {
            string modifiedCipherText = cipherText.Replace("-", "+").Replace("_", "/") + "==".Substring(0, (4 - cipherText.Length % 4) % 4);
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetAesKey(encryptionKey);
                aes.IV = new byte[16]; // Default IV to 0

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(modifiedCipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string GenerateRandomPassword()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            var random = new Random();
            char firstChar = chars[random.Next(chars.Length)];
            firstChar = char.ToUpper(firstChar);

            string middleChars = new string(Enumerable.Repeat(chars + numbers, 6)
                                                      .Select(s => s[random.Next(s.Length)])
                                                      .ToArray());
            char lastChar = numbers[random.Next(numbers.Length)];
            string password = firstChar + middleChars + lastChar;

            return password;
        }
    }
}

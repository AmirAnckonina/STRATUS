using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Utils.EncryptionHelpers
{
    public static class EncryptionHelper
    {
        private static byte[] encryptionKey = KeyGenerator.GenerateRandomKey(32); // Replace with your secret encryption key.

        public static string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted data as a base64-encoded string.
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] combined = Convert.FromBase64String(encryptedText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                byte[] encrypted = new byte[combined.Length - iv.Length];
                Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(combined, iv.Length, encrypted, 0, encrypted.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(encrypted))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}

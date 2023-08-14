using StratusApp.Settings;
using System;
using System.IO;
using System.Security.Cryptography;

namespace StratusApp.Services.EncryptionHelpers
{
    public static class EncryptionHelper
    {
        private static byte[] encryptionKey;
        private static byte[] fixedIV;

        public static string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                aesAlg.IV = fixedIV;

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
                aesAlg.IV = fixedIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(combined))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        internal static void SetSettings(EncryptionSettings encryptionSettings)
        {
            encryptionKey = KeyGenerator.GetBytesFromBase64EncodedKey(encryptionSettings.EncryptionKey);
            fixedIV = KeyGenerator.GetBytesFromBase64EncodedKey(encryptionSettings.FixedIV);
        }
    }
}

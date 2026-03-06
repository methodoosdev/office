using AutoMapper;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace App.Core.Infrastructure.Mapper
{
    public class EncryptConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context) =>
            AesEncryption.Encrypt(sourceMember);
    }

    public class DecryptConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context) =>
            AesEncryption.Decrypt(sourceMember);
    }

    public static class AesEncryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("b4c87bb4f2e13483"); // Ensure 16 bytes for AES-128
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("9a4bbf7d5e0faf1b"); // Ensure 16 bytes for AES

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

}

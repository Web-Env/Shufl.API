using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shufl.API.Infrastructure.Encryption
{
    public static class EncryptionService
    {
        public static string EncryptUserId(Guid userId)
        {
            return EncryptString(userId.ToString());
        }

        public static string EncryptString(string input)
        {
            var publicKey = FetchPublicKey();

            var cipher = new RSACryptoServiceProvider();
            cipher.FromXmlString(publicKey);
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] cipherText = cipher.Encrypt(data, false);

            return Convert.ToBase64String(cipherText);
        }

        private static string FetchPublicKey()
        {
            return File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Encryption/Keys/shufl_public_key.xml")
            );
        }
    }
}

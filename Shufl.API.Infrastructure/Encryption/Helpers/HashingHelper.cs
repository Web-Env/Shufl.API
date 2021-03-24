using System.Security.Cryptography;
using System.Text;

namespace Shufl.API.Infrastructure.Encryption.Helpers
{
    public static class HashingHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public static string HashIdentifier(string identifier)
        {
            return HashString(identifier, 2);
        }

        private static string HashString(string plainTextString, int iterations = 1)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainTextString));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                if (iterations == 1)
                {
                    return builder.ToString();
                }
                else
                {
                    iterations--;
                    return HashString(builder.ToString(), iterations);
                }
            }
        }
    }
}

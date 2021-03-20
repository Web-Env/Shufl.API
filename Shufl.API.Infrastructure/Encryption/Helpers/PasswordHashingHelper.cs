namespace Shufl.API.Infrastructure.Encryption.Helpers
{
    public static class PasswordHashingHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
    }
}

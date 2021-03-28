using System;
using System.Linq;

namespace Shufl.API.Models
{
    public static class ModelHelpers
    {
        public static string GenerateUniqueIdentifier(int identifierLength)
        {
            var random = new Random();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var resetIdentifier = new string(
                Enumerable.Repeat(chars, identifierLength)
                            .Select(s => s[random.Next(s.Length)])
                            .ToArray());

            return resetIdentifier;
        }
    }
}

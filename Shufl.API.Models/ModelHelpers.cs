using System;
using System.Linq;

namespace Shufl.API.Models
{
    public static class ModelHelpers
    {
        public static string GenerateUniqueIdentifier()
        {
            var random = new Random();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var resetIdentifier = new string(
                Enumerable.Repeat(chars, 64)
                            .Select(s => s[random.Next(s.Length)])
                            .ToArray());

            return resetIdentifier;
        }
    }
}

using Shufl.Domain.Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shufl.API.Infrastructure.Helpers
{
    public static class GenreFormattingHelper
    {
        public static Genre CreateNewGenre(string genre, string code)
        {
            TextInfo textInfo = new CultureInfo("en-IE", false).TextInfo;
            string expression = @"[-,&]+([a-zA-Z])";

            var formattedGenre = textInfo.ToTitleCase(genre);

            if (Regex.IsMatch(formattedGenre, expression))
            {
                char[] genreCharArray = formattedGenre.ToCharArray();
                foreach (Match match in Regex.Matches(formattedGenre, expression, RegexOptions.Singleline))
                {
                    genreCharArray[match.Groups[1].Index] = char.ToUpper(genreCharArray[match.Groups[1].Index]);

                    formattedGenre = new string(genreCharArray);
                }
            }

            var newGenre = new Genre
            {
                Code = code,
                Name = formattedGenre
            };

            return newGenre;
        }
    }
}

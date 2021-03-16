using System.Collections.Generic;

namespace Shufl.API.Infrastructure.Consts
{
    public static class SearchConsts
    {
        public static List<string> SearchTerms { get; } = new List<string>
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };

        public static List<string> SearchGenres { get; } = new List<string>
        {
            "acoustic", "alt-rock", "alternative", "ambient", "bluegrass", "blues", "british",
            "chill", "classical", "country", "dance", "dancehall", "disco", "drum-and-bass",
            "edm", "electro", "electronic", "emo", "folk", "funk", "garage", "goth", "grindcore",
            "groove", "grunge", "guitar", "happy", "hard-rock", "hardcore", "hardstyle", "hip-hop",
            "honky-tonk", "house", "indie", "indie-pop", "industrial", "jazz", "metal", "minimal-techno",
            "movie", "new-age", "opera", "party", "piano", "pop", "power-pop", "progressive-house",
            "psych-rock", "punk", "punk-rock", "r-n-b", "reggae", "rock", "rock-n-roll", "rockabilly",
            "romance", "singer-songwriter", "songwriter", "soul", "soundtrack", "synth-pop", "tango",
            "techno", "trance"
        };

        public static string SearchExcludedGenres { get; } = "NOT genre:kids NOT genre:children NOT genre:christian NOT genre:gospel NOT genre:worship";

        public static string VariousArtistsId { get; } = "0LyfQWJT6nXafLPZqxe9Of";
    }
}

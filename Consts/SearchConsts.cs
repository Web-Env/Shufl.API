using System.Collections.Generic;

namespace Shufl.API.Consts
{
    public static class SearchConsts
    {
        public static List<string> SearchTerms { get; } = new List<string>()
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };

        public static List<string> SearchGenres { get; } = new List<string>()
        {
            "acoustic", "afrobeat", "alt-rock", "alternative", "ambient", "bluegrass", "blues",
            "bossanova", "breakbeat", "british", "chicago-house", "chill", "classical", "club",
            "comedy", "country", "dance", "dancehall", "deep-house", "detroit-techno", "disco",
            "drum-and-bass", "dub", "dubstep", "edm", "electro", "electronic", "emo", "folk",
            "forro", "funk", "garage", "goth", "grindcore", "groove", "grunge", "guitar", "happy",
            "hard-rock", "hardcore", "hardstyle", "hip-hop", "honky-tonk", "house", "idm",
            "indie", "indie-pop", "industrial", "jazz", "latin", "latino", "metal", "minimal-techno",
            "movie", "mpb", "new-age", "opera", "party", "piano", "pop", "power-pop", "progressive-house", 
            "psych-rock", "punk", "punk-rock", "r-n-b", "rainy-day", "reggae", "reggaeton", "rock", 
            "rock-n-roll", "rockabilly", "romance", "sad", "salsa", "samba", "show-tunes", 
            "singer-songwriter", "sleep", "songwriter", "soul", "soundtrack", "synth-pop", "tango",
            "techno", "trance", "trip-hop", "world-music"
        };

        public static string SearchExcludedGenres { get; } = "christian-gospel-worship";

        public static string VariousArtistsId { get; } = "0LyfQWJT6nXafLPZqxe9Of";
    }
}

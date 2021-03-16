using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Settings;
using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Models.Helpers
{
    public static class SearchHelper
    {
        public static SpotifyClient CreateSpotifyClient(SpotifyAPICredentials spotifyAPICredentials)
        {
            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(spotifyAPICredentials.ClientId, spotifyAPICredentials.ClientSecret));
            return new SpotifyClient(config);
        }

        private static string BuildSearchQuery(string genre)
        {
            var searchQuery = $"genre:{genre} {SearchConsts.SearchExcludedGenres}";
            return searchQuery;
        }

        public static async Task<SearchResponse> PerformSearch(
            SearchRequest.Types type, 
            string searchQuery, 
            int limit, 
            int offset, 
            SpotifyAPICredentials spotifyAPICredentials,
            bool retryDueToException = false, 
            int retry = 0)
        {
            SearchResponse search;
            try
            {
                var spotify = CreateSpotifyClient(spotifyAPICredentials);
                search = await spotify.Search.Item(new SearchRequest(type, searchQuery)
                {
                    Limit = limit,
                    Offset = offset
                });
            }
            catch (Exception err)
            {
                if (!retryDueToException)
                {
                    return await PerformSearch(type, searchQuery, limit, offset, spotifyAPICredentials, true);
                }
                else
                {
                    if (err is APIException)
                    {
                        Console.Out.WriteLine("Failure due to Spotify API");
                    }
                    throw;
                }
            }

            if ((type == SearchRequest.Types.Artist && search.Artists.Items.Count == 0) ||
                (type == SearchRequest.Types.Album && search.Albums.Items.Count == 0) ||
                (type == SearchRequest.Types.Track && search.Tracks.Items.Count == 0))
            {
                if (retry == 5 || offset < limit)
                {
                    return search;
                }
                else
                {
                    retry++;
                    offset = (int)(offset * (0.1f * retry));
                    return await PerformSearch(type, searchQuery, limit, offset, spotifyAPICredentials, retry: retry);
                }
            }

            return search;
        }

        public static async Task<SearchResponse> PerformRandomSearch(
            SearchRequest.Types type, 
            SpotifyAPICredentials spotifyAPICredentials, 
            string genre = "")
        {
            var buildSearchQueryResult = BuildSearchQuery(genre);
            var searchQuery = buildSearchQueryResult;
            var offset = RandInt(0, 999);

            return await PerformSearch(type, searchQuery, 50, offset, spotifyAPICredentials);
        }

        public static int RandInt(int min, int max)
        {
            var rand = new Random();
            return rand.Next(min, max);
        }
    }
}

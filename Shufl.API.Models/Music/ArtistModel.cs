using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Helpers;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Models.Music
{
    public static class ArtistModel
    {
        public static async Task<FullArtist> FetchRandomArtistAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var rand = new Random();

            var searchGenre = string.IsNullOrWhiteSpace(genre) ?
                SearchConsts.SearchGenres[rand.Next(0, SearchConsts.SearchGenres.Count - 1)] : genre;

            var artists = await SearchHelper.PerformRandomSearch(SearchRequest.Types.Artist, spotifyAPICredentials, searchGenre);
            var artist = SelectRandomArtist(artists.Artists.Items);

            return artist;
        }

        private static FullArtist SelectRandomArtist(List<FullArtist> artists, int retry = 0)
        {
            var rand = new Random();
            var index = artists.Count > 1 ? rand.Next(0, artists.Count - 1) : 0;
            var artist = artists[index];

            if (artist.Id == SearchConsts.VariousArtistsId)
            {
                if (retry == 5)
                {
                    return artist;
                }
                else
                {
                    retry++;
                    return SelectRandomArtist(artists, retry);
                }
            }

            return artist;
        }

        public static async Task<FullArtist> FetchArtistAsync(string artistId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var artist = await spotifyClient.Artists.Get(artistId);

            return artist;
        }
    }
}

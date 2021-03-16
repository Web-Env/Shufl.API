using Shufl.API.Consts;
using Shufl.API.DownloadModel;
using Shufl.API.Extensions;
using Shufl.API.Settings;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Helpers
{
    public static class AlbumSearchHelper
    {
        public static async Task<AlbumDownloadModel> FetchRandomAlbumAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var randomArtist = await ArtistSearchHelper.FetchRandomArtistAsync(spotifyAPICredentials, genre);
            var randomArtistAlbums = await FetchArtistAlbumsAsync(randomArtist.Id, spotifyAPICredentials);
            randomArtistAlbums.Shuffle();
            var randomAlbum = GetRandomAlbum(randomArtistAlbums);
            return await FetchAlbumAsync(randomAlbum.Id, spotifyAPICredentials);
        }

        public static async Task<List<SimpleAlbum>> FetchArtistAlbumsAsync(string artistId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var albums = (await spotifyClient.Artists.GetAlbums(artistId)).Items;

            return albums;
        }

        public static async Task<AlbumDownloadModel> FetchAlbumAsync(string albumId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var album = await spotifyClient.Albums.Get(albumId);
            var artist = await ArtistSearchHelper.FetchArtistAsync(album.Artists.FirstOrDefault().Id, spotifyAPICredentials);

            var albumData = new AlbumDownloadModel
            {
                Genres = artist.Genres,
                Album = album
            };

            return albumData;
        }

        public static async Task<SearchResponse> PerformAlbumSearch(
            string name, 
            SpotifyAPICredentials spotifyAPICredentials, 
            bool retryDueToException = false)
        {
            SearchResponse search;
            try
            {
                var spotify = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);
                search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Album, $"name:{name}")
                {
                    Limit = 10,
                    Offset = 0
                });
            }
            catch (Exception err)
            {
                if (!retryDueToException)
                {
                    return await PerformAlbumSearch(name, spotifyAPICredentials, true);
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

            return search;
        }

        private static SimpleAlbum GetRandomAlbum(List<SimpleAlbum> randomAlbums, int index = 0)
        {
            var randomAlbum = randomAlbums[index];

            if ((randomAlbum.AlbumType == "single" || randomAlbum.AlbumType == "ep") && 
                index <= randomAlbums.Count - 1)
            {
                index++;
                return GetRandomAlbum(randomAlbums, index);
            }

            return randomAlbum;
        }
    }
}

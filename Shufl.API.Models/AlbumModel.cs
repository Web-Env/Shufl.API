using Shufl.API.DownloadModels.Album;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Extensions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Helpers;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models
{
    public static class AlbumModel
    {
        public static async Task<AlbumDownloadModel> FetchRandomAlbumAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var randomArtist = await ArtistModel.FetchRandomArtistAsync(spotifyAPICredentials, genre);
            var randomArtistAlbums = await FetchArtistAlbumsAsync(randomArtist.Id, spotifyAPICredentials).ConfigureAwait(false);
            randomArtistAlbums.Shuffle();
            var randomAlbum = GetRandomAlbum(randomArtistAlbums);
            return await FetchAlbumAsync(randomAlbum.Id, spotifyAPICredentials).ConfigureAwait(false);
        }

        public static async Task<List<SimpleAlbum>> FetchArtistAlbumsAsync(string artistId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var albumsRequest = new ArtistsAlbumsRequest
            {
                Market = "IE"
            };
            var albums = (await spotifyClient.Artists.GetAlbums(artistId, albumsRequest)).Items;

            return albums;
        }

        public static async Task<AlbumDownloadModel> FetchAlbumAsync(string albumId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var album = await spotifyClient.Albums.Get(albumId);
            var artist = await ArtistModel.FetchArtistAsync(album.Artists.FirstOrDefault().Id, spotifyAPICredentials);

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
                    return await PerformAlbumSearch(name, spotifyAPICredentials, true).ConfigureAwait(false);
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

            if (!IsValidAlbum(randomAlbum) && index < randomAlbums.Count - 1)
            {
                index++;
                return GetRandomAlbum(randomAlbums, index);
            }

            return randomAlbum;
        }

        private static bool IsValidAlbum(SimpleAlbum album)
        {
            if (album.AlbumType == "single" || album.Artists.Count == 0 || album.Artists[0].Id == SearchConsts.VariousArtistsId)
            {
                return false;
            }

            return true;
        }
    }
}

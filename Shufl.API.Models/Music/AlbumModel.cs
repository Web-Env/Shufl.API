using AutoMapper;
using Shufl.API.DownloadModels.Album;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Extensions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music.Helpers;
using Shufl.Domain.Entities;
using Shufl.Domain.Enums;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Music
{
    public static class AlbumModel
    {
        public static async Task<AlbumDownloadModel> FetchRandomAlbumAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var randomArtist = await ArtistModel.FetchRandomArtistAsync(spotifyAPICredentials, genre);
            var randomArtistAlbums = await FetchArtistAlbumsAsync(randomArtist.Id, spotifyAPICredentials).ConfigureAwait(false);
            if (randomArtistAlbums.Count > 0)
            {
                randomArtistAlbums.Shuffle();
                var randomAlbum = GetRandomAlbum(randomArtistAlbums);
                return await FetchAlbumAsync(randomAlbum.Id, spotifyAPICredentials).ConfigureAwait(false);
            }
            else
            {
                return await FetchRandomAlbumAsync(spotifyAPICredentials, genre);
            }
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

        public static async Task<AlbumDownloadModel> FetchAlbumAsync(string albumIdentifier, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var album = await spotifyClient.Albums.Get(albumIdentifier);
            var artist = await ArtistModel.FetchArtistAsync(album.Artists.FirstOrDefault().Id, spotifyAPICredentials);

            var albumData = new AlbumDownloadModel
            {
                Genres = artist.Genres,
                Album = album
            };

            return albumData;
        }

        private static async Task<SpotifyAlbumDownloadModel> FetchAlbumForIndexAsync(
            string albumIdentifier,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var album = await spotifyClient.Albums.Get(albumIdentifier);
            var artists = await ArtistModel.FetchArtistsAsync(album.Artists.Select(a => a.Id).ToList(), spotifyAPICredentials);

            var spotifyAlbumDownloadModel = new SpotifyAlbumDownloadModel
            {
                Artists = artists,
                Album = album
            };

            return spotifyAlbumDownloadModel;
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
                search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Album, name)
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

        public static async Task<Album> IndexNewAlbumAsync(
            string albumIdentifier,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            try
            {
                var spotifyAlbumDownloadModel = await FetchAlbumForIndexAsync(albumIdentifier, spotifyAPICredentials);

                var artists = await ArtistModel.CreateOrFetchArtistAsync(spotifyAlbumDownloadModel.Artists, repositoryManager, mapper);
                var albumArtists = artists.Select(a => a.Id);

                var newAlbum = new Album
                {
                    SpotifyId = spotifyAlbumDownloadModel.Album.Id,
                    Name = spotifyAlbumDownloadModel.Album.Name,
                    ReleaseDate = ParseReleaseDateToDateTime(spotifyAlbumDownloadModel.Album.ReleaseDate, spotifyAlbumDownloadModel.Album.ReleaseDatePrecision),
                    Type = (byte)MapAlbumTypeToEnum(spotifyAlbumDownloadModel.Album.Type),
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };

                newAlbum.AlbumArtists = MapArtistsToAlbumArtists(albumArtists);
                newAlbum.AlbumImages = mapper.Map<List<AlbumImage>>(spotifyAlbumDownloadModel.Album.Images);
                await repositoryManager.AlbumRepository.AddAsync(newAlbum);

                //var newAlbumTracks = await TrackModel.IndexNewAlbumTracksAsync(
                //    newAlbum.Id,
                //    spotifyAlbumDownloadModel.Album,
                //    artists,
                //    repositoryManager,
                //    mapper,
                //    spotifyAPICredentials);

                //await repositoryManager.TrackRepository.AddRangeAsync(newAlbumTracks);

                return newAlbum;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DateTime ParseReleaseDateToDateTime(string releaseDate, string releaseDatePrecision)
        {
            if (releaseDatePrecision == "year")
            {
                return DateTime.ParseExact(releaseDate, "yyyy", CultureInfo.InvariantCulture);
            }
            else if (releaseDatePrecision == "month")
            {
                return DateTime.ParseExact(releaseDate, "yyyy-MM", CultureInfo.InvariantCulture);
            }
            else if (releaseDatePrecision == "day")
            {
                return DateTime.Parse(releaseDate);
            }
            else
            {
                throw new Exception();
            }
        }

        private static List<AlbumArtist> MapArtistsToAlbumArtists(IEnumerable<Guid> artistIds)
        {
            List<AlbumArtist> albumArtists = new List<AlbumArtist>();

            foreach (var artistId in artistIds)
            {
                albumArtists.Add(new AlbumArtist
                {
                    ArtistId = artistId,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                });
            }

            return albumArtists;
        }

        private static AlbumType MapAlbumTypeToEnum(string albumType) =>
            albumType.ToLowerInvariant() switch
            {
                "album" => AlbumType.Album,
                "compilation" => AlbumType.Compilation,
                "ep" => AlbumType.EP,
                "single" => AlbumType.Single,
                _ => AlbumType.Other
            };

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

        private class SpotifyAlbumDownloadModel
        {
            public List<FullArtist> Artists { get; set; }

            public FullAlbum Album { get; set; }
        }
    }
}

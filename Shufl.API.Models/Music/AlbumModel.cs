using AutoMapper;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Extensions;
using Shufl.API.Infrastructure.Helpers;
using Shufl.API.Infrastructure.SearchResponseModels;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music.Helpers;
using Shufl.Domain.Entities;
using Shufl.Domain.Enums;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Music
{
    public static class AlbumModel
    {
        public static async Task<AlbumResponseModel> FetchRandomAlbumAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
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
                return await FetchRandomAlbumAsync(spotifyAPICredentials, genre).ConfigureAwait(false);
            }
        }
        public static async Task<AlbumResponseModel> FetchRandomGroupAlbumAsync(
            string groupIdentifier, 
            SpotifyAPICredentials spotifyAPICredentials, 
            IRepositoryManager repositoryManager,
            string genre = "")
        {
            var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

            if (group != null)
            {
                var groupAlbumCount = await repositoryManager.GroupAlbumRepository.GetCountByGroupIdAsync(group.Id);

                if (groupAlbumCount >= 30)
                {
                    var groupTopThirty = (await repositoryManager.GroupAlbumRepository.GetTopThirtyByGroupIdAsync(group.Id)).ToList();

                    var rand = new Random();
                    var randInt = rand.Next(0, groupTopThirty.Count - 1);
                    var groupAlbum = groupTopThirty[randInt];
                    var groupAlbumArtist = groupAlbum.Album.AlbumArtists.FirstOrDefault().Artist;

                    var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);
                    var relatedArtists = await spotifyClient.Artists.GetRelatedArtists(groupAlbumArtist.SpotifyId);
                    randInt = rand.Next(0, relatedArtists.Artists.Count - 1);
                    var relatedArtist = relatedArtists.Artists[randInt];

                    var randomArtistAlbums = await FetchArtistAlbumsAsync(relatedArtist.Id, spotifyAPICredentials).ConfigureAwait(false);
                    if (randomArtistAlbums.Count > 0)
                    {
                        randomArtistAlbums.Shuffle();
                        var randomAlbum = GetRandomAlbum(randomArtistAlbums);
                        var randomAlbumResponseModel = await FetchAlbumAsync(randomAlbum.Id, spotifyAPICredentials).ConfigureAwait(false);
                        randomAlbumResponseModel.RelatedGroupAlbum = groupAlbum;

                        return randomAlbumResponseModel;
                    }
                }
            }

            return await FetchRandomAlbumAsync(spotifyAPICredentials, genre).ConfigureAwait(false);
        }

        public static async Task<AlbumResponseModel> FetchAlbumAsync(string albumIdentifier, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var album = await spotifyClient.Albums.Get(albumIdentifier);

            var artistsRequest = new ArtistsRequest(album.Artists.Select(a => a.Id).ToList());
            var artists = await spotifyClient.Artists.GetSeveral(artistsRequest);

            var albumResponseModel = new AlbumResponseModel
            {
                Album = album,
                Artists = artists.Artists
            };

            return albumResponseModel;
        }

        public static async Task<List<SimpleAlbum>> FetchArtistAlbumsAsync(string artistId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var albumsRequest = new ArtistsAlbumsRequest
            {
                Market = "IE",
                Limit = 50
            };

            var albums = (await spotifyClient.Artists.GetAlbums(artistId, albumsRequest)).Items;

            return albums;
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
                    Offset = 0,
                    Market = "IE"
                });

                return search;
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
        }

        public static async Task<Album> IndexNewAlbumAsync(
            string albumIdentifier,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            try
            {
                var spotifyAlbumDownloadModel = await FetchAlbumForIndexAsync(albumIdentifier, spotifyAPICredentials).ConfigureAwait(false);

                var artists = await ArtistModel.CreateOrFetchArtistAsync(spotifyAlbumDownloadModel.Artists, repositoryManager, mapper);
                var albumArtists = artists.Select(a => a.Id);
                var releaseDate = ReleaseDateParsingHelper.ParseReleaseDateToDateTime(
                    spotifyAlbumDownloadModel.Album.ReleaseDate,
                    spotifyAlbumDownloadModel.Album.ReleaseDatePrecision);

                var newAlbum = new Album
                {
                    SpotifyId = spotifyAlbumDownloadModel.Album.Id,
                    Name = spotifyAlbumDownloadModel.Album.Name,
                    ReleaseDate = releaseDate,
                    Type = (byte)MapAlbumTypeToEnum(spotifyAlbumDownloadModel.Album.Type),
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };

                newAlbum.AlbumArtists = MapArtistsToAlbumArtists(albumArtists);
                newAlbum.AlbumImages = mapper.Map<List<AlbumImage>>(spotifyAlbumDownloadModel.Album.Images);
                await repositoryManager.AlbumRepository.AddAsync(newAlbum);

                return newAlbum;
            }
            catch (Exception)
            {
                throw;
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

using AutoMapper;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Helpers;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music.Helpers;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static async Task<List<FullArtist>> FetchArtistsAsync(
            List<string> artistsIds,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);
            var artistsRequest = new ArtistsRequest(artistsIds.Take(50).ToList());

            var artists = (await spotifyClient.Artists.GetSeveral(artistsRequest)).Artists;

            return artists;
        }

        public static async Task<SearchResponse> PerformArtistSearch(
            string name,
            SpotifyAPICredentials spotifyAPICredentials,
            bool retryDueToException = false)
        {
            SearchResponse search;
            try
            {
                var spotify = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);
                search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Artist, name)
                {
                    Limit = 10,
                    Offset = 0
                });

                return search;
            }
            catch (Exception err)
            {
                if (!retryDueToException)
                {
                    return await PerformArtistSearch(name, spotifyAPICredentials, true).ConfigureAwait(false);
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

        public static async Task<IEnumerable<Artist>> CreateOrFetchArtistAsync(
            List<FullArtist> fullArtists,
            IRepositoryManager repositoryManager,
            IMapper mapper)
        {

            var existingArtists = await repositoryManager.ArtistRepository.GetManyBySpotifyIdsAsync(fullArtists.Select(a => a.Id).ToList());
            var existingArtistSpotifyIds = existingArtists.Select(a => a.SpotifyId).ToHashSet();
            List<Artist> newArtists = new List<Artist>();
            fullArtists.RemoveAll(fa => existingArtistSpotifyIds.Contains(fa.Id));

            foreach (var fullArtist in fullArtists)
            {
                var newArtist = new Artist
                {
                    SpotifyId = fullArtist.Id,
                    Name = fullArtist.Name
                };

                var artistGenres = await CreateArtistGenres(fullArtist, repositoryManager);
                var artistImages = mapper.Map<List<ArtistImage>>(fullArtist.Images);

                newArtist.ArtistGenres = artistGenres;
                newArtist.ArtistImages = artistImages;

                newArtists.Add(newArtist);

                await repositoryManager.ArtistRepository.AddAsync(newArtist);
            }

            newArtists.AddRange(existingArtists);

            return newArtists;
        }

        private static async Task<List<ArtistGenre>> CreateArtistGenres(
            FullArtist fullArtist,
            IRepositoryManager repositoryManager)
        {
            List<ArtistGenre> artistGenres = new List<ArtistGenre>();
            HashSet<string> strippedGenres = fullArtist.Genres.Select(g => g.Replace(" ", "")).ToHashSet();
            var existingGenres = await repositoryManager.GenreRepository.GetManyByCodeAsync(strippedGenres);
            var existingGenreCodes = existingGenres.Select(g => g.Code).ToHashSet();

            foreach (var genre in fullArtist.Genres)
            {
                var strippedGenre = genre.Replace(" ", "");

                var existingGenre = existingGenreCodes.Contains(strippedGenre) ? 
                    existingGenres.Where(g => g.Code == strippedGenre).FirstOrDefault() : null;

                if (existingGenre != null)
                {
                    artistGenres.Add(new ArtistGenre
                    {
                        GenreId = existingGenre.Id
                    });
                }
                else
                {
                    var newGenre = GenreFormattingHelper.CreateNewGenre(genre, strippedGenre);

                    artistGenres.Add(new ArtistGenre
                    {
                        Genre = newGenre
                    });
                }
            }

            return artistGenres;
        }
    }
}

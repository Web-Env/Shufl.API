using AutoMapper;
using Shufl.API.Infrastructure.SearchResponseModels;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music.Helpers;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Music
{
    public static class TrackModel
    {
        public static async Task<AlbumResponseModel> FetchRandomTrackAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var album = await AlbumModel.FetchRandomAlbumAsync(spotifyAPICredentials, genre).ConfigureAwait(false);
            album.Album.Tracks.Items = GetRandomTrack(album.Album.Tracks.Items);

            return album;
        }

        public static async Task<AlbumResponseModel> FetchTrackAsync(string trackId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var track = await spotifyClient.Tracks.Get(trackId);
            var album = await AlbumModel.FetchAlbumAsync(track.Album.Id, spotifyAPICredentials).ConfigureAwait(false);
            album.Album.Tracks.Items = GetTrack(album.Album.Tracks.Items, trackId);

            return album;
        }

        public static async Task<List<Track>> IndexNewAlbumTracksAsync(
            Guid albumId,
            FullAlbum album,
            IEnumerable<Artist> albumArtists,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            List<Track> newAlbumTracks = new List<Track>();
            HashSet<string> albumArtistsSpotifyIds = albumArtists.Select(a => a.SpotifyId).ToHashSet();

            foreach (var track in album.Tracks.Items)
            {
                var newTrack = new Track
                {
                    SpotifyId = track.Id,
                    AlbumId = albumId,
                    Name = track.Name,
                    TrackNumber = (short)track.TrackNumber,
                    DiscNumber = (byte)track.DiscNumber,
                    Duration = track.DurationMs
                };

                newTrack.TrackArtists = await MapTrackArtistsAsync(
                    track,
                    albumArtistsSpotifyIds,
                    albumArtists,
                    repositoryManager,
                    mapper,
                    spotifyAPICredentials).ConfigureAwait(false);
            }

            return newAlbumTracks;
        }

        private static async Task<List<TrackArtist>> MapTrackArtistsAsync(
            SimpleTrack track,
            HashSet<string> albumArtistsSpotifyIds,
            IEnumerable<Artist> albumArtists,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            List<TrackArtist> trackArtists = new List<TrackArtist>();
            List<string> trackArtistsToBeCreated = new List<string>();
            var trackArtistSpotifyIds = track.Artists.Select(a => a.Id).ToHashSet();

            foreach (var trackArtist in track.Artists)
            {
                if (albumArtistsSpotifyIds.Contains(trackArtist.Id))
                {
                    var albumArtist = albumArtists.Where(a => a.SpotifyId == trackArtist.Id).FirstOrDefault();

                    trackArtists.Add(new TrackArtist
                    {
                        ArtistId = albumArtist.Id,
                        CreatedOn = DateTime.Now,
                        LastUpdatedOn = DateTime.Now
                    });
                }
                else
                {
                    trackArtistsToBeCreated.Add(trackArtist.Id);
                }
            }

            if(trackArtistsToBeCreated.Count > 0)
            {
                var newArtists = await ArtistModel.FetchArtistsAsync(trackArtistsToBeCreated, spotifyAPICredentials);
                var artists = await ArtistModel.CreateOrFetchArtistAsync(newArtists, repositoryManager, mapper);

                trackArtists.AddRange(MapArtistsToTrackArtists(artists.Select(a => a.Id).ToList()));
            }

            return trackArtists;
        }

        private static List<TrackArtist> MapArtistsToTrackArtists(IEnumerable<Guid> artistIds)
        {
            List<TrackArtist> trackArtists = new List<TrackArtist>();

            foreach (var artistId in artistIds)
            {
                trackArtists.Add(new TrackArtist
                {
                    ArtistId = artistId,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                });
            }

            return trackArtists;
        }

        private static List<SimpleTrack> GetRandomTrack(List<SimpleTrack> randomTracks)
        {
            var randIndex = SearchHelper.RandInt(0, randomTracks.Count - 1);
            var randomTrackList = new List<SimpleTrack>
            {
                randomTracks[randIndex]
            };

            return randomTrackList;
        }

        private static List<SimpleTrack> GetTrack(List<SimpleTrack> tracks, string trackId)
        {
            var track = tracks.FirstOrDefault(t => t.Id == trackId);
            var trackList = new List<SimpleTrack>
            {
                track
            };

            return trackList;
        }
    }
}

using Shufl.API.DownloadModels.Album;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Helpers;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models
{
    public static class TrackModel
    {
        public static async Task<AlbumDownloadModel> FetchRandomTrackAsync(SpotifyAPICredentials spotifyAPICredentials, string genre = "")
        {
            var album = await AlbumModel.FetchRandomAlbumAsync(spotifyAPICredentials, genre).ConfigureAwait(false);
            album.Album.Tracks.Items = GetRandomTrack(album.Album.Tracks.Items);

            return album;
        }

        public static async Task<AlbumDownloadModel> FetchTrackAsync(string trackId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var track = await spotifyClient.Tracks.Get(trackId);
            var album = await AlbumModel.FetchAlbumAsync(track.Album.Id, spotifyAPICredentials).ConfigureAwait(false);
            album.Album.Tracks.Items = GetTrack(album.Album.Tracks.Items, trackId);

            return album;
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

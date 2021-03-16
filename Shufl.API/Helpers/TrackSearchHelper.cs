using Shufl.API.Settings;
using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Helpers
{
    public class TrackSearchHelper
    {
        public static async Task<FullTrack> FetchRandomTrackAsync(SpotifyAPICredentials spotifyAPICredentials)
        {
            var rand = new Random();

            var tracks = await SearchHelper.PerformRandomSearch(SearchRequest.Types.Track, spotifyAPICredentials);
            var index = tracks.Tracks.Items.Count > 1 ? rand.Next(0, tracks.Tracks.Items.Count - 1) : 0;
            var track = tracks.Tracks.Items[index];

            return track;
        }

        public static async Task<FullTrack> FetchTrackAsync(string trackId, SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var track = await spotifyClient.Tracks.Get(trackId);

            return track;
        }
    }
}

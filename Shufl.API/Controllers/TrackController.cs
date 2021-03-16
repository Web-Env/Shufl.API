using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public TrackController(IOptions<SpotifyAPICredentials> spotifyAPICredentials)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomTrack")]
        public async Task<IActionResult> GetRandomTrackAsync()
        {
            try
            {
                var randomTrack = await TrackModel.FetchRandomTrackAsync(_spotifyAPICredentials);
                return Ok(randomTrack);
            }
            catch (Exception err)
            {
                return Problem("There was an error fetching a random track from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Track")]
        public async Task<IActionResult> GetTrackAsync(string trackId)
        {
            try
            {
                var track = await TrackModel.FetchTrackAsync(trackId, _spotifyAPICredentials);
                return Ok(track);
            }
            catch (Exception err)
            {
                return Problem("There was an error fetching the requested track from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

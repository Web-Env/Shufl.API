using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Music
{
    [ApiController]
    [Route("[controller]")]
    public class TrackController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public TrackController(IRepositoryManager repositoryManager,
                               ILogger<TrackController> logger,
                               IMapper mapper,
                               IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(repositoryManager, logger, mapper)
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
                LogException(err);
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
                LogException(err);
                return Problem("There was an error fetching the requested track from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

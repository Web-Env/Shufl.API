using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music;
using Shufl.Domain.Entities;
using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Music
{
    [ApiController]
    [Route("[controller]")]
    public class TrackController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public TrackController(ShuflContext shuflContext,
                               ILogger<TrackController> logger,
                               IMapper mapper,
                               IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(shuflContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomTrack")]
        public async Task<ActionResult<TrackDownloadModel>> GetRandomTrackAsync()
        {
            try
            {
                var randomTrack = await TrackModel.FetchRandomTrackAsync(_spotifyAPICredentials);

                return Ok(MapEntityToDownloadModel<FullAlbum, AlbumDownloadModel>(randomTrack));
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching a random track from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Track")]
        public async Task<ActionResult<TrackDownloadModel>> GetTrackAsync(string trackId)
        {
            try
            {
                var track = await TrackModel.FetchTrackAsync(trackId, _spotifyAPICredentials);

                return Ok(MapEntityToDownloadModel<FullAlbum, AlbumDownloadModel>(track));
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching the requested track from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

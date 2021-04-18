using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Spotify;
using Shufl.API.UploadModels.Spotify;
using Shufl.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Spotify
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class SpotifyController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public SpotifyController(ShuflContext dbContext,
                                 IMapper mapper,
                                 ILogger<SpotifyController> logger,
                                 IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(dbContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials?.Value;
        }

        [HttpPost("LinkSpotify")]
        public async Task<IActionResult> LinkSpotifyAsync(SpotifyLinkUploadModel spotifyLinkUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await SpotifyModel.LinkSpotifyAsync(spotifyLinkUploadModel, ExtractUserIdFromToken(), RepositoryManager, _spotifyAPICredentials);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpDelete("UnlinkSpotify")]
        public async Task<IActionResult> UnlinkSpotifyAsync()
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await SpotifyModel.UnlinkSpotifyAsync(ExtractUserIdFromToken(), RepositoryManager);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("QueueAlbum")]
        public async Task<IActionResult> QueueAlbumAsync(string albumId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await SpotifyModel.QueueAlbumAsync(albumId, ExtractUserIdFromToken(), RepositoryManager, _spotifyAPICredentials);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("QueueTrack")]
        public async Task<IActionResult> QueueTrackAsync(string trackId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await SpotifyModel.QueueTrackAsync(trackId, ExtractUserIdFromToken(), RepositoryManager, _spotifyAPICredentials);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music;
using Shufl.Domain.Entities;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Music
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class PlaylistController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public PlaylistController(ShuflContext shuflContext,
                                  ILogger<PlaylistController> logger,
                                  IMapper mapper,
                                  IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(shuflContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<PlaylistDownloadModel>>> GetUserPlaylistsAsync(int page, int pageSize)
        {
            try
            {
                var playlists = await PlaylistModel.GetUserPlaylistsAsync(
                    _spotifyAPICredentials,
                    ExtractUserIdFromToken(),
                    page,
                    pageSize,
                    RepositoryManager).ConfigureAwait(false);

                return Ok(MapEntitiesToDownloadModels<SimplePlaylist, PlaylistDownloadModel>(playlists));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem("There was an error fetching a random album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

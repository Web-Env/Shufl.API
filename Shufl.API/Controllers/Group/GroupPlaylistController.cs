using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Group;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Group
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class GroupPlaylistController : CustomControllerBase
    {

        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public GroupPlaylistController(ShuflContext dbContext,
                                       IMapper mapper,
                                       ILogger<GroupPlaylistController> logger,
                                       IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(dbContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }



        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GroupPlaylistDownloadModel>>> GetAllGroupPlaylistsAsync(string groupIdentifier, int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupSuggestions = await GroupPlaylistModel.GetGroupPlaylistsAsync(
                        groupIdentifier,
                        page,
                        pageSize,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntitiesToDownloadModels<GroupPlaylist, GroupPlaylistDownloadModel>(groupSuggestions));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (UserNotGroupMemberException)
            {
                return Forbid();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("Get")]
        public async Task<ActionResult<GroupSuggestionDownloadModel>> GetAllGroupSuggestionsAsync(string groupIdentifier, string groupSuggestionIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupSuggestion = await GroupSuggestionModel.GetGroupSuggestionAsync(
                        groupIdentifier,
                        groupSuggestionIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupSuggestion, GroupSuggestionDownloadModel>(groupSuggestion));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (UserNotGroupMemberException)
            {
                return Forbid();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateGroupPlaylistAsync(GroupPlaylistUploadModel groupPlaylistUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var newGroupPlaylistIdentifier = await GroupPlaylistModel.CreateNewGroupPlaylistAsync(
                        groupPlaylistUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager,
                        GetMapper(),
                        _spotifyAPICredentials);

                    return Ok(newGroupPlaylistIdentifier);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (UserNotGroupMemberException)
            {
                return Forbid();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

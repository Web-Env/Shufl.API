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
    public class GroupSuggestionController : CustomControllerBase
    {
        private SpotifyAPICredentials _spotifyAPICredentials;

        public GroupSuggestionController(ShuflContext dbContext,
                                         IMapper mapper,
                                         ILogger<GroupSuggestionController> logger,
                                         IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(dbContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<GroupSuggestionDownloadModel>>> GetGroupSuggestionsAsync(string groupIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupSuggestions = await GroupSuggestionModel.GetGroupSuggestionsAsync(
                        groupIdentifier, 
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntitiesToDownloadModels<GroupSuggestion, GroupSuggestionDownloadModel>(groupSuggestions));
                }
                else
                {
                    return Unauthorized();
                }
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
        public async Task<ActionResult<string>> CreateGroupSuggestionAsync(GroupSuggestionUploadModel groupSuggestionUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var newGroupSuggestionIdentifier = await GroupSuggestionModel.CreateNewGroupSuggestionAsync(
                        groupSuggestionUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager,
                        GetMapper(),
                        _spotifyAPICredentials);

                    return newGroupSuggestionIdentifier;
                }
                else
                {
                    return Unauthorized();
                }
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

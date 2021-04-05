using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Models.Group;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Group
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class GroupSuggestionRatingController : CustomControllerBase
    {
        public GroupSuggestionRatingController(ShuflContext dbContext,
                                               IMapper mapper,
                                               ILogger<GroupInviteController> logger) : base(dbContext, logger, mapper) { }

        [HttpPost("Create")]
        public async Task<ActionResult<GroupSuggestionRatingDownloadModel>> CreateGroupSuggestionRatingAsync(
            GroupSuggestionRatingUploadModel groupSuggestionRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupSuggestionRating = await GroupSuggestionRatingModel.CreateNewGroupSuggestionRatingAsync(
                        groupSuggestionRatingUploadModel.GroupIdentifier,
                        groupSuggestionRatingUploadModel.GroupSuggestionIdentifier,
                        MapUploadModelToEntity<GroupSuggestionRating>(groupSuggestionRatingUploadModel),
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupSuggestionRating, GroupSuggestionRatingDownloadModel>(groupSuggestionRating));
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
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult<GroupSuggestionRatingDownloadModel>> EditGroupSuggestionRatingAsync(
           GroupSuggestionRatingUploadModel groupSuggestionRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupSuggestionRating = await GroupSuggestionRatingModel.EditGroupSuggestionRatingAsync(
                        groupSuggestionRatingUploadModel.GroupSuggestionRatingId,
                        groupSuggestionRatingUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupSuggestionRating, GroupSuggestionRatingDownloadModel>(groupSuggestionRating));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (UserForbiddenException)
            {
                return Forbid();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<GroupSuggestionRatingDownloadModel>> DeleteGroupSuggestionAsync(Guid groupSuggestionRatingId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await GroupSuggestionRatingModel.DeleteGroupSuggestionRatingAsync(
                        groupSuggestionRatingId,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (UserForbiddenException)
            {
                return Forbid();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

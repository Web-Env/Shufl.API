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
    public class GroupPlaylistRatingController : CustomControllerBase
    {
        public GroupPlaylistRatingController(ShuflContext dbContext,
                                             IMapper mapper,
                                             ILogger<GroupPlaylistRatingController> logger) : base(dbContext, logger, mapper) { }

        [HttpPost("Create")]
        public async Task<ActionResult<GroupPlaylistRatingDownloadModel>> CreateGroupPlaylistRatingAsync(
            GroupPlaylistRatingUploadModel groupPlaylistRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupPlaylistRating = await GroupPlaylistRatingModel.CreateNewGroupPlaylistRatingAsync(
                        groupPlaylistRatingUploadModel.GroupIdentifier,
                        groupPlaylistRatingUploadModel.GroupPlaylistIdentifier,
                        MapUploadModelToEntity<GroupPlaylistRating>(groupPlaylistRatingUploadModel),
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupPlaylistRating, GroupPlaylistRatingDownloadModel>(groupPlaylistRating));
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
        public async Task<ActionResult<GroupPlaylistRatingDownloadModel>> EditGroupPlaylistRatingAsync(
           GroupPlaylistRatingUploadModel groupPlaylistRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupPlaylistRating = await GroupPlaylistRatingModel.EditGroupPlaylistRatingAsync(
                        groupPlaylistRatingUploadModel.GroupPlaylistRatingId,
                        groupPlaylistRatingUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupPlaylistRating, GroupPlaylistRatingDownloadModel>(groupPlaylistRating));
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
        public async Task<ActionResult<GroupPlaylistRatingDownloadModel>> DeleteGroupPlaylistAsync(Guid groupPlaylistRatingId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await GroupPlaylistRatingModel.DeleteGroupPlaylistRatingAsync(
                        groupPlaylistRatingId,
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

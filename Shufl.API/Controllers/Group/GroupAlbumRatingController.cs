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
    public class GroupAlbumRatingController : CustomControllerBase
    {
        public GroupAlbumRatingController(ShuflContext dbContext,
                                               IMapper mapper,
                                               ILogger<GroupAlbumRatingController> logger) : base(dbContext, logger, mapper) { }

        [HttpPost("Create")]
        public async Task<ActionResult<GroupAlbumRatingDownloadModel>> CreateGroupAlbumRatingAsync(
            GroupAlbumRatingUploadModel groupAlbumRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupAlbumRating = await GroupAlbumRatingModel.CreateNewGroupAlbumRatingAsync(
                        groupAlbumRatingUploadModel.GroupIdentifier,
                        groupAlbumRatingUploadModel.GroupAlbumIdentifier,
                        MapUploadModelToEntity<GroupAlbumRating>(groupAlbumRatingUploadModel),
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupAlbumRating, GroupAlbumRatingDownloadModel>(groupAlbumRating));
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
        public async Task<ActionResult<GroupAlbumRatingDownloadModel>> EditGroupAlbumRatingAsync(
           GroupAlbumRatingUploadModel groupAlbumRatingUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupAlbumRating = await GroupAlbumRatingModel.EditGroupAlbumRatingAsync(
                        groupAlbumRatingUploadModel.GroupAlbumRatingId,
                        groupAlbumRatingUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupAlbumRating, GroupAlbumRatingDownloadModel>(groupAlbumRating));
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
        public async Task<ActionResult<GroupAlbumRatingDownloadModel>> DeleteGroupAlbumAsync(Guid groupAlbumRatingId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await GroupAlbumRatingModel.DeleteGroupAlbumRatingAsync(
                        groupAlbumRatingId,
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

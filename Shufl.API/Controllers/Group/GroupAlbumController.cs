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
    public class GroupAlbumController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public GroupAlbumController(ShuflContext dbContext,
                                         IMapper mapper,
                                         ILogger<GroupAlbumController> logger,
                                         IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(dbContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GroupAlbumDownloadModel>>> GetAllGroupAlbumsAsync(string groupIdentifier, int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupAlbums = await GroupAlbumModel.GetGroupAlbumsAsync(
                        groupIdentifier,
                        page,
                        pageSize,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntitiesToDownloadModels<GroupAlbum, GroupAlbumDownloadModel>(groupAlbums));
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
        public async Task<ActionResult<GroupAlbumDownloadModel>> GetAllGroupAlbumsAsync(string groupIdentifier, string groupAlbumIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupAlbum = await GroupAlbumModel.GetGroupAlbumAsync(
                        groupIdentifier,
                        groupAlbumIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<GroupAlbum, GroupAlbumDownloadModel>(groupAlbum));
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
        public async Task<ActionResult<string>> CreateGroupAlbumAsync(GroupAlbumUploadModel groupAlbumUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var newGroupAlbumIdentifier = await GroupAlbumModel.CreateNewGroupAlbumAsync(
                        groupAlbumUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager,
                        GetMapper(),
                        _spotifyAPICredentials);

                    return Ok(newGroupAlbumIdentifier);
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

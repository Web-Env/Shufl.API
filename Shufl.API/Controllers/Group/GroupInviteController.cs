using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Models.Group;
using Shufl.Domain.Entities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Group
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class GroupInviteController : CustomControllerBase
    {
        public GroupInviteController(ShuflContext dbContext,
                                     IMapper mapper,
                                     ILogger<GroupInviteController> logger) : base(dbContext, logger, mapper) { }

        [HttpPost("Create")]
        public async Task<ActionResult<string>> CreateGroupInviteAsync(string groupIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupInviteIdentifier = await GroupInviteModel.CreateNewGroupInviteAsync(
                        groupIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(groupInviteIdentifier);
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

        [HttpPost("Join")]
        public async Task<ActionResult<string>> JoinGroupWithInviteAsync(string groupInviteIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    if (string.IsNullOrWhiteSpace(groupInviteIdentifier))
                    {
                        return BadRequest();
                    }

                    var group = await GroupInviteModel.JoinGroupByInviteAsync(
                        groupInviteIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(MapEntityToDownloadModel<Domain.Entities.Group, GroupDownloadModel>(group));
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
            catch (UserAlreadyGroupMemberException err)
            {
                return BadRequest(new UserAlreadyGroupMemberException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("Validate")]
        public async Task<ActionResult<GroupDownloadModel>> ValidateGroupInviteAsync(string groupInviteIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var groupAssociatedWithInvite = await GroupInviteModel.CheckUserGroupInviteValidAsync(
                        groupInviteIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    if (groupAssociatedWithInvite != null)
                    {
                        return Ok(MapEntityToDownloadModel<Domain.Entities.Group, GroupDownloadModel>(groupAssociatedWithInvite));
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
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
            catch (UserAlreadyGroupMemberException err)
            {
                return BadRequest(new UserAlreadyGroupMemberException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

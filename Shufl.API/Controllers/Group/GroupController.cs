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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Group
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class GroupController : CustomControllerBase
    {
        public GroupController(ShuflContext dbContext,
                               IMapper mapper,
                               ILogger<GroupController> logger) : base(dbContext, logger, mapper) { }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<GroupDownloadModel>>> GetAllUsersGroupsAsync() {
            try
            {
                if (await IsUserValidAsync())
                {
                    var usersGroups = await GroupModel.GetAllUsersGroupsAsync(
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    var usersGroupsDownloadModels = MapEntitiesToDownloadModels<Domain.Entities.Group, GroupDownloadModel>(usersGroups);

                    return Ok(usersGroupsDownloadModels);
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

        [HttpGet("Get")]
        public async Task<ActionResult<List<GroupDownloadModel>>> GetUsersGroupsAsync(string groupIdentifier)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var usersGroup = await GroupModel.GetUsersGroupAsync(
                        groupIdentifier,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    var usersGroupsDownloadModels = MapEntityToDownloadModel<Domain.Entities.Group, GroupDownloadModel>(usersGroup);

                    return Ok(usersGroupsDownloadModels);
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
        public async Task<ActionResult<string>> CreateNewGroupAsync(GroupUploadModel groupUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    if (!GroupModel.IsGroupUploadModelValid(groupUploadModel))
                    {
                        return BadRequest();
                    }

                    var groupIdentifier = await GroupModel.CreateNewGroupAsync(
                        groupUploadModel,
                        ExtractUserIdFromToken(),
                        RepositoryManager);

                    return Ok(groupIdentifier);
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

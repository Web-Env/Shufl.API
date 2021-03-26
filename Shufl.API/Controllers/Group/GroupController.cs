using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class GroupController : CustomControllerBase
    {
        public GroupController(ShuflContext dbContext,
                               IMapper mapper,
                               ILogger<GroupController> logger) : base(dbContext, logger, mapper) {}

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

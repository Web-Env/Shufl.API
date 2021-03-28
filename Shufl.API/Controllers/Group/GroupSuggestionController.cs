using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.DownloadModels.Group;
using Shufl.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Group
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class GroupSuggestionController : CustomControllerBase
    {
        public GroupSuggestionController(ShuflContext dbContext,
                               IMapper mapper,
                               ILogger<GroupSuggestionController> logger) : base(dbContext, logger, mapper) { }

        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<GroupSuggestionDownloadModel>>> GetGroupSuggestionsAsync(string groupIdentifier)
        {
            return new List<GroupSuggestionDownloadModel>();
        }
    }
}

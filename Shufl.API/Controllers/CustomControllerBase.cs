using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shufl.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public readonly ILogger Logger;

        public CustomControllerBase(ILogger<CustomControllerBase> logger)
        {
            Logger = logger;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shufl.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected readonly ILogger Logger;

        public CustomControllerBase(ILogger<CustomControllerBase> logger)
        {
            Logger = logger;
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Shufl.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : Controller
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion()
        {
            var version = typeof(Startup).Assembly.GetName().Version.ToString();
            return version;
        }
    }
}

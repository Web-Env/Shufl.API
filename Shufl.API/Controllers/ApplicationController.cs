using Microsoft.AspNetCore.Mvc;
using Shufl.API.Infrastructure.Encryption;

namespace Shufl.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion()
        {
            var version = typeof(Startup).Assembly.GetName().Version.ToString();
            return version;
        }

        //Remove
        [HttpPost("Encrypt")]
        public ActionResult<string> Encrypt(string plainText)
        {
            var encryptedString = EncryptionService.EncryptString(plainText);
            return encryptedString;
        }

        //Remove
        [HttpPost("Decrypt")]
        public ActionResult<string> Decrypt(string encryptedString)
        {
            var plainText = DecryptionService.DecryptString(encryptedString);
            return plainText;
        }
    }
}

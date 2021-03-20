using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Services.Auth;
using Shufl.API.UploadModels.Auth;
using Shufl.Domain.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.User
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        public AuthController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              ILogger<AuthController> logger,
                              AuthenticationService authenticationService) : base(repositoryManager, logger, mapper) 
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest authenticationRequest)
        {
            try
            {
                var response = await _authenticationService.AuthenticateAsync(authenticationRequest, RepositoryManager.UserRepository);

                return Ok(response);
            }
            catch (AuthenticationException authException)
            {
                return BadRequest(authException.ErrorMessage);
            }
        }

        [HttpPost("validate")]
        public IActionResult Validate()
        {
            return Ok();
        }
    }
}

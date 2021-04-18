using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.DownloadModels.Auth;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Services.Auth;
using Shufl.API.UploadModels.Auth;
using Shufl.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.User
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        public AuthController(ShuflContext dbContext,
                              IMapper mapper,
                              ILogger<AuthController> logger,
                              AuthenticationService authenticationService) : base(dbContext, logger, mapper) 
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Auth")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate(AuthenticationRequest authenticationRequest)
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
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("Validate")]
        public async Task<IActionResult> Validate()
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    return Ok();
                }

                return Unauthorized();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

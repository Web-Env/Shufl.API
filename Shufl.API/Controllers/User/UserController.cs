using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Models.User;
using Shufl.API.UploadModels.User;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using WebEnv.Util.Mailer.Settings;

namespace Shufl.API.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private readonly SmtpSettings _smtpSettings;
        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              IOptions<SmtpSettings> smtpSettings,
                              ILogger<UserController> logger) : base(repositoryManager, logger, mapper)
        {
            _smtpSettings = smtpSettings.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(UserUploadModel user)
        {
            try
            {
                var newUser = MapUploadModelToEntity<Domain.Entities.User>(user);
                await UserModel.CreateNewUserAsync(
                    newUser,
                    ExtractRequesterAddress(Request),
                    RepositoryManager,
                    _smtpSettings
                    );

                return Ok();
            }
            catch (EmailAlreadyRegisteredException err)
            {
                return BadRequest(new EmailAlreadyRegisteredException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                throw;
            }
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyUser(string verificationIdentifier)
        {
            try
            {
                await UserModel.VerifyUserAsync(
                    verificationIdentifier,
                    ExtractRequesterAddress(Request),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return BadRequest();
            }
        }

        [HttpPost("Verify/Validate")]
        public async Task<IActionResult> ValidateVerificationIdentifier(string verificationIdentifier)
        {
            try
            {
                var verificationIdentifierIsValid = await UserModel.ValidateVerificationIdentifierAsync(
                    verificationIdentifier,
                    RepositoryManager.UserVerificationRepository);

                if (verificationIdentifierIsValid)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return BadRequest();
            }
        }

        [HttpPost("Verify/New")]
        public async Task<IActionResult> CreateNewVerification(string emailAddress)
        {
            try
            {
                await UserModel.CreateNewVerficationAsync(
                    emailAddress,
                    ExtractRequesterAddress(Request),
                    RepositoryManager,
                    _smtpSettings);

                return Ok();
            }
            catch (UserAlreadyVerifiedException err)
            {
                return BadRequest(new UserAlreadyVerifiedException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return BadRequest();
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ResetPassword(string resetIdentifier, string newPassword)
        {
            try
            {
                await UserModel.ResetPasswordAsync(
                    resetIdentifier,
                    newPassword,
                    ExtractRequesterAddress(Request),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return BadRequest();
            }
        }

        [HttpPost("ForgotPassword/New")]
        public async Task<IActionResult> CreateNewResetPassword(string emailAddress)
        {
            try
            {
                await UserModel.CreateNewResetPasswordAsync(
                    emailAddress,
                    ExtractRequesterAddress(Request),
                    RepositoryManager,
                    _smtpSettings);

                return Ok();
            }
            catch(Exception err)
            {
                LogException(err);

                return BadRequest();
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.User;
using Shufl.API.UploadModels.User;
using Shufl.Domain.Entities;
using System;
using System.Threading.Tasks;
using WebEnv.Util.Mailer.Settings;

namespace Shufl.API.Controllers.User
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;
        public UserController(ShuflContext shuflContext,
                              IMapper mapper,
                              IOptions<SmtpSettings> smtpSettings,
                              IOptions<EmailSettings> emailSettings,
                              ILogger<UserController> logger) : base(shuflContext, logger, mapper)
        {
            _smtpSettings = smtpSettings.Value;
            _emailSettings = emailSettings.Value;
        }

        [HttpGet("CheckUsernameUnique")]
        public async Task<IActionResult> CheckUsernameUnique(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
            {
                return new BadRequestObjectResult("Username must be at least 4 characters");
            }

            try
            {
                var usernameUnique = await UserModel.CheckUsernameUniqueAsync(username, RepositoryManager.UserRepository);

                return Ok(usernameUnique);
            }
            catch(Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(UserUploadModel user)
        {
            try
            {
                var newUser = MapUploadModelToEntity<Domain.Entities.User>(user);
                await UserModel.CreateNewUserAsync(
                    newUser,
                    ExtractRequesterAddress(),
                    RepositoryManager,
                    _smtpSettings,
                    _emailSettings
                    );

                return Ok();
            }
            catch (EmailAlreadyRegisteredException err)
            {
                return BadRequest(new EmailAlreadyRegisteredException(err.ErrorMessage, err.ErrorData));
            }
            catch (UsernameAlreadyRegisteredException err)
            {
                return BadRequest(new UsernameAlreadyRegisteredException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyUser(string verificationIdentifier)
        {
            try
            {
                await UserModel.VerifyUserAsync(
                    verificationIdentifier,
                    ExtractRequesterAddress(),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("Verify/Validate")]
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
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Verify/New")]
        public async Task<IActionResult> CreateNewVerification(string email)
        {
            try
            {
                await UserModel.CreateNewVerficationAsync(
                    email,
                    ExtractRequesterAddress(),
                    RepositoryManager,
                    _smtpSettings,
                    _emailSettings);

                return Ok();
            }
            catch (UserAlreadyVerifiedException err)
            {
                return BadRequest(new UserAlreadyVerifiedException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ResetPassword(PasswordResetUploadModel passwordResetUploadModel)
        {
            try
            {
                await UserModel.ResetPasswordAsync(
                    passwordResetUploadModel.PasswordResetToken,
                    passwordResetUploadModel.NewPassword,
                    ExtractRequesterAddress(),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("ForgotPassword/Validate")]
        public async Task<IActionResult> ValidatePasswordResetToken(string passwordResetToken)
        {
            try
            {
                var passwordResetTokenIsValid = await UserModel.ValidatePasswordResetTokenAsync(
                    passwordResetToken,
                    RepositoryManager.PasswordResetRepository);

                if (passwordResetTokenIsValid)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("ForgotPassword/New")]
        public async Task<IActionResult> CreateNewResetPassword(PasswordResetRequestUploadModel passwordResetRequestUploadModel)
        {
            try
            {
                await UserModel.CreateNewResetPasswordAsync(
                    passwordResetRequestUploadModel.Email,
                    ExtractRequesterAddress(),
                    RepositoryManager,
                    _smtpSettings,
                    _emailSettings);

                return Ok();
            }
            catch(Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}

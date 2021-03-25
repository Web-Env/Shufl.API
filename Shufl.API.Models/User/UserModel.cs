using Shufl.API.Infrastructure.Emails;
using Shufl.API.Infrastructure.Emails.ViewModels;
using Shufl.API.Infrastructure.Encryption;
using Shufl.API.Infrastructure.Encryption.Helpers;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using Shufl.Domain.Repositories.User.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebEnv.Util.Mailer;
using WebEnv.Util.Mailer.Settings;

namespace Shufl.API.Models.User
{
    public static class UserModel
    {
        public static async Task<bool> CheckUsernameUniqueAsync(string username, IUserRepository userRepository)
        {
            var userWithUsername = await userRepository.FindByUsernameAsync(username);
            return userWithUsername == null;
        }

        public static async Task CreateNewUserAsync(
            Domain.Entities.User user,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings)
        {
            var (emailExists, _) = await CheckUserExistsWithEmailAsync(user.Email, repositoryManager.UserRepository)
                .ConfigureAwait(false);
            var (usernameExists, _) = await CheckUserExistsWithUsernameAsync(user.Username, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (emailExists)
            {
                throw (new EmailAlreadyRegisteredException(
                    "A User with this email address already exists",
                    "A User with this email address already exists"
                ));
            }
            if (usernameExists)
            {
                throw (new UsernameAlreadyRegisteredException(
                    "A User with this username already exists",
                    "A User with this username already exists"
                ));
            }

            user.DisplayName = $"{user.FirstName} {user.LastName}";
            user.Password = HashingHelper.HashPassword(user.Password);
            user.CreatedOn = DateTime.Now;

            try
            {
                await repositoryManager.UserRepository.AddAsync(user);

                await CreateNewVerficationAsync(
                    user.Email,
                    requesterAddress,
                    repositoryManager,
                    smtpSettings,
                    isFirstContact: true).ConfigureAwait(false);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<(bool exists, Domain.Entities.User user)> CheckUserExistsWithEmailAsync(
            string email,
            IUserRepository userRepository)
        {
            var existingEmail = await userRepository.FindAsync(u => u.Email == email);
            if (existingEmail.Any())
            {
                return (true, existingEmail.FirstOrDefault());
            }

            return (false, null);
        }

        private static async Task<(bool exists, Domain.Entities.User user)> CheckUserExistsWithUsernameAsync(
            string username,
            IUserRepository userRepository)
        {
            var existingUsername = await userRepository.FindByUsernameAsync(username);
            if (existingUsername != null)
            {
                return (true, existingUsername);
            }

            return (false, null);
        }

        public static async Task VerifyUserAsync(
            string verificationIdentifier,
            string requesterAddress,
            IRepositoryManager repositoryManager)
        {
            verificationIdentifier = HashingHelper.HashIdentifier(verificationIdentifier);
            var verificationIdentifierIsValid = await ValidateVerificationIdentifierAsync(
                    verificationIdentifier,
                    repositoryManager.UserVerificationRepository).ConfigureAwait(false);

            if (verificationIdentifierIsValid)
            {
                var userVerification = await repositoryManager.UserVerificationRepository.FindByIdentifierAsync(verificationIdentifier);
                var user = await repositoryManager.UserRepository.GetByIdAsync(userVerification.UserId);

                userVerification.Active = false;
                userVerification.UsedOn = DateTime.Now;
                userVerification.UsedByAddress = requesterAddress;
                user.IsVerified = true;

                await repositoryManager.UserVerificationRepository.UpdateAsync(userVerification);
                await repositoryManager.UserRepository.UpdateAsync(user);
            }
            else
            {
                throw new InvalidTokenException(InvalidTokenType.NoTokenFound, "The User Verification Identifier is invalid");
            }
        }

        public static async Task<bool> ValidateVerificationIdentifierAsync(
            string verificationIdentifier,
            IUserVerificationRepository userVerificationRepository)
        {
            var userVerification = await userVerificationRepository.FindByIdentifierAsync(
                verificationIdentifier);

            if (userVerification != null)
            {
                return IsUserVerificationValid(userVerification);
            }

            return false;
        }

        private static bool IsUserVerificationValid(UserVerification userVerification)
        {
            if (userVerification.ExpiryDate < DateTime.Now)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenExpired, "Token has expired");
            }

            if (userVerification.UsedOn != null || userVerification.UsedByAddress != null)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenUsed, "Token has been used");
            }

            if (userVerification.Active != null && !(bool)userVerification.Active)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenInactive, "Token is no longer active");
            }

            return true;
        }

        public static async Task CreateNewVerficationAsync(
            string email,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings,
            bool isFirstContact = false
            )
        {
            var (exists, user) = await CheckUserExistsWithEmailAsync(email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (user.IsVerified)
            {
                throw new UserAlreadyVerifiedException("User has already been verified", "User has already been verified");
            }

            if (!isFirstContact)
            {
                await DeactivateExistingUserVerificationsAsync(user.Id, repositoryManager.UserVerificationRepository)
                    .ConfigureAwait(false);
            }

            if (exists)
            {
                try
                {
                    var emailService = new EmailService();
                    var verificationIdentifier = ModelHelpers.GenerateUniqueIdentifier();
                    var hashedVerificationIdentifier = HashingHelper.HashIdentifier(verificationIdentifier);

                    var verification = new UserVerification
                    {
                        Identifier = hashedVerificationIdentifier,
                        UserId = user.Id,
                        ExpiryDate = DateTime.Now.AddDays(7),
                        RequesterAddress = requesterAddress
                    };
                    var verificationViewModel = new LinkEmailViewModel
                    {
                        FullName = $"{user.FirstName} {user.LastName}",
                        Link = verificationIdentifier
                    };

                    await repositoryManager.UserVerificationRepository.AddAsync(verification);

                    var verificationMessage = emailService.CreateHtmlMessage(
                        smtpSettings,
                        $"{user.FirstName} {user.LastName}",
                        user.Email,
                        isFirstContact ? "Welcome to Shufl" : "Verify Your Account",
                        isFirstContact ?
                            EmailCreationHelper.CreateWelcomeVerificationEmailString(verificationViewModel) :
                            EmailCreationHelper.CreateVerificationEmailString(verificationViewModel));

                    await emailService.SendEmailAsync(smtpSettings, verificationMessage);

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static async Task DeactivateExistingUserVerificationsAsync(Guid userId, IUserVerificationRepository verificationRepository)
        {
            var userVerifications = await verificationRepository.FindAsync(uv => uv.UserId == userId && (bool)uv.Active);

            if (userVerifications.Any())
            {
                foreach (UserVerification userVerification in userVerifications)
                {
                    userVerification.Active = false;
                    userVerification.LastUpdatedOn = DateTime.Now;
                }

                await verificationRepository.UpdateRangeAsync(userVerifications);
            }
        }

        public static async Task ResetPasswordAsync(
            string resetIdentifier,
            string newPassword,
            string requesterAddress,
            IRepositoryManager repositoryManager)
        {
            var decryptedResetIdentifier = DecryptionService.DecryptString(resetIdentifier);
            var hashedResetIdentifier = HashingHelper.HashIdentifier(decryptedResetIdentifier);
            var (existsAndValid, passwordReset) = await CheckPasswordResetIdentifierExistsAndIsValidAsync(
                hashedResetIdentifier,
                repositoryManager.PasswordResetRepository).ConfigureAwait(false);

            if (existsAndValid)
            {
                var userId = Guid.Parse(DecryptionService.DecryptString(passwordReset.UserId));
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                user.Password = HashingHelper.HashPassword(newPassword);
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedBy = userId;

                passwordReset.Active = false;
                passwordReset.UsedOn = DateTime.Now;
                passwordReset.UsedByAddress = requesterAddress;
                passwordReset.LastUpdatedOn = DateTime.Now;

                await repositoryManager.UserRepository.UpdateAsync(user);
                await repositoryManager.PasswordResetRepository.UpdateAsync(passwordReset);
            }
            else
            {
                throw new InvalidTokenException(InvalidTokenType.NoTokenFound, "The Password Reset Token is invalid");
            }
        }

        private static async Task<(bool existsAndValid, PasswordReset passwordReset)> CheckPasswordResetIdentifierExistsAndIsValidAsync(
            string resetIdentifier,
            IPasswordResetRepository passwordResetRepository)
        {
            var passwordReset = await passwordResetRepository.FetchByIdentifier(resetIdentifier);

            if (passwordReset != null)
            {
                if (IsPasswordResetValid(passwordReset))
                {
                    return (true, passwordReset);
                }
            }

            return (false, null);
        }

        private static bool IsPasswordResetValid(PasswordReset passwordReset)
        {
            if (passwordReset.ExpiryDate < DateTime.Now)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenExpired, "Token has expired");
            }

            if (passwordReset.UsedOn != null || passwordReset.UsedByAddress != null)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenUsed, "Token has been used");
            }

            if (passwordReset.Active != null && !(bool)passwordReset.Active)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenInactive, "Token is no longer active");
            }

            return true;
        }

        public static async Task CreateNewResetPasswordAsync(
            string email,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings)
        {
            var (exists, user) = await CheckUserExistsWithEmailAsync(email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (exists)
            {
                try
                {
                    var emailService = new EmailService();
                    var resetIdentifier = ModelHelpers.GenerateUniqueIdentifier();
                    var hashedResetIdentifier = HashingHelper.HashIdentifier(resetIdentifier);
                    var encryptedUserId = EncryptionService.EncryptString(user.Id.ToString());
                    var encryptedIdentifier = EncryptionService.EncryptString(resetIdentifier);

                    var passwordReset = new PasswordReset
                    {
                        Identifier = hashedResetIdentifier,
                        UserId = encryptedUserId,
                        ExpiryDate = DateTime.Now.AddHours(1),
                        RequesterAddress = requesterAddress
                    };
                    var verificationViewModel = new LinkEmailViewModel
                    {
                        FullName = $"{user.FirstName} {user.LastName}",
                        Link = encryptedIdentifier
                    };

                    await repositoryManager.PasswordResetRepository.AddAsync(passwordReset);

                    var verificationMessage = emailService.CreateHtmlMessage(
                        smtpSettings,
                        $"{user.FirstName} {user.LastName}",
                        user.Email,
                        "Reset Your Password",
                        EmailCreationHelper.CreatePasswordResetEmailString(verificationViewModel));

                    await emailService.SendEmailAsync(smtpSettings, verificationMessage);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

﻿using Microsoft.IdentityModel.Tokens;
using Shufl.API.DownloadModels.Auth;
using Shufl.API.Infrastructure.Encryption;
using Shufl.API.Infrastructure.Encryption.Certificates;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.UploadModels.Auth;
using Shufl.Domain.Repositories.User.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shufl.API.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest model, IUserRepository userRepository)
        {
            var user = (await userRepository.FindAsync(u =>
                            u.Email == model.Email
                        )).FirstOrDefault();

            if (user != null)
            {
                var passwordIsCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
                if (passwordIsCorrect)
                {
                    var authenticationResponse = new AuthenticationResponse
                    {
                        Token = GenerateJwtToken(user.Id.ToString(), user.UserSecret),
                        Username = user.Username,
                        DisplayName = user.DisplayName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        SpotifyUsername = user.SpotifyUsername,
                        SpotifyMarket = user.SpotifyMarket
                    };

                    return authenticationResponse;
                }
                else
                {
                    throw new AuthenticationException
                    (
                        "Invalid login",
                        "No user found with the provided login details"
                    );
                }
            }
            else
            {
                throw new AuthenticationException
                (
                    "Invalid login",
                    "No user found with the provided login details"
                );
            }
        }

        private string GenerateJwtToken(string userId, string userSecret)
        {
            var decryptedUserSecret = DecryptionService.DecryptString(userSecret);

            SecurityTokenDescriptor tokenDescriptor = GetTokenDescriptor(userId, decryptedUserSecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        private SecurityTokenDescriptor GetTokenDescriptor(string userId, string userSecret)
        {
            const int expiringDays = 365;
            var signingAudienceCertificate = new SigningAudienceCertificate();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.NameIdentifier, userSecret)
                }),
                Expires = DateTime.UtcNow.AddDays(expiringDays),
                SigningCredentials = signingAudienceCertificate.GetAudienceSigningKey()
            };

            return tokenDescriptor;
        }
    }
}

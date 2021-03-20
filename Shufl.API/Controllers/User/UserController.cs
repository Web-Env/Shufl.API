using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.Infrastructure.Encryption.Helpers;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.UploadModels.User;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              ILogger<UserController> logger) : base(repositoryManager, logger, mapper) { }

        [HttpPost]
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            var existingEmail = await RepositoryManager.UserRepository.FindAsync(u => u.Email == user.Email);
            if (existingEmail.Any())
            {
                return BadRequest(
                    new EmailAlreadyRegisteredException(
                        "A User with this email address already exists",
                        "A User with this email address already exists"
                    )
                );
            }

            var newUser = MapUploadModelToEntity<Domain.Entities.User>(user);
            newUser.Password = PasswordHashingHelper.HashPassword(newUser.Password);
            newUser.CreatedOn = DateTime.Now;

            try
            {
                var registeredUser = await RepositoryManager.UserRepository.AddAsync(newUser);

                return Ok();
            }
            catch(Exception err)
            {
                LogException(err);
                throw;
            }
        }
    }
}

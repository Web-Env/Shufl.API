using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shufl.API.Infrastructure.Encryption;
using Shufl.API.Models.User;
using Shufl.API.UploadModels;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shufl.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected readonly IRepositoryManager RepositoryManager;
        protected readonly ILogger Logger;
        private readonly IMapper _mapper;

        public CustomControllerBase(ShuflContext shuflContext,
                                    ILogger<CustomControllerBase> logger,
                                    IMapper mapper)
        {
            RepositoryManager = new RepositoryManager(shuflContext);
            Logger = logger;
            _mapper = mapper;
        }

        protected TDownloadModel MapEntityToDownloadModel<TEntity, TDownloadModel>(TEntity entity)
        {
            return _mapper.Map<TDownloadModel>(entity);
        }

        protected List<TDownloadModel> MapEntitiesToDownloadModels<TEntity, TDownloadModel>(IEnumerable<TEntity> entity)
        {
            return _mapper.Map<IEnumerable<TEntity>, List<TDownloadModel>>(entity);
        }

        protected TEntity MapUploadModelToEntity<TEntity>(IUploadModel uploadModel)
        {
            return _mapper.Map<TEntity>(uploadModel);
        }

        protected IMapper GetMapper()
        {
            return _mapper;
        }

        protected Guid ExtractUserIdFromToken()
        {
            var userIdString = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = Guid.Parse(userIdString);

            return userId;
        }

        protected string ExtractUserSecretFromToken()
        {
            var userSecret = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userSecret;
        }

        protected string ExtractRequesterAddress()
        {
            var requesterAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            
            return requesterAddress;
        }

        protected async Task<bool> IsUserValidAsync()
        {
            var userIsValid = await UserModel.CheckUserExistsByIdAsync(
                ExtractUserIdFromToken(), RepositoryManager.UserRepository);

            if (userIsValid)
            {
                var user = await UserModel.GetUserByIdAsync(
                    ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                if (user.UserSecret != null)
                {
                    var decryptedUserSecret = DecryptionService.DecryptString(user.UserSecret);

                    if (decryptedUserSecret == ExtractUserSecretFromToken())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return userIsValid;
            }
        }

        protected void LogException(Exception exception)
        {
            Logger.LogError(exception, exception.Message);
        }
    }
}

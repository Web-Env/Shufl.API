using AutoMapper;
using Shufl.API.UploadModels.User;
using Shufl.Domain.Entities;

namespace Shufl.API.Infrastructure.Mappers
{
    public class UploadModelToEntity : Profile
    {
        public UploadModelToEntity()
        {
            CreateMap<UserUploadModel, User>();
        }
    }
}

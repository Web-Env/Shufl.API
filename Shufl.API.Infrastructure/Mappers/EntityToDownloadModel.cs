using AutoMapper;
using Shufl.API.DownloadModels.User;
using Shufl.Domain.Entities;

namespace Shufl.API.Infrastructure.Mappers
{
    public class EntityToDownloadModel : Profile
    {
        public EntityToDownloadModel()
        {
            CreateMap<User, UserDownloadModel>();
        }
    }
}

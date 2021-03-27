using AutoMapper;
using Shufl.API.DownloadModels.Group;
using Shufl.API.DownloadModels.User;
using Shufl.Domain.Entities;

namespace Shufl.API.Infrastructure.Mappers
{
    public class EntityToDownloadModelProfile : Profile
    {
        public EntityToDownloadModelProfile()
        {
            CreateMap<User, UserDownloadModel>();

            CreateMap<Group, GroupDownloadModel>()
                .ForMember(dest => dest.Members, src => src.MapFrom(g => g.GroupMembers))
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(g => g.CreatedByNavigation));

            CreateMap<GroupMember, GroupMemberDownloadModel>()
                .ForMember(dest => dest.User, src => src.MapFrom(gm => gm.User));
        }
    }
}

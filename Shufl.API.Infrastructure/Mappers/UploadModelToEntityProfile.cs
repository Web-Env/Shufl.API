using AutoMapper;
using Shufl.API.Infrastructure.Mappers.Converters;
using Shufl.API.UploadModels.Group;
using Shufl.API.UploadModels.User;
using Shufl.Domain.Entities;

namespace Shufl.API.Infrastructure.Mappers
{
    public class UploadModelToEntityProfile : Profile
    {
        public UploadModelToEntityProfile()
        {
            CreateMap<UserUploadModel, User>()
                .ForMember(dest => dest.Username, src => src.ConvertUsing(new LowerCaseConverter()));

            CreateMap<GroupSuggestionRatingUploadModel, GroupSuggestionRating>();

            CreateMap<GroupPlaylistRatingUploadModel, GroupPlaylistRating>();
        }
    }
}

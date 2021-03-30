using AutoMapper;
using Shufl.API.Infrastructure.Mappers.Converters;
using Shufl.Domain.Entities;
using SpotifyAPI.Web;

namespace Shufl.API.Infrastructure.Mappers
{
    public class SpotifyApiModelToEntityProfile : Profile
    {
        public SpotifyApiModelToEntityProfile()
        {
            CreateMap<Image, ArtistImage>()
                .ForMember(dest => dest.Uri, src => src.ConvertUsing(new ImageUrlConverter(), src => src.Url))
                .ForMember(dest => dest.Width, src => src.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, src => src.MapFrom(src => src.Height));

            CreateMap<Image, AlbumImage>()
                .ForMember(dest => dest.Uri, src => src.ConvertUsing(new ImageUrlConverter(), src => src.Url))
                .ForMember(dest => dest.Width, src => src.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, src => src.MapFrom(src => src.Height));
        }
    }
}

using AutoMapper;
using Shufl.API.DownloadModels.Music;
using Shufl.API.DownloadModels.Group;
using Shufl.API.DownloadModels.User;
using Shufl.Domain.Entities;
using System.Linq;

namespace Shufl.API.Infrastructure.Mappers
{
    public class EntityToDownloadModelProfile : Profile
    {
        public EntityToDownloadModelProfile()
        {
            CreateMap<Artist, ArtistDownloadModel>()
                   .ForMember(dest => dest.Id, src => src.MapFrom(a => a.SpotifyId))
                   .ForMember(dest => dest.Name, src => src.MapFrom(a => a.Name))
                   .ForMember(dest => dest.Followers, src => src.Ignore())
                   .ForMember(dest => dest.Albums, src => src.MapFrom(a => a.AlbumArtists.Select(aa => aa.Album)))
                   .ForMember(dest => dest.ArtistGenres, src => src.MapFrom(a => a.ArtistGenres.Select(ag => ag.Genre)));

            CreateMap<Genre, ArtistGenreDownloadModel>();

            CreateMap<ArtistImage, ImageDownloadModel>();

            CreateMap<Album, AlbumDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(a => a.SpotifyId))
                .ForMember(dest => dest.Name, src => src.MapFrom(a => a.Name))
                .ForMember(dest => dest.ReleaseDate, src => src.MapFrom(a => a.ReleaseDate))
                .ForMember(dest => dest.Artists, src => src.MapFrom(a => a.AlbumArtists.Select(aa => aa.Artist)))
                .ForMember(dest => dest.Tracks, src => src.MapFrom(a => a.Tracks));

            CreateMap<AlbumImage, ImageDownloadModel>();

            CreateMap<Track, TrackDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(t => t.SpotifyId))
                .ForMember(dest => dest.Name, src => src.MapFrom(t => t.Name))
                .ForMember(dest => dest.TrackNumber, src => src.MapFrom(t => t.TrackNumber))
                .ForMember(dest => dest.Duration, src => src.MapFrom(t => t.Duration))
                .ForMember(dest => dest.Artists, src => src.MapFrom(t => t.TrackArtists.Select(ta => ta.Artist)));

            CreateMap<User, UserDownloadModel>();

            CreateMap<Group, GroupDownloadModel>()
                .ForMember(dest => dest.Members, src => src.MapFrom(g => g.GroupMembers))
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(g => g.CreatedByNavigation));

            CreateMap<GroupMember, GroupMemberDownloadModel>()
                .ForMember(dest => dest.User, src => src.MapFrom(gm => gm.User));

            CreateMap<GroupSuggestion, GroupSuggestionDownloadModel>()
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(g => g.CreatedByNavigation));

            CreateMap<GroupSuggestionRating, GroupSuggestionRatingDownloadModel>()
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(g => g.CreatedByNavigation));


        }
    }
}

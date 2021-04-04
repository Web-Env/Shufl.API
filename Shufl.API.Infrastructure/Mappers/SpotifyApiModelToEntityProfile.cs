using AutoMapper;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.Helpers;
using Shufl.API.Infrastructure.Mappers.Converters;
using Shufl.API.Infrastructure.SearchResponseModels;
using Shufl.Domain.Entities;
using SpotifyAPI.Web;

namespace Shufl.API.Infrastructure.Mappers
{
    public class SpotifyApiModelToEntityProfile : Profile
    {
        public SpotifyApiModelToEntityProfile()
        {
            CreateMap<FullArtist, ArtistDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.Followers, src => src.MapFrom(src => src.Followers.Total))
                .ForMember(dest => dest.ArtistImages, src => src.MapFrom(src => src.Images))
                .ForMember(dest => dest.ArtistGenres, opt => opt.ConvertUsing(new GenreConverter(), src => src.Genres));

            CreateMap<SimpleArtist, ArtistDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name));

            CreateMap<FullAlbum, AlbumDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.ReleaseDate, src => src.MapFrom(src => ReleaseDateParsingHelper.ParseReleaseDateToDateTime(src.ReleaseDate, src.ReleaseDatePrecision)))
                .ForMember(dest => dest.AlbumImages, src => src.MapFrom(src => src.Images))
                .ForMember(dest => dest.Artists, src => src.MapFrom(src => src.Artists))
                .ForMember(dest => dest.Tracks, src => src.MapFrom(src => src.Tracks.Items));

            CreateMap<AlbumResponseModel, AlbumDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Album.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Album.Name))
                .ForMember(dest => dest.ReleaseDate, src => src.MapFrom(src => ReleaseDateParsingHelper.ParseReleaseDateToDateTime(src.Album.ReleaseDate, src.Album.ReleaseDatePrecision)))
                .ForMember(dest => dest.AlbumImages, src => src.MapFrom(src => src.Album.Images))
                .ForMember(dest => dest.Artists, src => src.MapFrom(src => src.Artists))
                .ForMember(dest => dest.Tracks, src => src.MapFrom(src => src.Album.Tracks.Items));

            CreateMap<SimpleAlbum, AlbumDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.ReleaseDate, src => src.MapFrom(src => ReleaseDateParsingHelper.ParseReleaseDateToDateTime(src.ReleaseDate, src.ReleaseDatePrecision)))
                .ForMember(dest => dest.AlbumImages, src => src.MapFrom(src => src.Images))
                .ForMember(dest => dest.Artists, src => src.MapFrom(src => src.Artists));

            CreateMap<FullTrack, TrackDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.TrackNumber, src => src.MapFrom(src => src.TrackNumber))
                .ForMember(dest => dest.Duration, src => src.MapFrom(src => src.DurationMs))
                .ForMember(dest => dest.Artists, src => src.MapFrom(src => src.Artists));

            CreateMap<SimpleTrack, TrackDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(src => src.Name))
                .ForMember(dest => dest.TrackNumber, src => src.MapFrom(src => src.TrackNumber))
                .ForMember(dest => dest.Duration, src => src.MapFrom(src => src.DurationMs))
                .ForMember(dest => dest.Artists, src => src.MapFrom(src => src.Artists));

            CreateMap<Image, ImageDownloadModel>()
                .ForMember(dest => dest.Uri, src => src.ConvertUsing(new ImageUrlConverter(), src => src.Url))
                .ForMember(dest => dest.Width, src => src.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, src => src.MapFrom(src => src.Height));

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

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Shufl.API.Infrastructure.Mappers;

namespace Shufl.API.Infrastructure.Extensions
{
    public static class MappersConfig
    {
        public static void AddCustomMappers(this IServiceCollection services)
        {
            var mappersConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new EntityToDownloadModelProfile());
                cfg.AddProfile(new UploadModelToEntityProfile());
                cfg.AddProfile(new SpotifyApiModelToEntityProfile());
            });

            var mapper = mappersConfig.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}

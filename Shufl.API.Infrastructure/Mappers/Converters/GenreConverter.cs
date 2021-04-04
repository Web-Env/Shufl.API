using AutoMapper;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.Helpers;
using System.Collections.Generic;

namespace Shufl.API.Infrastructure.Mappers.Converters
{
    public class GenreConverter : IValueConverter<List<string>, List<ArtistGenreDownloadModel>>
    {
        public List<ArtistGenreDownloadModel> Convert(List<string> genres, ResolutionContext resolutionContext)
        {
            var mappedGenres = new List<ArtistGenreDownloadModel>();

            foreach (var genre in genres)
            {
                var formattedGenre = GenreFormattingHelper.CreateNewGenre(genre, genre.Replace(" ", ""));

                mappedGenres.Add(new ArtistGenreDownloadModel
                {
                    Name = formattedGenre.Name,
                    Code = formattedGenre.Code
                });
            }

            return mappedGenres;
        }
    }
}

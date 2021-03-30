using AutoMapper;
using System.Linq;

namespace Shufl.API.Infrastructure.Mappers.Converters
{
    public class ImageUrlConverter : IValueConverter<string, string>
    {
        public string Convert(string inputString, ResolutionContext resolutionContext)
        {
            return inputString?.Split("/").Last();
        }
    }
}

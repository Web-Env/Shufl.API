using AutoMapper;

namespace Shufl.API.Infrastructure.Mappers.Converters
{
    public class LowerCaseConverter : IValueConverter<string, string>
    {
        public string Convert(string inputString, ResolutionContext resolutionContext)
        {
            return inputString?.ToLower();
        }
    }
}

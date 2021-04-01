using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shufl.API.Infrastructure.Helpers
{
    public static class ReleaseDateParsingHelper
    {
        public static DateTime ParseReleaseDateToDateTime(string releaseDate, string releaseDatePrecision)
        {
            if (releaseDatePrecision == "year")
            {
                return DateTime.ParseExact(releaseDate, "yyyy", CultureInfo.InvariantCulture);
            }
            else if (releaseDatePrecision == "month")
            {
                return DateTime.ParseExact(releaseDate, "yyyy-MM", CultureInfo.InvariantCulture);
            }
            else if (releaseDatePrecision == "day")
            {
                return DateTime.Parse(releaseDate);
            }
            else
            {
                return DateTime.Parse(releaseDate);
            }
        }
    }
}

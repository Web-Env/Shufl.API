using Shufl.API.DownloadModels.Music;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.User
{
    public class UserDownloadModel
    {
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SpotifyUsername { get; set; }

        public string SpotifyMarket { get; set; }

        public IEnumerable<ImageDownloadModel> UserImages { get; set; }
    }
}

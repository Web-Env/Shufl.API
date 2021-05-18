using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Music
{
    public class PlaylistDownloadModel
    {
        public string Id { get; set; }

        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public IEnumerable<ImageDownloadModel> PlaylistImages { get; set; }
    }
}

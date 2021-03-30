using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Music
{
    public class TrackDownloadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int TrackNumber { get; set; }

        public int Duration { get; set; }

        public IEnumerable<ArtistDownloadModel> Artists { get; set; }
    }
}

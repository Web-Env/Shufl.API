using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Music
{
    public class ArtistDownloadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Followers { get; set; }

        public IEnumerable<ImageDownloadModel> ArtistImages { get; set; }

        public IEnumerable<ArtistGenreDownloadModel> ArtistGenres { get; set; }

        public IEnumerable<AlbumDownloadModel> Albums { get; set; }
    }
}

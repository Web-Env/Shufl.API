using Shufl.API.DownloadModels.Group;
using System;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Music
{
    public class AlbumDownloadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IEnumerable<ImageDownloadModel> AlbumImages { get; set; }

        public IEnumerable<ArtistDownloadModel> Artists { get; set; }

        public IEnumerable<TrackDownloadModel> Tracks { get; set; }

        public GroupAlbumDownloadModel? RelatedGroupAlbum { get; set; }
    }
}

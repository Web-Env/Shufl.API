using SpotifyAPI.Web;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Artist
{
    public class ArtistDownloadModel
    {
        public FullArtist Artist { get; set; }

        public List<SimpleAlbum> Albums { get; set; }
    }
}

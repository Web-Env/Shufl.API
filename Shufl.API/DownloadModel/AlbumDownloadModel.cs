using SpotifyAPI.Web;
using System.Collections.Generic;

namespace Shufl.API.DownloadModel
{
    public class AlbumDownloadModel
    {
        public List<string> Genres { get; set; }

        public FullAlbum Album { get; set; }
    }
}

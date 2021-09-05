using Shufl.Domain.Entities;
using SpotifyAPI.Web;
using System.Collections.Generic;

namespace Shufl.API.Infrastructure.SearchResponseModels
{
    public class AlbumResponseModel
    {
        public FullAlbum Album { get; set; }

        public IEnumerable<FullArtist> Artists { get; set; }

        public GroupAlbum? RelatedGroupAlbum { get; set; }
    }
}

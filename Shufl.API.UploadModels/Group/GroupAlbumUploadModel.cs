using System;

namespace Shufl.API.UploadModels.Group
{
    public class GroupAlbumUploadModel
    {
        public string GroupIdentifier { get; set; }

        public string AlbumIdentifier { get; set; }

        public Guid? RelatedGroupAlbumId { get; set; }

        public bool IsRandom { get; set; }
    }
}

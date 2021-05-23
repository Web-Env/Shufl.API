using System;

namespace Shufl.API.UploadModels.Group
{
    public class GroupPlaylistRatingUploadModel : IUploadModel
    {
        public string GroupIdentifier { get; set; }

        public string GroupPlaylistIdentifier { get; set; }

        public Guid GroupPlaylistRatingId { get; set; }

        public decimal OverallRating { get; set; }

        public string Comment { get; set; }
    }
}

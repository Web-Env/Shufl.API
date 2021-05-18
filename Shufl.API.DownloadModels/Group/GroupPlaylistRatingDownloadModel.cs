using Shufl.API.DownloadModels.User;
using System;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupPlaylistRatingDownloadModel
    {
        public string Id { get; set; }

        public decimal OverallRating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}

using Shufl.API.DownloadModels.Music;
using Shufl.API.DownloadModels.User;
using System;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupSuggestionDownloadModel
    {
        public string Identifier { get; set; }

        public bool IsRandom { get; set; }

        public AlbumDownloadModel Album { get; set; }

        public IEnumerable<GroupSuggestionRatingDownloadModel> GroupSuggestionRatings { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

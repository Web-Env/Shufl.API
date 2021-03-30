using Shufl.API.DownloadModels.Music;
using Shufl.API.DownloadModels.User;
using System;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupSuggestionDownloadModel
    {
        public bool IsRandom { get; set; }

        public AlbumDownloadModel Album { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

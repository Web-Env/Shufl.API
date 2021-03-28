using Shufl.API.DownloadModels.Music;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupSuggestionDownloadModel
    {
        public bool IsRandom { get; set; }

        public AlbumDownloadModel Album { get; set; }
    }
}

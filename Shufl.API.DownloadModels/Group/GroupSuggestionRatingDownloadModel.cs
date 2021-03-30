using Shufl.API.DownloadModels.User;
using System;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupSuggestionRatingDownloadModel
    {
        public string Id { get; set; }

        public byte OverallRating { get; set; }

        public byte LyricsRating { get; set; }

        public byte VocalsRating { get; set; }

        public byte InstrumentalsRating { get; set; }

        public byte CompositionRating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}

using Shufl.API.DownloadModels.User;
using System;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupSuggestionRatingDownloadModel
    {
        public string Id { get; set; }

        public decimal OverallRating { get; set; }

        public decimal LyricsRating { get; set; }

        public decimal VocalsRating { get; set; }

        public decimal InstrumentalsRating { get; set; }

        public decimal CompositionRating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}

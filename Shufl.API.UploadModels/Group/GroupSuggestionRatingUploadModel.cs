using System;

namespace Shufl.API.UploadModels.Group
{
    public class GroupSuggestionRatingUploadModel : IUploadModel
    {
        public string GroupIdentifier { get; set; }

        public string GroupSuggestionIdentifier { get; set; }

        public Guid GroupSuggestionRatingId { get; set; }

        public decimal OverallRating { get; set; }

        public decimal? LyricsRating { get; set; }

        public decimal? VocalsRating { get; set; }

        public decimal? InstrumentalsRating { get; set; }

        public decimal? CompositionRating { get; set; }

        public string Comment { get; set; }
    }
}

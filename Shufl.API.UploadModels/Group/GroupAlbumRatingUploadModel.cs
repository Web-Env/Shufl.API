using System;

namespace Shufl.API.UploadModels.Group
{
    public class GroupAlbumRatingUploadModel : IUploadModel
    {
        public string GroupIdentifier { get; set; }

        public string GroupAlbumIdentifier { get; set; }

        public Guid GroupAlbumRatingId { get; set; }

        public decimal OverallRating { get; set; }

        public decimal? LyricsRating { get; set; }

        public decimal? VocalsRating { get; set; }

        public decimal? InstrumentalsRating { get; set; }

        public decimal? StructureRating { get; set; }

        public string Comment { get; set; }
    }
}

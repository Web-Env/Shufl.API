using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shufl.API.UploadModels.Group
{
    public class GroupSuggestionRatingUploadModel : IUploadModel
    {
        public string GroupIdentifier { get; set; }

        public string GroupSuggestionIdentifier { get; set; }

        public decimal OverallRating { get; set; }

        public decimal LyricsRating { get; set; }

        public decimal VocalsRating { get; set; }

        public decimal InstrumentalsRating { get; set; }

        public decimal CompositionRating { get; set; }

        public string Comment { get; set; }
    }
}

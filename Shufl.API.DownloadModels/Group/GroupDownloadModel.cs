using Shufl.API.DownloadModels.User;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupDownloadModel
    {
        public string Name { get; set; }

        public string Identifier { get; set; }

        public bool IsPrivate { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public List<UserDownloadModel> Members { get; set; }
    }
}

using Shufl.API.DownloadModels.User;
using System.Collections.Generic;

namespace Shufl.API.DownloadModels.Group
{
    public class GroupDownloadModel
    {
        public string Name { get; set; }

        public string Identifier { get; set; }

        public UserDownloadModel CreatedBy { get; set; }

        public List<GroupMemberDownloadModel> Members { get; set; }
    }
}

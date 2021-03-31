using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupMemberModel
    {
        public static async Task<bool> CheckGroupMemberExistsAsync(
            string groupIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupIdentifier = groupIdentifier.ToUpperInvariant();
            var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

            return await CheckGroupMemberExistsAsync(
                group.Id,
                userId,
                repositoryManager.GroupMemberRepository).ConfigureAwait(false);
        }

        public static async Task<bool> CheckGroupMemberExistsAsync(
            Guid groupId,
            Guid userId,
            IGroupMemberRepository groupMemberRepository)
        {
            var groupMember = (await groupMemberRepository.FindAsync(gm =>
                gm.GroupId == groupId &&
                gm.UserId == userId)).FirstOrDefault();

            return groupMember != null;
        }
    }
}

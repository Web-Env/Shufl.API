using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupInviteModel
    {
        public static async Task<string> CreateNewGroupInviteAsync(
            string groupIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupIdentifier = groupIdentifier.ToUpper();

            try
            {
                var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

                if (group != null)
                {
                    var isUserMemberOfGroup = await GroupMemberModel.CheckGroupMemberExistsAsync(
                           groupIdentifier,
                           userId,
                           repositoryManager);

                    if (isUserMemberOfGroup)
                    {
                        var newGroupInviteIdentifier = await GenerateNewGroupInviteIdentifierAsync(
                            group.Id,
                            repositoryManager.GroupInviteRepository);
                        var newGroupInviteExpiryDate = DateTime.Now.AddDays(IdentifierConsts.GroupInviteIdentifierExpiryOffsetDays);

                        var newGroupInvite = new GroupInvite
                        {
                            GroupId = group.Id,
                            Identifier = newGroupInviteIdentifier,
                            ExpiryDate = newGroupInviteExpiryDate,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = userId
                        };

                        await repositoryManager.GroupInviteRepository.AddAsync(newGroupInvite);

                        return newGroupInviteIdentifier;
                    }
                    else
                    {
                        throw new UserNotGroupMemberException(
                            "You are not able to perform this action",
                            "You are not able to perform this action");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.NoTokenFound, "The requested group was not found");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<string> GenerateNewGroupInviteIdentifierAsync(
            Guid groupId,
            IGroupInviteRepository groupRepository)
        {
            var newGroupIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupInviteExistsForGroupWithIdentifier = await CheckGroupInviteExistsForGroupByIdentifierAsync(
                groupId,
                newGroupIdentifier,
                groupRepository);

            if (groupInviteExistsForGroupWithIdentifier)
            {
                return await GenerateNewGroupInviteIdentifierAsync(
                    groupId,
                    groupRepository);
            }
            else
            {
                return newGroupIdentifier;
            }
        }

        private static async Task<bool> CheckGroupInviteExistsForGroupByIdentifierAsync(
            Guid groupId,
            string groupInviteIdentifier,
            IGroupInviteRepository groupInviteRepository)
        {
            var groupInvite = await groupInviteRepository.FindAsync(gi => 
                gi.GroupId == groupId &&
                gi.Identifier == groupInviteIdentifier);

            return groupInvite != null;
        }

        public static async Task<string> JoinGroupByInviteAsync(
            string groupInviteIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupInviteIdentifier = groupInviteIdentifier.ToUpper();
            try
            {
                var invite = await repositoryManager.GroupInviteRepository.GetByIdentifierAsync(groupInviteIdentifier);
                var inviteIsValid = ValidateInvite(invite);

                if (inviteIsValid)
                {
                    var existingGroupMember = await repositoryManager.GroupMemberRepository.FindAsync(gm =>
                        gm.GroupId == invite.GroupId &&
                        gm.UserId == userId);

                    if (existingGroupMember == null)
                    {
                        var newGroupMember = new GroupMember
                        {
                            GroupId = invite.GroupId,
                            UserId = userId,
                            GroupInviteId = invite.Id,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = userId
                        };

                        await repositoryManager.GroupMemberRepository.AddAsync(newGroupMember);

                        var group = await repositoryManager.GroupRepository.GetByIdAsync(invite.GroupId);

                        return group.Identifier;
                    }
                    else
                    {
                        throw new UserAlreadyGroupMemberException(
                            "You are already a member of this group",
                            "You are already a member of this group");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.TokenExpired, "Invite link has expired");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool ValidateInvite(GroupInvite groupInvite)
        {
            if (groupInvite.ExpiryDate < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

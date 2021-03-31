using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Linq;
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
            groupIdentifier = groupIdentifier.ToUpperInvariant();

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
                        var existingGroupInviteIdentifierByUser = (await repositoryManager.GroupInviteRepository.FindAsync(gi =>
                            gi.GroupId == group.Id &&
                            gi.CreatedBy == userId &&
                            gi.ExpiryDate > DateTime.Now)).FirstOrDefault();

                        if (existingGroupInviteIdentifierByUser == null)
                        {
                            var newGroupInviteIdentifier = await GenerateNewGroupInviteIdentifierAsync(
                                group.Id,
                                repositoryManager.GroupInviteRepository).ConfigureAwait(false);
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
                            return existingGroupInviteIdentifierByUser.Identifier;
                        }
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
                    throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The requested Group was not found");
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
            var newGroupInviteIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupInviteExistsForGroupWithIdentifier = await CheckGroupInviteExistsByIdentifierAsync(
                newGroupInviteIdentifier,
                groupRepository).ConfigureAwait(false);

            if (groupInviteExistsForGroupWithIdentifier)
            {
                return await GenerateNewGroupInviteIdentifierAsync(
                    groupId,
                    groupRepository).ConfigureAwait(false);
            }
            else
            {
                return newGroupInviteIdentifier;
            }
        }

        private static async Task<bool> CheckGroupInviteExistsByIdentifierAsync(
            string groupInviteIdentifier,
            IGroupInviteRepository groupInviteRepository)
        {
            var groupInvite = await groupInviteRepository.GetByIdentifierAsync(groupInviteIdentifier);

            return groupInvite != null;
        }

        public static async Task<Domain.Entities.Group> JoinGroupByInviteAsync(
            string groupInviteIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupInviteIdentifier = groupInviteIdentifier.ToUpper();
            try
            {
                var invite = await repositoryManager.GroupInviteRepository.GetByIdentifierAsync(groupInviteIdentifier);
                if (invite != null)
                {
                    var inviteIsValid = ValidateGroupInvite(invite);

                    if (inviteIsValid)
                    {
                        var existingGroupMember = await repositoryManager.GroupMemberRepository.GetByUserIdAndGroupIdAsync(
                            userId,
                            invite.GroupId);

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

                            return group;
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
                        throw new InvalidTokenException(InvalidTokenType.TokenExpired, "This Group Invite has expired");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "This Group Invite is invalid");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<Domain.Entities.Group> CheckUserGroupInviteValidAsync(
            string groupInviteIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupInviteIdentifier = groupInviteIdentifier.ToUpperInvariant();

            try
            {
                var groupInviteExists = await CheckGroupInviteExistsByIdentifierAsync(
                    groupInviteIdentifier,
                    repositoryManager.GroupInviteRepository).ConfigureAwait(false);

                if (groupInviteExists)
                {
                    var groupInvite = await repositoryManager.GroupInviteRepository.GetByIdentifierAsync(groupInviteIdentifier);
                    var groupInviteIsValid = ValidateGroupInvite(groupInvite);

                    if (groupInviteIsValid)
                    {
                        var userIsMemberOfGroup = await GroupMemberModel.CheckGroupMemberExistsAsync(
                            groupInvite.GroupId,
                            userId,
                            repositoryManager.GroupMemberRepository);

                        if (!userIsMemberOfGroup)
                        {
                            var group = await repositoryManager.GroupRepository.GetByIdForDownloadAsync(groupInvite.GroupId);

                            return group;
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
                        throw new InvalidTokenException(InvalidTokenType.TokenExpired, "This Group Invite has expired");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "This Group Invite is invalid");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool ValidateGroupInvite(GroupInvite groupInvite)
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

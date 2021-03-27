using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Models.User;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupModel
    {
        public static async Task<List<Domain.Entities.Group>> GetAllUsersGroupsAsync(
        Guid userId,
        IRepositoryManager repositoryManager)
        {
            try
            {
                var usersGroupMemberships = await repositoryManager.GroupMemberRepository.FindAsync(gm => gm.UserId == userId);
                var usersGroupsIds = usersGroupMemberships.Select(gm => gm.GroupId);
                var usersGroups = await repositoryManager.GroupRepository.GetManyByIdForDownloadAsync(usersGroupsIds);

                return usersGroups;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<Domain.Entities.Group> GetUsersGroupAsync(
            string groupIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            try
            {
                var userIsMemberOfGroup = await CheckUserIsMemberOfGroupAsync(groupIdentifier, userId, repositoryManager);

                if (userIsMemberOfGroup)
                {
                    var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);
                    var usersGroup = await repositoryManager.GroupRepository.GetByIdForDownloadAsync(group.Id);

                    return usersGroup;
                }
                else
                {
                    throw new UserNotGroupMemberException(
                        "You do not have access to this group",
                        "You do not have access to this group");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsGroupUploadModelValid(GroupUploadModel groupUploadModel)
        {
            if (groupUploadModel.Name.Length > 150)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async Task<string> CreateNewGroupAsync(
            GroupUploadModel groupUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            try
            {
                var newGroupIdentifier = await GenerateNewGroupIdentifierAsync(repositoryManager.GroupRepository);

                var newGroup = new Domain.Entities.Group
                {
                    Identifier = newGroupIdentifier,
                    Name = groupUploadModel.Name,
                    IsPrivate = groupUploadModel.IsPrivate,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId,
                    LastUpdatedOn = DateTime.Now,
                    LastUpatedBy = userId
                };

                await repositoryManager.GroupRepository.AddAsync(newGroup);

                await CreateNewGroupMemberAsync(
                    newGroup.Id,
                    userId,
                    repositoryManager);

                return newGroupIdentifier;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<string> GenerateNewGroupIdentifierAsync(IGroupRepository groupRepository)
        {
            var newGroupIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupExistsWithIdentifier = await CheckGroupExistsByIdentifierAsync(newGroupIdentifier, groupRepository);

            if (groupExistsWithIdentifier) {
                return await GenerateNewGroupIdentifierAsync(groupRepository);
            }
            else
            {
                return newGroupIdentifier;
            }
        }

        private static async Task<bool> CheckGroupExistsByIdAsync(Guid groupId, IGroupRepository groupRepository)
        {
            var group = await groupRepository.GetByIdAsync(groupId);

            return group != null;
        }

        private static async Task<bool> CheckGroupExistsByIdentifierAsync(string groupIdentifier, IGroupRepository groupRepository)
        {
            var group = await groupRepository.GetByIdentifierAsync(groupIdentifier);

            return group != null;
        }

        private static async Task<bool> CheckUserIsMemberOfGroupAsync(
            string groupIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

            return await CheckUserIsMemberOfGroupAsync(group.Id, userId, repositoryManager.GroupMemberRepository);
        }

        private static async Task<bool> CheckUserIsMemberOfGroupAsync(
            Guid groupId,
            Guid userId,
            IGroupMemberRepository groupMemberRepository)
        {
            var groupMember = await groupMemberRepository.FindAsync(gm => 
                gm.GroupId == groupId && gm.UserId == userId);

            return groupMember != null;
        }

        private static async Task CreateNewGroupMemberAsync(
            Guid groupId,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            var groupExists = await CheckGroupExistsByIdAsync(groupId, repositoryManager.GroupRepository);
            var userExists = await UserModel.CheckUserExistsByIdAsync(userId, repositoryManager.UserRepository);

            if (groupExists && userExists)
            {
                try
                {
                    var newGroupMember = new GroupMember
                    {
                        GroupId = groupId,
                        UserId = userId,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userId,
                        LastUpdatedOn = DateTime.Now,
                        LastUpdatedBy = userId
                    };

                    await repositoryManager.GroupMemberRepository.AddAsync(newGroupMember);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

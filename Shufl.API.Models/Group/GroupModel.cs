using Shufl.API.Infrastructure.Consts;
using Shufl.API.Models.User;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupModel
    {
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

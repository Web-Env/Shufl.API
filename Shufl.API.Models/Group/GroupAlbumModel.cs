using AutoMapper;
using Shufl.API.Infrastructure.Consts;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Group.Interfaces;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupAlbumModel
    {
        public static async Task<IEnumerable<GroupAlbum>> GetGroupAlbumsAsync(
            string groupIdentifier,
            int page,
            int pageSize,
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
                        return await repositoryManager.GroupAlbumRepository.GetByGroupIdAsync(group.Id, page, pageSize);
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

        public static async Task<GroupAlbum> GetGroupAlbumAsync(
            string groupIdentifier,
            string groupAlbumIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupIdentifier = groupIdentifier.ToUpperInvariant();
            groupAlbumIdentifier = groupAlbumIdentifier.ToUpperInvariant();

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
                        return await repositoryManager.GroupAlbumRepository.GetByIdentifierAndGroupIdAsync(groupAlbumIdentifier, group.Id);
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

        public static async Task<string> CreateNewGroupAlbumAsync(
            GroupAlbumUploadModel groupAlbumUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var groupIdentifier = groupAlbumUploadModel.GroupIdentifier.ToUpperInvariant();

            try
            {
                var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

                if (group != null)
                {
                    var isUserMemberOfGroup = await GroupMemberModel.CheckGroupMemberExistsAsync(
                           groupAlbumUploadModel.GroupIdentifier,
                           userId,
                           repositoryManager);

                    if (isUserMemberOfGroup)
                    {
                        var newGroupAlbumIdentifier = await GenerateNewGroupAlbumIdentifierAsync(
                            group.Id,
                            repositoryManager.GroupAlbumRepository).ConfigureAwait(false);

                        var newGroupAlbum = new GroupAlbum
                        {
                            GroupId = group.Id,
                            Identifier = newGroupAlbumIdentifier,
                            IsRandom = groupAlbumUploadModel.IsRandom,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = userId
                        };

                        var existingAlbum = await repositoryManager.AlbumRepository.CheckExistsBySpotifyIdAsync(
                            groupAlbumUploadModel.AlbumIdentifier);

                        if (existingAlbum != null)
                        {
                            newGroupAlbum.AlbumId = existingAlbum.Id;

                        }
                        else
                        {
                            var newAlbum = await AlbumModel.IndexNewAlbumAsync(
                                groupAlbumUploadModel.AlbumIdentifier,
                                repositoryManager,
                                mapper,
                                spotifyAPICredentials);

                            newGroupAlbum.AlbumId = newAlbum.Id;
                        }

                        await repositoryManager.GroupAlbumRepository.AddAsync(newGroupAlbum);

                        return newGroupAlbumIdentifier;
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

        private static async Task<string> GenerateNewGroupAlbumIdentifierAsync(
            Guid groupId,
            IGroupAlbumRepository groupAlbumRepository)
        {
            var newGroupAlbumIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupAlbumIdentifierExistsForGroup = await CheckGroupAlbumIdentifierExistsForGroupAsync(
                newGroupAlbumIdentifier,
                groupId,
                groupAlbumRepository).ConfigureAwait(false);

            if (groupAlbumIdentifierExistsForGroup)
            {
                return await GenerateNewGroupAlbumIdentifierAsync(groupId, groupAlbumRepository).ConfigureAwait(false);
            }
            else
            {
                return newGroupAlbumIdentifier;
            }
        }

        private static async Task<bool> CheckGroupAlbumIdentifierExistsForGroupAsync(
            string groupAlbumIdentifier,
            Guid userId,
            IGroupAlbumRepository groupAlbumRepository)
        {
            var group = await groupAlbumRepository.CheckExistsByIdentifierAndGroupIdAsync(groupAlbumIdentifier, userId);

            return group != null;
        }
    }
}

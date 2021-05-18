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
    public static class GroupPlaylistModel
    {
        public static async Task<IEnumerable<GroupPlaylist>> GetGroupPlaylistsAsync(
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
                        return await repositoryManager.GroupPlaylistRepository.GetByGroupIdAsync(group.Id, page, pageSize);
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

        public static async Task<GroupPlaylist> GetGroupPlaylistAsync(
            string groupIdentifier,
            string groupPlaylistIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupIdentifier = groupIdentifier.ToUpperInvariant();
            groupPlaylistIdentifier = groupPlaylistIdentifier.ToUpperInvariant();

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
                        return await repositoryManager.GroupPlaylistRepository.GetByIdentifierAndGroupIdAsync(groupPlaylistIdentifier, group.Id);
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

        public static async Task<string> CreateNewGroupPlaylistAsync(
            GroupPlaylistUploadModel groupPlaylistUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var groupIdentifier = groupPlaylistUploadModel.GroupIdentifier.ToUpperInvariant();

            try
            {
                var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

                if (group != null)
                {
                    var isUserMemberOfGroup = await GroupMemberModel.CheckGroupMemberExistsAsync(
                           groupPlaylistUploadModel.GroupIdentifier,
                           userId,
                           repositoryManager);

                    if (isUserMemberOfGroup)
                    {
                        var newGroupPlaylistIdentifier = await GenerateNewGroupPlaylistIdentifierAsync(
                            group.Id,
                            repositoryManager.GroupPlaylistRepository).ConfigureAwait(false);

                        var newGroupPlaylist = new GroupPlaylist
                        {
                            GroupId = group.Id,
                            Identifier = newGroupPlaylistIdentifier,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = userId
                        };

                        var existingPlaylist = await repositoryManager.PlaylistRepository.CheckExistsBySpotifyIdAsync(
                            groupPlaylistUploadModel.PlaylistIdentifier);

                        if (existingPlaylist != null)
                        {
                            newGroupPlaylist.PlaylistId = existingPlaylist.Id;
                        }
                        else
                        {
                            var newPlaylist = await PlaylistModel.IndexNewPlaylistAsync(
                                groupPlaylistUploadModel.PlaylistIdentifier,
                                userId,
                                repositoryManager,
                                mapper,
                                spotifyAPICredentials);

                            newGroupPlaylist.PlaylistId = newPlaylist.Id;
                        }

                        await repositoryManager.GroupPlaylistRepository.AddAsync(newGroupPlaylist);

                        return newGroupPlaylistIdentifier;
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

        private static async Task<string> GenerateNewGroupPlaylistIdentifierAsync(
            Guid groupId,
            IGroupPlaylistRepository groupPlaylistRepository)
        {
            var newGroupPlaylistIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupPlaylistIdentifierExistsForGroup = await CheckGroupPlaylistIdentifierExistsForGroupAsync(
                newGroupPlaylistIdentifier,
                groupId,
                groupPlaylistRepository).ConfigureAwait(false);

            if (groupPlaylistIdentifierExistsForGroup)
            {
                return await GenerateNewGroupPlaylistIdentifierAsync(groupId, groupPlaylistRepository).ConfigureAwait(false);
            }
            else
            {
                return newGroupPlaylistIdentifier;
            }
        }

        private static async Task<bool> CheckGroupPlaylistIdentifierExistsForGroupAsync(
            string groupPlaylistIdentifier,
            Guid userId,
            IGroupPlaylistRepository groupPlaylistRepository)
        {
            var groupPlaylist = await groupPlaylistRepository.CheckExistsByIdentifierAndGroupIdAsync(groupPlaylistIdentifier, userId);

            return groupPlaylist != null;
        }
    }
}

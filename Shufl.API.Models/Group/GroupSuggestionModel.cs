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
    public static class GroupSuggestionModel
    {
        public static async Task<IEnumerable<GroupSuggestion>> GetGroupSuggestionsAsync(
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
                        return await repositoryManager.GroupSuggestionRepository.GetByGroupIdAsync(group.Id, page, pageSize);
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

        public static async Task<GroupSuggestion> GetGroupSuggestionAsync(
            string groupIdentifier,
            string groupSuggestionIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            groupIdentifier = groupIdentifier.ToUpperInvariant();
            groupSuggestionIdentifier = groupSuggestionIdentifier.ToUpperInvariant();

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
                        return await repositoryManager.GroupSuggestionRepository.GetByIdentifierAndGroupIdAsync(groupSuggestionIdentifier, group.Id);
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

        public static async Task<string> CreateNewGroupSuggestionAsync(
            GroupSuggestionUploadModel groupSuggestionUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var groupIdentifier = groupSuggestionUploadModel.GroupIdentifier.ToUpperInvariant();

            try
            {
                var group = await repositoryManager.GroupRepository.GetByIdentifierAsync(groupIdentifier);

                if (group != null)
                {
                    var isUserMemberOfGroup = await GroupMemberModel.CheckGroupMemberExistsAsync(
                           groupSuggestionUploadModel.GroupIdentifier,
                           userId,
                           repositoryManager);

                    if (isUserMemberOfGroup)
                    {
                        var newGroupSuggestionIdentifier = await GenerateNewGroupSuggestionIdentifierAsync(
                            group.Id,
                            repositoryManager.GroupSuggestionRepository).ConfigureAwait(false);

                        var newGroupSuggestion = new GroupSuggestion
                        {
                            GroupId = group.Id,
                            Identifier = newGroupSuggestionIdentifier,
                            IsRandom = groupSuggestionUploadModel.IsRandom,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            LastUpdatedOn = DateTime.Now,
                            LastUpdatedBy = userId
                        };

                        var existingAlbum = await repositoryManager.AlbumRepository.CheckExistsBySpotifyIdAsync(
                            groupSuggestionUploadModel.AlbumIdentifier);

                        if (existingAlbum != null)
                        {
                            newGroupSuggestion.AlbumId = existingAlbum.Id;

                        }
                        else
                        {
                            var newAlbum = await AlbumModel.IndexNewAlbumAsync(
                                groupSuggestionUploadModel.AlbumIdentifier,
                                repositoryManager,
                                mapper,
                                spotifyAPICredentials);

                            newGroupSuggestion.AlbumId = newAlbum.Id;
                        }

                        await repositoryManager.GroupSuggestionRepository.AddAsync(newGroupSuggestion);

                        return newGroupSuggestionIdentifier;
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

        private static async Task<string> GenerateNewGroupSuggestionIdentifierAsync(
            Guid groupId,
            IGroupSuggestionRepository groupSuggestionRepository)
        {
            var newGroupSuggestionIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.GroupIdentifierLength);
            var groupSuggestionIdentifierExistsForGroup = await CheckGroupSuggestionIdentifierExistsForGroupAsync(
                newGroupSuggestionIdentifier,
                groupId,
                groupSuggestionRepository).ConfigureAwait(false);

            if (groupSuggestionIdentifierExistsForGroup)
            {
                return await GenerateNewGroupSuggestionIdentifierAsync(groupId, groupSuggestionRepository).ConfigureAwait(false);
            }
            else
            {
                return newGroupSuggestionIdentifier;
            }
        }

        private static async Task<bool> CheckGroupSuggestionIdentifierExistsForGroupAsync(
            string groupSuggestionIdentifier,
            Guid userId,
            IGroupSuggestionRepository groupSuggestionRepository)
        {
            var group = await groupSuggestionRepository.CheckExistsByIdentifierAndGroupIdAsync(groupSuggestionIdentifier, userId);

            return group != null;
        }
    }
}

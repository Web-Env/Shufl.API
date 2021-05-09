using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.UploadModels.Group;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public static class GroupSuggestionRatingModel
    {
        public static async Task<GroupSuggestionRating> CreateNewGroupSuggestionRatingAsync(
            string groupIdentifier,
            string groupSuggestionIdentifier,
            GroupSuggestionRating groupSuggestionRating,
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
                        var groupSuggestion = await repositoryManager.GroupSuggestionRepository.GetByIdentifierAndGroupIdAsync(
                            groupSuggestionIdentifier,
                            group.Id);

                        if (groupSuggestion != null)
                        {
                            var existingGroupSuggestionRating = (await repositoryManager.GroupSuggestionRatingRepository.FindAsync(gsr =>
                                gsr.GroupSuggestionId == groupSuggestion.Id &&
                                gsr.CreatedBy == userId)).FirstOrDefault();

                            if (existingGroupSuggestionRating != null)
                            {
                                existingGroupSuggestionRating.OverallRating = groupSuggestionRating.OverallRating;
                                existingGroupSuggestionRating.LyricsRating = groupSuggestionRating.LyricsRating;
                                existingGroupSuggestionRating.VocalsRating = groupSuggestionRating.VocalsRating;
                                existingGroupSuggestionRating.InstrumentalsRating = groupSuggestionRating.InstrumentalsRating;
                                existingGroupSuggestionRating.StructureRating = groupSuggestionRating.StructureRating;
                                existingGroupSuggestionRating.Comment = groupSuggestionRating.Comment;
                                groupSuggestionRating.LastUpdatedOn = DateTime.Now;
                                groupSuggestionRating.LastUpdatedBy = userId;

                                return await repositoryManager.GroupSuggestionRatingRepository.UpdateAsync(existingGroupSuggestionRating);
                            }
                            else
                            {
                                groupSuggestionRating.GroupSuggestionId = groupSuggestion.Id;
                                groupSuggestionRating.CreatedOn = DateTime.Now;
                                groupSuggestionRating.CreatedBy = userId;
                                groupSuggestionRating.LastUpdatedOn = DateTime.Now;
                                groupSuggestionRating.LastUpdatedBy = userId;

                                return await repositoryManager.GroupSuggestionRatingRepository.AddAsync(groupSuggestionRating);
                            }
                        }
                        else
                        {
                            throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The requested Group Suggestion was not found");
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

        public static async Task<GroupSuggestionRating> EditGroupSuggestionRatingAsync(
            Guid groupSuggestionRatingId,
            GroupSuggestionRatingUploadModel groupSuggestionRatingUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            try
            {
                var groupSuggestionRating = await repositoryManager.GroupSuggestionRatingRepository.GetByIdAsync(groupSuggestionRatingId);

                if (groupSuggestionRating != null)
                {
                    if (groupSuggestionRating.CreatedBy == userId)
                    {
                        groupSuggestionRating.OverallRating = groupSuggestionRatingUploadModel.OverallRating;
                        groupSuggestionRating.LyricsRating = groupSuggestionRatingUploadModel.LyricsRating;
                        groupSuggestionRating.VocalsRating = groupSuggestionRatingUploadModel.VocalsRating;
                        groupSuggestionRating.InstrumentalsRating = groupSuggestionRatingUploadModel.InstrumentalsRating;
                        groupSuggestionRating.StructureRating = groupSuggestionRatingUploadModel.StructureRating;
                        groupSuggestionRating.Comment = groupSuggestionRatingUploadModel.Comment;
                        groupSuggestionRating.LastUpdatedOn = DateTime.Now;
                        groupSuggestionRating.LastUpdatedBy = userId;

                        await repositoryManager.GroupSuggestionRatingRepository.UpdateAsync(groupSuggestionRating);

                        return groupSuggestionRating;
                    }
                    else
                    {
                        throw new UserForbiddenException(
                            "You are not allowed to perform this action",
                            "You are not allowed to perform this action");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The requested Group Suggestion Rating was not found");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task DeleteGroupSuggestionRatingAsync(
            Guid groupSuggestionRatingId,
            Guid userId,
            IRepositoryManager repositoryManager)
        {
            try
            {
                var groupSuggestionRating = await repositoryManager.GroupSuggestionRatingRepository.GetByIdAsync(groupSuggestionRatingId);

                if (groupSuggestionRating != null)
                {
                    if (groupSuggestionRating.CreatedBy == userId)
                    {
                        await repositoryManager.GroupSuggestionRatingRepository.RemoveAsync(groupSuggestionRating);
                    }
                    else
                    {
                        throw new UserForbiddenException(
                            "You are not allowed to perform this action",
                            "You are not allowed to perform this action");
                    }
                }
                else
                {
                    throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The requested Group Suggestion Rating was not found");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

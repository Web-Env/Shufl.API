using Shufl.API.DownloadModels.Group;
using Shufl.API.Infrastructure.Enums;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Models.Group
{
    public class GroupSuggestionRatingModel
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
                            groupSuggestionRating.GroupSuggestionId = groupSuggestion.Id;
                            groupSuggestionRating.CreatedOn = DateTime.Now;
                            groupSuggestionRating.CreatedBy = userId;
                            groupSuggestionRating.LastUpdatedOn = DateTime.Now;
                            groupSuggestionRating.LastUpdatedBy = userId;

                            return await repositoryManager.GroupSuggestionRatingRepository.AddAsync(groupSuggestionRating);
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
    }
}

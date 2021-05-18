using AutoMapper;
using Shufl.API.Infrastructure.Encryption;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music.Helpers;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Models.Music
{
    public static class PlaylistModel
    {
        public static async Task<Playlist> IndexNewPlaylistAsync(
            string playlistIdentifier,
            Guid userId,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyPlaylist = await FetchPlaylistForIndexAsync(playlistIdentifier, spotifyAPICredentials);

            var newPlaylist = new Playlist
            {
                SpotifyId = spotifyPlaylist.Id,
                Name = spotifyPlaylist.Name,
                CreatedOn = DateTime.Now,
                CreatedBy = userId,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = userId
            };

            newPlaylist.PlaylistImages = mapper.Map<List<PlaylistImage>>(spotifyPlaylist.Images);

            await repositoryManager.PlaylistRepository.AddAsync(newPlaylist);

            return newPlaylist;
        }

        private static async Task<FullPlaylist> FetchPlaylistForIndexAsync(
            string playlistIdentifier,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            var spotifyClient = SearchHelper.CreateSpotifyClient(spotifyAPICredentials);

            var playlist = await spotifyClient.Playlists.Get(playlistIdentifier);

            return playlist;
        }

        public static async Task<IEnumerable<SimplePlaylist>> GetUserPlaylistsAsync(
            SpotifyAPICredentials spotifyAPICredentials,
            Guid userId,
            int page,
            int pageSize,
            IRepositoryManager repositoryManager)
        {
            try
            {
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                if (user.SpotifyRefreshToken != null)
                {
                    var spotifyAuthRefreshRequest = new AuthorizationCodeRefreshRequest
                    (
                        spotifyAPICredentials.ClientId,
                        spotifyAPICredentials.ClientSecret,
                        DecryptionService.DecryptString(user.SpotifyRefreshToken)
                    );

                    var spotifyAuthResponse = await new OAuthClient().RequestToken(spotifyAuthRefreshRequest);
                    var spotifyToken = spotifyAuthResponse.AccessToken;

                    var spotifyClient = new SpotifyClient(spotifyToken);

                    var playlistsRequest = new PlaylistCurrentUsersRequest
                    {
                        Limit = pageSize,
                        Offset = page * pageSize
                    };

                    var playlists = await spotifyClient.Playlists.CurrentUsers(playlistsRequest);

                    return playlists.Items;
                }
                else
                {
                    throw new SpotifyNotLinkedException("Spotify Account not Linked", "Spotify Account not Linked");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

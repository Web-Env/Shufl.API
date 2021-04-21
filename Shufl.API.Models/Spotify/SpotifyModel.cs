using Newtonsoft.Json;
using Shufl.API.Infrastructure.Encryption;
using Shufl.API.Infrastructure.Exceptions;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.UploadModels.Spotify;
using Shufl.Domain.Entities;
using Shufl.Domain.Repositories.Interfaces;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shufl.API.Models.Spotify
{
    public static class SpotifyModel
    {
        public static async Task LinkSpotifyAsync(
            SpotifyLinkUploadModel spotifyLinkUploadModel,
            Guid userId,
            IRepositoryManager repositoryManager,
            SpotifyAPICredentials spotifyAPICredentials)
        {
            try
            {
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                if (user != null)
                {
                    var spotifyAuthRequest = new AuthorizationCodeTokenRequest
                    (
                        spotifyAPICredentials.ClientId,
                        spotifyAPICredentials.ClientSecret,
                        spotifyLinkUploadModel.Code,
                        new Uri($"https://{spotifyLinkUploadModel.CallbackUrl}/callback")
                    );

                    var spotifyAuthResponse = await new OAuthClient().RequestToken(spotifyAuthRequest);
                    var spotifyToken = spotifyAuthResponse.AccessToken;

                    var httpClient = new HttpClient();
                    var spotifyUserRequest = new HttpRequestMessage(new HttpMethod("GET"), "https://api.spotify.com/v1/me");
                    spotifyUserRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {spotifyToken}");

                    var response = await httpClient.SendAsync(spotifyUserRequest);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var spotifyUserResponse = JsonConvert.DeserializeObject<SpotifyUserResponse>(responseString);

                    var userImages = new List<UserImage>();
                    foreach (var image in spotifyUserResponse.Images)
                    {
                        userImages.Add(new UserImage
                        {
                            UserId = user.Id,
                            Width = image.Width != null ? (short)image.Width : null,
                            Height = image.Height != null ? (short)image.Height : null,
                            Uri = image.Uri.Split("/").Last()
                        });
                    }

                    user.SpotifyRefreshToken = EncryptionService.EncryptString(spotifyAuthResponse.RefreshToken);
                    user.SpotifyUsername = spotifyUserResponse.Id;
                    user.SpotifyMarket = spotifyUserResponse.Country;
                    user.UserImages = userImages;
                    user.LastUpdatedOn = DateTime.Now;
                    user.LastUpdatedBy = userId;

                    await repositoryManager.UserRepository.UpdateAsync(user);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task UnlinkSpotifyAsync(Guid userId, IRepositoryManager repositoryManager)
        {
            try
            {
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                if (user != null)
                {
                    user.SpotifyRefreshToken = null;
                    user.SpotifyUsername = null;
                    user.SpotifyMarket = null;
                    user.LastUpdatedOn = DateTime.Now;
                    user.LastUpdatedBy = userId;

                    await repositoryManager.UserRepository.UpdateAsync(user);

                    var userImages = await repositoryManager.UserImageRepository.GetByUserIdAsync(user.Id);
                    await repositoryManager.UserImageRepository.RemoveRangeAsync(userImages);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task QueueAlbumAsync(
           string albumId,
           Guid userId,
           IRepositoryManager repositoryManager,
           SpotifyAPICredentials spotifyAPICredentials)
        {
            try
            {
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                if (user != null)
                {
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

                        var activeDevices = await spotifyClient.Player.GetAvailableDevices();

                        if (activeDevices.Devices.Count > 0)
                        {
                            var album = await spotifyClient.Albums.Get(albumId);

                            foreach (var track in album.Tracks.Items)
                            {
                                var trackQueueRequest = new PlayerAddToQueueRequest(track.Uri);

                                await spotifyClient.Player.AddToQueue(trackQueueRequest);
                            }
                        }
                        else
                        {
                            throw new SpotifyNoActiveDevicesException("No Active Devices Found", "No Active Devices Found");
                        }
                    }
                    else
                    {
                        throw new SpotifyNotLinkedException("Spotify Account not Linked", "Spotify Account not Linked");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task QueueTrackAsync(
           string trackId,
           Guid userId,
           IRepositoryManager repositoryManager,
           SpotifyAPICredentials spotifyAPICredentials)
        {
            try
            {
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                if (user != null)
                {
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

                        var activeDevices = await spotifyClient.Player.GetAvailableDevices();

                        if (activeDevices.Devices.Count > 0)
                        {
                            var trackQueueRequest = new PlayerAddToQueueRequest($"spotify:track:{trackId}");

                            await spotifyClient.Player.AddToQueue(trackQueueRequest);
                        }
                        else
                        {
                            throw new SpotifyNoActiveDevicesException("No Active Devices Found", "No Active Devices Found");
                        }
                    }
                    else
                    {
                        throw new SpotifyNotLinkedException("Spotify Account not Linked", "Spotify Account not Linked");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    internal class SpotifyUserResponse
    {
        public string Country { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        public string Id { get; set; }

        public ICollection<SpotifyUserImageModel> Images { get; set; }

        public class SpotifyUserImageModel
        {
            public int? Width { get; set; }

            public int? Height { get; set; }

            [JsonProperty("url")]
            public string Uri { get; set; }
        }
    }
}

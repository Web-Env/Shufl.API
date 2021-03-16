using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Properties
{
    [ApiController]
    [Route("[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public AlbumController(IOptions<SpotifyAPICredentials> spotifyAPICredentials)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomAlbum")]
        public async Task<IActionResult> GetRandomAlbumAsync(string genre = "", bool failed = false)
        {
            try
            {
                var randomAlbum = await AlbumModel.FetchRandomAlbumAsync(_spotifyAPICredentials, genre).ConfigureAwait(false);
                return Ok(randomAlbum);
            }
            catch (Exception err)
            {
                if (!failed)
                {
                    return await GetRandomAlbumAsync(genre, true).ConfigureAwait(false);
                }
                else
                {
                    return Problem("There was an error fetching a random album from Spotify", statusCode: 500, type: err.GetType().ToString());
                }
            }
        }

        [HttpGet("Album")]
        public async Task<IActionResult> GetAlbumAsync(string albumId)
        {
            try
            {
                var album = await AlbumModel.FetchAlbumAsync(albumId, _spotifyAPICredentials).ConfigureAwait(false);
                return Ok(album);
            }
            catch (Exception err)
            {
                return Problem("There was an error fetching the requested album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchAlbumAsync(string name)
        {
            try
            {
                var albums = (await AlbumModel.PerformAlbumSearch(
                    name,
                    _spotifyAPICredentials).ConfigureAwait(false)).Albums.Items;

                return Ok(albums);
            }
            catch (Exception err)
            {
                return Problem("There was an error searching for the album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shufl.API.Helpers;
using Shufl.API.Settings;
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
                var randomAlbum = await AlbumSearchHelper.FetchRandomAlbumAsync(_spotifyAPICredentials, genre);
                return Ok(randomAlbum);
            }
            catch (Exception err)
            {
                if (!failed)
                {
                    return await GetRandomAlbumAsync(genre, true);
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
                var album = await AlbumSearchHelper.FetchAlbumAsync(albumId, _spotifyAPICredentials);
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
                var albums = (await SearchHelper.PerformSearch(
                    SpotifyAPI.Web.SearchRequest.Types.Album,
                    name,
                    10,
                    0,
                    _spotifyAPICredentials
                )).Albums.Items;
                return Ok(albums);
            }
            catch (Exception err)
            {
                return Problem("There was an error searching for the album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

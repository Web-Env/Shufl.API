﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Artist;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public ArtistController(IOptions<SpotifyAPICredentials> spotifyAPICredentials)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomArtist")]
        public async Task<IActionResult> GetRandomArtistAsync()
        {
            try
            {
                var randomArtist = await ArtistModel.FetchRandomArtistAsync(_spotifyAPICredentials);
                var randomArtistAlbums = await AlbumModel.FetchArtistAlbumsAsync(randomArtist.Id, _spotifyAPICredentials);

                var artist = new ArtistDownloadModel
                {
                    Artist = randomArtist,
                    Albums = randomArtistAlbums
                };

                return Ok(artist);
            }
            catch (Exception err)
            {
                return Problem("There was an error fetching a random artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Artist")]
        public async Task<IActionResult> GetArtistAsync(string artistId)
        {
            try
            {
                var artist = await ArtistModel.FetchArtistAsync(artistId, _spotifyAPICredentials);
                return Ok(artist);
            }
            catch (Exception err)
            {
                return Problem("There was an error fetching the requested artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

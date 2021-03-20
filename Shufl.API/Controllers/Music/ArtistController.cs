using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Artist;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models;
using Shufl.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Music
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public ArtistController(IRepositoryManager repositoryManager,
                                ILogger<ArtistController> logger,
                                IMapper mapper,
                                IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(repositoryManager, logger, mapper)
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
                LogException(err);
                return Problem("There was an error fetching a random artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Artist")]
        public async Task<IActionResult> GetArtistAsync(string artistId)
        {
            try
            {
                var requestedArtist = await ArtistModel.FetchArtistAsync(artistId, _spotifyAPICredentials);
                var artistAlbums = await AlbumModel.FetchArtistAlbumsAsync(requestedArtist.Id, _spotifyAPICredentials);

                var artist = new ArtistDownloadModel
                {
                    Artist = requestedArtist,
                    Albums = artistAlbums
                };
                return Ok(artist);
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching the requested artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

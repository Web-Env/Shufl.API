using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.Settings;
using Shufl.API.Models.Music;
using Shufl.Domain.Entities;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shufl.API.Controllers.Music
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public ArtistController(ShuflContext shuflContext,
                                ILogger<ArtistController> logger,
                                IMapper mapper,
                                IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(shuflContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomArtist")]
        public async Task<ActionResult<ArtistDownloadModel>> GetRandomArtistAsync()
        {
            try
            {
                var randomArtist = await ArtistModel.FetchRandomArtistAsync(_spotifyAPICredentials);
                var randomArtistAlbums = await AlbumModel.FetchArtistAlbumsAsync(randomArtist.Id, _spotifyAPICredentials);

                var artist = MapEntityToDownloadModel<FullArtist, ArtistDownloadModel>(randomArtist);
                artist.Albums = MapEntitiesToDownloadModels<SimpleAlbum, AlbumDownloadModel>(randomArtistAlbums);

                return Ok(artist);
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching a random artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Artist")]
        public async Task<ActionResult<ArtistDownloadModel>> GetArtistAsync(string artistId)
        {
            try
            {
                var requestedArtist = await ArtistModel.FetchArtistAsync(artistId, _spotifyAPICredentials);
                var artistAlbums = await AlbumModel.FetchArtistAlbumsAsync(requestedArtist.Id, _spotifyAPICredentials);

                var artist = MapEntityToDownloadModel<FullArtist, ArtistDownloadModel>(requestedArtist);
                artist.Albums = MapEntitiesToDownloadModels<SimpleAlbum, AlbumDownloadModel>(artistAlbums);

                return Ok(artist);
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching the requested artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<ArtistDownloadModel>>> SearchArtistAsync(string name)
        {
            try
            {
                var artists = (await ArtistModel.PerformArtistSearch(
                    name,
                    _spotifyAPICredentials).ConfigureAwait(false)).Artists.Items;

                return Ok(MapEntitiesToDownloadModels<FullArtist, ArtistDownloadModel>(artists));
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error searching for the artist from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

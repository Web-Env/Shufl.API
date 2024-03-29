﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shufl.API.DownloadModels.Music;
using Shufl.API.Infrastructure.SearchResponseModels;
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
    public class AlbumController : CustomControllerBase
    {
        private readonly SpotifyAPICredentials _spotifyAPICredentials;

        public AlbumController(ShuflContext shuflContext,
                               ILogger<AlbumController> logger,
                               IMapper mapper,
                               IOptions<SpotifyAPICredentials> spotifyAPICredentials) : base(shuflContext, logger, mapper)
        {
            _spotifyAPICredentials = spotifyAPICredentials.Value;
        }

        [HttpGet("RandomAlbum")]
        public async Task<ActionResult<AlbumDownloadModel>> GetRandomAlbumAsync(string genre = "", bool failed = false)
        {
            try
            {
                var randomAlbum = await AlbumModel.FetchRandomAlbumAsync(_spotifyAPICredentials, genre).ConfigureAwait(false);

                return Ok(MapEntityToDownloadModel<AlbumResponseModel, AlbumDownloadModel>(randomAlbum));
            }
            catch (Exception err)
            {
                if (!failed)
                {
                    return await GetRandomAlbumAsync(genre, true).ConfigureAwait(false);
                }
                else
                {
                    LogException(err);
                    return Problem("There was an error fetching a random album from Spotify", statusCode: 500, type: err.GetType().ToString());
                }
            }
        }

        [HttpGet("RandomGroupAlbum")]
        public async Task<ActionResult<AlbumDownloadModel>> GetRandomGroupAlbumAsync(string groupIdentifier, string genre = "", bool failed = false)
        {
            try
            {
                var randomAlbum = await AlbumModel.FetchRandomGroupAlbumAsync(
                    groupIdentifier, 
                    _spotifyAPICredentials, 
                    RepositoryManager,
                    genre).ConfigureAwait(false);

                return Ok(MapEntityToDownloadModel<AlbumResponseModel, AlbumDownloadModel>(randomAlbum));
            }
            catch (Exception err)
            {
                if (!failed)
                {
                    return await GetRandomAlbumAsync(genre, true).ConfigureAwait(false);
                }
                else
                {
                    LogException(err);
                    return Problem("There was an error fetching a random album from Spotify", statusCode: 500, type: err.GetType().ToString());
                }
            }
        }

        [HttpGet("Album")]
        public async Task<ActionResult<AlbumDownloadModel>> GetAlbumAsync(string albumId)
        {
            try
            {
                var album = await AlbumModel.FetchAlbumAsync(albumId, _spotifyAPICredentials).ConfigureAwait(false);

                return Ok(MapEntityToDownloadModel<AlbumResponseModel, AlbumDownloadModel>(album));
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error fetching the requested album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<AlbumDownloadModel>>> SearchAlbumAsync(string name)
        {
            try
            {
                var albums = (await AlbumModel.PerformAlbumSearch(
                    name,
                    _spotifyAPICredentials).ConfigureAwait(false)).Albums.Items;

                return Ok(MapEntitiesToDownloadModels<SimpleAlbum, AlbumDownloadModel>(albums));
            }
            catch (Exception err)
            {
                LogException(err);
                return Problem("There was an error searching for the album from Spotify", statusCode: 500, type: err.GetType().ToString());
            }
        }
    }
}

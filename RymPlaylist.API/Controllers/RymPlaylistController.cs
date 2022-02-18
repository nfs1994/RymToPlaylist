using Microsoft.AspNetCore.Mvc;
using RymPlaylist.Service.Models;
using RymPlaylist.Service;
using RymPlaylist.API.Models;

namespace RymPlaylist.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RymPlaylistController : ControllerBase
    {
        private readonly ILogger<RymPlaylistController> _logger;

        public RymPlaylistController(ILogger<RymPlaylistController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "CreatePlaylistFromCharts")]
        public async Task<string> CreatePlaylistFromCharts(CreatePlaylistFromChartsRequestParameters createPlaylistRequestParameters)
        {
            var spotifyService = new SpotifyService();
            var chartsSearchParameters = new ChartsSearchParameters()
            {
                Count = createPlaylistRequestParameters.Count,
                Descriptor = createPlaylistRequestParameters.Descriptor,
                Genre = createPlaylistRequestParameters.Genre,
                Year = createPlaylistRequestParameters.Year
            };

            List<string> topcharts = await RymCrawler.GetTopCharts(chartsSearchParameters);
            return await spotifyService.CreatePlaylist(createPlaylistRequestParameters.PlaylistName, topcharts);
        }
    }
}
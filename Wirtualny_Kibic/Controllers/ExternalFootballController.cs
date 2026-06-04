using Microsoft.AspNetCore.Mvc;
using Wirtualny_Kibic.Services;

namespace Wirtualny_Kibic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalFootballController : ControllerBase
    {
        private readonly FootballApiService _footballApiService;

        public ExternalFootballController(FootballApiService footballApiService)
        {
            _footballApiService = footballApiService;
        }
        [HttpGet("team/{teamId}/fixtures")]
        public async Task<IActionResult> GetFixtures(int teamId)
        {
            var data = await _footballApiService.GetNextFixturesForTeamRawAsync(teamId);
            return Content(data, "application/json");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wirtualny_Kibic.Data;
using Wirtualny_Kibic.Entities;
using Wirtualny_Kibic.Services;


namespace Wirtualny_Kibic.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FixturesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly FootballApiService _footballApiService;

    public FixturesController(ApplicationDbContext context, FootballApiService footballApiService)
    {
        _context = context;
        _footballApiService = footballApiService;
    }
    [HttpGet("my-team-next-games")]
    public async Task<IActionResult> GetMyTeamExternalFixtures()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Brak userId w tokenie.");

        var staffProfile = await _context.StaffProfiles
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staffProfile == null)
            return NotFound("Nie znaleziono profilu staff dla zalogowanego użytkownika.");

        if (staffProfile.Team == null)
            return NotFound("Użytkownik nie ma przypisanej drużyny.");

        if (staffProfile.Team.ExternalApiTeamId == null)
            return BadRequest("Ta drużyna nie ma ustawionego ExternalApiId.");

        var data = await _footballApiService.GetNextPremierLeagueFixturesForTeamAsync(
    staffProfile.Team.ExternalApiTeamId.Value,
    season: 2025,
    next: 10);

        return Ok(data);
    }

    [HttpGet("my-team-premier-league-season")]
    public async Task<IActionResult> GetMyTeamPremierLeagueSeasonFixtures()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Brak userId w tokenie.");

        var staffProfile = await _context.StaffProfiles
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staffProfile == null)
            return NotFound("Nie znaleziono profilu staff dla zalogowanego użytkownika.");

        if (staffProfile.Team == null)
            return NotFound("Użytkownik nie ma przypisanej drużyny.");

        if (staffProfile.Team.ExternalApiTeamId == null)
            return BadRequest("Ta drużyna nie ma ustawionego ExternalApiId.");

        var data = await _footballApiService.GetAllPremierLeagueFixturesForTeamAsync(
            staffProfile.Team.ExternalApiTeamId.Value,
            season: 2025);

        return Ok(data);
    }

    [HttpGet("premier-league-standings")]
    public async Task<IActionResult> GetStandings()
    {
        var data = await _footballApiService.GetPremierLeagueStandingsAsync(2025);
        return Ok(data);
    }

    [HttpGet("{fixtureId}/statistics")]
    public async Task<IActionResult> GetFixtureStatistics(int fixtureId)
    {
        var data = await _footballApiService.GetFixtureStatisticsAsync(fixtureId);

        if (data == null)
            return NotFound("Brak statystyk dla tego meczu.");

        return Ok(data);
    }

}
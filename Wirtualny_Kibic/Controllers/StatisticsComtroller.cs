using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wirtualny_Kibic.Data;
using Wirtualny_Kibic.DTOs.Statistics;
using Wirtualny_Kibic.Services;

namespace Wirtualny_Kibic.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly FootballApiService _footballApiService;

    public StatisticsController(
        ApplicationDbContext context,
        FootballApiService footballApiService)
    {
        _context = context;
        _footballApiService = footballApiService;
    }

    [HttpGet("my-team-full")]
    public async Task<ActionResult<TeamFullStatisticsDto>> GetMyTeamFullStatistics()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var staff = await _context.StaffProfiles
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return NotFound("Nie znaleziono profilu sztabu.");

        if (staff.Team == null)
            return NotFound("Nie znaleziono drużyny użytkownika.");

        if (staff.Team.ExternalApiTeamId == null)
            return BadRequest("Ta drużyna nie ma przypisanego ExternalApiTeamId.");

        var result = await _footballApiService.GetTeamFullStatisticsAsync(
            staff.Team.Id,
            staff.Team.Name,
            staff.Team.ExternalApiTeamId.Value,
            2025
        );

        return Ok(result);
    }

    [HttpGet("team/{teamId}/full")]
    public async Task<ActionResult<TeamFullStatisticsDto>> GetTeamFullStatistics(int teamId)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
            return NotFound("Nie znaleziono drużyny.");

        if (team.ExternalApiTeamId == null)
            return BadRequest("Ta drużyna nie ma przypisanego ExternalApiTeamId.");

        var result = await _footballApiService.GetTeamFullStatisticsAsync(
            team.Id,
            team.Name,
            team.ExternalApiTeamId.Value,
            2025
        );

        return Ok(result);
    }

}
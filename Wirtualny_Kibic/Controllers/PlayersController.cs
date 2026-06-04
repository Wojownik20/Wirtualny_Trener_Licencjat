using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wirtualny_Kibic.Data;
using Wirtualny_Kibic.DTOs;
using Wirtualny_Kibic.Services;
using Wirtualny_Kibic.Entities;

namespace Wirtualny_Kibic.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly FootballApiService _footballApiService;

    public PlayersController(
        ApplicationDbContext context,
        FootballApiService footballApiService)
    {
        _context = context;
        _footballApiService = footballApiService;
    }

    [Authorize]
    [HttpGet("my-team")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetMyTeamPlayers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var staff = await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return Unauthorized();

        var players = await _context.Players
            .Where(p => p.TeamId == staff.TeamId)
            .OrderBy(p => p.Number)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName,
                Number = p.Number,
                Position = p.Position,
                Age = p.Age,
                Nationality = p.Nationality,
                InjuryStatus = p.InjuryStatus,
                Notes = p.Notes
            })
            .ToListAsync();

        return Ok(players);
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var staff = await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return Unauthorized();

        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id && p.TeamId == staff.TeamId);

        if (player == null)
            return NotFound();

        var playerDto = new PlayerDto
        {
            Id = player.Id,
            Name = player.FirstName + " " + player.LastName,
            Number = player.Number,
            Position = player.Position,
            Age = player.Age,
            Nationality = player.Nationality,
            InjuryStatus = player.InjuryStatus,
            Notes = player.Notes
        };


        return Ok(playerDto);
    }

    [Authorize]
    [HttpPut("{id}/staff-data")]
    public async Task<IActionResult> UpdatePlayerStaffData(int id, UpdatePlayerStaffDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var staff = await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return Unauthorized();

        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id && p.TeamId == staff.TeamId);

        if (player == null)
            return NotFound();

        player.InjuryStatus = dto.InjuryStatus;
        player.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpGet("my-team-live")]
    public async Task<IActionResult> GetMyTeamPlayersLive()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var staff = await _context.StaffProfiles
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return Unauthorized();

        if (staff.Team == null)
            return BadRequest("User is not assigned to any team.");

        if (staff.Team.ExternalApiTeamId == null)
            return BadRequest("Team does not have ExternalApiTeamId.");

        var players = await _footballApiService.GetTeamPlayersLiveAsync(
            staff.Team.ExternalApiTeamId.Value,
            2025
        );

        var staffNotes = await _context.PlayerStaffNotes
            .Where(n => n.TeamId == staff.TeamId)
            .ToListAsync();

        foreach (var player in players)
        {
            var note = staffNotes.FirstOrDefault(n =>
                n.ExternalPlayerId == player.ExternalPlayerId);

            if (note != null)
            {
                player.InjuryStatus = note.InjuryStatus;
                player.Notes = note.Notes;
            }
        }

        return Ok(players);
    }

    [Authorize]
    [HttpPut("external/{externalPlayerId}/staff-data")]
    public async Task<IActionResult> UpdateExternalPlayerStaffData(
    int externalPlayerId,
    UpdatePlayerStaffDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var staff = await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff == null)
            return Unauthorized();

        var note = await _context.PlayerStaffNotes
            .FirstOrDefaultAsync(n =>
                n.ExternalPlayerId == externalPlayerId &&
                n.TeamId == staff.TeamId);

        if (note == null)
        {
            note = new PlayerStaffNote
            {
                ExternalPlayerId = externalPlayerId,
                TeamId = staff.TeamId,
                InjuryStatus = dto.InjuryStatus,
                Notes = dto.Notes
            };

            _context.PlayerStaffNotes.Add(note);
        }
        else
        {
            note.InjuryStatus = dto.InjuryStatus;
            note.Notes = dto.Notes;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

}
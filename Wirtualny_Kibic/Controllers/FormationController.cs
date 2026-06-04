using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using Wirtualny_Kibic.Data;
using Wirtualny_Kibic.DTOs;
using Wirtualny_Kibic.DTOs.Formations;
using Wirtualny_Kibic.DTOs.TrainingPlan;
using Wirtualny_Kibic.Entities;

namespace Wirtualny_Kibic.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FormationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormation(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Brak userId w tokenie.");

        var staffProfile = await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staffProfile == null)
            return NotFound("Nie znaleziono profilu staff dla zalogowanego użytkownika.");

        var formation = await _context.FormationPlans
            .Include(f => f.FormationPlayers)
            .FirstOrDefaultAsync(f => f.Id == id && f.TeamId == staffProfile.TeamId);

        if (formation == null)
            return NotFound("Nie znaleziono formacji dla tej drużyny.");

        _context.FormationPlayers.RemoveRange(formation.FormationPlayers);
        _context.FormationPlans.Remove(formation);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Formation deleted successfully." });
    }

    [HttpGet("current/{formation}")]
    public async Task<ActionResult<FormationPlanDto>> GetCurrentFormation(string formation)
    {
        var staff = await GetCurrentStaff();
        if (staff == null) return Unauthorized();

        var plan = await EnsureFormationExists(staff.TeamId, formation, User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return Ok(ToDto(plan));
    }

    [HttpPut("current/{formation}")]
    public async Task<IActionResult> UpdateCurrentFormation(string formation, CreateFormationPlanDto dto)
    {
        var staff = await GetCurrentStaff();
        if (staff == null) return Unauthorized();

        var plan = await EnsureFormationExists(staff.TeamId, formation, User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        foreach (var slot in plan.FormationPlayers)
        {
            var incoming = dto.Players.FirstOrDefault(p => p.SlotId == slot.SlotId);

            slot.ExternalPlayerId = incoming?.ExternalPlayerId;
            slot.PlayerName = incoming?.PlayerName;
            slot.PlayerPhoto = incoming?.PlayerPhoto;
            slot.PlayerNumber = incoming?.PlayerNumber;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Formation updated successfully." });
    }

    private async Task<StaffProfile?> GetCurrentStaff()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return await _context.StaffProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    private async Task<FormationPlan> EnsureFormationExists(int teamId, string formation, string userId)
    {
        var plan = await _context.FormationPlans
            .Include(f => f.FormationPlayers)
            .FirstOrDefaultAsync(f => f.TeamId == teamId && f.Formation == formation);

        if (plan != null)
        {
            if (!plan.FormationPlayers.Any())
            {
                foreach (var slot in GetDefaultSlots(formation))
                {
                    _context.FormationPlayers.Add(new FormationPlayer
                    {
                        FormationPlanId = plan.Id,
                        SlotId = slot.SlotId,
                        PositionLabel = slot.PositionLabel
                    });
                }

                await _context.SaveChangesAsync();

                plan = await _context.FormationPlans
                    .Include(f => f.FormationPlayers)
                    .FirstAsync(f => f.Id == plan.Id);
            }

            return plan;
        }

        plan = new FormationPlan
        {
            Name = $"Formacja {formation}",
            Formation = formation,
            TeamId = teamId,
            CreatedByUserId = userId
        };

        _context.FormationPlans.Add(plan);
        await _context.SaveChangesAsync();

        foreach (var slot in GetDefaultSlots(formation))
        {
            _context.FormationPlayers.Add(new FormationPlayer
            {
                FormationPlanId = plan.Id,
                SlotId = slot.SlotId,
                PositionLabel = slot.PositionLabel
            });
        }

        await _context.SaveChangesAsync();

        return await _context.FormationPlans
            .Include(f => f.FormationPlayers)
            .FirstAsync(f => f.Id == plan.Id);
    }

    private FormationPlanDto ToDto(FormationPlan plan)
    {
        return new FormationPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Formation = plan.Formation,
            Players = plan.FormationPlayers
                .OrderBy(p => p.SlotId)
                .Select(p => new FormationPlayerDto
                {
                    SlotId = p.SlotId,
                    PositionLabel = p.PositionLabel,
                    ExternalPlayerId = p.ExternalPlayerId,
                    PlayerName = p.PlayerName,
                    PlayerPhoto = p.PlayerPhoto,
                    PlayerNumber = p.PlayerNumber
                })
                .ToList()
        };
    }

    private List<(int SlotId, string PositionLabel)> GetDefaultSlots(string formation)
    {
        return formation switch
        {
            "4-3-3" => new()
        {
            (1, "GK"), (2, "LB"), (3, "CB"), (4, "CB"), (5, "RB"),
            (6, "CM"), (7, "CM"), (8, "CM"),
            (9, "LW"), (10, "ST"), (11, "RW")
        },

            "4-2-3-1" => new()
        {
            (1, "GK"), (2, "LB"), (3, "CB"), (4, "CB"), (5, "RB"),
            (6, "CDM"), (7, "CDM"),
            (8, "LW"), (9, "CAM"), (10, "RW"), (11, "ST")
        },

            "4-4-2" => new()
        {
            (1, "GK"), (2, "LB"), (3, "CB"), (4, "CB"), (5, "RB"),
            (6, "LM"), (7, "CM"), (8, "CM"), (9, "RM"),
            (10, "ST"), (11, "ST")
        },
            "3-5-2" => new()
{
    (1, "GK"), (2, "CB"), (3, "CB"), (4, "CB"),
    (5, "LM"), (6, "CM"), (7, "CM"), (8, "CM"), (9, "RM"),
    (10, "ST"), (11, "ST")
},

            "3-4-3" => new()
{
    (1, "GK"), (2, "CB"), (3, "CB"), (4, "CB"),
    (5, "LM"), (6, "CM"), (7, "CM"), (8, "RM"),
    (9, "LW"), (10, "ST"), (11, "RW")
},

            "5-3-2" => new()
{
    (1, "GK"), (2, "LWB"), (3, "CB"), (4, "CB"), (5, "CB"), (6, "RWB"),
    (7, "CM"), (8, "CM"), (9, "CM"),
    (10, "ST"), (11, "ST")
},

            "4-1-4-1" => new()
{
    (1, "GK"), (2, "LB"), (3, "CB"), (4, "CB"), (5, "RB"),
    (6, "CDM"),
    (7, "LM"), (8, "CM"), (9, "CM"), (10, "RM"),
    (11, "ST")
},

            "4-5-1" => new()
{
    (1, "GK"), (2, "LB"), (3, "CB"), (4, "CB"), (5, "RB"),
    (6, "LM"), (7, "CM"), (8, "CM"), (9, "CM"), (10, "RM"),
    (11, "ST")
},

            _ => new()
        };
    }


}


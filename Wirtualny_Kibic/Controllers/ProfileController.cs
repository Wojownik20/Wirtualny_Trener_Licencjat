using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wirtualny_Kibic.Data;
using Wirtualny_Kibic.DTOs.Profile;
using Wirtualny_Kibic.DTOs._2FA;
using Wirtualny_Kibic.Entities;

namespace Wirtualny_Kibic.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var staff = await _context.StaffProfiles
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (staff == null)
            return NotFound("Nie znaleziono profilu sztabowca.");

        var dto = new ProfileMeDto
        {
            Email = user.Email ?? "",
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Role = staff.Role,
            TeamName = staff.Team != null ? staff.Team.Name : null,
            TeamLogoUrl = staff.Team != null ? staff.Team.LogoUrl : null,
            TwoFactorEnabled = user.TwoFactorEnabled
        };

        return Ok(dto);
    }

    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmail(ChangeEmailDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.NewEmail))
            return BadRequest("Nowy email jest wymagany.");

        var existingUser = await _userManager.FindByEmailAsync(dto.NewEmail);

        if (existingUser != null && existingUser.Id != user.Id)
            return BadRequest("Ten email jest już zajęty.");

        var setEmailResult = await _userManager.SetEmailAsync(user, dto.NewEmail);

        if (!setEmailResult.Succeeded)
            return BadRequest(setEmailResult.Errors);

        var setUserNameResult = await _userManager.SetUserNameAsync(user, dto.NewEmail);

        if (!setUserNameResult.Succeeded)
            return BadRequest(setUserNameResult.Errors);

        return Ok(new
        {
            message = "Email został zmieniony.",
            email = dto.NewEmail
        });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
            string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return BadRequest("Aktualne i nowe hasło są wymagane.");
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new
        {
            message = "Hasło zostało zmienione."
        });
    }

    [HttpGet("2fa/setup")]
    public async Task<IActionResult> SetupTwoFactor()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var key = await _userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = user.Email ?? "user";
        var issuer = "Wirtualny Trener";

        var authenticatorUri =
            $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
            $"?secret={key}" +
            $"&issuer={Uri.EscapeDataString(issuer)}" +
            $"&digits=6";

        return Ok(new
        {
            sharedKey = key,
            email = user.Email,
            authenticatorUri,
            message = "Zeskanuj kod QR lub dodaj klucz ręcznie."
        });
    }

    [HttpPost("2fa/enable")]
    public async Task<IActionResult> EnableTwoFactor(Enable2FaDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest("Kod 2FA jest wymagany.");

        var code = dto.Code.Replace(" ", "").Replace("-", "");

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            code
        );

        if (!isValid)
            return BadRequest("Nieprawidłowy kod 2FA.");

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        return Ok(new
        {
            message = "Weryfikacja dwuetapowa została włączona."
        });
    }

    [HttpPost("2fa/disable")]
    public async Task<IActionResult> DisableTwoFactor()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);

        return Ok(new
        {
            message = "Weryfikacja dwuetapowa została wyłączona."
        });
    }
}
namespace Wirtualny_Kibic.DTOs.Profile;

public class ProfileMeDto
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public string? TeamName { get; set; }
    public string? TeamLogoUrl { get; set; }
    public bool TwoFactorEnabled { get; set; }
}
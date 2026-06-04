namespace Wirtualny_Kibic.Entities;

public class PlayerStaffNote
{
    public int Id { get; set; }

    public int ExternalPlayerId { get; set; }
    public int TeamId { get; set; }

    public string? InjuryStatus { get; set; }
    public string? Notes { get; set; }
}
namespace Wirtualny_Kibic.DTOs.Formations;

public class FormationPlayerDto
{
    public int SlotId { get; set; }
    public string PositionLabel { get; set; } = string.Empty;

    public int? ExternalPlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? PlayerPhoto { get; set; }
    public int? PlayerNumber { get; set; }
}

namespace Wirtualny_Kibic.Entities;

public class FormationPlayer
{
    public int Id { get; set; }

    public int FormationPlanId { get; set; }
    public FormationPlan FormationPlan { get; set; } = null!;

    public int SlotId { get; set; }
    public string PositionLabel { get; set; } = string.Empty;

    public int? ExternalPlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? PlayerPhoto { get; set; }
    public int? PlayerNumber { get; set; }
}
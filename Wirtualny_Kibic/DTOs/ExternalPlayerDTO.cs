namespace Wirtualny_Kibic.DTOs;

public class ExternalPlayerDto
{
    public int ExternalPlayerId { get; set; }

    public string Name { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? Position { get; set; }
    public int? Number { get; set; }

    public int? Age { get; set; }
    public string? Nationality { get; set; }

    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? Photo { get; set; }

    public int? Appearances { get; set; }
    public int? Lineups { get; set; }
    public int? Minutes { get; set; }
    public string? Rating { get; set; }

    public int? Goals { get; set; }
    public int? Assists { get; set; }

    public int? ShotsTotal { get; set; }
    public int? ShotsOnTarget { get; set; }

    public int? PassesTotal { get; set; }
    public int? KeyPasses { get; set; }

    public int? Tackles { get; set; }
    public int? DuelsWon { get; set; }

    public int? YellowCards { get; set; }
    public int? RedCards { get; set; }
    public string? InjuryStatus { get; set; }
    public string? Notes { get; set; }
}
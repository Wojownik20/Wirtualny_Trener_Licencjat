namespace Wirtualny_Kibic.DTOs.External;

public class ExternalMatchStatisticsDto
{
    public int FixtureId { get; set; }

    public string HomeTeamName { get; set; } = string.Empty;
    public string HomeTeamLogo { get; set; } = string.Empty;

    public string AwayTeamName { get; set; } = string.Empty;
    public string AwayTeamLogo { get; set; } = string.Empty;

    public List<MatchStatisticRowDto> Statistics { get; set; } = new();
}

public class MatchStatisticRowDto
{
    public string Type { get; set; } = string.Empty;
    public string HomeValue { get; set; } = "-";
    public string AwayValue { get; set; } = "-";
} 
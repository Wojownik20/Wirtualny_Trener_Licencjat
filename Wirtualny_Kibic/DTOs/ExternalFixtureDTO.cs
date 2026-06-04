namespace Wirtualny_Kibic.DTOs
{
    public class ExternalFixtureDto
    {
        public int FixtureId { get; set; }
        public DateTime Date { get; set; }

        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;

        public string HomeTeamLogo { get; set; } = string.Empty;
        public string AwayTeamLogo { get; set; } = string.Empty;

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        public string Status { get; set; } = string.Empty;
        public string LeagueName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string? Round { get; set; }
    }
}
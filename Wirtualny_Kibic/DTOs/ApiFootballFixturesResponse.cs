using System.Text.Json.Serialization;

namespace Wirtualny_Kibic.DTOs.External
{
    public class ApiFootballFixturesResponse
    {
        [JsonPropertyName("response")]
        public List<ApiFootballFixtureItem> Response { get; set; } = new();
    }

    public class ApiFootballFixtureItem
    {
        [JsonPropertyName("fixture")]
        public ApiFootballFixture Fixture { get; set; } = new();

        [JsonPropertyName("league")]
        public ApiFootballLeague League { get; set; } = new();

        [JsonPropertyName("teams")]
        public ApiFootballTeams Teams { get; set; } = new();

        [JsonPropertyName("goals")]
        public ApiFootballGoals Goals { get; set; } = new();
    }

    public class ApiFootballFixture
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("status")]
        public ApiFootballStatus Status { get; set; } = new();

        [JsonPropertyName("venue")]
        public ApiFootballVenue Venue { get; set; } = new();
    }

    public class ApiFootballStatus
    {
        [JsonPropertyName("long")]
        public string Long { get; set; } = string.Empty;
    }

    public class ApiFootballVenue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class ApiFootballLeague
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("round")]
        public string? Round { get; set; }
    }

    public class ApiFootballTeams
    {
        [JsonPropertyName("home")]
        public ApiFootballTeam Home { get; set; } = new();

        [JsonPropertyName("away")]
        public ApiFootballTeam Away { get; set; } = new();
    }

    public class ApiFootballTeam
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;
    }

    public class ApiFootballGoals
    {
        [JsonPropertyName("home")]
        public int? Home { get; set; }

        [JsonPropertyName("away")]
        public int? Away { get; set; }
    }
}
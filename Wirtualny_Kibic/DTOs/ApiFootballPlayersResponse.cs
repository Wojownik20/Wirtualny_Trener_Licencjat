using System.Text.Json.Serialization;

namespace Wirtualny_Kibic.DTOs;

using System.Text.Json.Serialization;

public class ApiFootballPlayersResponse
{
    [JsonPropertyName("response")]
    public List<ApiFootballPlayerResponse> Response { get; set; } = new();

    [JsonPropertyName("paging")]
    public ApiFootballPaging? Paging { get; set; }
}

public class ApiFootballPaging
{
    [JsonPropertyName("current")]
    public int Current { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class ApiFootballPlayerResponse
{
    [JsonPropertyName("player")]
    public ApiFootballPlayer Player { get; set; } = new();

    [JsonPropertyName("statistics")]
    public List<ApiFootballPlayerStatistics> Statistics { get; set; } = new();
}

public class ApiFootballPlayer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("firstname")]
    public string? Firstname { get; set; }

    [JsonPropertyName("lastname")]
    public string? Lastname { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("height")]
    public string? Height { get; set; }

    [JsonPropertyName("weight")]
    public string? Weight { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }

    [JsonPropertyName("birth")]
    public ApiFootballBirth? Birth { get; set; }
}

public class ApiFootballBirth
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }
}

public class ApiFootballPlayerStatistics
{
    [JsonPropertyName("league")]
    public ApiFootballPlayerLeague? League { get; set; }

    public class ApiFootballPlayerLeague
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    [JsonPropertyName("games")]
    public ApiFootballGames? Games { get; set; }

    [JsonPropertyName("goals")]
    public ApiFootballGoals? Goals { get; set; }

    [JsonPropertyName("shots")]
    public ApiFootballShots? Shots { get; set; }

    [JsonPropertyName("passes")]
    public ApiFootballPasses? Passes { get; set; }

    [JsonPropertyName("tackles")]
    public ApiFootballTackles? Tackles { get; set; }

    [JsonPropertyName("duels")]
    public ApiFootballDuels? Duels { get; set; }

    [JsonPropertyName("cards")]
    public ApiFootballCards? Cards { get; set; }


}

public class ApiFootballGames
{
    [JsonPropertyName("appearences")]
    public int? Appearences { get; set; }

    [JsonPropertyName("lineups")]
    public int? Lineups { get; set; }

    [JsonPropertyName("minutes")]
    public int? Minutes { get; set; }

    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }
}

public class ApiFootballGoals
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("assists")]
    public int? Assists { get; set; }
}

public class ApiFootballShots
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("on")]
    public int? On { get; set; }
}

public class ApiFootballPasses
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("key")]
    public int? Key { get; set; }
}

public class ApiFootballTackles
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

public class ApiFootballDuels
{
    [JsonPropertyName("won")]
    public int? Won { get; set; }
}

public class ApiFootballCards
{
    [JsonPropertyName("yellow")]
    public int? Yellow { get; set; }

    [JsonPropertyName("red")]
    public int? Red { get; set; }
}
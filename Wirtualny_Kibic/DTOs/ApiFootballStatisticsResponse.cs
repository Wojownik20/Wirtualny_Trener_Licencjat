using System.Text.Json.Serialization;

namespace Wirtualny_Kibic.DTOs.External;



public class ApiFootballStatisticsResponse
{
    [JsonPropertyName("response")]
    public List<ApiFootballStatisticsTeamItem> Response { get; set; } = new();
}

public class ApiFootballStatisticsTeamItem
{
    [JsonPropertyName("team")]
    public ApiFootballTeam Team { get; set; } = new();

    [JsonPropertyName("statistics")]
    public List<ApiFootballStatisticItem> Statistics { get; set; } = new();
}

public class ApiFootballStatisticItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public object? Value { get; set; }
}
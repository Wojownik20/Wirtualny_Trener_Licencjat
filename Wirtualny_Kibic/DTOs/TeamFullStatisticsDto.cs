namespace Wirtualny_Kibic.DTOs.Statistics;

public class TeamFullStatisticsDto
{
    public TeamStatisticsInfoDto Team { get; set; } = new();
    public SeasonSummaryDto SeasonSummary { get; set; } = new();
    public LeagueStandingDto? LeagueStanding { get; set; }

    public HomeAwayStatsDto HomeStats { get; set; } = new();
    public HomeAwayStatsDto AwayStats { get; set; } = new();

    public GoalsStatsDto Goals { get; set; } = new();
    public FormStatsDto Form { get; set; } = new();

    public List<PlayerSeasonStatsDto> Players { get; set; } = new();
    public List<FixtureStatsDto> Fixtures { get; set; } = new();

    public PlayerSeasonStatsDto? TopScorer { get; set; }
    public PlayerSeasonStatsDto? TopAssistant { get; set; }
    public PlayerSeasonStatsDto? MostMinutes { get; set; }
    public PlayerSeasonStatsDto? BestRatedPlayer { get; set; }
    public PlayerSeasonStatsDto? MostYellowCards { get; set; }
    public PlayerSeasonStatsDto? MostRedCards { get; set; }
}

public class TeamStatisticsInfoDto
{
    public int TeamId { get; set; }
    public int? ExternalTeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? TeamLogo { get; set; }
    public int Season { get; set; }
    public string LeagueName { get; set; } = "Premier League";
    public int LeagueId { get; set; } = 39;
}

public class SeasonSummaryDto
{
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }

    public int Points { get; set; }

    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }

    public double AverageGoalsFor { get; set; }
    public double AverageGoalsAgainst { get; set; }

    public int CleanSheets { get; set; }
    public int FailedToScore { get; set; }

    public int BiggestWinMargin { get; set; }
    public int BiggestLossMargin { get; set; }

    public string? BiggestWin { get; set; }
    public string? BiggestLoss { get; set; }
}

public class LeagueStandingDto
{
    public int Rank { get; set; }
    public int Points { get; set; }
    public int GoalsDiff { get; set; }
    public string Form { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class HomeAwayStatsDto
{
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }

    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }

    public double AverageGoalsFor { get; set; }
    public double AverageGoalsAgainst { get; set; }

    public int CleanSheets { get; set; }
    public int FailedToScore { get; set; }
}

public class GoalsStatsDto
{
    public int TotalFor { get; set; }
    public int TotalAgainst { get; set; }

    public int HomeFor { get; set; }
    public int HomeAgainst { get; set; }

    public int AwayFor { get; set; }
    public int AwayAgainst { get; set; }

    public double AverageFor { get; set; }
    public double AverageAgainst { get; set; }

    public List<GoalMinuteStatDto> GoalsForByMinute { get; set; } = new();
    public List<GoalMinuteStatDto> GoalsAgainstByMinute { get; set; } = new();
}

public class GoalMinuteStatDto
{
    public string MinuteRange { get; set; } = string.Empty;
    public int Total { get; set; }
    public double Percentage { get; set; }
}

public class FormStatsDto
{
    public List<string> LastFiveResults { get; set; } = new();
    public List<string> LastTenResults { get; set; } = new();

    public int CurrentWinStreak { get; set; }
    public int CurrentUnbeatenStreak { get; set; }
    public int CurrentLosingStreak { get; set; }

    public int LongestWinStreak { get; set; }
    public int LongestUnbeatenStreak { get; set; }
    public int LongestLosingStreak { get; set; }
}

public class PlayerSeasonStatsDto
{
    public int ExternalPlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public string? Position { get; set; }
    public int? Number { get; set; }
    public string? Nationality { get; set; }

    public int Appearances { get; set; }
    public int Lineups { get; set; }
    public int Minutes { get; set; }
    public double? Rating { get; set; }

    public int Goals { get; set; }
    public int Assists { get; set; }

    public int ShotsTotal { get; set; }
    public int ShotsOnTarget { get; set; }

    public int PassesTotal { get; set; }
    public int KeyPasses { get; set; }

    public int Tackles { get; set; }
    public int DuelsTotal { get; set; }
    public int DuelsWon { get; set; }

    public int YellowCards { get; set; }
    public int RedCards { get; set; }
}

public class FixtureStatsDto
{
    public int FixtureId { get; set; }
    public DateTime Date { get; set; }

    public string Round { get; set; } = string.Empty;

    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string? HomeTeamLogo { get; set; }

    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public string? AwayTeamLogo { get; set; }

    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }

    public bool IsHome { get; set; }
    public bool IsPlayed { get; set; }

    public string Result { get; set; } = string.Empty;
    public string ResultType { get; set; } = string.Empty;
    // W / D / L / NOT_PLAYED

    public int TeamGoals { get; set; }
    public int OpponentGoals { get; set; }
}
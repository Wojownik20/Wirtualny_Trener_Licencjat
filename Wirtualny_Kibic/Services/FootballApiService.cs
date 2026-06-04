using Microsoft.Extensions.Options;
using System.Text.Json;
using Wirtualny_Kibic.Options;
using Wirtualny_Kibic.DTOs;
using Wirtualny_Kibic.DTOs.External;
using Wirtualny_Kibic.DTOs.Statistics;


namespace Wirtualny_Kibic.Services
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private readonly FootballApiOptions _options;

        public FootballApiService(HttpClient httpClient, IOptions<FootballApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GetNextFixturesForTeamRawAsync(int teamId, int next = 10)
        {
            var response = await _httpClient.GetAsync($"/fixtures?team={teamId}&next={next}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            return content;
        }
        public async Task<List<ExternalFixtureDto>> GetNextPremierLeagueFixturesForTeamAsync(
            int teamId,
            int season = 2025,
            int next = 10)
        {
            var response = await _httpClient.GetAsync(
                $"/fixtures?team={teamId}&league=39&season={season}&next={next}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(content, options);

            if (apiResponse?.Response == null)
                return new List<ExternalFixtureDto>();

            return apiResponse.Response.Select(f => new ExternalFixtureDto
            {
                FixtureId = f.Fixture.Id,
                Date = f.Fixture.Date,
                HomeTeamName = f.Teams.Home.Name,
                AwayTeamName = f.Teams.Away.Name,
                HomeTeamLogo = f.Teams.Home.Logo,
                AwayTeamLogo = f.Teams.Away.Logo,
                HomeGoals = f.Goals.Home,
                AwayGoals = f.Goals.Away,
                Status = f.Fixture.Status.Long,
                LeagueName = f.League.Name,
                VenueName = f.Fixture.Venue.Name,
                Round = f.League.Round

            }).ToList();
        }
        public async Task<List<ExternalFixtureDto>> GetAllPremierLeagueFixturesForTeamAsync(
            int teamId,
            int season = 2025)
        {
            var response = await _httpClient.GetAsync(
                $"/fixtures?team={teamId}&league=39&season={season}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(content, options);

            if (apiResponse?.Response == null)
                return new List<ExternalFixtureDto>();

            return apiResponse.Response
                .OrderBy(f => f.Fixture.Date)
                .Select(f => new ExternalFixtureDto
                {
                    FixtureId = f.Fixture.Id,
                    Date = f.Fixture.Date,
                    HomeTeamName = f.Teams.Home.Name,
                    AwayTeamName = f.Teams.Away.Name,
                    HomeTeamLogo = f.Teams.Home.Logo,
                    AwayTeamLogo = f.Teams.Away.Logo,
                    HomeGoals = f.Goals.Home,
                    AwayGoals = f.Goals.Away,
                    Status = f.Fixture.Status.Long,
                    LeagueName = f.League.Name,
                    VenueName = f.Fixture.Venue.Name,
                    Round = f.League.Round
                }).ToList();
        }

        public async Task<string> GetPremierLeagueStandingsRawAsync(int season = 2025)
        {
            var response = await _httpClient.GetAsync($"/standings?league=39&season={season}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            return content;
        }
        public async Task<List<ExternalStandingDto>> GetPremierLeagueStandingsAsync(int season = 2025)
        {
            var response = await _httpClient.GetAsync($"/standings?league=39&season={season}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            using var doc = JsonDocument.Parse(content);

            var standings = doc.RootElement
                .GetProperty("response")[0]
                .GetProperty("league")
                .GetProperty("standings")[0];

            var result = new List<ExternalStandingDto>();

            foreach (var team in standings.EnumerateArray())
            {
                result.Add(new ExternalStandingDto
                {
                    Rank = team.GetProperty("rank").GetInt32(),
                    TeamName = team.GetProperty("team").GetProperty("name").GetString()!,
                    TeamLogo = team.GetProperty("team").GetProperty("logo").GetString()!,

                    Played = team.GetProperty("all").GetProperty("played").GetInt32(),
                    Wins = team.GetProperty("all").GetProperty("win").GetInt32(),
                    Draws = team.GetProperty("all").GetProperty("draw").GetInt32(),
                    Losses = team.GetProperty("all").GetProperty("lose").GetInt32(),

                    GoalsFor = team.GetProperty("all").GetProperty("goals").GetProperty("for").GetInt32(),
                    GoalsAgainst = team.GetProperty("all").GetProperty("goals").GetProperty("against").GetInt32(),

                    Points = team.GetProperty("points").GetInt32()
                });
            }

            return result;
        }

        public async Task<ExternalMatchStatisticsDto?> GetFixtureStatisticsAsync(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"/fixtures/statistics?fixture={fixtureId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API-Football error: {(int)response.StatusCode} {response.ReasonPhrase} | {content}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiFootballStatisticsResponse>(content, options);

            if (apiResponse?.Response == null || apiResponse.Response.Count < 2)
                return null;

            var home = apiResponse.Response[0];
            var away = apiResponse.Response[1];

            var result = new ExternalMatchStatisticsDto
            {
                FixtureId = fixtureId,
                HomeTeamName = home.Team.Name,
                HomeTeamLogo = home.Team.Logo,
                AwayTeamName = away.Team.Name,
                AwayTeamLogo = away.Team.Logo
            };

            foreach (var homeStat in home.Statistics)
            {
                var awayStat = away.Statistics.FirstOrDefault(s => s.Type == homeStat.Type);

                result.Statistics.Add(new MatchStatisticRowDto
                {
                    Type = homeStat.Type,
                    HomeValue = FormatStatValue(homeStat.Value),
                    AwayValue = FormatStatValue(awayStat?.Value)
                });
            }

            return result;
        }

        private static string FormatStatValue(object? value)
        {
            if (value == null)
                return "-";

            return value.ToString() ?? "-";
        }

        public async Task<List<ExternalPlayerDto>> GetTeamPlayersLiveAsync(
    int externalTeamId,
    int season = 2025)
        {
            var allPlayers = new List<ApiFootballPlayerResponse>();
            var page = 1;
            var totalPages = 1;

            do
            {
                var url = $"/players?team={externalTeamId}&league=39&season={season}&page={page}";

                var httpResponse = await _httpClient.GetAsync(url);
                var content = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine("=== API FOOTBALL PLAYERS RAW JSON ===");
                Console.WriteLine(content);
                Console.WriteLine("=== END RAW JSON ===");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"API-Football error: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase} | {content}");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = JsonSerializer.Deserialize<ApiFootballPlayersResponse>(content, options);

                if (response?.Response != null)
                {
                    allPlayers.AddRange(response.Response);
                }

                totalPages = response?.Paging?.Total ?? 1;
                page++;

            } while (page <= totalPages);
            var players = allPlayers
    .GroupBy(p => p.Player.Id)
    .Select(g => g.First())
    .Where(p =>
    {
        var stats = p.Statistics.FirstOrDefault();

        return
            (stats?.Games?.Appearences ?? 0) > 0
            || (stats?.Games?.Minutes ?? 0) > 0;
    })
    .Select(p =>
    {
        var premierLeagueStats = p.Statistics.FirstOrDefault(s =>
            s.League?.Id == 39 || s.League?.Name == "Premier League"
        ) ?? p.Statistics.FirstOrDefault();

        return new ExternalPlayerDto
        {
            ExternalPlayerId = p.Player.Id,
            Name = p.Player.Name ?? "",
            FirstName = p.Player.Firstname,
            LastName = p.Player.Lastname,
            Age = p.Player.Age,
            Nationality = p.Player.Nationality,
            Height = p.Player.Height,
            Weight = p.Player.Weight,
            Photo = p.Player.Photo,

            Position = premierLeagueStats?.Games?.Position,
            Number = premierLeagueStats?.Games?.Number,

            Appearances = premierLeagueStats?.Games?.Appearences,
            Lineups = premierLeagueStats?.Games?.Lineups,
            Minutes = premierLeagueStats?.Games?.Minutes,
            Rating = premierLeagueStats?.Games?.Rating,

            Goals = premierLeagueStats?.Goals?.Total,
            Assists = premierLeagueStats?.Goals?.Assists,

            ShotsTotal = premierLeagueStats?.Shots?.Total,
            ShotsOnTarget = premierLeagueStats?.Shots?.On,

            PassesTotal = premierLeagueStats?.Passes?.Total,
            KeyPasses = premierLeagueStats?.Passes?.Key,

            Tackles = premierLeagueStats?.Tackles?.Total,
            DuelsWon = premierLeagueStats?.Duels?.Won,

            YellowCards = premierLeagueStats?.Cards?.Yellow,
            RedCards = premierLeagueStats?.Cards?.Red,
        };
    })
    .ToList();
return players;

        }



        public async Task<TeamFullStatisticsDto> GetTeamFullStatisticsAsync(
    int localTeamId,
    string teamName,
    int externalTeamId,
    int season = 2025)
        {
            var fixtures = await GetAllPremierLeagueFixturesForTeamAsync(externalTeamId, season);
            var standings = await GetPremierLeagueStandingsAsync(season);
            var players = await GetTeamPlayersLiveAsync(externalTeamId, season);

            var playedFixtures = fixtures
                .Where(f => f.HomeGoals.HasValue && f.AwayGoals.HasValue)
                .OrderBy(f => f.Date)
                .ToList();

            var fixtureStats = playedFixtures.Select(f =>
            {
                var isHome = IsSameTeam(f.HomeTeamName, teamName);

                var teamGoals = isHome ? f.HomeGoals!.Value : f.AwayGoals!.Value;
                var opponentGoals = isHome ? f.AwayGoals!.Value : f.HomeGoals!.Value;

                var resultType = teamGoals > opponentGoals
                    ? "W"
                    : teamGoals == opponentGoals
                        ? "D"
                        : "L";

                return new FixtureStatsDto
                {
                    FixtureId = f.FixtureId,
                    Date = f.Date,
                    Round = f.Round,

                    HomeTeamName = f.HomeTeamName,
                    HomeTeamLogo = f.HomeTeamLogo,
                    AwayTeamName = f.AwayTeamName,
                    AwayTeamLogo = f.AwayTeamLogo,

                    HomeGoals = f.HomeGoals,
                    AwayGoals = f.AwayGoals,

                    IsHome = isHome,
                    IsPlayed = true,

                    ResultType = resultType,
                    Result = $"{teamGoals}:{opponentGoals}",

                    TeamGoals = teamGoals,
                    OpponentGoals = opponentGoals
                };
            }).ToList();

            var homeFixtures = fixtureStats.Where(f => f.IsHome).ToList();
            var awayFixtures = fixtureStats.Where(f => !f.IsHome).ToList();

            var seasonSummary = BuildSeasonSummary(fixtureStats);
            var homeStats = BuildHomeAwayStats(homeFixtures);
            var awayStats = BuildHomeAwayStats(awayFixtures);

            var mappedPlayers = players.Select(p => new PlayerSeasonStatsDto
            {
                ExternalPlayerId = p.ExternalPlayerId,
                Name = p.Name,
                Photo = p.Photo,
                Position = p.Position,
                Number = p.Number,
                Nationality = p.Nationality,

                Appearances = p.Appearances ?? 0,
                Lineups = p.Lineups ?? 0,
                Minutes = p.Minutes ?? 0,
                Rating = ParseRating(p.Rating),

                Goals = p.Goals ?? 0,
                Assists = p.Assists ?? 0,

                ShotsTotal = p.ShotsTotal ?? 0,
                ShotsOnTarget = p.ShotsOnTarget ?? 0,

                PassesTotal = p.PassesTotal ?? 0,
                KeyPasses = p.KeyPasses ?? 0,

                Tackles = p.Tackles ?? 0,
                DuelsWon = p.DuelsWon ?? 0,

                YellowCards = p.YellowCards ?? 0,
                RedCards = p.RedCards ?? 0
            }).ToList();

            var standing = standings.FirstOrDefault(s => IsSameTeam(s.TeamName, teamName));

            return new TeamFullStatisticsDto
            {
                Team = new TeamStatisticsInfoDto
                {
                    TeamId = localTeamId,
                    ExternalTeamId = externalTeamId,
                    TeamName = teamName,
                    TeamLogo = standing?.TeamLogo,
                    Season = season,
                    LeagueName = "Premier League",
                    LeagueId = 39
                },

                SeasonSummary = seasonSummary,

                LeagueStanding = standing == null ? null : new LeagueStandingDto
                {
                    Rank = standing.Rank,
                    Points = standing.Points,
                    GoalsDiff = standing.GoalsFor - standing.GoalsAgainst
                },

                HomeStats = homeStats,
                AwayStats = awayStats,

                Goals = new GoalsStatsDto
                {
                    TotalFor = seasonSummary.GoalsFor,
                    TotalAgainst = seasonSummary.GoalsAgainst,

                    HomeFor = homeStats.GoalsFor,
                    HomeAgainst = homeStats.GoalsAgainst,

                    AwayFor = awayStats.GoalsFor,
                    AwayAgainst = awayStats.GoalsAgainst,

                    AverageFor = seasonSummary.AverageGoalsFor,
                    AverageAgainst = seasonSummary.AverageGoalsAgainst
                },

                Form = BuildFormStats(fixtureStats),

                Players = mappedPlayers
                    .OrderByDescending(p => p.Minutes)
                    .ToList(),

                Fixtures = fixtureStats
                    .OrderByDescending(f => f.Date)
                    .ToList(),

                TopScorer = mappedPlayers.OrderByDescending(p => p.Goals).FirstOrDefault(),
                TopAssistant = mappedPlayers.OrderByDescending(p => p.Assists).FirstOrDefault(),
                MostMinutes = mappedPlayers.OrderByDescending(p => p.Minutes).FirstOrDefault(),
                BestRatedPlayer = mappedPlayers
                    .Where(p => p.Rating.HasValue)
                    .OrderByDescending(p => p.Rating)
                    .FirstOrDefault(),
                MostYellowCards = mappedPlayers.OrderByDescending(p => p.YellowCards).FirstOrDefault(),
                MostRedCards = mappedPlayers
    .Where(p => p.RedCards > 0)
    .OrderByDescending(p => p.RedCards)
    .FirstOrDefault()
            };
        }

        private static SeasonSummaryDto BuildSeasonSummary(List<FixtureStatsDto> fixtures)
        {
            var played = fixtures.Count;

            var wins = fixtures.Count(f => f.ResultType == "W");
            var draws = fixtures.Count(f => f.ResultType == "D");
            var losses = fixtures.Count(f => f.ResultType == "L");

            var goalsFor = fixtures.Sum(f => f.TeamGoals);
            var goalsAgainst = fixtures.Sum(f => f.OpponentGoals);

            var biggestWin = fixtures
                .Where(f => f.ResultType == "W")
                .OrderByDescending(f => f.TeamGoals - f.OpponentGoals)
                .FirstOrDefault();

            var biggestLoss = fixtures
                .Where(f => f.ResultType == "L")
                .OrderByDescending(f => f.OpponentGoals - f.TeamGoals)
                .FirstOrDefault();

            return new SeasonSummaryDto
            {
                Played = played,
                Wins = wins,
                Draws = draws,
                Losses = losses,

                Points = wins * 3 + draws,

                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst,
                GoalDifference = goalsFor - goalsAgainst,

                AverageGoalsFor = played == 0 ? 0 : Math.Round((double)goalsFor / played, 2),
                AverageGoalsAgainst = played == 0 ? 0 : Math.Round((double)goalsAgainst / played, 2),

                CleanSheets = fixtures.Count(f => f.OpponentGoals == 0),
                FailedToScore = fixtures.Count(f => f.TeamGoals == 0),

                BiggestWinMargin = biggestWin == null ? 0 : biggestWin.TeamGoals - biggestWin.OpponentGoals,
                BiggestLossMargin = biggestLoss == null ? 0 : biggestLoss.OpponentGoals - biggestLoss.TeamGoals,

                BiggestWin = biggestWin == null
                    ? null
                    : $"{biggestWin.HomeTeamName} {biggestWin.HomeGoals}:{biggestWin.AwayGoals} {biggestWin.AwayTeamName}",

                BiggestLoss = biggestLoss == null
                    ? null
                    : $"{biggestLoss.HomeTeamName} {biggestLoss.HomeGoals}:{biggestLoss.AwayGoals} {biggestLoss.AwayTeamName}"
            };
        }

        private static HomeAwayStatsDto BuildHomeAwayStats(List<FixtureStatsDto> fixtures)
        {
            var played = fixtures.Count;
            var goalsFor = fixtures.Sum(f => f.TeamGoals);
            var goalsAgainst = fixtures.Sum(f => f.OpponentGoals);

            return new HomeAwayStatsDto
            {
                Played = played,
                Wins = fixtures.Count(f => f.ResultType == "W"),
                Draws = fixtures.Count(f => f.ResultType == "D"),
                Losses = fixtures.Count(f => f.ResultType == "L"),

                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst,
                GoalDifference = goalsFor - goalsAgainst,

                AverageGoalsFor = played == 0 ? 0 : Math.Round((double)goalsFor / played, 2),
                AverageGoalsAgainst = played == 0 ? 0 : Math.Round((double)goalsAgainst / played, 2),

                CleanSheets = fixtures.Count(f => f.OpponentGoals == 0),
                FailedToScore = fixtures.Count(f => f.TeamGoals == 0)
            };
        }

        private static FormStatsDto BuildFormStats(List<FixtureStatsDto> fixtures)
        {
            var ordered = fixtures.OrderByDescending(f => f.Date).ToList();
            var chronological = fixtures.OrderBy(f => f.Date).ToList();

            return new FormStatsDto
            {
                LastFiveResults = ordered.Take(5).Select(f => f.ResultType).ToList(),
                LastTenResults = ordered.Take(10).Select(f => f.ResultType).ToList(),

                CurrentWinStreak = CountCurrentStreak(ordered, "W"),
                CurrentUnbeatenStreak = CountCurrentUnbeatenStreak(ordered),
                CurrentLosingStreak = CountCurrentStreak(ordered, "L"),

                LongestWinStreak = CountLongestStreak(chronological, "W"),
                LongestUnbeatenStreak = CountLongestUnbeatenStreak(chronological),
                LongestLosingStreak = CountLongestStreak(chronological, "L")
            };
        }

        private static int CountCurrentStreak(List<FixtureStatsDto> fixtures, string resultType)
        {
            var count = 0;

            foreach (var fixture in fixtures)
            {
                if (fixture.ResultType == resultType)
                    count++;
                else
                    break;
            }

            return count;
        }

        private static int CountCurrentUnbeatenStreak(List<FixtureStatsDto> fixtures)
        {
            var count = 0;

            foreach (var fixture in fixtures)
            {
                if (fixture.ResultType is "W" or "D")
                    count++;
                else
                    break;
            }

            return count;
        }

        private static int CountLongestStreak(List<FixtureStatsDto> fixtures, string resultType)
        {
            var best = 0;
            var current = 0;

            foreach (var fixture in fixtures)
            {
                if (fixture.ResultType == resultType)
                {
                    current++;
                    best = Math.Max(best, current);
                }
                else
                {
                    current = 0;
                }
            }

            return best;
        }

        private static int CountLongestUnbeatenStreak(List<FixtureStatsDto> fixtures)
        {
            var best = 0;
            var current = 0;

            foreach (var fixture in fixtures)
            {
                if (fixture.ResultType is "W" or "D")
                {
                    current++;
                    best = Math.Max(best, current);
                }
                else
                {
                    current = 0;
                }
            }

            return best;
        }

        private static bool IsSameTeam(string apiTeamName, string localTeamName)
        {
            static string Normalize(string value)
            {
                return value
                    .ToLower()
                    .Replace("fc", "")
                    .Replace("afc", "")
                    .Replace("the", "")
                    .Replace("&", "and")
                    .Trim();
            }

            return Normalize(apiTeamName).Contains(Normalize(localTeamName))
                || Normalize(localTeamName).Contains(Normalize(apiTeamName));
        }

        private static double? ParseRating(object? rating)
        {
            if (rating == null)
                return null;

            if (double.TryParse(
                rating.ToString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var parsed))
            {
                return Math.Round(parsed, 2);
            }

            return null;
        }
    }
}
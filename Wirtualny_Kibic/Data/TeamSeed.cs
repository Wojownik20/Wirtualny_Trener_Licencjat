using Wirtualny_Kibic.Entities;

namespace Wirtualny_Kibic.Data;

public static class TeamSeed
{
    public static async Task SeedTeams(ApplicationDbContext context)
    {
        context.Teams.RemoveRange(context.Teams);
        await context.SaveChangesAsync();

        var teams = new List<Team>
{
    new() { Name="Arsenal", ShortName="ARS", City="London", Stadium="Emirates Stadium", ExternalApiTeamId=42, LogoUrl="https://media.api-sports.io/football/teams/42.png" },
    new() { Name="Aston Villa", ShortName="AVL", City="Birmingham", Stadium="Villa Park", ExternalApiTeamId=66, LogoUrl="https://media.api-sports.io/football/teams/66.png" },
    new() { Name="Burnley", ShortName="BUR", City="Burnley", Stadium="Turf Moor", ExternalApiTeamId=44, LogoUrl="https://media.api-sports.io/football/teams/44.png" },
    new() { Name="Bournemouth", ShortName="BOU", City="Bournemouth", Stadium="Vitality Stadium", ExternalApiTeamId=35, LogoUrl="https://media.api-sports.io/football/teams/35.png" },
    new() { Name="Brentford", ShortName="BRE", City="London", Stadium="Gtech Community Stadium", ExternalApiTeamId=55, LogoUrl="https://media.api-sports.io/football/teams/55.png" },
    new() { Name="Brighton & Hove Albion", ShortName="BHA", City="Brighton", Stadium="Amex Stadium", ExternalApiTeamId=51, LogoUrl="https://media.api-sports.io/football/teams/51.png" },
    new() { Name="Chelsea", ShortName="CHE", City="London", Stadium="Stamford Bridge", ExternalApiTeamId=49, LogoUrl="https://media.api-sports.io/football/teams/49.png" },
    new() { Name="Crystal Palace", ShortName="CRY", City="London", Stadium="Selhurst Park", ExternalApiTeamId=52, LogoUrl="https://media.api-sports.io/football/teams/52.png" },
    new() { Name="Everton", ShortName="EVE", City="Liverpool", Stadium="Goodison Park", ExternalApiTeamId=45, LogoUrl="https://media.api-sports.io/football/teams/45.png" },
    new() { Name="Fulham", ShortName="FUL", City="London", Stadium="Craven Cottage", ExternalApiTeamId=36, LogoUrl="https://media.api-sports.io/football/teams/36.png" },
    new() { Name="Liverpool", ShortName="LIV", City="Liverpool", Stadium="Anfield", ExternalApiTeamId=40, LogoUrl="https://media.api-sports.io/football/teams/40.png" },
    new() { Name="Leeds United", ShortName="LEE", City="Leeds", Stadium="Elland Road", ExternalApiTeamId=63, LogoUrl="https://media.api-sports.io/football/teams/63.png" },
    new() { Name="Sunderland", ShortName="SUN", City="Sunderland", Stadium="Stadium of Light", ExternalApiTeamId=39, LogoUrl="https://media.api-sports.io/football/teams/39.png" },
    new() { Name="Manchester City", ShortName="MCI", City="Manchester", Stadium="Etihad Stadium", ExternalApiTeamId=50, LogoUrl="https://media.api-sports.io/football/teams/50.png" },
    new() { Name="Manchester United", ShortName="MUN", City="Manchester", Stadium="Old Trafford", ExternalApiTeamId=33, LogoUrl="https://media.api-sports.io/football/teams/33.png" },
    new() { Name="Newcastle United", ShortName="NEW", City="Newcastle", Stadium="St James' Park", ExternalApiTeamId=34, LogoUrl="https://media.api-sports.io/football/teams/34.png" },
    new() { Name="Nottingham Forest", ShortName="NFO", City="Nottingham", Stadium="City Ground", ExternalApiTeamId=65, LogoUrl="https://media.api-sports.io/football/teams/65.png" },
    new() { Name="Tottenham Hotspur", ShortName="TOT", City="London", Stadium="Tottenham Hotspur Stadium", ExternalApiTeamId=47, LogoUrl="https://media.api-sports.io/football/teams/47.png" },
    new() { Name="West Ham United", ShortName="WHU", City="London", Stadium="London Stadium", ExternalApiTeamId=48, LogoUrl="https://media.api-sports.io/football/teams/48.png" },
    new() { Name="Wolverhampton Wanderers", ShortName="WOL", City="Wolverhampton", Stadium="Molineux", ExternalApiTeamId=39, LogoUrl="https://media.api-sports.io/football/teams/39.png" }
};

        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();
    }
}
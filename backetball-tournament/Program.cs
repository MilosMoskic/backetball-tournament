using backetball_tournament.Models;
using backetball_tournament.Services;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        MatchSimulator simulator = new MatchSimulator();
        var scheduler = new TournamentScheduler(simulator);
        TeamRankingSystem teamRankingSystem = new();

        string jsonTeams = @"C:\Users\Molee\source\repos\backetball-tournament\backetball-tournament\Files\groups.json";
        var groups = JsonConvert.DeserializeObject<Groups>(File.ReadAllText(jsonTeams));

        var standingsA = groups.A.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            Points = 0
        }).ToList();

        var standingsB = groups.B.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            Points = 0
        }).ToList();

        var standingsC = groups.C.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            Points = 0
        }).ToList();

        SimulateAndPrintGroupMatches(groups.A, scheduler, "A", standingsA);
        SimulateAndPrintGroupMatches(groups.B, scheduler, "B", standingsB);
        SimulateAndPrintGroupMatches(groups.C, scheduler, "C", standingsC);

        teamRankingSystem.RankTeamsAcrossGroups(new List<List<TeamStanding>> { standingsA, standingsB, standingsC });
    }

    static void SimulateAndPrintGroupMatches(List<TeamInfo> teams, TournamentScheduler scheduler, string groupName, List<TeamStanding> standings)
    {
        Console.WriteLine($"\n=============== Grupa {groupName} ===============");

        var rounds = scheduler.GenerateMatchesAndRounds(teams);
        scheduler.SimulateAndPrintMatches(rounds, standings);
    }
}
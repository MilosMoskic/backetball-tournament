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
        var eliminationPhaseScheduler = new EliminationPhaseScheduler(simulator);

        string jsonTeams = @"C:\Users\Molee\source\repos\backetball-tournament\backetball-tournament\Files\groups.json";
        var groups = JsonConvert.DeserializeObject<Groups>(File.ReadAllText(jsonTeams));

        var standingsA = groups.A.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            TeamInfo = t,
            Points = 0
        }).ToList();

        var standingsB = groups.B.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            TeamInfo = t,
            Points = 0
        }).ToList();

        var standingsC = groups.C.Select(t => new TeamStanding
        {
            ISOCode = t.ISOCode,
            TeamName = t.Team,
            TeamInfo = t,
            Points = 0
        }).ToList();

        List<Match> groupStageMatches = new List<Match>();

        groupStageMatches.AddRange(SimulateAndPrintGroupMatches(groups.A, scheduler, "A", standingsA));
        groupStageMatches.AddRange(SimulateAndPrintGroupMatches(groups.B, scheduler, "B", standingsB));
        groupStageMatches.AddRange(SimulateAndPrintGroupMatches(groups.C, scheduler, "C", standingsC));

        teamRankingSystem.RankTeamsAcrossGroups(new List<List<TeamStanding>> { standingsA, standingsB, standingsC });

        var topTeams = GetTopTeamsForEliminationPhase(standingsA, standingsB, standingsC);

        eliminationPhaseScheduler.RunEliminationPhase(topTeams, groupStageMatches);
    }

    static List<Match> SimulateAndPrintGroupMatches(List<TeamInfo> teams, TournamentScheduler scheduler, string groupName, List<TeamStanding> standings)
    {
        Console.WriteLine($"\n=============== Grupa {groupName} ===============");

        var rounds = scheduler.GenerateMatchesAndRounds(teams);

        var allMatches = rounds.SelectMany(r => r.Value).ToList();

        scheduler.SimulateAndPrintMatches(allMatches, standings);

        return allMatches;
    }

    static List<TeamStanding> GetTopTeamsForEliminationPhase(params List<TeamStanding>[] standingsLists)
    {
        var allStandings = standingsLists.SelectMany(list => list).ToList();

        var rankedTeams = allStandings
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.PointsScored - t.PointsAgainst)
            .ThenByDescending(t => t.PointsScored)
            .Take(8)
            .ToList();

        return rankedTeams;
    }
}
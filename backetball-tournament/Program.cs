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
        var teamStandingService = new TeamStandingService();

        string jsonTeams = @"C:\Users\Molee\source\repos\backetball-tournament\backetball-tournament\Files\groups.json";
        var groups = JsonConvert.DeserializeObject<Groups>(File.ReadAllText(jsonTeams));

        var standingsA = teamStandingService.ConvertTeamsToStandings(groups.A);
        var standingsB = teamStandingService.ConvertTeamsToStandings(groups.B);
        var standingsC = teamStandingService.ConvertTeamsToStandings(groups.C);

        List<Match> groupStageMatches = new List<Match>();

        groupStageMatches.AddRange(scheduler.SimulateAndPrintGroupMatches(groups.A, scheduler, "A", standingsA));
        groupStageMatches.AddRange(scheduler.SimulateAndPrintGroupMatches(groups.B, scheduler, "B", standingsB));
        groupStageMatches.AddRange(scheduler.SimulateAndPrintGroupMatches(groups.C, scheduler, "C", standingsC));

        var topTeams = teamRankingSystem.RankTeamsAcrossGroups(new List<List<TeamStanding>> { standingsA, standingsB, standingsC });

        eliminationPhaseScheduler.RunEliminationPhase(topTeams, groupStageMatches);
    }
}
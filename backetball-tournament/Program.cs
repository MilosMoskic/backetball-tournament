using backetball_tournament.Models;
using backetball_tournament.Services;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        MatchSimulator simulator = new MatchSimulator();
        var scheduler = new TournamentScheduler(simulator);

        string jsonTeams = @"C:\Users\Molee\source\repos\backetball-tournament\backetball-tournament\Files\groups.json";
        var groups = JsonConvert.DeserializeObject<Groups>(File.ReadAllText(jsonTeams));

        SimulateAndPrintGroupMatches(groups.A, scheduler, "A");
        SimulateAndPrintGroupMatches(groups.B, scheduler, "B");
        SimulateAndPrintGroupMatches(groups.C, scheduler, "C");
    }

    static void SimulateAndPrintGroupMatches(List<TeamInfo> teams, TournamentScheduler scheduler, string groupName)
    {
        Console.WriteLine($"=============== Grupa {groupName} ===============");

        var rounds = scheduler.GenerateMatchesAndRounds(teams);
        scheduler.SimulateAndPrintMatches(rounds);
    }
}
using backetball_tournament.Models;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string jsonTeams = @"C:\Users\Molee\source\repos\backetball-tournament\backetball-tournament\Files\groups.json";
        var groups = JsonConvert.DeserializeObject<Groups>(File.ReadAllText(jsonTeams));

        foreach (var team in groups.A)
        {
            Console.WriteLine($"{team.Team} ({team.ISOCode}) - FIBA Ranking: {team.FIBARanking}");
        }
    }
}
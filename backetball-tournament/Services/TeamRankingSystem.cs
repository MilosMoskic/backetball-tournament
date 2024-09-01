using backetball_tournament.Models;

namespace backetball_tournament.Services
{
    public class TeamRankingSystem
    {
        public void RankTeamsAcrossGroups(List<List<TeamStanding>> groupStandings)
        {
            var topTeams = groupStandings.Select(g => g.OrderByDescending(t => t.Points)
                                                       .ThenByDescending(t => t.PointsDifference)
                                                       .ThenByDescending(t => t.PointsScored).Take(3)).ToList();

            var rankedTeams = new List<TeamStanding>();

            for (int i = 0; i < 3; i++)
            {
                var rankedSubgroup = topTeams.Select(group => group.ElementAt(i))
                                                                   .OrderByDescending(t => t.Points)
                                                                   .ThenByDescending(t => t.PointsScored - t.PointsAgainst)
                                                                   .ThenByDescending(t => t.PointsScored)
                                                                   .Take(8)
                                                                   .ToList();

                rankedTeams.AddRange(rankedSubgroup);
            }

            for (int i = 0; i < rankedTeams.Count; i++)
            {
                rankedTeams[i].Rank = i + 1;
            }

            Console.WriteLine("\nRangiranje nakon grupa:");
            foreach (var team in rankedTeams)
            {
                Console.WriteLine($"Rank {team.Rank}: {team.TeamName} - {team.Points} poena, {team.PointsDifference} poen razlika, {team.PointsScored} primljeni poeni");
            }
        }
    }
}

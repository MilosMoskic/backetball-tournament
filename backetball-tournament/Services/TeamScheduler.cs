using backetball_tournament.Models;

namespace backetball_tournament.Services
{
    public class TournamentScheduler
    {
        private readonly MatchSimulator _simulator;

        public TournamentScheduler(MatchSimulator simulator)
        {
            _simulator = simulator;
        }

        public Dictionary<int, List<Match>> GenerateMatchesAndRounds(List<TeamInfo> teams)
        {
            var matches = GenerateMatches(teams);
            return OrganizeMatchesIntoRounds(matches, teams.Count);
        }

        private List<Match> GenerateMatches(List<TeamInfo> teams)
        {
            var matches = new List<Match>();
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matches.Add(new Match { TeamA = teams[i], TeamB = teams[j] });
                }
            }
            return matches;
        }

        private Dictionary<int, List<Match>> OrganizeMatchesIntoRounds(List<Match> matches, int teamCount)
        {
            var rounds = new Dictionary<int, List<Match>>();
            var round = 1;

            while (matches.Any())
            {
                var currentRoundMatches = new List<Match>();
                var teamsInCurrentRound = new HashSet<string>();

                foreach (var match in matches.ToList())
                {
                    if (!teamsInCurrentRound.Contains(match.TeamA.ISOCode) && !teamsInCurrentRound.Contains(match.TeamB.ISOCode))
                    {
                        currentRoundMatches.Add(match);
                        teamsInCurrentRound.Add(match.TeamA.ISOCode);
                        teamsInCurrentRound.Add(match.TeamB.ISOCode);
                        matches.Remove(match);

                        if (currentRoundMatches.Count == teamCount / 2)
                        {
                            break;
                        }
                    }
                }
                rounds[round++] = currentRoundMatches;
            }

            return rounds;
        }

        public void SimulateAndPrintMatches(Dictionary<int, List<Match>> rounds)
        {
            foreach (var round in rounds)
            {
                Console.WriteLine($"Kolo {round.Key}:");
                foreach (var match in round.Value)
                {
                    var (pointsA, pointsB) = _simulator.SimulateMatch(match.TeamA.FIBARanking, match.TeamB.FIBARanking);
                    Console.WriteLine($"{match.TeamA.Team} vs {match.TeamB.Team} - {pointsA} - {pointsB}");
                }
                Console.WriteLine();
            }
        }
    }

}

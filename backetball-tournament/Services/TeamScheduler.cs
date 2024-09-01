using backetball_tournament.Models;

namespace backetball_tournament.Services
{
    public class TournamentScheduler
    {
        private readonly MatchSimulator _simulator;
        private List<Match> _allMatches = new List<Match>();

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

        public void SimulateAndPrintMatches(List<Match> matches, List<TeamStanding> standings)
        {
            var rounds = new Dictionary<int, List<Match>>
            {
                { 1, matches }
            };

            foreach (var round in rounds)
            {
                Console.WriteLine($"Kolo {round.Key}:");
                foreach (var match in round.Value)
                {
                    var (pointsA, pointsB) = _simulator.SimulateMatch(match.TeamA.FIBARanking, match.TeamB.FIBARanking);
                    match.PointsA = pointsA;
                    match.PointsB = pointsB;

                    UpdateStandings(standings, match.TeamA.ISOCode, match.TeamB.ISOCode, pointsA, pointsB);

                    Console.WriteLine($"{match.TeamA.Team} vs {match.TeamB.Team} - {pointsA} - {pointsB}");
                }
                Console.WriteLine();
            }

            PrintStandings(standings);
        }


        private void UpdateStandings(List<TeamStanding> standings, string isoCodeA, string isoCodeB, int pointsA, int pointsB)
        {
            var teamA = standings.FirstOrDefault(t => t.ISOCode == isoCodeA);
            var teamB = standings.FirstOrDefault(t => t.ISOCode == isoCodeB);

            if (pointsA > pointsB)
            {
                teamA.Points += 2;
                teamA.Wins += 1;
                teamB.Losses += 1;
            }
            else if (pointsB > pointsA)
            {
                teamB.Points += 2;
                teamB.Wins += 1;
                teamA.Losses += 1;
            }
            else
            {
                teamA.Points += 1;
                teamB.Points += 1;
            }

            teamA.PointsScored += pointsA;
            teamA.PointsAgainst += pointsB;
            teamB.PointsScored += pointsB;
            teamB.PointsAgainst += pointsA;
        }

        private void PrintStandings(List<TeamStanding> standings)
        {
            var sortedStandings = standings
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.Wins)
                .ThenByDescending(s => s.PointsDifference)
                .ToList();

            Console.WriteLine("Rank  Naziv Tima       Poeni  Pobede  Gubitci  Osvojeni  Primljeni  Razlika");
            foreach (var standing in sortedStandings)
            {
                Console.WriteLine($"{sortedStandings.IndexOf(standing) + 1,-5} {standing.TeamName,-20} {standing.Points,-6} {standing.Wins,-4} {standing.Losses,-6} {standing.PointsScored,-6} {standing.PointsAgainst,-7} {standing.PointsDifference,-10}");
            }
        }

        public List<Match> GetAllMatches()
        {
            return _allMatches;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using backetball_tournament.Models;

namespace backetball_tournament.Services
{
    public class EliminationPhaseScheduler
    {
        private readonly Random _random;
        private readonly MatchSimulator _matchSimulator;

        public EliminationPhaseScheduler(MatchSimulator matchSimulator)
        {
            _random = new Random();
            _matchSimulator = matchSimulator;
        }

        public void RunEliminationPhase(List<TeamStanding> rankedTeams, List<Match> groupStageMatches)
        {
            var pots = CreatePots(rankedTeams);

            var quarterFinalWinners = GenerateQuarterFinals(pots, groupStageMatches);

            var semiFinals = GenerateSemiFinals(quarterFinalWinners);

            GenerateFinals(semiFinals);

        }

        private Dictionary<string, List<TeamStanding>> CreatePots(List<TeamStanding> rankedTeams)
        {
            var pots = new Dictionary<string, List<TeamStanding>>
            {
                { "D", new List<TeamStanding> { rankedTeams[0], rankedTeams[1] } },
                { "E", new List<TeamStanding> { rankedTeams[2], rankedTeams[3] } },
                { "F", new List<TeamStanding> { rankedTeams[4], rankedTeams[5] } },
                { "G", new List<TeamStanding> { rankedTeams[6], rankedTeams[7] } }
            };

            Console.WriteLine("\nŠeširi:");
            foreach (var pot in pots)
            {
                Console.WriteLine($"    Šešir {pot.Key}");
                foreach (var team in pot.Value)
                {
                    Console.WriteLine($"        {team.TeamName}");
                }
            }

            return pots;
        }

        private List<TeamInfo> GenerateQuarterFinals(Dictionary<string, List<TeamStanding>> pots, List<Match> groupStageMatches)
        {
            var quarterFinals = new List<Match>();

            // Generate the quarter-final matches
            quarterFinals.AddRange(GeneratePairings(pots["D"], pots["G"], groupStageMatches));
            quarterFinals.AddRange(GeneratePairings(pots["E"], pots["F"], groupStageMatches));

            // Simulate and display results for each quarter-final match
            Console.WriteLine("\nČetvrtine Finala:");
            var quarterFinalResults = new List<TeamInfo>();
            foreach (var quarterFinal in quarterFinals)
            {
                var (pointsA, pointsB) = _matchSimulator.SimulateMatch(quarterFinal.TeamA.FIBARanking, quarterFinal.TeamB.FIBARanking);

                var winner = pointsA > pointsB ? quarterFinal.TeamA : quarterFinal.TeamB;
                var loser = pointsA > pointsB ? quarterFinal.TeamB : quarterFinal.TeamA;

                Console.WriteLine($"{quarterFinal.TeamA.Team} vs {quarterFinal.TeamB.Team} - {pointsA} - {pointsB}");

                // Store the winner for the next round
                quarterFinalResults.Add(winner);
            }

            return quarterFinalResults;
        }

        private List<Match> GeneratePairings(List<TeamStanding> potA, List<TeamStanding> potB, List<Match> groupStageMatches)
        {
            var pairings = new List<Match>();
            var shuffledPotA = potA.OrderBy(_ => _random.Next()).ToList();
            var shuffledPotB = potB.OrderBy(_ => _random.Next()).ToList();

            while (shuffledPotA.Any() && shuffledPotB.Any())
            {
                var teamA = shuffledPotA.First();
                TeamStanding opponent = null;

                foreach (var teamB in shuffledPotB)
                {
                    if (!HavePlayedEachOther(teamA, teamB, groupStageMatches))
                    {
                        opponent = teamB;
                        break;
                    }
                }

                if (opponent == null)
                {
                    // If no valid opponent is found, just select the first team from potB
                    opponent = shuffledPotB.First();
                }

                pairings.Add(new Match { TeamA = teamA.TeamInfo, TeamB = opponent.TeamInfo });
                shuffledPotA.Remove(teamA);
                shuffledPotB.Remove(opponent);
            }

            return pairings;
        }

        private bool HavePlayedEachOther(TeamStanding teamA, TeamStanding teamB, List<Match> groupStageMatches)
        {
            return groupStageMatches.Any(match =>
                (match.TeamA.Team == teamA.TeamInfo.Team && match.TeamB.Team == teamB.TeamInfo.Team) ||
                (match.TeamA.Team == teamB.TeamInfo.Team && match.TeamB.Team == teamA.TeamInfo.Team));
        }

        private List<Match> GenerateSemiFinals(List<TeamInfo> quarterFinalWinners)
        {
            var semiFinals = new List<Match>();

            var shuffledWinners = quarterFinalWinners.OrderBy(_ => _random.Next()).ToList();
            semiFinals.Add(new Match { TeamA = shuffledWinners[0], TeamB = shuffledWinners[1] });
            semiFinals.Add(new Match { TeamA = shuffledWinners[2], TeamB = shuffledWinners[3] });

            return semiFinals;
        }

        private void GenerateFinals(List<Match> semiFinals)
        {
            var semiFinalResults = new Dictionary<Match, (TeamInfo winner, TeamInfo loser)>();

            // Simulate and display results for each semi-final match
            Console.WriteLine("\nPolufinala:");
            foreach (var semiFinal in semiFinals)
            {
                var (pointsA, pointsB) = _matchSimulator.SimulateMatch(semiFinal.TeamA.FIBARanking, semiFinal.TeamB.FIBARanking);

                var winner = pointsA > pointsB ? semiFinal.TeamA : semiFinal.TeamB;
                var loser = pointsA > pointsB ? semiFinal.TeamB : semiFinal.TeamA;

                semiFinalResults[semiFinal] = (winner, loser);

                Console.WriteLine($"{semiFinal.TeamA.Team} vs {semiFinal.TeamB.Team} - {pointsA} - {pointsB}");
            }

            // Determine the teams for the final and third-place match
            var winners = semiFinalResults.Values.Select(result => result.winner).ToList();
            var losers = semiFinalResults.Values.Select(result => result.loser).ToList();

            var thirdPlaceMatch = new Match { TeamA = losers[0], TeamB = losers[1] };
            var finalMatch = new Match { TeamA = winners[0], TeamB = winners[1] };

            // Simulate and display the final match
            Console.WriteLine("\nFinale:");
            var (finalPointsA, finalPointsB) = _matchSimulator.SimulateMatch(finalMatch.TeamA.FIBARanking, finalMatch.TeamB.FIBARanking);
            var finalWinner = finalPointsA > finalPointsB ? finalMatch.TeamA : finalMatch.TeamB;
            var finalLoser = finalPointsA > finalPointsB ? finalMatch.TeamB : finalMatch.TeamA;

            Console.WriteLine($"{finalMatch.TeamA.Team} vs {finalMatch.TeamB.Team} - {finalPointsA} - {finalPointsB}");

            // Simulate and display the third-place match
            Console.WriteLine("\nTreće mesto:");
            var (thirdPlacePointsA, thirdPlacePointsB) = _matchSimulator.SimulateMatch(thirdPlaceMatch.TeamA.FIBARanking, thirdPlaceMatch.TeamB.FIBARanking);
            var thirdPlaceWinner = thirdPlacePointsA > thirdPlacePointsB ? thirdPlaceMatch.TeamA : thirdPlaceMatch.TeamB;

            Console.WriteLine($"{thirdPlaceMatch.TeamA.Team} vs {thirdPlaceMatch.TeamB.Team}  - {thirdPlacePointsA} - {thirdPlacePointsB}");

            Console.WriteLine("\nMedalje:");
            Console.WriteLine($"1. {finalWinner.Team}");
            Console.WriteLine($"2. {finalLoser.Team}");
            Console.WriteLine($"3. {thirdPlaceWinner.Team}");
        }
    }
}

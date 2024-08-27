namespace backetball_tournament.Services
{
    public class MatchSimulator
    {
        private Random random = new Random();

        public (int pointsA, int pointsB) SimulateMatch(int rankingA, int rankingB)
        {
            int basePointsA = random.Next(70, 101);
            int basePointsB = random.Next(70, 101);

            double winProbabilityA = 1.0 / (1.0 + Math.Exp((rankingB - rankingA) / -10.0));
            if (random.NextDouble() < winProbabilityA)
            {
                basePointsA += random.Next(5, 16);
                if (basePointsB >= basePointsA)
                    basePointsB = basePointsA - 1;
            }
            else
            {
                basePointsB += random.Next(5, 16);
                if (basePointsA >= basePointsB)
                    basePointsA = basePointsB - 1;
            }

            return (basePointsA, basePointsB);
        }
    }
}

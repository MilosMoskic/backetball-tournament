namespace backetball_tournament.Models
{
    public class Match
    {
        public TeamInfo TeamA { get; set; }
        public TeamInfo TeamB { get; set; }
        public int PointsA { get; set; }
        public int PointsB { get; set; }
    }
}

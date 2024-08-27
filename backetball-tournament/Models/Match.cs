namespace backetball_tournament.Models
{
    public class Match
    {
        public TeamInfo TeamA { get; set; }
        public TeamInfo TeamB { get; set; }
        public int Round { get; set; }
    }
}

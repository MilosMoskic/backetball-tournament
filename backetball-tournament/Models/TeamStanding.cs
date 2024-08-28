namespace backetball_tournament.Models
{
    public class TeamStanding
    {
        public string ISOCode { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsScored { get; set; }
        public int PointsAgainst { get; set; }
        public int PointsDifference => PointsScored - PointsAgainst;
        public int Rank { get; set; }
    }
}

using backetball_tournament.Models;

namespace backetball_tournament.Services
{
    public class TeamStandingService
    {
        public List<TeamStanding> ConvertTeamsToStandings(IEnumerable<TeamInfo> teams)
        {
            return teams.Select(t => new TeamStanding
            {
                ISOCode = t.ISOCode,
                TeamName = t.Team,
                TeamInfo = t,
                Points = 0
            }).ToList();
        }
    }
}
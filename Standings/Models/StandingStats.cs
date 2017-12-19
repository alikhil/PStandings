using System.Collections.Generic;

namespace Standings.Models
{
    public class StandingStats
    {
        public List<string> Contests { get; set; }
        public List<UserResult> Results { get; set; }
        
    }
}
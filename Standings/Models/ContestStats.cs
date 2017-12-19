using System.Collections.Generic;

namespace Standings.Models
{
    public class ContestStats
    {
        public List<Problem> Problems { get; set; }
        public List<ContestUserResult> Results { get; set; }
        
        public string Name { get; set; }
        
    }
}
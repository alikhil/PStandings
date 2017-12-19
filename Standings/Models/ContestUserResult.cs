using System.Collections.Generic;

namespace Standings.Models
{
    public class ContestUserResult
    {
        public int TotalSolved { get; set; }
        public string Name { get; set; }

        public List<string> ProblemsStatus { get; set; }
    }
}
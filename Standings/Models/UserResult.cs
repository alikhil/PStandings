using System.Collections.Generic;

namespace Standings.Models
{
    public class UserResult
    {
        public UserResult(string name, int totalSolved, List<int> solved)
        {
            this.Name = name;
            this.TotalSolved = totalSolved;
            this.SolvedInContests = solved;
        }
        public string Name { get; set; }
        public int TotalSolved { get; set; }
        public List<int> SolvedInContests { get; set; }

    }
}
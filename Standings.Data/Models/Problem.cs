using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Standings.Data.Models
{
    public class Problem
    {
        public string Alias { get; set; }
        public string Name { get; set; }

        public string Id { get; set; }
        public Contest Contest { get; set; }
        
        public ICollection<Submission> Submissions { get; set; }

        public ICollection<ProblemStudent> Submitters { get; set; }
        
    }

}
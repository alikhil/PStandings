using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Standings.Data.Models
{
    public class Contest
    {
        [Key]
        public string PcmsId { get; set; }
        public string Name { get; set; }
        
        // For admin purposes
        public bool Hidden { get; set; }
        // GR1, GR2, ...
        public string Generation { get; set; }
        
        public ICollection<Submission> Submissions { get; set; }
        public ICollection<Problem> Problems { get; set; }
        // public ICollection<Student> Participants { get; set; }
    }
}
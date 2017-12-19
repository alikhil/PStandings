using System.ComponentModel.DataAnnotations;

namespace Standings.Data.Models
{
    public class Submission
    {
        public long Time { get; set; }

        public string ContestId { get; set; }
        public Contest Contest { get; set; }
        
        public string ProblemId { get; set; } 
        public Problem Problem { get; set; }
        
        public string SubmitterId { get; set; }
        public Student Submitter { get; set; }
        public bool IsAccepted { get; set; }
        
    }
}
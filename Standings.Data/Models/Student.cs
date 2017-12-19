using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Standings.Data.Models
{
    public class Student
    {
        [Key]
        public string Name { get; set; }
        public ICollection<Submission> Submissions { get; set; }

        // public ICollection<Contest> Contests { get; set; }

        public ICollection<ProblemStudent> SubmittedProblems { get; set; }
        
    }

    public class ProblemStudent
    {
        public string StudentId { get; set; }
        public Student Student { get; set; }
        
        public string ProblemId { get; set; }
        public Problem Problem { get; set; }
        
    }
}
namespace Standings.Models
{
    public class Submission 
    {
        public string Username { get; set; }
        public long Time { get; set; }
        public bool Accepted { get; set; }
        public string ProblemAlias { get; set; }
        public string Contest { get; set; }
        public string ProblemTitle { get; set; }

    }
}
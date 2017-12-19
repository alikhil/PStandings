using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Standings.Models;

namespace Standings.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        public async Task<IEnumerable<Submission>> FindWhichStartsWith(string prefix = "", int count = 5, int page = 0)
        {
            prefix = prefix ?? ""; 
            var list = new List<Submission> {
                new Submission { Username = "G1 Alik", ProblemAlias= "A", Time = 1, Accepted = true, ProblemTitle = "A in B", Contest = "Strings" },
                new Submission { Username = "G1 Timur", ProblemAlias= "A", Time = 2, Accepted = false, ProblemTitle = "A in B", Contest = "Strings" },
                new Submission { Username = "G2 Ayrat", ProblemAlias= "A", Time = 3, Accepted = false, ProblemTitle = "Min distance", Contest = "Graphs" },
                new Submission { Username = "G2 Ruslan", ProblemAlias= "B", Time = 4, Accepted = true, ProblemTitle = "Area of polygon", Contest = "Geometry" },
                new Submission { Username = "G3 Vlad", ProblemAlias= "C", Time = 5, Accepted = false, ProblemTitle = "A in B", Contest = "Strings" }
            };
            list.Reverse();
            return await Task.FromResult(
                list.Where(s => s.Username.StartsWith(prefix))
                    .Skip(count * page)
                    .Take(count)
            );
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Standings.Models;

namespace Standings.Repositories
{
    public interface ISubmissionRepository
    {
         Task<IEnumerable<Submission>> FindWhichStartsWith(string prefix = "", int count = 5, int offset = 0);
    }
}
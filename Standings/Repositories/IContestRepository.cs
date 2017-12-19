using System.Collections.Generic;
using System.Threading.Tasks;

namespace Standings.Repositories
{
   public interface IContestRepository
    {
        Task<IEnumerable<Contest>> GetAll();
    }
}
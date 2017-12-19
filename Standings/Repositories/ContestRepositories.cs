using System.Collections.Generic;
using System.Threading.Tasks;

namespace Standings.Repositories
{
     public class ContestRepository : IContestRepository
    {
        public ContestRepository()
        {
        }

        public async Task<IEnumerable<Contest>> GetAll()
        {
             return await Task.FromResult(new Contest[] { 
                new Contest() { Name = "first contest", Status = "over", Id = "1", Generation = "GR1" },
                new Contest() { Name = "first contest", Status = "over", Id = "2", Generation = "GR2" },
                new Contest() { Name = "second contest", Status = "over", Id = "5", Generation = "GR2" },
                new Contest() { Name = "final contest", Status = "running", Id = "3", Generation = "GR1" },
                new Contest() { Name = "final contest", Status = "running", Id = "4", Generation = "GR2"}
            });
        }
    }
}
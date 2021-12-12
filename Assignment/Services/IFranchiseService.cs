using Assignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public interface IFranchiseService
    {
        public Task<IEnumerable<Franchise>> GetFranchises();
        public Task<Franchise> GetFranchise(int id);
        public Task<Franchise> GetFranchiseByName(string name);
        public Task<IEnumerable<Franchise>> GetSomeFranchises(int offset, int number);
        public Task PutFranchise(int id, Franchise franchise);
        public Task<Franchise> PostFranchise(Franchise franchise);
        public Task DeleteFranchise(int id);
        public Task UpdateMoviesInFranchise(int id, List<int> movies);
        public bool FranchiseExists(int id);

    }
}

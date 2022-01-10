using Assignment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public class FranchiseService : IFranchiseService
    {
        private readonly AssignmentDbContext _context;
        // Constructor with context
        public FranchiseService(AssignmentDbContext context)
        {
            _context = context;
        }

        public async Task DeleteFranchise(int id)
        {
            // Remove this franchise from all movies with it in, setting franchise ID to 1(which is "No known franchise or missing information")
            foreach (Movie movie in _context.Movies)
            {
                if (movie.FranchiseId == id)
                {
                    movie.FranchiseId = 1;
                }
            }
            // Find and remove franchise
            var franchise = await _context.Franchises.FindAsync(id);
            _context.Franchises.Remove(franchise);
            await _context.SaveChangesAsync();
        }

        public bool FranchiseExists(int id)
        {
            // return true if ID is found in set
            return _context.Franchises.Any(e => e.Id == id);
        }
        public bool NameExistInDB(Franchise franchise)
        {
            return _context.Franchises.Any(e => e.Name == franchise.Name);
        }

        public async Task<Franchise> GetFranchise(int id)
        {
            // return Franchise by ID
            return await _context.Franchises.FindAsync(id);
        }

        public async Task<Franchise> GetFranchiseByName(string name)
        {
            // Find Franchise by exact name(ignoring upper/lower)
            return await _context.Franchises.Where(m => m.Name == name).FirstAsync();
        }

        public async Task<IEnumerable<Franchise>> GetFranchises()
        {
            // Get all Franchises
            return await _context.Franchises.ToListAsync();
        }

        public async Task<IEnumerable<Franchise>> GetSomeFranchises(int offset, int number)
        {
            return await _context.Franchises
                .Skip(offset)
                .Take(number)
                .ToListAsync();
        }

        public async Task<Franchise> PostFranchise(Franchise franchise)
        {
            // Save new franchise if name does not exist in db
            if (!NameExistInDB(franchise))
            {
                _context.Franchises.Add(franchise);
                await _context.SaveChangesAsync();
                return franchise;
            }
            else
            {
                return null;
            }
        }

        public async Task PutFranchise(int id, Franchise franchise)
        {
            // Make EF update franchise
            _context.Entry(franchise).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task UpdateMoviesInFranchise(int id, List<int> movies)
        {
            // Load the franchise with a collection of current movies
            Franchise franchiseToUpdateMovies = await _context.Franchises
                .Include(c => c.Movies)
                .Where(c => c.Id == id)
                .FirstAsync();

            // Loop through the list of movies
            foreach (int movId in movies)
            {
                // Try to find the movie
                Movie mov = await _context.Movies.FindAsync(movId);
                if (mov != null)
                {
                    // If the franchise does not contain the movie, add it
                    if (!franchiseToUpdateMovies.Movies.Contains(mov))
                    {
                        franchiseToUpdateMovies.Movies.Add(mov);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

using Assignment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public class MovieService : IMovieService
    {
        private readonly AssignmentDbContext _context;
        public MovieService(AssignmentDbContext context)
        {
            _context = context;
        }
        public async Task DeleteMovie(int id)
        {
            // get movie if it exists
            var movie = await _context.Movies.FindAsync(id);
            // try to remove it and save changes
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<Movie> GetMovie(int id)
        {
            // Find the movie
            var movie = await _context.Movies.FindAsync(id);
            // load movies into the object
            await _context.Entry(movie).Collection(i => i.Characters).LoadAsync();
            return movie;
        }

        public async Task<Movie> GetMovieByName(string name)
        {
            // Find movie with name as Title
            Movie movie = await _context.Movies.Where(m => m.Title == name).FirstAsync();
            // load the characters
            await _context.Entry(movie).Collection(i => i.Characters).LoadAsync();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            // return all movies with characters
            return await _context.Movies
                .Include(c => c.Characters)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetSomeMovies(int offset, int number)
        {
            // return list of (number) movies, starting at (offset)
            return await _context.Movies
                .Skip(offset)
                .Take(number)
                .ToListAsync();
        }

        public bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        public async Task<Movie> PostMovie(Movie movie)
        {

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task PutMovie(int id, Movie movie)
        {
            // Update and save movie
            _context.Entry(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCharactersInMovie(int id, List<int> characters)
        {
            // Get the movie from id, and its characters
            Movie movieToUpdateCharacters = await _context.Movies
                .Include(c => c.Characters)
                .Where(c => c.Id == id)
                .FirstAsync();

            // Loop through character IDs, test and assign to Movies character list
            foreach (int chaId in characters)
            {
                Character chara = await _context.Characters.FindAsync(chaId);
                // Cant find a character with that ID, so we continue to next.
                if (chara == null) continue ;
                // If character is in the list, we continue to next.  No need for doubles
                if (movieToUpdateCharacters.Characters.Contains(chara)) continue;
                movieToUpdateCharacters.Characters.Add(chara);
            }
            await _context.SaveChangesAsync();
        }
    }
}

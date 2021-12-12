using Assignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public interface IMovieService
    {
        public Task<IEnumerable<Movie>> GetMovies();
        public Task<Movie> GetMovie(int id);
        public Task<Movie> GetMovieByName(string name);
        public Task<IEnumerable<Movie>> GetSomeMovies(int offset, int number);
        public Task PutMovie(int id, Movie movie);
        public Task<Movie> PostMovie(Movie movie);
        public Task DeleteMovie(int id);
        public Task UpdateCharactersInMovie(int id, List<int> movies);
        public bool MovieExists(int id);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment.Models;
using System.Net.Mime;
using AutoMapper;
using Assignment.Models.DTO.Movie;

namespace Assignment.Controllers
{
    /// <summary>
    /// Controller for the Movies
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class MovieController : ControllerBase
    {
        private readonly AssignmentDbContext _context;
        private readonly IMapper _mapper;
        
        /// <summary>
        /// Constructor with context and mapper
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public MovieController(AssignmentDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Return all movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetMovies()
        {
            return _mapper.Map<List<MovieReadDTO>>(await _context.Movies
                .Include(c => c.Characters)
                .ToListAsync());
        }

        /// <summary>
        /// Return movie with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieReadDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            await _context.Entry(movie).Collection(i => i.Characters).LoadAsync();
            return _mapper.Map<MovieReadDTO>(movie);
        }

        /// <summary>
        /// Return movies with franchiseId == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/byFranchise")]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetMoviesByFranchise(int id)
        {
            var movie = await _context.Movies.Where(m => m.FranchiseId == id)
                .Include(c => c.Characters)
                .ToListAsync();

            if (movie.Count == 0)
            {
                return NotFound();
            }
            return _mapper.Map<List<MovieReadDTO>>(movie);
        }

        /// <summary>
        /// Return movie by title
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}/byname")]
        public async Task<ActionResult<MovieReadDTO>> GetMovieByName(string name)
        {
            Movie movie;
            try
            {
                movie = await _context.Movies.Where(m => m.Title == name).FirstAsync();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            await _context.Entry(movie).Collection(i => i.Characters).LoadAsync();
            return _mapper.Map<MovieReadDTO>(movie);
        }

        /// <summary>
        /// Gets a selection of movies.  Offset==how many you would like to skip.  Number==how many you want returned
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{offset}/group")]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetSomeMovies(int offset, int number)
        {
            // return BadRequest if number is too low
            if (number < 1) return BadRequest("Number < 1");

            List<Movie> movies;
            // getting the requested character records
            try
            {
                movies = await _context.Movies
                .Skip(offset)
                .Take(number)
                .ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            if (movies.Count == 0)
            {
                return NotFound("No records found");
            }
            // Gets all FranchiseDTO's
            return _mapper.Map<List<MovieReadDTO>>(movies);
        }


        /// <summary>
        /// Update the movie with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieEditDTO movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            Movie rMovie = _mapper.Map<Movie>(movie);
            _context.Entry(rMovie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Add/create a new Movie(default franchise is id=1, unknown or none)
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(MovieCreateDTO movie)
        {
            Movie rMovie = _mapper.Map<Movie>(movie);
            _context.Movies.Add(rMovie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = rMovie.Id }, movie);
        }


        /// <summary>
        /// Delete Movie with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Update characters in a movie(delete old list).  Movie id, List if character IDs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        [HttpPut("{id}/characters")]
        public async Task<IActionResult> UpdateCharactersInMovie(int id, List<int> characters)
        {
            // If the moveie doesnt exist, return NotFound
            if (!MovieExists(id))
            {
                return NotFound();
            }
            
            Movie movieToUpdateCharacters = await _context.Movies
                .Include(c => c.Characters)
                .Where(c => c.Id == id)
                .FirstAsync();

            foreach (int chaId in characters)
            {
                Character chara = await _context.Characters.FindAsync(chaId);
                // Cant find a character with that ID, so we continue to next.
                if (chara == null) continue;
                // If character is in the list, we continue to next.  No need for doubles
                if (movieToUpdateCharacters.Characters.Contains(chara)) continue;
                movieToUpdateCharacters.Characters.Add(chara);
            }
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }





        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}

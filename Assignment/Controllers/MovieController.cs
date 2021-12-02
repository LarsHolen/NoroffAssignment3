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
        /// Add/create a new Movie
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
        /// Adds characters to a movie.  Movie id, List if character IDs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        [HttpPut("{id}/characters")]
        public async Task<IActionResult> UpdateCharactersInMovie(int id, List<int> characters)
        {
            if (!MovieExists(id))
            {
                return NotFound();
            }
            
            Movie movieToUpdateCharacters = await _context.Movies
                .Include(c => c.Characters)
                .Where(c => c.Id == id)
                .FirstAsync();
            
            // Loop through character IDs, try and assign to coach
            // Trying to see if there is a nicer way of doing this, dont like the multiple calls
            List<Character> charas = new();
            foreach (int chaId in characters)
            {
                Character chara = await _context.Characters.FindAsync(chaId);
                if (chara == null)
                    return BadRequest("Character does not exist!");
                charas.Add(chara);
            }
            movieToUpdateCharacters.Characters = charas;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }





        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}

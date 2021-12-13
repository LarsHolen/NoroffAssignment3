using Assignment.Models;
using Assignment.Models.DTO.Movie;
using Assignment.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class MovieServiceController : ControllerBase
    {
        // Add automapper DI
        private readonly IMapper _mapper;
        // Controllers still are responsible for mapping, services/repositories work with domain objects
        private readonly IMovieService _movieService;

        // Constructor
        public MovieServiceController(IMapper mapper, IMovieService movieService)
        {
            _mapper = mapper;
            _movieService = movieService;
        }

        /// <summary>
        /// Get all movies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetMovies()
        {
            // Call service 
            return _mapper.Map<List<MovieReadDTO>>(await _movieService.GetMovies());
        }

        /// <summary>
        /// Gets a specific movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieReadDTO>> GetMovie(int id)
        {
            // Call service
            Movie movie = await _movieService.GetMovie(id);
            // If null, return 404
            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieReadDTO>(movie);
        }

        /// <summary>
        /// Returns a movie by title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpGet("{title}/bytitle")]
        public async Task<ActionResult<MovieReadDTO>> GetMovieByName(string title)
        {
            Movie movie;
            try
            {
                // call service
                movie = await _movieService.GetMovieByName(title);
            }
            catch (Exception e)
            {
                // on exception, return 404 with exception message
                return NotFound(e.Message);
            }
            return _mapper.Map<MovieReadDTO>(movie);
        }

        /// <summary>
        /// Gets a selection of movies.  Skip offset and select the next number records
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{offset}/group")]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetSomeMovies(int offset, int number)
        {
            // return BadRequest if offset or number is too low
            if (offset < 0) return BadRequest("Offset < 0");
            if (number < 1) return BadRequest("Number < 1");

            List<Movie> movies;
            // getting the requested character records
            try
            {
                // Call service
                movies = (List<Movie>)await _movieService.GetSomeMovies(offset, number);
            }
            catch (Exception e)
            {
                // return 400 with message on exception
                return BadRequest(e.Message);
            }
            // If the list is empty, return 404 NotFound
            if (movies.Count == 0)
            {
                return NotFound("No records found");
            }

            // Return CharacterDTO's
            return _mapper.Map<List<MovieReadDTO>>(movies);
        }

        /// <summary>
        /// Changes movie with id == {id}, to movie in params
        /// </summary>
        /// <param name="id"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieEditDTO movie)
        {
            // Check if the correct ID is in
            if (id != movie.Id)
            {
                return BadRequest("id must be the same as movie.id");
            }

            // Create a Character object from the DTO
            Movie rMovie = _mapper.Map<Movie>(movie);

            // catching errors
            try
            {
                // Calling service
                await _movieService.PutMovie(id, rMovie);
            }
            catch (Exception e)
            {
                // Testing if the id can be found, and if not return 404
                if (!_movieService.MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Not quite sure what errors we can get here.  Dropping a badRequest with the event message
                    // if we get an exception and the id exist
                    return BadRequest(e.Message);
                }
            }
            return NoContent();
        }

        /// <summary>
        /// Add a new movie to db
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Character>> PostMovie(MovieCreateDTO movie)
        {
            // Making sure the movie to be created atleast have a title.  "string" is default in swagger
            if (movie.Title == "string" || movie.Title == string.Empty)
            {
                return BadRequest("Please add a Title");
            }
            // Mapping movie DTO to a Movie object
            Movie movieToAdd = _mapper.Map<Movie>(movie);

            // Calling service
            await _movieService.PostMovie(movieToAdd);

            if (movieToAdd.Id == 0) return BadRequest("Duplicate title!");

            return CreatedAtAction("GetMovie",
                new { id = movieToAdd.Id },
                _mapper.Map<MovieReadDTO>(movieToAdd));
        }

        /// <summary>
        ///  Deletes a movie from DB with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                // Calling service
                await _movieService.DeleteMovie(id);
            }
            catch (Exception e)
            {
                // Return 404 if not found pluss message
                return NotFound(e.Message);
            }
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

            try
            {
                await _movieService.UpdateCharactersInMovie(id, characters);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
            return NoContent();
        }


        public bool MovieExists(int id)
        {
            // Call service
            return _movieService.MovieExists(id);
        }

    }
}

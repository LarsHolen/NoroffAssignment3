using Assignment.Models;
using Assignment.Models.DTO.Franchise;
using Assignment.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class FranchiseServiceController : ControllerBase
    {
        // Add automapper DI
        private readonly IMapper _mapper;
        // Controllers still are responsible for mapping, services/repositories work with domain objects
        private readonly IFranchiseService _franchiseService;

        // Constructor
        public FranchiseServiceController(IMapper mapper, IFranchiseService franchiseService)
        {
            _mapper = mapper;
            _franchiseService = franchiseService;
        }

        /// <summary>
        /// Get all franchises.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FranchiseReadDTO>>> GetCharacters()
        {
            // Call service 
            return _mapper.Map<List<FranchiseReadDTO>>(await _franchiseService.GetFranchises());
        }

        /// <summary>
        /// Gets a specific franchise by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FranchiseReadDTO>> GetFranchise(int id)
        {
            // Call service
            Franchise franchise = await _franchiseService.GetFranchise(id);
            // If null, return 404
            if (franchise == null)
            {
                return NotFound();
            }

            return _mapper.Map<FranchiseReadDTO>(franchise);
        }

        /// <summary>
        /// Returns a franchise by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}/byname")]
        public async Task<ActionResult<FranchiseReadDTO>> GetFranchiseByName(string name)
        {
            Franchise franchise;
            try
            {
                // call service
                franchise = await _franchiseService.GetFranchiseByName(name);
            }
            catch (Exception e)
            {
                // on exception, return 404 with exception message
                return NotFound(e.Message);
            }
            return _mapper.Map<FranchiseReadDTO>(franchise);
        }

        /// <summary>
        /// Gets a selection of franchises.  Skip offset and select the next number records
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{offset}/group")]
        public async Task<ActionResult<IEnumerable<FranchiseReadDTO>>> GetSomeFranchises(int offset, int number)
        {
            // return BadRequest if offset or number is too low
            if (offset < 0) return BadRequest("Offset < 0");
            if (number < 1) return BadRequest("Number < 1");

            List<Franchise> franchises;
            // getting the requested franchise records
            try
            {
                // Call service
                franchises = (List<Franchise>)await _franchiseService.GetSomeFranchises(offset, number);
            }
            catch (Exception e)
            {
                // return 400 with message on exception
                return BadRequest(e.Message);
            }
            // If the list is empty, return 404 NotFound
            if (franchises.Count == 0)
            {
                return NotFound("No records found");
            }

            // Return CharacterDTO's
            return _mapper.Map<List<FranchiseReadDTO>>(franchises);
        }

        /// <summary>
        /// Changes franchise with id == {id}, to franchise in params
        /// </summary>
        /// <param name="id"></param>
        /// <param name="franchise"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFranchise(int id, FranchiseReadDTO franchise)
        {
            // Check if the correct ID is in
            if (id != franchise.Id)
            {
                return BadRequest("id must be the same as character.id");
            }

            // Create a Franchise object from the DTO
            Franchise rFran = _mapper.Map<Franchise>(franchise);

            // catching errors
            try
            {
                // Calling service
                await _franchiseService.PutFranchise(id, rFran);
            }
            catch (Exception e)
            {
                // Testing if the id can be found, and if not return 404
                if (!_franchiseService.FranchiseExists(id))
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
        /// Add a new franchise to db
        /// </summary>
        /// <param name="franchise"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Character>> PostCharacter(FranchiseCreateDTO franchise)
        {
            // Making sure the character to be created atleast have a name.  "string" is default in swagger
            if (franchise.Name == "string" || franchise.Name == string.Empty)
            {
                return BadRequest("Please add a name");
            }
            // Mapping FranchiseDTO to a Franchise object
            Franchise franchiseToAdd = _mapper.Map<Franchise>(franchise);

            // Calling service
            await _franchiseService.PostFranchise(franchiseToAdd);

            return CreatedAtAction("GetFranchise",
                new { id = franchiseToAdd.Id },
                _mapper.Map<FranchiseReadDTO>(franchiseToAdd));
        }

        /// <summary>
        ///  Deletes a franchise from DB with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFranchise(int id)
        {
            try
            {
                // Calling service
                await _franchiseService.DeleteFranchise(id);
            }
            catch (Exception e)
            {
                // Return 404 if not found pluss message
                return NotFound(e.Message);
            }
            return NoContent();
        }

        /// <summary>
        /// Takes an franchise ID, and add the franchise to a list of movies
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movies"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateMoviesInFranchise(int id, List<int> movies)
        {
            // Abort if ID is 0 or less, and if list is empty
            if (id <= 0 || movies.Count == 0)
            {
                return BadRequest("Invalid id or empty list");
            }

            // try to call service
            try
            {
                await _franchiseService.UpdateMoviesInFranchise(id, movies);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }

        public bool CharacterExists(int id)
        {
            // Call service
            return _franchiseService.FranchiseExists(id);
        }

    }
}

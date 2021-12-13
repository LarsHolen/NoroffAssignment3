using Assignment.Models;
using Assignment.Models.DTO.Character;
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
    public class CharacterServiceController : ControllerBase
    {
        // Add automapper DI
        private readonly IMapper _mapper;
        // Controllers still are responsible for mapping, services/repositories work with domain objects
        private readonly ICharacterService _characterService;

        // Constructor
        public CharacterServiceController(IMapper mapper, ICharacterService characterService)
        {
            _mapper = mapper;
            _characterService = characterService;
        }

        /// <summary>
        /// Get all characters.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadDTO>>> GetCharacters()
        {
            // Call service 
            return _mapper.Map<List<CharacterReadDTO>>(await _characterService.GetCharacters());
        }

        /// <summary>
        /// Gets a specific character by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterReadDTO>> GetCharacter(int id)
        {
            // Call service
            Character character = await _characterService.GetCharacter(id);
            // If null, return 404
            if (character == null)
            {
                return NotFound();
            }

            return _mapper.Map<CharacterReadDTO>(character);
        }

        /// <summary>
        /// Returns a character by full name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}/byname")]
        public async Task<ActionResult<CharacterReadDTO>> GetCharacterByName(string name)
        {
            Character character;
            try
            {
                // call service
                character = await _characterService.GetCharacterByName(name);
            }
            catch (Exception e)
            {
                // on exception, return 404 with exception message
                return NotFound(e.Message);
            }
            return _mapper.Map<CharacterReadDTO>(character);
        }

        /// <summary>
        /// Gets a selection of characters.  Skip offset and select the next number records
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{offset}/group")]
        public async Task<ActionResult<IEnumerable<CharacterReadDTO>>> GetSomeCharacters(int offset, int number)
        {
            // return BadRequest if offset or number is too low
            if (offset < 0) return BadRequest("Offset < 0");
            if (number < 1) return BadRequest("Number < 1");

            List<Character> characters;
            // getting the requested character records
            try
            {
                // Call service
                characters = (List<Character>)await _characterService.GetSomeCharacters(offset, number);
            }
            catch (Exception e)
            {
                // return 400 with message on exception
                return BadRequest(e.Message);
            }
            // If the list is empty, return 404 NotFound
            if (characters.Count == 0)
            {
                return NotFound("No records found");
            }

            // Return CharacterDTO's
            return _mapper.Map<List<CharacterReadDTO>>(characters);
        }

        /// <summary>
        /// Changes character with id == {id}, to character in params
        /// </summary>
        /// <param name="id"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCharacter(int id, CharacterEditDTO character)
        {
            // Check if the correct ID is in
            if (id != character.Id)
            {
                return BadRequest("id must be the same as character.id");
            }

            // Create a Character object from the DTO
            Character rChar = _mapper.Map<Character>(character);

            // catching errors
            try
            {
                // Calling service
                await _characterService.PutCharacter(id, rChar);
            }
            catch (Exception e)
            {
                // Testing if the id can be found, and if not return 404
                if (!_characterService.CharacterExists(id))
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
        /// Add a new character to db
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Character>> PostCharacter(CharacterCreateDTO character)
        {
            // Making sure the character to be created atleast have a name.  "string" is default in swagger
            if (character.FullName == "string" || character.FullName == string.Empty)
            {
                return BadRequest("Please add a name");
            }
            // Mapping character DTO to a Character object
            Character characterToAdd = _mapper.Map<Character>(character);
            
            // Calling service
            await _characterService.PostCharacter(characterToAdd);

            if (characterToAdd.Id == 0) return BadRequest("Duplicate name!");

            return CreatedAtAction("GetCharacter",
                new { id = characterToAdd.Id },
                _mapper.Map<CharacterReadDTO>(characterToAdd));
        }

        /// <summary>
        ///  Deletes a character from DB with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            try
            {
                // Calling service
                await _characterService.DeleteCharacter(id);
            }
            catch (Exception e)
            {
                // Return 404 if not found pluss message
                return NotFound(e.Message);
            }
            return NoContent();
        }

        public bool CharacterExists(int id)
        {
            // Call service
            return _characterService.CharacterExists(id);
        }

    }
}

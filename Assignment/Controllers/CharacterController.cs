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
using Assignment.Models.DTO.Character;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))] // Defaulting 200,201---404 msg's
    public class CharacterController : ControllerBase
    {
        private readonly AssignmentDbContext _context;

        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// Sets up context and mapper
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public CharacterController(AssignmentDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a list of all characters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadDTO>>> GetCharacters()
        {
            // Gets all CharacterDTO's
            return _mapper.Map<List<CharacterReadDTO>>(await _context.Characters
                .Include(c=>c.Movies)
                .ToListAsync());
        }

        /// <summary>
        /// Returns a single character with id == {id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterReadDTO>> GetCharacter(int id)
        {
            // finds character with id
            var character = await _context
                .Characters.FindAsync(id);

            // return NotFound if character remain null
            if (character == null)
            {
                return NotFound();
            }
            // Load the characters movies(not saved in the character table)
            await _context.Entry(character).Collection(i => i.Movies).LoadAsync();

            // Map the chatacter to the DTO and return it
            return _mapper.Map<CharacterReadDTO>(character);
        }

        /// <summary>
        /// Return Character by full name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}/byname")]
        public async Task<ActionResult<CharacterReadDTO>> GetCharacterByName(string name)
        {
            Character character;
            try
            {
                character = await _context.Characters.Where(m => m.FullName == name).FirstAsync();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            await _context.Entry(character).Collection(i => i.Movies).LoadAsync();
            return _mapper.Map<CharacterReadDTO>(character);
        }

        /// <summary>
        /// Gets a selection of characters.  Offset==how many you would like to skip.  Number==how many you want returned
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{offset}/group")]
        public async Task<ActionResult<IEnumerable<CharacterReadDTO>>> GetSomeCharacters(int offset, int number)
        {
            // return BadRequest if number is too low
            if (number < 1) return BadRequest("Number < 1");
  
            List<Character> characters;
            // getting the requested character records
            try
            {
                characters = await _context.Characters
                .Skip(offset)
                .Take(number)                               
                .Include(c => c.Movies)
                .ToListAsync();
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            if(characters.Count == 0)
            {
                return NotFound("No records found");
            }
           
            // Gets all CharacterDTO's
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
                return BadRequest();
            }

            // Create a Character object from the DTO
            Character rChar = _mapper.Map<Character>(character);

            // Set the midified state, so EF update it when savechanges is called
            _context.Entry(rChar).State = EntityState.Modified;

            // catching errors
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(id))
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
        /// Add a new character to db
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Character>> PostCharacter(CharacterCreateDTO character)
        {
            if (character.FullName == "string") return BadRequest("Please add a name");

            // Mapping character DTO to a Character object
            Character characterToAdd = _mapper.Map<Character>(character);
            // Adding the character object
            _context.Characters.Add(characterToAdd);
            // Save changes
            await _context.SaveChangesAsync();

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
            // Check if character with id exist
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            // Remove character and save changes
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Check in _context if the character with id == id exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CharacterExists(int id)
        {
            return _context.Characters.Any(e => e.Id == id);
        }
    }
}

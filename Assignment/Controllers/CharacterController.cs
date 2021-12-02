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
            var character = await _context
                .Characters.FindAsync(id);

            
            if (character == null)
            {
                return NotFound();
            }
            await _context.Entry(character).Collection(i => i.Movies).LoadAsync();


            return _mapper.Map<CharacterReadDTO>(character);
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
            if (id != character.Id)
            {
                return BadRequest();
            }

            Character rChar = _mapper.Map<Character>(character);

            _context.Entry(rChar).State = EntityState.Modified;

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
            Character characterToAdd = _mapper.Map<Character>(character);
            _context.Characters.Add(characterToAdd);
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
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

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

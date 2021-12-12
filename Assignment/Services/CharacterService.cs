using Assignment.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly AssignmentDbContext _context;
        // Constructor with context
        public CharacterService(AssignmentDbContext context)
        {
            _context = context;
        }

        public bool CharacterExists(int id)
        {
            // Test is Any Characters in the set had Id
            return _context.Characters.Any(e => e.Id == id);
        }

        public async Task DeleteCharacter(int id)
        {
            // Get character, if not found it will throw exception in CharacterServiceController
            var character = await _context.Characters.FindAsync(id);
            // Remove character and save changes
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
        }

        public async Task<Character> GetCharacter(int id)
        {
            // Get character, if not found it will throw exception in CharacterServiceController
            var character = await _context.Characters.FindAsync(id);
            // Load the characters movies(not saved in the character table)
            await _context.Entry(character).Collection(i => i.Movies).LoadAsync();

            // Map the chatacter to the DTO and return it
            return character;
        }

        public async Task<Character> GetCharacterByName(string name)
        {
            // Get character by string name(exact value, but ignore upper/lower cases)
            Character character = await _context.Characters.Where(m => m.FullName == name).FirstAsync();
            await _context.Entry(character).Collection(i => i.Movies).LoadAsync();
            return character;
        }

        public async Task<IEnumerable<Character>> GetCharacters()
        {
            // Gets all Character
            return await _context.Characters
                .Include(c => c.Movies)
                .ToListAsync();
        }

        public async Task<IEnumerable<Character>> GetSomeCharacters(int offset, int number)
        {
            // Return a list of characters
            return await _context.Characters
                .Skip(offset)
                .Take(number)
                .Include(c => c.Movies)
                .ToListAsync();
        }

        public async Task<Character> PostCharacter(Character character)
        {
            // Save a new character
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return character;
        }

        public async Task PutCharacter(int id, Character character)
        {
            // Update a character
            _context.Entry(character).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}

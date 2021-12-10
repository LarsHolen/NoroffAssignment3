using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment.Services
{
    public interface ICharacterService
    {
        public Task<IEnumerable<Character>> GetCharacters();
        public Task<Character> GetCharacter(int id);
        public Task<Character> GetCharacterByName(string name);
        public Task<IEnumerable<Character>> GetSomeCharacters(int offset, int number);
        public Task PutCharacter(int id, Character character);
        public Task<Character> PostCharacter(Character character);
        public Task DeleteCharacter(int id);
        public bool CharacterExists(int id);
    }
}
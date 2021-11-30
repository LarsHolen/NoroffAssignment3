using Assignment.Models;
using Assignment.Models.DTO.Character;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Profiles
{
    public class CharacterProfile : Profile
    {
        public CharacterProfile()
        {
            // Character->CharacterReadDTO
            CreateMap<Character, CharacterReadDTO>()
                // Turning related movies into int arrays
                .ForMember(cdto => cdto.Movies, opt => opt
                .MapFrom(c => c.Movies.Select(c => c.Id).ToArray()));
            // CHaracterCreateDTO->Character
            CreateMap<CharacterCreateDTO, Character>();
            // CharacterEditDTO->Character
            CreateMap<CharacterEditDTO, Character>();
        }
    }
}

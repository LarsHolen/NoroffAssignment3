using Assignment.Models;
using Assignment.Models.DTO.Movie;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            // Movie->MovieReadDTO
            CreateMap<Movie, MovieReadDTO>()
                // Turning related characters into int arrays
                .ForMember(cdto => cdto.Characters, opt => opt
                .MapFrom(c => c.Characters.Select(c => c.Id).ToArray()));
            // MovieCreateDTO->Movie
            CreateMap<MovieCreateDTO, Movie>();
            // CharacterEditDTO->Character
            CreateMap<MovieEditDTO, Movie>();
        }
    }
}

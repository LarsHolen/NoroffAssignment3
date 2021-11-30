using Assignment.Models;
using Assignment.Models.DTO.Franchise;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Profiles
{
    public class FranchiseProfile : Profile
    {
        public FranchiseProfile()
        {
           
            // Franchise<->FranchiseReadDTO
            CreateMap<Franchise, FranchiseReadDTO>()
                .ReverseMap();
            // Franchise<->FranchiseCreateDTO
            CreateMap<Franchise, FranchiseCreateDTO>()
                .ReverseMap();
            
        }
    }
}

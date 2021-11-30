using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Models.DTO.Movie
{
    public class MovieReadDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string ReleaseYear { get; set; }
        public string Director { get; set; }
        public string ImageURL { get; set; }
        public string TrailerURL { get; set; }
        public List<int> Characters { get; set; }
        public int FranchiseId { get; set; }
    }
}

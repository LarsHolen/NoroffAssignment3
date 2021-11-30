using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Models
{
    [Table("Character")]
    public class Character
    {
        // Primary key
        public int Id { get; set; }

        // Fields
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [MaxLength(50)]
        public string Alias { get; set; }
        [MaxLength(25)]
        public string Gender { get; set; }
        [MaxLength(250)]
        public string ImageURL { get; set; }
        // Relationships
        public ICollection<Movie> Movies { get; set; }

    }
}

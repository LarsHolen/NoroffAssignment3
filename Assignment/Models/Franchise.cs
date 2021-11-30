using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Models
{
    [Table("Franchise")]
    public class Franchise
    {
        // Primary key
        public int Id { get; set; }

        // Fields
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

        // Relationships
        public ICollection<Movie> Movies { get; set; }

    }
}

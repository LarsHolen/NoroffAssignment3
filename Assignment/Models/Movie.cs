using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Models
{
    [Table("Movie")]
    public class Movie
    {
        // Primary Key
        public int Id { get; set; }

        // Fields
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Genre { get; set; }

        [Required]
        [MaxLength(4)]
        [Column(TypeName = "varchar(4)")]
        // Making the variable varchar, not nvarchar. We only need a digit, no special characters.  
        // Saving a byte of space for each digit.  Only added for practice and for no real practical
        // reasons, since its length 4 not really needed to optimization.
        public string ReleaseYear { get; set; }

        [Required]
        [MaxLength(50)]
        public string Director { get; set; }

        [MaxLength(250)]
        public string ImageURL { get; set; }

        [MaxLength(250)]
        public string TrailerURL { get; set; }


        // Relationships
        public ICollection<Character> Characters { get; set; }

        public int FranchiseId { get; set; }
        public Franchise Franchise { get; set; }

    }
}

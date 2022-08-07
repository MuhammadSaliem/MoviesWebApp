using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesWebApp.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte GenreId { get; set; }
        public string? Name { get; set; }
    }
}

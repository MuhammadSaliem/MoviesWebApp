namespace MoviesWebApp.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public int Year { get; set; }
        public float Rate { get; set; }
        public string? Storyline { get; set; }
        public byte[] Poster { get; set; }
        public byte GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}

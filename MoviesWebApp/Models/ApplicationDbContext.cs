using Microsoft.EntityFrameworkCore;

namespace MoviesWebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>(m =>
            {
                m.HasOne(x => x.Genre)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

                m.HasKey(x => x.MovieId);
                m.Property(x => x.Title).HasColumnType("NvarChar(250)").IsRequired(true);
                m.Property(x => x.Storyline).HasColumnType("nvarchar(2500)").IsRequired(true);
                m.Property(x => x.Poster).IsRequired(true);

                
            });

            modelBuilder.Entity<Genre>(m =>
            {
                m.HasKey(x => x.GenreId);
                m.Property(x => x.Name).HasMaxLength(100).IsRequired(true);
                
            });
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}

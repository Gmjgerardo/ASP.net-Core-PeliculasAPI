using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entities;

namespace PeliculasAPI
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurating merge primary keys
            modelBuilder.Entity<MovieGenre>().HasKey(e => new{ e.GenreId, e.MovieId });
            modelBuilder.Entity<MovieCinema>().HasKey(e => new { e.CinemaId, e.MovieId });
            modelBuilder.Entity<MovieActor>().HasKey(e => new { e.ActorId, e.MovieId });
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MoviesGenres { get; set; }
        public DbSet<MovieCinema> MoviesCinemas { get; set; }
        public DbSet<MovieActor> MoviesActors { get; set; }
    }
}

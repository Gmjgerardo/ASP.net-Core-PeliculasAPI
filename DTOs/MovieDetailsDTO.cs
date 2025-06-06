using PeliculasAPI.Entities;

namespace PeliculasAPI.DTOs
{
    public class MovieDetailsDTO: MovieDTO
    {
        public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
        public List<CinemaDTO> Cinemas { get; set; } = new List<CinemaDTO>();
        public List<MovieActorDTO> Actors { get; set; } = new List<MovieActorDTO>();
    }
}

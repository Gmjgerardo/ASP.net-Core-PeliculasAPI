namespace PeliculasAPI.DTOs
{
    public class MoviePutGetDTO
    {
        public MovieDTO Movie { get; set; } = null!;
        public List<GenreDTO> SelectedGenres { get; set; } = new List<GenreDTO>();
        public List<GenreDTO> NotSelectedGenres { get; set; } = new List<GenreDTO>();

        public List<CinemaDTO> SelectedCinemas { get; set; } = new List<CinemaDTO>();
        public List<CinemaDTO> NotSelectedCinemas { get; set; } = new List<CinemaDTO>();

        public List<MovieActorDTO> Actors { get; set; } = new List<MovieActorDTO>();
    }
}

namespace PeliculasAPI.DTOs
{
    public class LandingPageDTO
    {
        public List<MovieDTO> OnCinemas { get; set; } = new List<MovieDTO>();
        public List<MovieDTO> Upcoming { get; set; } = new List<MovieDTO>();
    }
}

namespace PeliculasAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string? Trailer { get; set; }
        public required string Image { get; set; }
    }
}

using PeliculasAPI.Entities;

namespace PeliculasAPI.DTOs
{
    public class MovieActorDTO : IId
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public string Character { get; set; } = null!;
    }
}

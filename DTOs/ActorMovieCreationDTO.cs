using PeliculasAPI.Entities;

namespace PeliculasAPI.DTOs
{
    public class ActorMovieCreationDTO : IId
    {
        public int Id { get; set; }
        public required string Character { get; set; }
    }
}

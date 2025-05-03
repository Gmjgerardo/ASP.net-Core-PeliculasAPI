using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class MovieActor
    {
        public int ActorId { get; set; }
        public int MovieId { get; set; }

        [StringLength(maximumLength: 60)]
        public string? Character { get; set; }
        public int Order { get; set; }

        public Actor Actor { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
    }
}

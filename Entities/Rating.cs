using Microsoft.AspNetCore.Identity;

namespace PeliculasAPI.Entities
{
    public class Rating : IId
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public int MovieId { get; set; }
        public required string UserId { get; set; }

        // Navigation propertys
        public Movie Movie { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}

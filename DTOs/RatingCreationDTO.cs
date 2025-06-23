using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class RatingCreationDTO
    {
        [Required]
        public int MovieId { get; set; }

        [Required()]
        [Range(1, 5)]
        public int Rate { get; set; }

    }
}

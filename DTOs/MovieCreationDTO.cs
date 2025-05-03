using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class MovieCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 200, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Title { get; set; }

        [Required]
        public DateOnly ReleaseDate { get; set; }

        public string? Trailer { get; set; }

        [Required]
        public required IFormFile Image { get; set; }
    }
}

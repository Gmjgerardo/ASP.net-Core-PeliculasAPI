using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ActorCreationDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

    public class ActorDTO
    {
    }
}

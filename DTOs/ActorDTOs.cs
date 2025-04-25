using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entities;
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

    public class ActorDTO: IId
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ProfileImage { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ActorCreationDTO
    {
        public required string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

    public class ActorDTO
    {
    }
}

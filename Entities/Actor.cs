using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public required string Name { get; set; }
        public DateTime BirthDate { get; set; }

        [Unicode(false)]
        public string? ProfileImage { get; set; }
    }
}

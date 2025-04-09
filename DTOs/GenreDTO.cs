using PeliculasAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

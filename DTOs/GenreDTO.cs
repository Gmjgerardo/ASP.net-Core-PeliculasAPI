using PeliculasAPI.Entities;
using PeliculasAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class GenreDTO: IId
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

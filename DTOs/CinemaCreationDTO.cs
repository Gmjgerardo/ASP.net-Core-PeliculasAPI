using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class CinemaCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 75)]
        public required string Name { get; set; }
        public required Point location { get; set; }
    }
}

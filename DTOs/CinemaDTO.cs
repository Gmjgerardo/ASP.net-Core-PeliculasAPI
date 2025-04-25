using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class CinemaDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Point location { get; set; }
    }
}

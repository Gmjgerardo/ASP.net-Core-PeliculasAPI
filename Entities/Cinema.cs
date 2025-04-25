using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class Cinema: IId
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 75)]
        public required string Name { get; set; }

        public required Point location { get; set; }
    }
}

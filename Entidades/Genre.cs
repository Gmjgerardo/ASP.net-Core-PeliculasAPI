using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(10, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Name { get; set; }

        //[Range(18, 120)]
        //public int Edad { get; set; }

        //[CreditCard(ErrorMessage = "El campo {0} no tiene el formato correcto!")]
        //public string? TarjetaDeCredito { get; set; }

        //[Url]
        //public string? Url { get; set; }
    }
}

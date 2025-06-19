using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class UserCredentialsDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public required string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validations
{
    public class FirstLetterInUppercaseAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(value?.ToString()))
            {
                string firstLetter = value.ToString()![0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                    return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class NotEmptyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) 
                return new ValidationResult("Пустое поле");
            if (value is not string valueString)
                return new ValidationResult("Не строка");
            if (string.IsNullOrWhiteSpace(valueString))
                return new ValidationResult("Поле не может быть пустым или содержать одни лишь пробелы");
            return ValidationResult.Success;

        }
    }
}

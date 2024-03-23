using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class NotEmptyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) 
                return new("Пустое поле");
            if (value is not string valueString)
                return new("Не строка");
            if (string.IsNullOrWhiteSpace(valueString))
                return new("Поле не может быть пустым или содержать одни лишь пробелы");
            return ValidationResult.Success;

        }
    }
}

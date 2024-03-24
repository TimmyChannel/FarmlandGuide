using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class PasswordAttribute : ValidationAttribute
    {
        public PasswordAttribute(string pattern)
        {
            Expression = new(pattern);
        }
        Regex Expression { get; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return new("Пустое поле");
            if (value is not string valueString)
                return new("Не строка");
            if (!Expression.IsMatch(valueString))
                return new("Пароль должен иметь от 6 символов, иметь крупные и маленькие символы, цифры");
            return ValidationResult.Success;

        }
    }
}

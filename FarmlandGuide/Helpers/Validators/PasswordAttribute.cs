using FarmlandGuide.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class PasswordAttribute : ValidationAttribute
    {
        public PasswordAttribute(string pattern)
        {
            Expression = new Regex(pattern);
        }

        private Regex Expression { get; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is EmployeesPageViewModel vm)
            {
                if (value is not string stringValue)
                    return new ValidationResult("Не строка");

                if (vm.IsEdit && string.IsNullOrEmpty(stringValue))
                    return ValidationResult.Success;
            }
            if (value is null)
                return new ValidationResult("Пустое поле");
            if (value is not string valueString)
                return new ValidationResult("Не строка");
            if (!Expression.IsMatch(valueString))
                return new ValidationResult("Пароль должен иметь от 6 символов, иметь крупные и маленькие символы, цифры");
            return ValidationResult.Success;

        }
    }
}

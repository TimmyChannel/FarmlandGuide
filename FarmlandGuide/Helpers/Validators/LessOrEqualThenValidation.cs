using System;
using System.ComponentModel.DataAnnotations;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class LessOrEqualThenValidation : ValidationAttribute
    {
        public LessOrEqualThenValidation(double number)
        {
            Number = number;
        }

        public double Number { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object numberNewType = Convert.ChangeType(Number, value.GetType());
            if (numberNewType == null) return new ValidationResult("Ошибка ввода! Проверьте введённое число");
            if (((IComparable)value).CompareTo(numberNewType) > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Число не может быть меньше или равно {Number}");

        }

    }
}

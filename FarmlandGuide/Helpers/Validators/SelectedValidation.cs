using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Validators
{
    public sealed class ShouldBeSelectedAttribute : ValidationAttribute
    {
        public ShouldBeSelectedAttribute()
        {
            Message = "Необходимо выбрать значение";
        }
        public ShouldBeSelectedAttribute(string message)
        {
            Message = message;
        }
        public string Message { get; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return new ValidationResult(Message);

            return ValidationResult.Success;

        }
    }
}

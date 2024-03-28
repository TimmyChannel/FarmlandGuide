using System.ComponentModel.DataAnnotations;

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
            return value is null ? new ValidationResult(Message) : ValidationResult.Success;
        }
    }
}

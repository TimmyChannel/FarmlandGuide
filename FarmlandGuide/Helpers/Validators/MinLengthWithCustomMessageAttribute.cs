using System.ComponentModel.DataAnnotations;

namespace FarmlandGuide.Helpers.Validators
{
    public class MinLengthWithCustomMessageAttribute : MinLengthAttribute
    {
        public MinLengthWithCustomMessageAttribute(int length, string message) : base(length)
        {
            ErrorMessage = message;
        }
    }
}

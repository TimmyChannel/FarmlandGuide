using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FarmlandGuide.Helpers.ValidationRules
{
    public class NotEmptyValidationRule : ValidationRule
    {
        private bool hadValue = false;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                if (hadValue)
                    return new ValidationResult(false, "Поле не может быть пустым.");
                else
                {
                    hadValue = true;
                    return new ValidationResult(true, "Поле не может быть пустым.");
                }

            }
            return ValidationResult.ValidResult;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Helpers.Validators
{
    public class MinLengthWithCustomMessageAttribute : MinLengthAttribute
    {
        public MinLengthWithCustomMessageAttribute(int length, string message) : base(length)
        {
            this.ErrorMessage = message;
        }
    }
}

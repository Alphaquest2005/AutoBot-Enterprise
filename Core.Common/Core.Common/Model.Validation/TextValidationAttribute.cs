using System;

namespace Core.Common.Validation
{
    public class TextValidationAttribute : Attribute, IValidationRule
    {
        public int MinLength { get; set; }

        public TextValidationAttribute()
        {

        }

        public void Validate(object value, out bool isValid, out string errorMessage)
        {
            isValid = false;
            errorMessage = "";

            if (value != null && value.ToString().Length >= MinLength)
                isValid = true;

            if (!isValid)
                errorMessage = value + " is not equal to or longer than " + MinLength;
        }
    }
}

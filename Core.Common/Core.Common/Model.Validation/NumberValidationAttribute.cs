using System;
using System.Collections.Generic;

namespace Core.Common.Validation
{
    public class NumberValidationAttribute : Attribute, IValidationRule
    {
        public NumberValidationAttribute()
        {

        }

        public void Validate(object value, out bool isValid, out string errorMessage)
        {
            double result = 0;
            isValid = false;
            if (value != null)
                isValid = double.TryParse(value.ToString(), out result);
            errorMessage = "";

            if (!isValid)
                errorMessage = value + " is not a valid number";
        }
    }
}

using System;

namespace Core.Common.Validation
{
    public class RequiredValidationAttribute : Attribute, IValidationRule
    {
        public string ErrorMessage = "this field is required!";

        public void Validate(object value, out bool isValid, out string errorMessage)
        {
            isValid = false;

            isValid = value != null;
            errorMessage = "";

            if (!isValid)
                errorMessage = ErrorMessage;
        }
    }
}
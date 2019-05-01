using System;

namespace Core.Common.Validation
{
    public class AddressValidationAttribute : Attribute, IValidationRule
    {
       
        public AddressValidationAttribute()
        {

        }

        public void Validate(object value, out bool isValid, out string errorMessage)
        {
            isValid = false;
            errorMessage = "";

            if (value != null && value.ToString().Split(',').Length >= 2)// address and parish
                isValid = true;

            if (!isValid)
                errorMessage = "'" + value + "' is the Address, please include the Parish with a comma. eg.'address, Parish'";
        }
    }
}


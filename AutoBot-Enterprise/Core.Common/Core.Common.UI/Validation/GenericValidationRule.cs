using System.Globalization;
using System.Windows.Controls;
using Core.Common.Validation;

namespace Core.Common.UI.Validation
{
    public class GenericValidationRule : ValidationRule
    {
        private readonly IValidationRule ValidationRule;

        public GenericValidationRule(IValidationRule validationRule)
        {
            ValidationRule = validationRule;
            ValidatesOnTargetUpdated = true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isValid = false;
            var errorMessage = "";

            ValidationRule.Validate(value, out isValid, out errorMessage);

            var result = new ValidationResult(isValid, errorMessage);
            return result;
        }
    }
}
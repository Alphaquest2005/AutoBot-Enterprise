using System;

namespace Core.Common.Validation
{
    public interface IValidationRule
    {
        void Validate(object value, out bool isValid, out string errorMessage);
    }
}

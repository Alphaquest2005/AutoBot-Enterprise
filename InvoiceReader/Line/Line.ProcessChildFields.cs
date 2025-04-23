using System.Collections.Generic;
using System.Linq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private void ProcessChildFields(string instance, Dictionary<(Fields field, string instance), string> values,
            Fields field, string methodName, int? fieldId,
            (Fields field, string instance) valueKey)
        {
            if (field.ChildFields != null && field.ChildFields.Any())
            {
                _logger.Verbose(
                    "{MethodName}: Processing {Count} ChildFields for FieldId: {FieldId}, Instance: {Instance}...",
                    methodName, field.ChildFields.Count, fieldId, instance);
                string currentValueForChild =
                    values.TryGetValue(valueKey, out var currentVal)
                        ? currentVal
                        : ""; // Use potentially updated value
                int childFieldIndex = 0;
                foreach (var childField in field.ChildFields.Where(cf => cf != null)) // Safe iteration
                {
                    childFieldIndex++;
                    _logger.Verbose(
                        "{MethodName}: Calling ReadChildField for ChildField {Index}/{Total} (Id: {ChildFieldId}) of Parent FieldId: {ParentFieldId}...",
                        methodName, childFieldIndex, field.ChildFields.Count, childField.Id, fieldId);
                    // ReadChildField should handle its own logging
                    ReadChildField(childField, values, currentValueForChild);
                }

                _logger.Verbose(
                    "{MethodName}: Finished processing ChildFields for FieldId: {FieldId}, Instance: {Instance}",
                    methodName, fieldId, instance);
            }
        }
    }
}
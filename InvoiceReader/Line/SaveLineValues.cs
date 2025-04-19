using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Line
{
    private void SaveLineValues(int lineNumber, string section, int instance,
        Dictionary<(Fields Fields, int Instance), string> values)
    {
        // Check if key exists before adding/updating
        if (Values.ContainsKey((lineNumber, section)))
        {
            // Merge new values with existing ones for the same line/section, potentially handling duplicates if necessary
            foreach (var kvp in values)
            {
                // Ensure the key uses the correct instance number passed from the parent
                // Ensure the key uses the correct instance number passed from the parent and correct field access
                var correctInstanceKey = (kvp.Key.Fields, instance);
                if (!Values[(lineNumber, section)].ContainsKey(correctInstanceKey))
                {
                    Values[(lineNumber, section)].Add(correctInstanceKey, kvp.Value);
                }
                else
                {
                    // Handle potential duplicate key scenario if needed (e.g., log, overwrite, append)
                    // Current logic seems to append strings or add numbers based on field.DataType in lines 63-74
                    // For simplicity, let's assume overwrite or the existing append logic handles it.
                    // Ensure update uses the correct instance key
                    Values[(lineNumber, section)][correctInstanceKey] = kvp.Value;
                }
            }
        }
        else
        {
            Values[(lineNumber, section)] = values;
        }
    }
}
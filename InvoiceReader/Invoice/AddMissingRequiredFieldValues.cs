using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private void AddMissingRequiredFieldValues()
    {
        var requiredFieldsList = this.Lines.SelectMany(x => x.OCR_Lines.Fields)
            .Where(z => z.IsRequired && !string.IsNullOrEmpty(z.FieldValue?.Value)).ToList();
        var instances = Lines.SelectMany(x => x.Values)
            .Where(x => x.Key.section == "Single")
            .SelectMany(x => x.Value.Keys.Select(k => (Instance: k.instance, LineNumber: x.Key.lineNumber)))
            .DistinctBy(x => x.Instance)
            .ToList();
        foreach (var field in requiredFieldsList)
        {
            var lines = this.Lines.Where(x => LineHasField(x, field)
                                              && ValueContainsRequiredField(x, field)
                                              && ValueForAllInstances(x, instances)
            ).ToList();
            foreach (var line in lines)
            {
                var lineInstances = line.Values.SelectMany(z => z.Value.Keys.Select(k => k.instance)).Distinct()
                    .ToList();
                foreach (var instance in instances)
                {
                    if (!lineInstances.Contains(instance.Instance))
                    {
                        var key = (instance.LineNumber, "Single");
                        var innerValueKey = (field, instance.Instance);
                        var valueToAdd = field.FieldValue.Value;

                        if (line.Values.TryGetValue(key, out var innerDict))
                        {
                            // Key exists, add to inner dictionary if the specific field/instance doesn't exist yet
                            if (!innerDict.ContainsKey(innerValueKey))
                            {
                                innerDict.Add(innerValueKey, valueToAdd);
                            }
                            // Optional: Handle case where innerValueKey already exists (e.g., log, update, ignore)
                        }
                        else
                        {
                            // Key doesn't exist, add new entry with the inner dictionary
                            line.Values.Add(key,
                                new Dictionary<(Fields fields, int instance), string>
                                    { { innerValueKey, valueToAdd } });
                        }
                    }
                }
            }
        }
    }
}
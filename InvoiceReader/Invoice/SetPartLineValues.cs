using Core.Common.Extensions;
using Core.Common; // Added for BetterExpando
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Part, Line, Fields

namespace WaterNut.DataSpace
{
    public partial class Invoice
    {
        private List<IDictionary<string, object>> SetPartLineValues(Part part, string filterInstance = null)
        {
            var currentPart = (WaterNut.DataSpace.Part)part;
            int? partId = currentPart?.OCR_Part?.Id;
            string filterInstanceStr = filterInstance?.ToString() ?? "None (Top Level)";
            string methodName = nameof(SetPartLineValues);

            _logger.Verbose("Entering {MethodName} for PartId: {PartId}, FilterInstance: {FilterInstance}",
                methodName, partId, filterInstanceStr);

            var finalPartItems = new List<IDictionary<string, object>>();

            if (!ValidateInput(currentPart, partId, methodName))
                return finalPartItems;

            try
            {
                var instancesToProcess = DetermineInstancesToProcess(currentPart, filterInstance, partId, methodName);

                if (!instancesToProcess.Any())
                {
                    LogNoInstancesToProcess(partId, filterInstance, methodName);
                    return finalPartItems;
                }

                foreach (var currentInstance in instancesToProcess)
                {
                    ProcessInstance(currentPart, currentInstance, partId, methodName, finalPartItems);
                }

                _logger.Information(
                    "Exiting {MethodName} successfully for PartId: {PartId}. Returning {ItemCount} items.", methodName,
                    partId, finalPartItems.Count);
                _logger.Verbose("{MethodName}: PartId: {PartId} - Final items before returning: {@FinalItems}",
                    methodName, partId, finalPartItems);

                return finalPartItems;
            }
            catch (Exception e)
            {
                _logger.Error(e,
                    "{MethodName}: Unhandled exception during processing for PartId: {PartId}, FilterInstance: {FilterInstance}",
                    methodName, partId, filterInstanceStr);
                _logger.Information("Exiting {MethodName} for PartId: {PartId} due to exception.", methodName, partId);
                throw;
            }
        }

        private bool ValidateInput(Part currentPart, int? partId, string methodName)
        {
            if (currentPart == null || currentPart.OCR_Part == null)
            {
                _logger.Error("{MethodName}: Called with null Part or OCR_Part for PartId: {PartId}. Exiting.",
                    methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null Part/OCR_Part.", methodName,
                    partId);
                return false;
            }

            if (currentPart.Lines == null)
            {
                _logger.Warning("{MethodName}: PartId: {PartId} has null Lines collection. Exiting.", methodName,
                    partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null Lines collection.", methodName,
                    partId);
                return false;
            }

            _logger.Verbose("{MethodName}: Input validation passed for PartId: {PartId}.", methodName, partId);
            return true;
        }

        private List<IGrouping<string, string>> DetermineInstancesToProcess(Part currentPart, string filterInstance, int? partId, string methodName)
        {
            _logger.Verbose(
                "{MethodName}: Determining instances to process for PartId: {PartId}, FilterInstance: {FilterInstance}",
                methodName, partId, filterInstance?.ToString() ?? "None");

            List<IGrouping<string, string>> instancesToProcess = currentPart.Lines
                .Where(line => line?.Values != null)
                .SelectMany(line =>
                    line.Values.SelectMany(v =>
                        v.Value?.Keys ?? Enumerable.Empty<(Fields fields, string instance)>()))
                .Where(k => string.IsNullOrEmpty(filterInstance?.ToString()) || k.instance == filterInstance?.ToString())
                .Select(k => k.instance)
                .Distinct()
                .OrderBy(x => int.Parse(x.Split('-')[0]))
                .ThenBy(x => int.Parse(x.Split('-')[1]))
                .GroupBy(x => x.Split('-')[0])
                .ToList();

            _logger.Verbose("{MethodName}: Found {Count} initial instances for PartId: {PartId}: [{Instances}]",
                methodName, instancesToProcess.Count, partId, string.Join(",", instancesToProcess));

            if (filterInstance != null && !instancesToProcess.Any())
            {
                instancesToProcess = CheckChildPartsForInstances(currentPart, filterInstance   , partId, methodName);
            }

            return instancesToProcess;
        }

        private List<IGrouping<string, string>> CheckChildPartsForInstances(Part currentPart, string filterInstance, int? partId, string methodName)
        {
            _logger.Verbose(
                "{MethodName}: Checking child parts for FilterInstance: {FilterInstance} as parent PartId: {PartId} has no direct data for it.",
                methodName, filterInstance, partId);

            bool childHasDataForInstance = currentPart.ChildParts != null && currentPart.ChildParts
                .Where(cp => cp?.AllLines != null)
                .SelectMany(cp => cp.AllLines)
                .Where(l => l?.Values != null)
                .SelectMany(l =>
                    l.Values.SelectMany(v =>
                        v.Value?.Keys ?? Enumerable.Empty<(Fields fields, string instance)>()))
                .Any(k => k.instance == filterInstance);

            if (childHasDataForInstance)
            {
                _logger.Information(
                    "{MethodName}: PartId: {PartId}: No direct data for FilterInstance: {FilterInstance}, but children have data. Adding instance for child aggregation.",
                    methodName, partId, filterInstance);
                return new List<IGrouping<string, string>> { new Grouping<string, string>(filterInstance, new[] { filterInstance.ToString() }) };


            }

            _logger.Verbose(
                "{MethodName}: PartId: {PartId}: Neither parent nor children have data for FilterInstance: {FilterInstance}.",
                methodName, partId, filterInstance);

            return new List<IGrouping<string, string>>();
        }

        private void LogNoInstancesToProcess(int? partId, string filterInstance, string methodName)
        {
            _logger.Information(
                "{MethodName}: PartId: {PartId}: No relevant instances found to process{FilterContext}. Exiting.",
                methodName, partId,
                string.IsNullOrEmpty(filterInstance) ? $" for FilterInstance: {filterInstance}" : "");
            _logger.Verbose("Exiting {MethodName} for PartId: {PartId} because no instances were found.",
                methodName, partId);
        }

        private void ProcessInstance(Part currentPart, IGrouping<string, string> currentInstance, int? partId, string methodName, List<IDictionary<string, object>> finalPartItems)
        {
            _logger.Debug("{MethodName}: Starting processing for Instance: {Instance} of PartId: {PartId}",
                methodName, currentInstance, partId);

            var parentItem = new BetterExpando();
            var parentDitm = (IDictionary<string, object>)parentItem;
            bool parentDataFound = false;

            PopulateParentFields(currentPart, currentInstance.First(), parentDitm, ref parentDataFound, methodName);

            foreach (var childInstance in currentInstance)
            {
               ProcessChildParts(currentPart, childInstance, parentDitm, ref parentDataFound, partId, methodName); 
            }

            

            if (parentDataFound)
            {
                _logger.Information(
                    "{MethodName}: PartId: {PartId}: Adding assembled item for Instance: {Instance} to final results.",
                    methodName, partId, currentInstance);
                finalPartItems.Add(parentItem);
            }
            else
            {
                _logger.Warning(
                    "{MethodName}: PartId: {PartId}: Skipping empty or incomplete item for Instance: {Instance} (no parent or child data found/attached).",
                    methodName, partId, currentInstance);
            }

            _logger.Debug("{MethodName}: Finished processing for Instance: {Instance} of PartId: {PartId}",
                methodName, currentInstance, partId);
        }

        private void PopulateParentFields(Part currentPart, string currentInstance, IDictionary<string, object> parentDitm, ref bool parentDataFound, string methodName)
        {
            var sectionsInOrder = new[] { "Single", "Ripped", "Sparse" };
            _logger.Verbose(
                "{MethodName}: Instance: {Instance} - Iterating through sections for parent data: {Sections}",
                methodName, currentInstance, string.Join(", ", sectionsInOrder));

            foreach (var sectionName in sectionsInOrder)
            {
                _logger.Verbose("{MethodName}: Instance: {Instance} - Checking section: '{SectionName}'",
                    methodName, currentInstance, sectionName);

                var sectionInstanceValues = currentPart.Lines
                    .Where(line => line?.Values != null)
                    .SelectMany(line => line.Values
                        .Where(v => v.Key.section == sectionName && v.Value != null)
                        .SelectMany(v => v.Value.Where(kvp => kvp.Key.Instance == currentInstance))
                    ).ToList();

                if (sectionInstanceValues.Any())
                {
                    _logger.Debug(
                        "{MethodName}: Instance: {Instance} - Found {Count} field values in section '{SectionName}'",
                        methodName, currentInstance, sectionInstanceValues.Count, sectionName);
                    parentDataFound = true;

                    SetParentMetadata(sectionInstanceValues, parentDitm, currentInstance, sectionName, methodName);
                    ProcessFieldsInSection(sectionInstanceValues, parentDitm, methodName, currentInstance, sectionName);
                }
                else
                {
                    _logger.Verbose(
                        "{MethodName}: Instance: {Instance} - No field values found in section '{SectionName}'",
                        methodName, currentInstance, sectionName);
                }
            }
        }

        private void SetParentMetadata(List<KeyValuePair<(Fields Fields, string Instance), string>> sectionInstanceValues, IDictionary<string, object> parentDitm, string currentInstance, string sectionName, string methodName)
        {
            if (!parentDitm.ContainsKey("Instance"))
            {
                var firstValue = sectionInstanceValues.First();
                parentDitm["Instance"] = currentInstance;
                parentDitm["Section"] = sectionName;
                parentDitm["FileLineNumber"] = 0; // Default value
                _logger.Verbose(
                    "{MethodName}: Instance: {Instance} - Set initial metadata: FileLineNumber={LineNum}, Section='{Section}'",
                    methodName, currentInstance, parentDitm["FileLineNumber"], sectionName);
            }
        }

        private void ProcessFieldsInSection(List<KeyValuePair<(Fields Fields, string Instance), string>> sectionInstanceValues, IDictionary<string, object> parentDitm, string methodName, string currentInstance, string sectionName)
        {
            foreach (var fieldKvp in sectionInstanceValues)
            {
                var field = fieldKvp.Key.Fields;
                if (field == null || string.IsNullOrEmpty(field.Field))
                {
                    _logger.Warning(
                        "{MethodName}: Skipping field value in Instance: {Instance}, Section: {Section} because Field object or Field name is null.",
                        methodName, currentInstance, sectionName);
                    continue;
                }

                var fieldName = field.Field;
                parentDitm[fieldName] = GetValue(fieldKvp);
                _logger.Verbose(
                    "{MethodName}: Instance: {Instance} - Field '{FieldName}' set. Value: '{Value}'",
                    methodName, currentInstance, fieldName, parentDitm[fieldName]);
            }
        }

        private void ProcessChildParts(Part currentPart, string currentInstance, IDictionary<string, object> parentDitm, ref bool parentDataFound, int? partId, string methodName)
        {
            _logger.Verbose(
                "{MethodName}: Instance: {Instance} - Starting processing of child parts for PartId: {PartId}",
                methodName, currentInstance, partId);

            if (currentPart.ChildParts != null)
            {
                foreach (var childPartRaw in currentPart.ChildParts.Where(cp => cp != null))
                {
                    var childPart = (WaterNut.DataSpace.Part)childPartRaw;
                    var childPartId = childPart.OCR_Part?.Id ?? -1;

                    if (childPart.AllLines == null || !childPart.AllLines.Any())
                    {
                        _logger.Verbose(
                            "{MethodName}: Instance: {Instance} - Skipping ChildPartId: {ChildPartId} because it has no lines.",
                            methodName, currentInstance, childPartId);
                        continue;
                    }

                    _logger.Debug("{MethodName}: Instance: {Instance} - Processing ChildPartId: {ChildPartId}",
                        methodName, currentInstance, childPartId);

                    var allChildItemsForPart = SetPartLineValues(childPart, currentInstance);

                    if (allChildItemsForPart != null && allChildItemsForPart.Any())
                    {
                        parentDataFound = true;
                        AttachChildItems(parentDitm, allChildItemsForPart, childPart, childPartId, methodName, currentInstance);
                    }
                }
            }
        }

        private void AttachChildItems(IDictionary<string, object> parentDitm, List<IDictionary<string, object>> childItems, Part childPart, int childPartId, string methodName, string currentInstance)
        {
            var entityTypeField = childPart.AllLines.FirstOrDefault()?
                .OCR_Lines?.Fields?.FirstOrDefault()?
                .EntityType;
            var fieldname = !string.IsNullOrEmpty(entityTypeField)
                ? entityTypeField
                : $"ChildPart_{childPartId}";

            if (parentDitm.ContainsKey(fieldname))
            {
                if (parentDitm[fieldname] is List<IDictionary<string, object>> existingList)
                {
                    existingList.AddRange(childItems);
                }
                else
                {
                    parentDitm[fieldname] = childItems;
                }
            }
            else
            {
                parentDitm[fieldname] = childItems;
            }
        }


    }
}

// Helper class to create a grouping
public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
{
    public TKey Key { get; }
    private readonly IEnumerable<TElement> _elements;

    public Grouping(TKey key, IEnumerable<TElement> elements)
    {
        Key = key;
        _elements = elements;
    }

    public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
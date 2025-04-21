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
        // Assuming _logger exists from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Invoice>();

        private List<IDictionary<string, object>> SetPartLineValues(Part part, int? filterInstance = null)
        {
            // Explicitly qualify Part type due to potential namespace conflicts (CS0436 warning)
            var currentPart = (WaterNut.DataSpace.Part)part;
            int? partId = currentPart?.OCR_Part?.Id; // Safe access
            string filterInstanceStr = filterInstance?.ToString() ?? "None (Top Level)";
            string methodName = nameof(SetPartLineValues);

            _logger.Verbose("Entering {MethodName} for PartId: {PartId}, FilterInstance: {FilterInstance}",
                methodName, partId, filterInstanceStr);

            var finalPartItems = new List<IDictionary<string, object>>();

            // --- Input Validation ---
            if (currentPart == null || currentPart.OCR_Part == null)
            {
                _logger.Error("{MethodName}: Called with null Part or OCR_Part for PartId: {PartId}. Exiting.",
                    methodName, partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null Part/OCR_Part.", methodName,
                    partId);
                return finalPartItems;
            }

            if (currentPart.Lines == null)
            {
                _logger.Warning("{MethodName}: PartId: {PartId} has null Lines collection. Exiting.", methodName,
                    partId);
                _logger.Verbose("Exiting {MethodName} for PartId: {PartId} due to null Lines collection.", methodName,
                    partId);
                return finalPartItems;
            }

            _logger.Verbose("{MethodName}: Input validation passed for PartId: {PartId}.", methodName, partId);


            try
            {
                // --- Determine Instances to Process ---
                _logger.Verbose(
                    "{MethodName}: Determining instances to process for PartId: {PartId}, FilterInstance: {FilterInstance}",
                    methodName, partId, filterInstanceStr);
                var instancesToProcess = currentPart.Lines
                    .Where(line => line?.Values != null) // Safe navigation
                    .SelectMany(line =>
                        line.Values.SelectMany(v =>
                            v.Value?.Keys ?? Enumerable.Empty<(Fields fields, int instance)>())) // Safe navigation
                    .Where(k => !filterInstance.HasValue || k.instance == filterInstance.Value)
                    .Select(k => k.instance)
                    .Distinct()
                    .OrderBy(instance => instance)
                    .ToList();
                _logger.Verbose("{MethodName}: Found {Count} initial instances for PartId: {PartId}: [{Instances}]",
                    methodName, instancesToProcess.Count, partId, string.Join(",", instancesToProcess));

                // --- Check Children if Filtering and Parent has no data for instance ---
                if (filterInstance.HasValue && !instancesToProcess.Any())
                {
                    _logger.Verbose(
                        "{MethodName}: Checking child parts for FilterInstance: {FilterInstance} as parent PartId: {PartId} has no direct data for it.",
                        methodName, filterInstance.Value, partId);
                    bool childHasDataForInstance = currentPart.ChildParts != null && currentPart.ChildParts
                        .Where(cp => cp?.AllLines != null) // Check child part and its lines
                        .SelectMany(cp => cp.AllLines)
                        .Where(l => l?.Values != null) // Check line and its values
                        .SelectMany(l =>
                            l.Values.SelectMany(v =>
                                v.Value?.Keys ??
                                Enumerable.Empty<(Fields fields, int instance)>())) // Check inner dict and keys
                        .Any(k => k.instance == filterInstance.Value);

                    if (childHasDataForInstance)
                    {
                        _logger.Information(
                            "{MethodName}: PartId: {PartId}: No direct data for FilterInstance: {FilterInstance}, but children have data. Adding instance for child aggregation.",
                            methodName, partId, filterInstance.Value);
                        instancesToProcess = new List<int> { filterInstance.Value };
                    }
                    else
                    {
                        _logger.Verbose(
                            "{MethodName}: PartId: {PartId}: Neither parent nor children have data for FilterInstance: {FilterInstance}.",
                            methodName, partId, filterInstance.Value);
                    }
                }

                // --- Exit if No Instances ---
                if (!instancesToProcess.Any())
                {
                    _logger.Information(
                        "{MethodName}: PartId: {PartId}: No relevant instances found to process{FilterContext}. Exiting.",
                        methodName, partId,
                        filterInstance.HasValue ? $" for FilterInstance: {filterInstance.Value}" : "");
                    _logger.Verbose("Exiting {MethodName} for PartId: {PartId} because no instances were found.",
                        methodName, partId);
                    return finalPartItems;
                }

                _logger.Information("{MethodName}: PartId: {PartId}: Processing instances [{Instances}]{FilterContext}",
                    methodName, partId, string.Join(", ", instancesToProcess),
                    filterInstance.HasValue ? $" (Filtered by Parent Instance: {filterInstance.Value})" : "");


                // --- Iterate Through Instances ---
                foreach (var currentInstance in instancesToProcess)
                {
                    _logger.Debug("{MethodName}: Starting processing for Instance: {Instance} of PartId: {PartId}",
                        methodName, currentInstance, partId);
                    var parentItem = new BetterExpando();
                    var parentDitm = (IDictionary<string, object>)parentItem;
                    bool parentDataFound = false;
                    var fieldSourceSections = new Dictionary<string, string>(); // Track field source for precedence

                    // --- Populate Parent Fields by Section Precedence ---
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
                                .SelectMany(v => v.Value.Where(kvp => kvp.Key.instance == currentInstance))
                            ).ToList();

                        if (sectionInstanceValues.Any())
                        {
                            _logger.Debug(
                                "{MethodName}: Instance: {Instance} - Found {Count} field values in section '{SectionName}'",
                                methodName, currentInstance, sectionInstanceValues.Count, sectionName);
                            parentDataFound = true;

                            // --- Set Initial Metadata (Instance, FileLineNumber, Section) ---
                            if (!parentDitm.ContainsKey("Instance"))
                            {
                                var firstValue = sectionInstanceValues.First();
                                var sourceLineValue = currentPart.Lines
                                    .Where(l => l?.Values != null)
                                    .SelectMany(l =>
                                        l.Values.Select(
                                            v => new { LineNumber = v.Key.lineNumber, ValuesDict = v.Value }))
                                    .FirstOrDefault(lv =>
                                        lv.ValuesDict != null && lv.ValuesDict.Any(kvp => kvp.Key == firstValue.Key));

                                if (sourceLineValue != null)
                                {
                                    parentDitm["FileLineNumber"] = sourceLineValue.LineNumber + 1; // Assuming 1-based
                                    parentDitm["Instance"] = currentInstance;
                                    parentDitm["Section"] = sectionName;
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - Set initial metadata: FileLineNumber={LineNum}, Section='{Section}'",
                                        methodName, currentInstance, parentDitm["FileLineNumber"], sectionName);
                                }
                                else
                                {
                                    parentDitm["FileLineNumber"] = 0; // Fallback
                                    parentDitm["Instance"] = currentInstance;
                                    parentDitm["Section"] = sectionName;
                                    _logger.Warning(
                                        "{MethodName}: Could not find source line number for first value in Instance: {Instance}, Section: {Section}. Setting FileLineNumber to 0.",
                                        methodName, currentInstance, sectionName);
                                }
                            }

                            // --- Process Fields in Section ---
                            foreach (var fieldKvp in sectionInstanceValues)
                            {
                                var field = fieldKvp.Key.fields;
                                if (field == null || string.IsNullOrEmpty(field.Field))
                                {
                                    _logger.Warning(
                                        "{MethodName}: Skipping field value in Instance: {Instance}, Section: {Section} because Field object or Field name is null.",
                                        methodName, currentInstance, sectionName);
                                    continue;
                                }

                                var fieldName = field.Field;

                                // --- Apply Section Precedence ---
                                if (!fieldSourceSections.ContainsKey(fieldName))
                                {
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - Setting Field '{FieldName}' from Section '{SectionName}'.",
                                        methodName, currentInstance, fieldName, sectionName);
                                    parentDitm[fieldName] =
                                        GetValue(fieldKvp); // GetValue handles conversion and logging
                                    fieldSourceSections.Add(fieldName, sectionName);
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - Field '{FieldName}' set. Value: '{Value}'",
                                        methodName, currentInstance, fieldName, parentDitm[fieldName]);
                                }
                                else
                                {
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - Field '{FieldName}' already set from Section '{ExistingSection}'. Ignoring value '{Value}' from Section '{CurrentSection}'.",
                                        methodName, currentInstance, fieldName, fieldSourceSections[fieldName],
                                        fieldKvp.Value, sectionName);
                                }
                            }
                        }
                        else
                        {
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - No field values found in section '{SectionName}'",
                                methodName, currentInstance, sectionName);
                        }
                    } // End foreach sectionName

                    _logger.Verbose(
                        "{MethodName}: Instance: {Instance} - Finished processing sections for parent data.",
                        methodName, currentInstance);


                    // --- Process Child Parts ---
                    _logger.Verbose(
                        "{MethodName}: Instance: {Instance} - Starting processing of child parts for PartId: {PartId}",
                        methodName, currentInstance, partId);
                    if (currentPart.ChildParts != null)
                    {
                        foreach (var childPartRaw in currentPart.ChildParts.Where(cp => cp != null)) // Safe iteration
                        {
                            // Explicitly qualify Part type due to potential namespace conflicts (CS0436 warning)
                            var childPart = (WaterNut.DataSpace.Part)childPartRaw;
                            var childPartId = childPart.OCR_Part?.Id ?? -1; // Safe access

                            // Check if child part has any lines before proceeding
                            if (childPart.AllLines == null || !childPart.AllLines.Any())
                            {
                                _logger.Verbose(
                                    "{MethodName}: Instance: {Instance} - Skipping ChildPartId: {ChildPartId} because it has no lines.",
                                    methodName, currentInstance, childPartId);
                                continue;
                            }

                            _logger.Debug("{MethodName}: Instance: {Instance} - Processing ChildPartId: {ChildPartId}",
                                methodName, currentInstance, childPartId);

                            // --- Recursive Call ---
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - Recursively calling {MethodName} for ChildPartId: {ChildPartId} (FilterInstance=null)...",
                                methodName, currentInstance, methodName, childPartId);
                            var allChildItemsForPart = SetPartLineValues(childPart, null); // Recursive call
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - Recursive call finished for ChildPartId: {ChildPartId}. Received {TotalChildItems} total items.",
                                methodName, currentInstance, childPartId, allChildItemsForPart?.Count ?? 0);


                            // --- Filter Recursive Results ---
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - Filtering {TotalChildItems} recursive results for ChildPartId: {ChildPartId} to match parent instance.",
                                methodName, currentInstance, allChildItemsForPart?.Count ?? 0, childPartId);
                            var rawChildItems = allChildItemsForPart?
                                .Where(item => item != null &&
                                               item.TryGetValue("Instance", out var instObj) &&
                                               instObj is int inst &&
                                               inst == currentInstance)
                                .ToList() ?? new List<IDictionary<string, object>>();
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - Found {FilteredChildCount} items matching parent instance for ChildPartId: {ChildPartId}.",
                                methodName, currentInstance, rawChildItems.Count, childPartId);

                            if (!rawChildItems.Any())
                            {
                                _logger.Verbose(
                                    "{MethodName}: Instance: {Instance} - No items matched parent instance for ChildPartId: {ChildPartId}. Skipping deduplication and attachment.",
                                    methodName, currentInstance, childPartId);
                                continue; // Skip to next child part if no relevant items found
                            }

                            // --- Deduplicate Filtered Child Items ---
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - Deduplicating {FilteredChildCount} child items for ChildPartId: {ChildPartId} based on positional index and section precedence...",
                                methodName, currentInstance, rawChildItems.Count, childPartId);
                            var itemsWithIndex = rawChildItems
                                .GroupBy(item => item.TryGetValue("Section", out var sec) ? sec as string : "Unknown")
                                .SelectMany(sectionGroup => sectionGroup.Select((item, index) =>
                                    new { Item = item, Index = index, Section = sectionGroup.Key }))
                                .ToList();
                            _logger.Verbose(
                                "{MethodName}: Instance: {Instance} - ChildPartId: {ChildPartId} - Items with index: {Count}",
                                methodName, currentInstance, childPartId, itemsWithIndex.Count);


                            var deduplicatedChildItems = itemsWithIndex
                                .GroupBy(indexedItem => indexedItem.Index)
                                .OrderBy(indexGroup => indexGroup.Key)
                                .Select(indexGroup =>
                                {
                                    var orderedByPrecedence = indexGroup
                                        .OrderBy(item => GetSectionPrecedence(item.Section)).ToList();
                                    var chosenItem = orderedByPrecedence.First().Item;
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - ChildPartId: {ChildPartId} - Index {Index}: Choosing item from Section '{ChosenSection}' (Precedence: {Precedence}). Candidates: [{Candidates}]",
                                        methodName, currentInstance, childPartId, indexGroup.Key,
                                        orderedByPrecedence.First().Section,
                                        GetSectionPrecedence(orderedByPrecedence.First().Section),
                                        string.Join(", ", orderedByPrecedence.Select(i => i.Section)));
                                    return chosenItem;
                                })
                                .ToList();

                            _logger.Information(
                                "{MethodName}: PartId: {PartId}, Instance: {Instance}: ChildPartId: {ChildPartId} - Raw items: {RawCount}, Deduplicated items: {DedupCount} (using positional index).",
                                methodName, partId, currentInstance, childPartId, rawChildItems.Count,
                                deduplicatedChildItems.Count);

                            // --- Attach Deduplicated Child List ---
                            var entityTypeField = childPart.AllLines.FirstOrDefault()?
                                .OCR_Lines?.Fields?.FirstOrDefault()?
                                .EntityType;
                            var fieldname = !string.IsNullOrEmpty(entityTypeField)
                                ? entityTypeField
                                : $"ChildPart_{childPartId}"; // Use EntityType or default

                            _logger.Debug(
                                "{MethodName}: Instance: {Instance} - Attaching {DedupCount} deduplicated child items for ChildPartId: {ChildPartId} under field '{FieldName}'.",
                                methodName, currentInstance, deduplicatedChildItems.Count, childPartId, fieldname);
                            if (parentDitm.ContainsKey(fieldname))
                            {
                                if (parentDitm[fieldname] is List<IDictionary<string, object>> existingList)
                                {
                                    _logger.Verbose(
                                        "{MethodName}: Instance: {Instance} - Appending child items to existing list under field '{FieldName}'.",
                                        methodName, currentInstance, fieldname);
                                    existingList.AddRange(deduplicatedChildItems);
                                }
                                else
                                {
                                    _logger.Warning(
                                        "{MethodName}: Field '{FieldName}' exists but is not a List<IDictionary<string, object>> for Instance: {Instance}, ChildPartId: {ChildPartId}. Overwriting with new list.",
                                        methodName, fieldname, currentInstance, childPartId);
                                    parentDitm[fieldname] = deduplicatedChildItems;
                                }
                            }
                            else
                            {
                                _logger.Verbose(
                                    "{MethodName}: Instance: {Instance} - Adding new list of child items under field '{FieldName}'.",
                                    methodName, currentInstance, fieldname);
                                parentDitm[fieldname] = deduplicatedChildItems;
                            }

                            if (deduplicatedChildItems.Any())
                                parentDataFound = true; // Mark parent as having data if children have data
                        } // End foreach childPart

                        _logger.Verbose(
                            "{MethodName}: Instance: {Instance} - Finished processing child parts for PartId: {PartId}",
                            methodName, currentInstance, partId);
                    }
                    else
                    {
                        _logger.Verbose(
                            "{MethodName}: Instance: {Instance} - No child parts found for PartId: {PartId}",
                            methodName, currentInstance, partId);
                    }


                    // --- Add Assembled Item to Final List ---
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
                } // End foreach currentInstance


                _logger.Information(
                    "Exiting {MethodName} successfully for PartId: {PartId}. Returning {ItemCount} items.", methodName,
                    partId, finalPartItems.Count);
                return finalPartItems;
            }
            catch (Exception e)
            {
                _logger.Error(e,
                    "{MethodName}: Unhandled exception during processing for PartId: {PartId}, FilterInstance: {FilterInstance}",
                    methodName, partId, filterInstanceStr);
                _logger.Information("Exiting {MethodName} for PartId: {PartId} due to exception.", methodName, partId);
                throw; // Re-throw exception
            }
        }
    }
}
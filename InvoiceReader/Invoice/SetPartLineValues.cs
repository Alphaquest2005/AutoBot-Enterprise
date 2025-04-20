using Core.Common.Extensions;
using Core.Common; // Added for BetterExpando
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Part, Line, Fields

namespace WaterNut.DataSpace;

public partial class Invoice
{
    // Assuming _logger exists from another partial part
    // private static readonly ILogger _logger = Log.ForContext<Invoice>();

    private List<IDictionary<string, object>> SetPartLineValues(Part part, int? filterInstance = null)
    {
        int? partId = part?.OCR_Part?.Id; // Safe access
        _logger.Debug("Entering SetPartLineValues for PartId: {PartId}, FilterInstance: {FilterInstance}",
            partId, filterInstance?.ToString() ?? "None (Top Level)");

        var finalPartItems = new List<IDictionary<string, object>>();

        // Add null check for part and critical properties
        if (part == null || part.OCR_Part == null)
        {
             _logger.Error("SetPartLineValues called with null Part or OCR_Part. Returning empty list.");
             return finalPartItems;
        }
         if (part.Lines == null)
         {
              _logger.Warning("PartId: {PartId} has null Lines collection. Returning empty list.", partId);
              return finalPartItems;
         }


        try
        {
            // 2. Determine the instances to process for THIS part based on the filter
             _logger.Verbose("Determining instances to process for PartId: {PartId}, FilterInstance: {FilterInstance}", partId, filterInstance?.ToString() ?? "None");
            var instancesToProcess = part.Lines
                .Where(line => line?.Values != null) // Safe navigation
                .SelectMany(line => line.Values.SelectMany(v => v.Value?.Keys ?? Enumerable.Empty<(Fields fields, int instance)>())) // Safe navigation
                .Where(k => !filterInstance.HasValue || k.instance == filterInstance.Value)
                .Select(k => k.instance)
                .Distinct()
                .OrderBy(instance => instance)
                .ToList();
             _logger.Verbose("Found {Count} initial instances for PartId: {PartId}: [{Instances}]", instancesToProcess.Count, partId, string.Join(",", instancesToProcess));

            // If filtering, check if children might have data even if parent doesn't
            if (filterInstance.HasValue && !instancesToProcess.Any())
            {
                 _logger.Verbose("Checking child parts for FilterInstance: {FilterInstance} as parent PartId: {PartId} has no direct data for it.", filterInstance.Value, partId);
                 // Added null checks for safety
                 bool childHasDataForInstance = part.ChildParts != null && part.ChildParts
                    .Where(cp => cp?.AllLines != null) // Check child part and its lines
                    .SelectMany(cp => cp.AllLines)
                    .Where(l => l?.Values != null) // Check line and its values
                    .SelectMany(l => l.Values.SelectMany(v => v.Value?.Keys ?? Enumerable.Empty<(Fields fields, int instance)>())) // Check inner dict and keys
                    .Any(k => k.instance == filterInstance.Value);

                if (childHasDataForInstance)
                {
                     _logger.Information("PartId: {PartId}: No direct data for FilterInstance: {FilterInstance}, but children have data. Adding instance for child aggregation.", partId, filterInstance.Value);
                    instancesToProcess = new List<int> { filterInstance.Value };
                } else {
                     _logger.Verbose("PartId: {PartId}: Neither parent nor children have data for FilterInstance: {FilterInstance}.", partId, filterInstance.Value);
                }
            }

            // If still no instances after checking children (or not filtering), return empty.
            if (!instancesToProcess.Any())
            {
                 _logger.Information("PartId: {PartId}: No relevant instances found to process{FilterContext}. Returning empty list.",
                    partId, filterInstance.HasValue ? $" for FilterInstance: {filterInstance.Value}" : "");
                return finalPartItems;
            }

             _logger.Information("PartId: {PartId}: Processing instances [{Instances}]{FilterContext}",
                partId, string.Join(", ", instancesToProcess), filterInstance.HasValue ? $" (Filtered by Parent Instance: {filterInstance.Value})" : "");


            // 3. Iterate through each distinct instance found for this part
            foreach (var currentInstance in instancesToProcess)
            {
                 _logger.Debug("Processing Instance: {Instance} for PartId: {PartId}", currentInstance, partId);
                var parentItem = new BetterExpando();
                var parentDitm = (IDictionary<string, object>)parentItem;
                bool parentDataFound = false;
                // Keep track of fields populated and their source section for precedence
                var fieldSourceSections = new Dictionary<string, string>();

                // 4. Populate fields for the current parent instance, respecting section precedence
                var sectionsInOrder = new[] { "Single", "Ripped", "Sparse" }; // Order based on precedence
                 _logger.Verbose("Instance: {Instance} - Iterating through sections for parent data: {Sections}", currentInstance, sectionsInOrder);
                foreach (var sectionName in sectionsInOrder)
                {
                     _logger.Verbose("Instance: {Instance} - Checking section: '{SectionName}'", currentInstance, sectionName);
                     // Find all values for the current instance AND current section safely
                     var sectionInstanceValues = part.Lines
                         .Where(line => line?.Values != null) // Ensure line and Values exist
                         .SelectMany(line => line.Values
                             .Where(v => v.Key.section == sectionName && v.Value != null) // Filter by section, ensure inner dict not null
                             .SelectMany(v => v.Value.Where(kvp => kvp.Key.instance == currentInstance)) // Filter by instance
                         ).ToList();

                    if (sectionInstanceValues.Any())
                    {
                         _logger.Debug("Instance: {Instance} - Found {Count} field values in section '{SectionName}'", currentInstance, sectionInstanceValues.Count, sectionName);
                         parentDataFound = true; // Mark data found if any section has values for this instance

                         // Set FileLineNumber, Instance, Section based on the *first* value found in any section for this instance
                         if (!parentDitm.ContainsKey("Instance")) // Check if instance details already set
                         {
                             var firstValue = sectionInstanceValues.First();
                             // Find the original Line object and its KeyValuePair to get the line number accurately
                             var sourceLineValue = part.Lines
                                 .Where(l => l?.Values != null)
                                 .SelectMany(l => l.Values.Select(v => new { LineNumber = v.Key.lineNumber, ValuesDict = v.Value }))
                                 .FirstOrDefault(lv => lv.ValuesDict != null && lv.ValuesDict.Any(kvp => kvp.Key == firstValue.Key));

                             if (sourceLineValue != null)
                             {
                                 parentDitm["FileLineNumber"] = sourceLineValue.LineNumber + 1; // Adjust to 1-based? Check requirement.
                                 parentDitm["Instance"] = currentInstance;
                                 parentDitm["Section"] = sectionName; // Use the section where the first value was found
                                  _logger.Verbose("Instance: {Instance} - Set initial metadata: FileLineNumber={LineNum}, Section='{Section}'", currentInstance, parentDitm["FileLineNumber"], sectionName);
                             }
                             else
                             {
                                 // Fallback if somehow the source line isn't found
                                 parentDitm["FileLineNumber"] = 0;
                                 parentDitm["Instance"] = currentInstance;
                                 parentDitm["Section"] = sectionName;
                                  _logger.Warning("Could not find source line number for first value in Instance: {Instance}, Section: {Section}. Setting FileLineNumber to 0.", currentInstance, sectionName);
                             }
                         }

                         // Process each field found in this section for this instance
                         foreach (var fieldKvp in sectionInstanceValues)
                         {
                             var field = fieldKvp.Key.fields;
                             // Safe check for field and field name
                             if (field == null || string.IsNullOrEmpty(field.Field))
                             {
                                  _logger.Warning("Skipping field value in Instance: {Instance}, Section: {Section} because Field object or Field name is null.", currentInstance, sectionName);
                                  continue;
                             }
                             var fieldName = field.Field;

                             // Check precedence: Only add/overwrite if the field hasn't been populated by a higher-precedence section
                             if (!fieldSourceSections.ContainsKey(fieldName))
                             {
                                 // This field hasn't been seen yet for this instance.
                                 _logger.Verbose("Instance: {Instance} - Setting Field '{FieldName}' from Section '{SectionName}'.", currentInstance, fieldName, sectionName);
                                 parentDitm[fieldName] = GetValue(fieldKvp); // GetValue handles conversion and logging
                                 fieldSourceSections.Add(fieldName, sectionName); // Record that this field was populated and from which section
                                  _logger.Verbose("Instance: {Instance} - Field '{FieldName}' set from Section '{SectionName}'. Value: '{Value}'", currentInstance, fieldName, sectionName, parentDitm[fieldName]);
                             }
                             else
                             {
                                 // Field already set from a higher precedence section, ignore this one.
                                 _logger.Verbose("Instance: {Instance} - Field '{FieldName}' already set from Section '{ExistingSection}'. Ignoring value '{Value}' from Section '{CurrentSection}'.",
                                    currentInstance, fieldName, fieldSourceSections[fieldName], fieldKvp.Value, sectionName);
                             }
                         }
                    } else {
                         _logger.Verbose("Instance: {Instance} - No field values found in section '{SectionName}'", currentInstance, sectionName);
                    }
                } // End foreach sectionName

                // 5. Process and attach child part data for the CURRENT parent instance
                 _logger.Verbose("Instance: {Instance} - Processing child parts for PartId: {PartId}", currentInstance, partId);
                 if (part.ChildParts != null)
                 {
                     foreach (var childPart in part.ChildParts.Where(cp => cp != null && cp.AllLines != null && cp.AllLines.Any())) // Safe iteration
                     {
                         var childPartId = childPart.OCR_Part?.Id ?? -1; // Safe access
                          _logger.Debug("Instance: {Instance} - Processing ChildPartId: {ChildPartId}", currentInstance, childPartId);

                         // Call SetPartLineValues recursively for the child WITHOUT filtering by instance yet
                          _logger.Verbose("Instance: {Instance} - Recursively calling SetPartLineValues for ChildPartId: {ChildPartId} (FilterInstance=null)", currentInstance, childPartId);
                         var allChildItemsForPart = SetPartLineValues(childPart, null); // Recursive call

                         // Filter the results to get only items matching the parent's currentInstance
                          _logger.Verbose("Instance: {Instance} - Filtering {TotalChildItems} recursive results for ChildPartId: {ChildPartId} to match parent instance.",
                             currentInstance, allChildItemsForPart?.Count ?? 0, childPartId);
                         var rawChildItems = allChildItemsForPart?
                             .Where(item => item != null && // Check item not null
                                             item.TryGetValue("Instance", out var instObj) &&
                                             instObj is int inst &&
                                             inst == currentInstance)
                             .ToList() ?? new List<IDictionary<string, object>>(); // Handle null allChildItemsForPart
                          _logger.Verbose("Instance: {Instance} - Found {FilteredChildCount} items matching parent instance for ChildPartId: {ChildPartId}.",
                             currentInstance, rawChildItems.Count, childPartId);


                         // Deduplicate the filtered child items based on positional index and section precedence
                          _logger.Verbose("Instance: {Instance} - Deduplicating {FilteredChildCount} child items for ChildPartId: {ChildPartId} based on positional index and section precedence.",
                             currentInstance, rawChildItems.Count, childPartId);
                         var itemsWithIndex = rawChildItems
                             .GroupBy(item => item.TryGetValue("Section", out var sec) ? sec as string : "Unknown") // Group by Section first
                             .SelectMany(sectionGroup => sectionGroup.Select((item, index) => new { Item = item, Index = index, Section = sectionGroup.Key })) // Assign index within section
                             .ToList();

                         var deduplicatedChildItems = itemsWithIndex
                             .GroupBy(indexedItem => indexedItem.Index) // Group by positional index across sections
                             .OrderBy(indexGroup => indexGroup.Key) // Order by index
                             .Select(indexGroup => indexGroup
                                     .OrderBy(item => GetSectionPrecedence(item.Section)) // Order by section precedence within index group
                                     .First().Item // Select the item from the highest precedence section for this index
                             ).ToList();

                          _logger.Information("PartId: {PartId}, Instance: {Instance}: ChildPartId: {ChildPartId} - Raw items: {RawCount}, Deduplicated items: {DedupCount} (using positional index).",
                             partId, currentInstance, childPartId, rawChildItems.Count, deduplicatedChildItems.Count);

                         // Determine field name for attaching child list
                         var fieldname = childPart.AllLines.FirstOrDefault()? // Safe access
                                             .OCR_Lines?.Fields?.FirstOrDefault()? // Safe access
                                             .EntityType ?? $"ChildPart_{childPartId}"; // Default name

                         // Attach the deduplicated list
                          _logger.Debug("Instance: {Instance} - Attaching {DedupCount} deduplicated child items for ChildPartId: {ChildPartId} under field '{FieldName}'.",
                             currentInstance, deduplicatedChildItems.Count, childPartId, fieldname);
                         if (parentDitm.ContainsKey(fieldname))
                         {
                             // Handle potential type mismatch if key exists but isn't a list
                             if (parentDitm[fieldname] is List<IDictionary<string, object>> existingList)
                             {
                                  _logger.Verbose("Instance: {Instance} - Appending child items to existing list under field '{FieldName}'.", currentInstance, fieldname);
                                  existingList.AddRange(deduplicatedChildItems);
                             }
                             else
                             {
                                  _logger.Warning("Field '{FieldName}' exists but is not a List<IDictionary<string, object>> for Instance: {Instance}, ChildPartId: {ChildPartId}. Overwriting with new list.",
                                     fieldname, currentInstance, childPartId);
                                  parentDitm[fieldname] = deduplicatedChildItems;
                             }
                         }
                         else
                         {
                              _logger.Verbose("Instance: {Instance} - Adding new list of child items under field '{FieldName}'.", currentInstance, fieldname);
                              parentDitm[fieldname] = deduplicatedChildItems;
                         }

                         if (deduplicatedChildItems.Any()) parentDataFound = true; // Mark parent as having data if children have data
                     } // End foreach childPart
                 } else {
                      _logger.Verbose("Instance: {Instance} - No child parts found for PartId: {PartId}", currentInstance, partId);
                 }


                // 6. Add the assembled parent item (with its associated child data) to the final list
                // Only add if we actually found data for this parent instance OR its children attached data
                if (parentDataFound) // Add if any data (parent or child) was found for this instance
                {
                     _logger.Information("PartId: {PartId}: Adding assembled item for Instance: {Instance} to final results.", partId, currentInstance);
                     finalPartItems.Add(parentItem);
                }
                else
                {
                     _logger.Warning("PartId: {PartId}: Skipping empty or incomplete item for Instance: {Instance} (no parent or child data found).", partId, currentInstance);
                }
            } // End foreach currentInstance


             _logger.Information("Finished SetPartLineValues for PartId: {PartId}. Returning {ItemCount} items.", partId, finalPartItems.Count);
            return finalPartItems;
        }
        catch (Exception e)
        {
             _logger.Error(e, "Error during SetPartLineValues for PartId: {PartId}, FilterInstance: {FilterInstance}", partId, filterInstance?.ToString() ?? "None");
             throw; // Re-throw exception as per original code
        }
    }
}
using Core.Common.Extensions;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private List<IDictionary<string, object>> SetPartLineValues(Part part, int? filterInstance = null)
    {
        Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Processing Part ID {part.OCR_Part.Id}" +
                          (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : " (Top Level)"));
        var finalPartItems = new List<IDictionary<string, object>>();
        try
        {
            // Child parts are processed recursively *within* the instance loop below.

            // 2. Determine the instances to process for THIS part based on the filter
            var instancesToProcess = part.Lines
                .SelectMany(line => line.Values.SelectMany(v => v.Value.Keys))
                .Where(k => !filterInstance.HasValue || k.instance == filterInstance.Value)
                .Select(k => k.instance)
                .Distinct()
                .OrderBy(instance => instance)
                .ToList();

            // If filtering, check if children might have data even if parent doesn't
            if (filterInstance.HasValue && !instancesToProcess.Any())
            {
                if (part.ChildParts.Any(cp => cp.AllLines.Any(l =>
                        l.Values.SelectMany(v => v.Value.Keys).Any(k => k.instance == filterInstance.Value))))
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: No direct data for Filter Instance {filterInstance.Value}, but children might. Processing instance {filterInstance.Value} for child aggregation.");
                    instancesToProcess = new List<int> { filterInstance.Value };
                }
            }

            // If still no instances after checking children (or not filtering), return empty.
            if (!instancesToProcess.Any())
            {
                Console.WriteLine(
                    $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: No relevant instances found" +
                    (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : "") +
                    ". Returning empty list.");
                return finalPartItems;
            }

            Console.WriteLine(
                $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Processing instances [{string.Join(", ", instancesToProcess)}]" +
                (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : ""));

            // Child parts already processed above


            // 3. Iterate through each distinct instance found for the PARENT part
            // 3. Iterate through the relevant instances for this part
            foreach (var currentInstance in instancesToProcess)
            {
                Console.WriteLine(
                    $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Processing instance {currentInstance}.");
                var parentItem = new BetterExpando();
                var parentDitm = (IDictionary<string, object>)parentItem;
                bool parentDataFound = false;
                // Keep track of fields populated and their source section for precedence
                var fieldSourceSections = new Dictionary<string, string>();

                // 4. Populate fields for the current parent instance, respecting section precedence
                // Iterate through sections in order of precedence: Single -> Ripped -> Sparse (NEW ORDER based on user feedback)
                var sectionsInOrder = new[] { "Single", "Ripped", "Sparse" };
                foreach (var sectionName in sectionsInOrder)
                {
                    // Find all values for the current instance AND current section
                    var sectionInstanceValues = part.Lines
                        .SelectMany(line => line.Values
                            .Where(v => v.Key.section == sectionName) // Filter by section first
                            .SelectMany(v => v.Value.Where(kvp => kvp.Key.instance == currentInstance))
                        ).ToList();

                    if (sectionInstanceValues.Any())
                    {
                        parentDataFound = true; // Mark data found if any section has values for this instance

                        // Set FileLineNumber, Instance, Section based on the *first* value found in any section for this instance
                        if (!parentDitm.ContainsKey("Instance"))
                        {
                            var firstValue = sectionInstanceValues.First();
                            // Find the original Line object and its KeyValuePair to get the line number accurately
                            var sourceLineValue = part.Lines
                                .SelectMany(l => l.Values.Select(v => new { Line = l, ValuePair = v }))
                                .FirstOrDefault(lv => lv.ValuePair.Value.Any(kvp => kvp.Key == firstValue.Key));

                            if (sourceLineValue != null)
                            {
                                parentDitm["FileLineNumber"] = sourceLineValue.ValuePair.Key.lineNumber + 1;
                                parentDitm["Instance"] = currentInstance;
                                parentDitm["Section"] = sectionName; // Use the section where the first value was found
                            }
                            else
                            {
                                // Fallback if somehow the source line isn't found (shouldn't happen)
                                parentDitm["FileLineNumber"] = 0;
                                parentDitm["Instance"] = currentInstance;
                                parentDitm["Section"] = sectionName;
                                Console.WriteLine(
                                    $"[OCR WARNING] SetPartLineValues: Could not find source line for first value in instance {currentInstance}, section {sectionName}."); // Use Console.WriteLine
                            }
                        }

                        foreach (var fieldKvp in sectionInstanceValues)
                        {
                            var field = fieldKvp.Key.fields;
                            var fieldName = field.Field;

                            // Check precedence: Only add/overwrite if the field hasn't been populated by a higher-precedence section
                            if (!fieldSourceSections.ContainsKey(fieldName))
                            {
                                // This field hasn't been seen yet for this instance.
                                parentDitm[fieldName] = GetValue(fieldKvp);
                                fieldSourceSections.Add(fieldName,
                                    sectionName); // Record that this field was populated and from which section
                                Console.WriteLine(
                                    $"[OCR DEBUG] SetPartLineValues: Instance {currentInstance}, Field '{fieldName}' set from section '{sectionName}'. Value: '{parentDitm[fieldName]}'");
                            }
                            else
                            {
                                // Field already set from a higher precedence section, ignore this one.
                                Console.WriteLine(
                                    $"[OCR DEBUG] SetPartLineValues: Instance {currentInstance}, Field '{fieldName}' already set from section '{fieldSourceSections[fieldName]}'. Ignoring value '{fieldKvp.Value}' from section '{sectionName}'.");
                            }
                            // NOTE: The AppendValues logic for summing *different* fields needs separate implementation if required.
                            // This current logic focuses solely on section precedence for the *same* field.
                        }
                    }
                } // End foreach sectionName

                // 5. Process and attach child part data for the CURRENT parent instance
                // 4. Process and attach child part data for the CURRENT parent instance
                foreach (var childPart in part.ChildParts.Where(x => x.AllLines.Any()))
                {
                    var childPartId = childPart.OCR_Part.Id;
                    // Call SetPartLineValues recursively for the child WITHOUT filtering by instance yet
                    var allChildItemsForPart = SetPartLineValues(childPart, null);

                    // NOW filter the results to get only items matching the parent's currentInstance
                    var rawChildItems = allChildItemsForPart.Where(item =>
                        item.TryGetValue("Instance", out var instObj) &&
                        instObj is int inst &&
                        inst == currentInstance).ToList();

                    // Deduplicate the filtered child items based on positional index and section precedence
                    var itemsWithIndex = rawChildItems
                        .GroupBy(item =>
                            item.TryGetValue("Section", out var sec)
                                ? sec as string
                                : "Unknown") // Group by Section first
                        .SelectMany(sectionGroup => sectionGroup.Select((item, index) =>
                            new
                            {
                                Item = item, Index = index, Section = sectionGroup.Key
                            })) // Assign index within section
                        .ToList();

                    var deduplicatedChildItems = itemsWithIndex
                        .GroupBy(indexedItem => indexedItem.Index) // Group by positional index across sections
                        .OrderBy(indexGroup => indexGroup.Key) // Order by index
                        .Select(indexGroup => indexGroup
                                .OrderBy(item =>
                                    GetSectionPrecedence(item
                                        .Section)) // Order by section precedence within index group
                                .First().Item // Select the item from the highest precedence section for this index
                        ).ToList();

                    Console.WriteLine(
                        $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}, Instance {currentInstance}: Child Part ID {childPartId} - Raw items: {rawChildItems.Count}, Deduplicated items: {deduplicatedChildItems.Count} (using positional index).");

                    var fieldname =
                        childPart.AllLines.FirstOrDefault()?.OCR_Lines?.Fields?.FirstOrDefault()?.EntityType ??
                        $"ChildPart_{childPartId}";

                    // Attach the deduplicated list (always treat as a list, even if non-recurring child results in 0 or 1 item after deduplication)
                    Console.WriteLine(
                        $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}, Instance {currentInstance}: Attaching {deduplicatedChildItems.Count} deduplicated child items for Child Part ID {childPartId} under field '{fieldname}'.");
                    if (parentDitm.ContainsKey(fieldname))
                    {
                        if (parentDitm[fieldname] is List<IDictionary<string, object>> existingList)
                        {
                            existingList.AddRange(deduplicatedChildItems);
                        }
                        else
                        {
                            Console.WriteLine(
                                $"[OCR WARNING] Invoice.SetPartLineValues: Field '{fieldname}' exists but is not a list for child {childPartId}. Overwriting.");
                            parentDitm[fieldname] = deduplicatedChildItems;
                        }
                    }
                    else
                    {
                        parentDitm[fieldname] = deduplicatedChildItems;
                    }

                    if (deduplicatedChildItems.Any()) parentDataFound = true;
                } // End foreach childPart

                // 6. Add the assembled parent item (with its associated child data) to the final list
                // Only add if we actually found data for this parent instance OR its children attached data
                if (parentDataFound) // Add if any data (parent or child) was found for this instance
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Adding assembled item for instance {currentInstance} to results.");
                    finalPartItems.Add(parentItem);
                }
                else
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Skipping empty or incomplete item for instance {currentInstance}.");
                }
            }


            Console.WriteLine(
                $"[OCR DEBUG] Invoice.SetPartLineValues: Finished processing Part ID {part.OCR_Part.Id}. Returning {finalPartItems.Count} items.");
            return finalPartItems;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                $"[OCR ERROR] Invoice.SetPartLineValues: Exception processing Part ID {part.OCR_Part.Id}: {e}");
            throw;
        }
    }
}
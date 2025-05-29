using Core.Common.Extensions;
using Core.Common; // Added for BetterExpando
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Part, Line, Fields

namespace WaterNut.DataSpace
{
    using MoreLinq;

    public partial class Invoice
    {
        private List<IDictionary<string, object>> SetPartLineValues(Part part, string filterInstance = null)
        {
            //_logger.Debug("SetPartLineValues - Parameters: Part: {@part}, FilterInstance: {filterInstance}", part.Lines, filterInstance);

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
                    ProcessInstanceWithItemConsolidation(currentPart, currentInstance, partId, methodName, finalPartItems);
                }

                _logger.Information(
                    "Exiting {MethodName} successfully for PartId: {PartId}. Returning {ItemCount} items.", methodName,
                    partId, finalPartItems.Count);
                _logger.Verbose("{MethodName}: PartId: {PartId} - Final items before returning: {@FinalItems}",
                    methodName, partId, finalPartItems);
                //   _logger.Debug("SetPartLineValues - Return Parameters: finalPartItems: {@finalPartItems}", finalPartItems);
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
                instancesToProcess = CheckChildPartsForInstances(currentPart, filterInstance, partId, methodName);
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

        private void ProcessInstanceWithItemConsolidation(Part currentPart, IGrouping<string, string> currentInstance, int? partId, string methodName, List<IDictionary<string, object>> finalPartItems)
        {
            _logger.Debug("{MethodName}: Starting consolidation processing for Instance: {Instance} of PartId: {PartId}",
                methodName, currentInstance.Key, partId);

            // Collect ALL field data across all sections and lines for this instance
            var allFieldDataForInstance = CollectAllFieldDataForInstance(currentPart, currentInstance.First(), methodName);

            if (!allFieldDataForInstance.Any())
            {
                _logger.Information("{MethodName}: No field data found for Instance: {Instance}, checking child parts only",
                    methodName, currentInstance.Key);

                // Even if no parent data, we might have child data
                var emptyParentItem = new BetterExpando();
                var emptyParentDict = (IDictionary<string, object>)emptyParentItem;
                bool hasChildData = false;

                foreach (var childInstance in currentInstance)
                {
                    ProcessChildParts(currentPart, childInstance, emptyParentDict, ref hasChildData, partId, methodName);
                }

                if (hasChildData)
                {
                    SetInstanceMetadata(emptyParentDict, currentInstance.First(), "Unknown", 0);
                    finalPartItems.Add(emptyParentItem);
                }
                return;
            }

            // Group field data by logical invoice items (based on line groupings)
            var logicalInvoiceItems = GroupIntoLogicalInvoiceItems(allFieldDataForInstance, methodName, currentInstance.Key);

            _logger.Information("{MethodName}: Instance: {Instance} - Found {LogicalItemCount} logical invoice items after consolidation",
                methodName, currentInstance.Key, logicalInvoiceItems.Count);

            // Create a final item for each logical invoice item
            foreach (var logicalItem in logicalInvoiceItems)
            {
                var parentItem = new BetterExpando();
                var parentDict = (IDictionary<string, object>)parentItem;
                bool parentDataFound = true;

                // Set consolidated field data
                SetInstanceMetadata(parentDict, currentInstance.First(), logicalItem.BestSection, logicalItem.PrimaryLineNumber);

                foreach (var fieldData in logicalItem.ConsolidatedFields)
                {
                    parentDict[fieldData.Key] = fieldData.Value;
                }

                // Process child parts for this instance
                foreach (var childInstance in currentInstance)
                {
                    ProcessChildParts(currentPart, childInstance, parentDict, ref parentDataFound, partId, methodName);
                }

                if (parentDataFound)
                {
                    _logger.Information(
                        "{MethodName}: PartId: {PartId}: Adding consolidated item for Instance: {Instance}, Line: {LineNumber}",
                        methodName, partId, currentInstance.Key, logicalItem.PrimaryLineNumber);
                    finalPartItems.Add(parentItem);
                }
            }

            _logger.Debug("{MethodName}: Finished consolidation processing for Instance: {Instance} of PartId: {PartId}, created {ItemCount} logical items",
                methodName, currentInstance.Key, partId, logicalInvoiceItems.Count);
        }

        private List<FieldCapture> CollectAllFieldDataForInstance(Part currentPart, string currentInstance, string methodName)
        {
            var allFieldData = new List<FieldCapture>();
            var sectionsInOrder = new[] { "Single", "Ripped", "Sparse" };

            _logger.Verbose("{MethodName}: Collecting all field data for Instance: {Instance}", methodName, currentInstance);

            foreach (var sectionName in sectionsInOrder)
            {
                _logger.Verbose("{MethodName}: Processing section '{SectionName}' for Instance: {Instance}",
                    methodName, sectionName, currentInstance);

                var sectionFieldData = currentPart.Lines
                    .Where(line => line?.Values != null)
                    .SelectMany(line => line.Values
                        .Where(v => v.Key.section == sectionName && v.Value != null)
                        .SelectMany(v =>
                        {

                            return v.Value.Where(kvp => kvp.Key.Instance == currentInstance).Select(g => (v.Key, g));
                        })
                        .Select(kvp => new FieldCapture
                        {
                            Section = sectionName,
                            LineNumber = kvp.Key.lineNumber,
                            FieldName = kvp.g.Key.Fields?.Field,
                            FieldValue = GetValue(kvp.g, _logger),
                            Field = kvp.g.Key.Fields,
                            RawValue = kvp.g.Value
                        })
                    )
                    .Where(fc => !string.IsNullOrEmpty(fc.FieldName))
                    .ToList();

                allFieldData.AddRange(sectionFieldData);

                _logger.Verbose("{MethodName}: Section '{SectionName}' contributed {FieldCount} field captures for Instance: {Instance}",
                    methodName, sectionName, sectionFieldData.Count, currentInstance);
            }

            _logger.Debug("{MethodName}: Collected {TotalFieldCount} total field captures for Instance: {Instance}",
                methodName, allFieldData.Count, currentInstance);

            return allFieldData;
        }

        private List<LogicalInvoiceItem> GroupIntoLogicalInvoiceItems(List<FieldCapture> allFieldData, string methodName, string currentInstance)
        {
            // **VERSION CHECK**: This log message proves the new code is running
            _logger.Information("**NEW CODE VERSION**: GroupIntoLogicalInvoiceItems called for Instance: {Instance} with {FieldCount} total captures",
                currentInstance, allFieldData.Count);

            _logger.Verbose("{MethodName}: Grouping {FieldCount} field captures into logical items for Instance: {Instance}",
                methodName, allFieldData.Count, currentInstance);

            var logicalItems = new List<LogicalInvoiceItem>();

            if (!allFieldData.Any())
            {
                _logger.Verbose("{MethodName}: No field data found for Instance: {Instance}", methodName, currentInstance);
                return logicalItems;
            }

            // **CRITICAL FIX**: For header/summary parts, we want ALL fields including totals, taxes, shipping, etc.
            // Don't filter out "header" fields - they ARE the data we want for invoice summaries
            var allFieldsForProcessing = allFieldData.Where(fc => !string.IsNullOrWhiteSpace(fc.RawValue)).ToList();

            _logger.Information("{MethodName}: Instance: {Instance} - Processing {TotalFieldCount} fields (keeping ALL fields for invoice summary)",
                methodName, currentInstance, allFieldsForProcessing.Count);

            // Log all the field names we're processing so we can verify we're not filtering out important ones
            var fieldNames = allFieldsForProcessing.Select(fc => fc.FieldName).Distinct().OrderBy(f => f).ToList();
            _logger.Information("{MethodName}: Instance: {Instance} - Field names being processed: [{FieldNames}]",
                methodName, currentInstance, string.Join(", ", fieldNames));

            // **DEBUG**: Log all raw field data to see what products we have
            var productDescriptions = allFieldsForProcessing
                .Where(fc => fc.FieldName?.ToLower().Contains("description") == true ||
                            fc.FieldName?.ToLower().Contains("item") == true)
                .Select(fc => $"'{fc.RawValue}' (Line:{fc.LineNumber}, Section:{fc.Section})")
                .ToList();

            if (productDescriptions.Any())
            {
                _logger.Information("{MethodName}: Instance: {Instance} - Found product descriptions: [{ProductDescriptions}]",
                    methodName, currentInstance, string.Join(", ", productDescriptions));
            }

            // **DEBUG**: Group by line number to see how many logical products we should have
            var lineGroups = allFieldsForProcessing
                .GroupBy(fc => fc.LineNumber)
                .Select(lg => new {
                    LineNumber = lg.Key,
                    FieldCount = lg.Count(),
                    ProductDesc = lg.FirstOrDefault(fc => fc.FieldName?.ToLower().Contains("description") == true)?.RawValue ?? "Unknown"
                })
                .ToList();

            _logger.Information("{MethodName}: Instance: {Instance} - Line groups found: [{LineGroups}]",
                methodName, currentInstance,
                string.Join(", ", lineGroups.Select(lg => $"Line{lg.LineNumber}({lg.FieldCount}fields)='{lg.ProductDesc}'")));

            if (!allFieldsForProcessing.Any())
            {
                _logger.Information("{MethodName}: Instance: {Instance} - No field data found after filtering empty values", methodName, currentInstance);
                return logicalItems;
            }

            // **CRITICAL DECISION**: Check if this looks like line item data or header data
            var hasMultipleLines = lineGroups.Count > 1;
            var hasProductFields = productDescriptions.Any();

            // **IMPROVED LOGIC**: Also check field names to detect line item parts
            var hasLineItemFieldNames = fieldNames.Any(fn =>
                fn.ToLower().Contains("item") ||
                fn.ToLower().Contains("cost") ||
                fn.ToLower().Contains("quantity") ||
                fn.ToLower().Contains("description"));

            var looksLikeLineItemPart = hasProductFields && hasLineItemFieldNames;

            _logger.Information("{MethodName}: Instance: {Instance} - Analysis: MultipleLines={MultipleLines}, ProductFields={ProductFields}, LineItemFields={LineItemFields}",
                methodName, currentInstance, hasMultipleLines, hasProductFields, hasLineItemFieldNames);

            if (looksLikeLineItemPart && hasMultipleLines)
            {
                _logger.Information("{MethodName}: Instance: {Instance} - Detected MULTIPLE PRODUCTS ({ProductCount} lines), switching to line-item processing mode",
                    methodName, currentInstance, lineGroups.Count);

                // Process as separate line items (like the old product grouping logic)
                return ProcessAsLineItems(allFieldsForProcessing, methodName, currentInstance);
            }
            else if (looksLikeLineItemPart)
            {
                _logger.Information("{MethodName}: Instance: {Instance} - Detected LINE ITEM PART but single line, checking for product consolidation needs",
                    methodName, currentInstance);

                // Even with single line, if we have line item fields, we might need special handling
                // Check if all fields belong to the same product or if we need to split by product identifier
                return ProcessAsLineItems(allFieldsForProcessing, methodName, currentInstance);
            }
            else
            {
                _logger.Information("{MethodName}: Instance: {Instance} - Detected HEADER/SUMMARY data, using consolidated processing",
                    methodName, currentInstance);

                // Process as single consolidated item (current logic)
                return ProcessAsSingleItem(allFieldsForProcessing, methodName, currentInstance);
            }
        }

        private List<LogicalInvoiceItem> ProcessAsLineItems(List<FieldCapture> allFieldData, string methodName, string currentInstance)
        {
            _logger.Information("{MethodName}: ProcessAsLineItems - Creating separate items for each line/product for Instance: {Instance}",
                methodName, currentInstance);

            var logicalItems = new List<LogicalInvoiceItem>();

            // **NEW LOGICAL APPROACH**: Use line numbers as the primary indicator of logical inventory items
            // This is more reliable than product description matching since each line represents a real item

            _logger.Information("{MethodName}: Using LINE-BASED logical item detection for Instance: {Instance}",
                methodName, currentInstance);

            // Step 1: Find all unique line numbers across all sections - this tells us how many logical items we should have
            var allUniqueLineNumbers = allFieldData
                .Select(fc => fc.LineNumber)
                .Distinct()
                .OrderBy(ln => ln)
                .ToList();

            _logger.Information("{MethodName}: Found {UniqueLineCount} unique line numbers across all sections: [{LineNumbers}]",
                methodName, allUniqueLineNumbers.Count, string.Join(", ", allUniqueLineNumbers));

            // Step 2: For each logical line number, collect all field data across all sections
            var sequentialLineNumber = 1; // **FIX**: Add proper sequential numbering

            foreach (var lineNumber in allUniqueLineNumbers)
            {
                var allCapturesForLine = allFieldData
                    .Where(fc => fc.LineNumber == lineNumber)
                    .ToList();

                if (!allCapturesForLine.Any())
                    continue;

                // Determine the best section for this line (highest priority with data)
                var bestSection = DetermineBestSection(allCapturesForLine);

                var lineItem = new LogicalInvoiceItem
                {
                    PrimaryLineNumber = lineNumber, // Keep original file line number for reference
                    ConsolidatedFields = new Dictionary<string, object>(),
                    BestSection = bestSection,
                    AllCaptures = allCapturesForLine
                };

                // Step 3: Consolidate all fields for this line across all sections
                var fieldGroups = allCapturesForLine.GroupBy(fc => fc.FieldName);
                foreach (var fieldGroup in fieldGroups)
                {
                    var bestCapture = SelectBestFieldCapture(fieldGroup.ToList(), methodName, currentInstance);
                    if (!string.IsNullOrWhiteSpace(bestCapture.RawValue))
                    {
                        lineItem.ConsolidatedFields[bestCapture.FieldName] = bestCapture.FieldValue;
                    }
                }

                // **FIX**: Add sequential line number to the consolidated fields
                lineItem.ConsolidatedFields["LineNumber"] = sequentialLineNumber;
                lineItem.ConsolidatedFields["FileLineNumber"] = lineNumber; // Keep original for reference

                if (lineItem.ConsolidatedFields.Any())
                {
                    logicalItems.Add(lineItem);

                    // Show which sections contributed to this line
                    var sectionContributions = allCapturesForLine
                        .GroupBy(fc => fc.Section)
                        .OrderByDescending(sg => GetSectionPriority(sg.Key))
                        .Select(sg => $"{sg.Key}(p:{GetSectionPriority(sg.Key)},f:{sg.Count()})")
                        .ToList();

                    // Get product description for logging
                    var productDesc = lineItem.ConsolidatedFields
                        .Where(kvp => kvp.Key.ToLower().Contains("description") || kvp.Key.ToLower().Contains("item"))
                        .Select(kvp => kvp.Value?.ToString())
                        .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))
                        ?? "Unknown Product";

                    _logger.Information("{MethodName}: Created logical item #{SequentialNumber} (FileLine:{FileLineNumber}) from Line {LineNumber} ({BestSection}) - '{ProductDesc}' with {FieldCount} fields from sections: [{SectionContributions}]",
                        methodName, sequentialLineNumber, lineNumber, lineNumber, bestSection,
                        productDesc.Length > 50 ? productDesc.Substring(0, 50) + "..." : productDesc,
                        lineItem.ConsolidatedFields.Count, string.Join(", ", sectionContributions));

                    // **DIAGNOSTIC**: Show section coverage for this line
                    var missingSections = new[] { "Single", "Ripped", "Sparse" }
                        .Except(allCapturesForLine.Select(fc => fc.Section).Distinct())
                        .ToList();

                    if (missingSections.Any())
                    {
                        _logger.Verbose("{MethodName}: Line {LineNumber} missing data from sections: [{MissingSections}] (likely regex failures)",
                            methodName, lineNumber, string.Join(", ", missingSections));
                    }

                    sequentialLineNumber++; // **FIX**: Increment sequential counter
                }
            }

            _logger.Information("{MethodName}: ProcessAsLineItems - Created {ItemCount} logical items from {LineCount} unique lines for Instance: {Instance}",
                methodName, logicalItems.Count, allUniqueLineNumbers.Count, currentInstance);

            // **VALIDATION**: Compare expected vs actual item count
            if (logicalItems.Count != allUniqueLineNumbers.Count)
            {
                _logger.Warning("{MethodName}: Mismatch! Expected {ExpectedCount} items (from unique lines) but created {ActualCount} items",
                    methodName, allUniqueLineNumbers.Count, logicalItems.Count);
            }

            return logicalItems;
        }

        private List<LogicalInvoiceItem> ProcessAsSingleItem(List<FieldCapture> allFieldData, string methodName, string currentInstance)
        {
            _logger.Information("{MethodName}: ProcessAsSingleItem - Creating single consolidated item for Instance: {Instance}",
                methodName, currentInstance);

            var logicalItems = new List<LogicalInvoiceItem>();

            var consolidatedItem = new LogicalInvoiceItem
            {
                PrimaryLineNumber = allFieldData.Min(fc => fc.LineNumber),
                ConsolidatedFields = new Dictionary<string, object>(),
                BestSection = DetermineBestSection(allFieldData),
                AllCaptures = allFieldData
            };

            // Consolidate ALL fields across all sections - no filtering!
            var fieldGroups = allFieldData.GroupBy(fc => fc.FieldName);
            foreach (var fieldGroup in fieldGroups)
            {
                var bestCapture = SelectBestFieldCapture(fieldGroup.ToList(), methodName, currentInstance);
                if (!string.IsNullOrWhiteSpace(bestCapture.RawValue))
                {
                    consolidatedItem.ConsolidatedFields[bestCapture.FieldName] = bestCapture.FieldValue;
                    _logger.Verbose("{MethodName}: Added field '{FieldName}' = '{Value}' from section '{Section}'",
                        methodName, bestCapture.FieldName, bestCapture.RawValue, bestCapture.Section);
                }
            }

            if (consolidatedItem.ConsolidatedFields.Any())
            {
                logicalItems.Add(consolidatedItem);

                var sectionContributions = allFieldData
                    .GroupBy(fc => fc.Section)
                    .OrderByDescending(sg => GetSectionPriority(sg.Key))
                    .Select(sg => $"{sg.Key}(priority:{GetSectionPriority(sg.Key)},fields:{sg.Count()})")
                    .ToList();

                _logger.Information("{MethodName}: Instance: {Instance} - Created consolidated item with {FieldCount} fields from sections: [{SectionContributions}] - Primary: {BestSection}",
                    methodName, currentInstance, consolidatedItem.ConsolidatedFields.Count, string.Join(", ", sectionContributions), consolidatedItem.BestSection);

                // Log a sample of the fields we're including
                var sampleFields = consolidatedItem.ConsolidatedFields.Take(10)
                    .Select(kvp => $"{kvp.Key}='{kvp.Value}'")
                    .ToList();
                _logger.Information("{MethodName}: Sample fields included: [{SampleFields}]",
                    methodName, string.Join(", ", sampleFields));
            }

            _logger.Information("{MethodName}: ProcessAsSingleItem - Returning {ItemCount} consolidated items for Instance: {Instance}",
                methodName, logicalItems.Count, currentInstance);

            return logicalItems;
        }

        private FieldCapture SelectBestFieldCapture(List<FieldCapture> fieldCaptures, string methodName, string currentInstance)
        {
            // Strategy: Select the most complete/accurate field value
            // 1. Prefer non-empty values
            // 2. Prefer higher quality sections (Single > Ripped > Sparse)
            // 3. Prefer longer values (more complete capture)

            var sectionPriority = new Dictionary<string, int>
    {
        { "Single", 3 },
        { "Ripped", 2 },
        { "Sparse", 1 }
    };

            var bestCapture = fieldCaptures
                .Where(fc => !string.IsNullOrWhiteSpace(fc.RawValue)) // Prefer non-empty
                .OrderByDescending(fc => sectionPriority.ContainsKey(fc.Section) ? sectionPriority[fc.Section] : 0) // Section priority (DESCENDING for higher priority)
                .ThenByDescending(fc => fc.RawValue?.Length ?? 0) // Prefer longer values
                .FirstOrDefault() ?? fieldCaptures.FirstOrDefault(); // Fallback to any value

            if (fieldCaptures.Count > 1)
            {
                var allOptions = fieldCaptures
                    .Select(fc => $"{fc.Section}(p:{(sectionPriority.ContainsKey(fc.Section) ? sectionPriority[fc.Section] : 0)},len:{fc.RawValue?.Length ?? 0})")
                    .ToList();

                _logger.Verbose("{MethodName}: Instance: {Instance} - Selected field '{FieldName}' from {Section} out of options: [{AllOptions}]",
                    methodName, currentInstance, bestCapture?.FieldName, bestCapture?.Section, string.Join(", ", allOptions));
            }

            return bestCapture;
        }


        private bool IsInvoiceHeaderField(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            var headerFieldPatterns = new[]
            {
                "invoice", "order", "total", "subtotal", "tax", "shipping", "freight",
                "date", "number", "amount", "balance", "due", "paid", "credit",
                "bill", "account", "customer", "vendor", "supplier", "company",
                "address", "phone", "email", "reference", "po", "terms"
            };

            var fieldLower = fieldName.ToLower();
            return headerFieldPatterns.Any(pattern => fieldLower.Contains(pattern));
        }

        private string GetProductIdentifier(List<FieldCapture> captures)
        {
            // Look for product name/description field - this will be our deduplication key
            var productCapture = captures
                .Where(fc => !string.IsNullOrWhiteSpace(fc.RawValue))
                .Where(fc => fc.FieldName?.ToLower().Contains("product") == true ||
                           fc.FieldName?.ToLower().Contains("description") == true ||
                           fc.FieldName?.ToLower().Contains("item") == true ||
                           fc.FieldName?.ToLower().Contains("name") == true)
                .OrderByDescending(fc => fc.RawValue?.Length ?? 0) // Prefer longer descriptions
                .FirstOrDefault();

            if (productCapture != null)
                return productCapture.RawValue?.Trim();

            // Fallback: use the longest non-numeric field value as identifier
            var fallbackCapture = captures
                .Where(fc => !string.IsNullOrWhiteSpace(fc.RawValue))
                .Where(fc => !IsNumericValue(fc.RawValue)) // Skip prices, quantities, etc.
                .OrderByDescending(fc => fc.RawValue?.Length ?? 0)
                .FirstOrDefault();

            return fallbackCapture?.RawValue?.Trim() ?? $"Unknown_Line_{captures.FirstOrDefault()?.LineNumber}";
        }

        private bool IsNumericValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Remove common price/currency symbols and whitespace
            var cleanValue = value.Trim().Replace("$", "").Replace(",", "").Replace(" ", "");

            return decimal.TryParse(cleanValue, out _) || int.TryParse(cleanValue, out _);
        }

        private int GetSectionPriority(string section)
        {
            // Higher number = higher priority
            return section switch
            {
                "Single" => 3,
                "Ripped" => 2,
                "Sparse" => 1,
                _ => 0
            };
        }

        private string DetermineBestSection(List<FieldCapture> captures)
        {
            // Prioritize sections: Single > Ripped > Sparse
            var sectionPriority = new Dictionary<string, int>
            {
                { "Single", 3 },
                { "Ripped", 2 },
                { "Sparse", 1 }
            };

            return captures
                .GroupBy(c => c.Section)
                .OrderByDescending(g => sectionPriority.ContainsKey(g.Key) ? sectionPriority[g.Key] : 0)
                .ThenByDescending(g => g.Count()) // More fields = better
                .First().Key;
        }

        private void SetInstanceMetadata(IDictionary<string, object> parentDict, string currentInstance, string sectionName, int lineNumber)
        {
            parentDict["Instance"] = currentInstance;
            parentDict["Section"] = sectionName;
            parentDict["FileLineNumber"] = lineNumber;
        }

        private void ProcessChildParts(Part currentPart, string currentInstance, IDictionary<string, object> parentDict, ref bool parentDataFound, int? partId, string methodName)
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
                        AttachChildItems(parentDict, allChildItemsForPart, childPart, childPartId, methodName, currentInstance);
                    }
                }
            }
        }

        private void AttachChildItems(IDictionary<string, object> parentDict, List<IDictionary<string, object>> childItems, Part childPart, int childPartId, string methodName, string currentInstance)
        {
            var entityTypeField = childPart.AllLines.FirstOrDefault()?
                .OCR_Lines?.Fields?.FirstOrDefault()?
                .EntityType;
            var fieldname = !string.IsNullOrEmpty(entityTypeField)
                ? entityTypeField
                : $"ChildPart_{childPartId}";

            if (parentDict.ContainsKey(fieldname))
            {
                if (parentDict[fieldname] is List<IDictionary<string, object>> existingList)
                {
                    // **HYBRID APPROACH**: Remove same-line duplicates but keep different-line items
                    var deduplicatedItems = RemoveSameLineDuplicates(existingList, childItems, methodName, currentInstance);
                    existingList.AddRange(deduplicatedItems);

                    _logger.Information("{MethodName}: Added {NewCount} child items (filtered out {DuplicateCount} same-line duplicates) - Total items now: {TotalCount}",
                        methodName, deduplicatedItems.Count, childItems.Count - deduplicatedItems.Count, existingList.Count);
                }
                else
                {
                    parentDict[fieldname] = childItems;
                }
            }
            else
            {
                parentDict[fieldname] = childItems;
            }

            var finalCount = parentDict[fieldname] is List<IDictionary<string, object>> finalList ? finalList.Count : 1;
            _logger.Debug("{MethodName}: Attached child items to field '{FieldName}' for Instance: {Instance} - Total items now: {TotalCount}",
                methodName, fieldname, currentInstance, finalCount);
        }

        private List<IDictionary<string, object>> RemoveSameLineDuplicates(List<IDictionary<string, object>> existingItems, List<IDictionary<string, object>> newItems, string methodName, string currentInstance)
        {
            var uniqueNewItems = new List<IDictionary<string, object>>();

            foreach (var newItem in newItems)
            {
                var isDuplicateOfExisting = existingItems.Any(existingItem => AreSameLineDuplicates(existingItem, newItem, methodName));

                if (!isDuplicateOfExisting)
                {
                    uniqueNewItems.Add(newItem);

                    var productDesc = GetProductDescriptionFromItem(newItem);
                    var lineNumber = newItem.ContainsKey("FileLineNumber") ? newItem["FileLineNumber"] : "Unknown";
                    _logger.Verbose("{MethodName}: Keeping unique item from line {LineNumber}: '{ProductDesc}'",
                        methodName, lineNumber, productDesc);
                }
                else
                {
                    var productDesc = GetProductDescriptionFromItem(newItem);
                    var lineNumber = newItem.ContainsKey("FileLineNumber") ? newItem["FileLineNumber"] : "Unknown";
                    _logger.Verbose("{MethodName}: Filtering out same-line duplicate from line {LineNumber}: '{ProductDesc}'",
                        methodName, lineNumber, productDesc);
                }
            }

            return uniqueNewItems;
        }

        private bool AreSameLineDuplicates(IDictionary<string, object> item1, IDictionary<string, object> item2, string methodName)
        {
            // **SAME-LINE DUPLICATE DETECTION**: Only consider items duplicates if they come from the same file line
            // and have substantially similar content (indicating double regex matching)

            // First check: Are they from the same file line?
            var line1 = item1.ContainsKey("FileLineNumber") ? item1["FileLineNumber"]?.ToString() : "";
            var line2 = item2.ContainsKey("FileLineNumber") ? item2["FileLineNumber"]?.ToString() : "";

            if (string.IsNullOrEmpty(line1) || string.IsNullOrEmpty(line2) || line1 != line2)
            {
                // Different lines or missing line info = not duplicates (keep both)
                return false;
            }

            // Same line - now check if the content is substantially similar (indicating regex double-match)
            var similarityScore = CalculateItemSimilarity(item1, item2);
            var threshold = 0.8; // 80% similarity threshold for same-line duplicates

            var isDuplicate = similarityScore >= threshold;

            if (isDuplicate)
            {
                var productDesc = GetProductDescriptionFromItem(item1);
                _logger.Verbose("AreSameLineDuplicates: Found same-line duplicate (similarity: {Similarity:P0}) - Line: {LineNumber}, Product: '{ProductDesc}'",
                    similarityScore, line1, productDesc);
            }
            else
            {
                var productDesc = GetProductDescriptionFromItem(item1);
                _logger.Verbose("AreSameLineDuplicates: Same line but different content (similarity: {Similarity:P0}) - Line: {LineNumber}, Product: '{ProductDesc}' - keeping both",
                    similarityScore, line1, productDesc);
            }

            return isDuplicate;
        }

        private double CalculateItemSimilarity(IDictionary<string, object> item1, IDictionary<string, object> item2)
        {
            // Calculate similarity based on key fields to detect regex double-matches
            var keyFields = new[] { "ItemDescription", "ItemNumber", "Cost", "Quantity", "TotalCost" };

            var totalFields = 0;
            var matchingFields = 0;

            foreach (var field in keyFields)
            {
                if (item1.ContainsKey(field) && item2.ContainsKey(field))
                {
                    var value1 = item1[field]?.ToString()?.Trim() ?? "";
                    var value2 = item2[field]?.ToString()?.Trim() ?? "";

                    if (!string.IsNullOrEmpty(value1) && !string.IsNullOrEmpty(value2))
                    {
                        totalFields++;

                        if (string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingFields++;
                        }
                    }
                }
            }

            return totalFields > 0 ? (double)matchingFields / totalFields : 0.0;
        }

        private List<IDictionary<string, object>> DeduplicateChildItems(List<IDictionary<string, object>> existingItems, List<IDictionary<string, object>> newItems, string methodName, string currentInstance)
        {
            var uniqueNewItems = new List<IDictionary<string, object>>();

            foreach (var newItem in newItems)
            {
                var isDuplicate = existingItems.Any(existingItem => AreItemsDuplicate(existingItem, newItem, methodName));

                if (!isDuplicate)
                {
                    uniqueNewItems.Add(newItem);

                    var productDesc = GetProductDescriptionFromItem(newItem);
                    _logger.Verbose("{MethodName}: Keeping unique item: '{ProductDesc}'", methodName, productDesc);
                }
                else
                {
                    var productDesc = GetProductDescriptionFromItem(newItem);
                    _logger.Verbose("{MethodName}: Filtering out duplicate item: '{ProductDesc}'", methodName, productDesc);
                }
            }

            return uniqueNewItems;
        }

        private bool AreItemsDuplicate(IDictionary<string, object> item1, IDictionary<string, object> item2, string methodName)
        {
            // Compare key identifying fields to determine if items are duplicates
            var identifyingFields = new[] { "ItemDescription", "ItemNumber", "Cost", "ProductName", "Description" };

            foreach (var field in identifyingFields)
            {
                if (item1.ContainsKey(field) && item2.ContainsKey(field))
                {
                    var value1 = item1[field]?.ToString()?.Trim() ?? "";
                    var value2 = item2[field]?.ToString()?.Trim() ?? "";

                    if (!string.IsNullOrEmpty(value1) && !string.IsNullOrEmpty(value2))
                    {
                        // If the values are identical, it's likely a duplicate
                        if (string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.Verbose("AreItemsDuplicate: Found duplicate based on field '{Field}': '{Value}'", field, value1);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private string GetProductDescriptionFromItem(IDictionary<string, object> item)
        {
            var descriptionFields = new[] { "ItemDescription", "ProductName", "Description", "ItemNumber" };

            foreach (var field in descriptionFields)
            {
                if (item.ContainsKey(field) && !string.IsNullOrWhiteSpace(item[field]?.ToString()))
                {
                    var desc = item[field].ToString();
                    return desc.Length > 50 ? desc.Substring(0, 50) + "..." : desc;
                }
            }

            return "Unknown Product";
        }
    }

    // Helper classes for invoice item consolidation
    public class FieldCapture
    {
        public string Section { get; set; }
        public int LineNumber { get; set; }
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public Fields Field { get; set; }
        public string RawValue { get; set; }
    }

    public class LogicalInvoiceItem
    {
        public int PrimaryLineNumber { get; set; }
        public string BestSection { get; set; }
        public Dictionary<string, object> ConsolidatedFields { get; set; }
        public List<FieldCapture> AllCaptures { get; set; }
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
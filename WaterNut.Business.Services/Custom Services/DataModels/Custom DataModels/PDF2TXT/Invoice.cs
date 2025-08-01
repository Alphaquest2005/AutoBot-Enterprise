﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Extensions;
using EntryDataDS.Business.Entities;
using MoreLinq;
using OCR.Business.Entities;
using Invoices = OCR.Business.Entities.Invoices;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace WaterNut.DataSpace
{
    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        public Invoices OcrInvoices { get; }
        public List<Part> Parts { get; set; }
        public bool Success => Parts.All(x => x.Success);
        public List<Line> Lines => MoreEnumerable.DistinctBy(Parts.SelectMany(x => x.AllLines), x => x.OCR_Lines.Id).ToList();

        public Invoice(Invoices ocrInvoices)
        {
            OcrInvoices = ocrInvoices;
            Parts = ocrInvoices.Parts
                .Where(x => (x.ParentParts.Any() && !x.ChildParts.Any()) ||
                            (!x.ParentParts.Any() && !x.ChildParts.Any()))
                //.Where(x => x.Id == 7)
                .Select(z => new Part(z)).ToList();
        }

        public List<dynamic> Read(string text)
        {
            return Read(text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
        }

        private static readonly Dictionary<string, string> Sections = new Dictionary<string, string>()
        {
            { "Single", "---Single Column---" },
            { "Sparse", "---SparseText---" },
            { "Ripped", "---Ripped Text---" }
        };

        public List<dynamic> Read(List<string> text)
        {
            Console.WriteLine($"[OCR DEBUG] Invoice.Read: Starting read for Invoice ID {OcrInvoices.Id}, Name '{OcrInvoices.Name}'. Received {text.Count} lines.");
            try
            {


                var lineCount = 0;
                var section = "";
                foreach (var line in text)
                {
                    
                    Sections.ForEach(s =>
                    {
                        if (line.Contains(s.Value)) section = s.Key;
                    });

                    lineCount += 1;
                    var iLine = new List<InvoiceLine>(){ new InvoiceLine(line, lineCount) };
                    Parts.ForEach(x => x.Read(iLine, section, text.Count)); // Part.Read will log its own entry
                }

                AddMissingRequiredFieldValues();

                if (!this.Lines.SelectMany(x => x.Values.Values).Any()) return new List<dynamic>();//!Success



                var ores = Parts.Select(x =>
                    {
                        // Pass null for top-level calls, indicating no instance filtering yet
                        var lst = SetPartLineValues(x, null);
                        return lst;
                    }
                ).ToList();

                
                // ores contains results from SetPartLineValues for each top-level part.
                // Each item in ores is a List<IDictionary<string, object>> representing instances found for that part.
                var finalResultList = ores.SelectMany(x => x.ToList()).ToList(); // This should now be a list of correctly formed invoice instances.

                if (!finalResultList.Any())
                {
                    Console.WriteLine($"[OCR DEBUG] Invoice.Read: No instances found for Invoice ID {OcrInvoices.Id}. Returning empty list structure.");
                    // Return the expected structure but with an empty inner list
                    return new List<dynamic> { new List<IDictionary<string, object>>() };
                }

                Console.WriteLine($"[OCR DEBUG] Invoice.Read: Finished read for Invoice ID {OcrInvoices.Id}. Returning {finalResultList.Count} assembled instances.");

                // Return the list of instances, wrapped in List<dynamic> as expected by the caller.
                // The caller expects `csvLines.First()` to be the `List<IDictionary<string, object>>`
                return new List<dynamic> { finalResultList };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


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
                    var lineInstances = line.Values.SelectMany(z => z.Value.Keys.Select(k => k.instance)).Distinct().ToList();
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
                                line.Values.Add(key, new Dictionary<(Fields fields, int instance), string> { { innerValueKey, valueToAdd } });
                            }
                        }
                    }
                    
                    
                }
            }
        }

        private static bool LineHasField(Line x, Fields field)
        {
            return x.OCR_Lines.Fields.Any(z => z.Field == field.Field);
        }

        private static bool ValueContainsRequiredField(Line x, Fields field)
        {
            return x.Values.All(v => v.Value.Keys.Any(k => k.fields == field));
        }

        private static bool ValueForAllInstances(Line x, List<(int Instance, int LineNumber)> instances)
        {
            var lst = instances.Select(x => x.Instance).ToList();
            return x.Values.SelectMany(z => z.Value.Keys.Select(k => k.instance)).Distinct().ToList().Union(lst).Count() == lst.Count();
        }

        public double MaxLinesCheckedToStart { get; set; } = 0.5;
        private static readonly Dictionary<string, List<BetterExpando>> table = new Dictionary<string, List<BetterExpando>>();

        // Add filterInstance parameter
        private List<IDictionary<string, object>> SetPartLineValues(Part part, int? filterInstance = null)
        {
            Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Processing Part ID {part.OCR_Part.Id}" + (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : " (Top Level)"));
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
                     if (part.ChildParts.Any(cp => cp.AllLines.Any(l => l.Values.SelectMany(v => v.Value.Keys).Any(k => k.instance == filterInstance.Value))))
                     {
                         Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: No direct data for Filter Instance {filterInstance.Value}, but children might. Processing instance {filterInstance.Value} for child aggregation.");
                         instancesToProcess = new List<int> { filterInstance.Value };
                     }
                 }

                 // If still no instances after checking children (or not filtering), return empty.
                 if (!instancesToProcess.Any())
                 {
                     Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: No relevant instances found" + (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : "") + ". Returning empty list.");
                     return finalPartItems;
                 }

                Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Processing instances [{string.Join(", ", instancesToProcess)}]" + (filterInstance.HasValue ? $" for Filter Instance {filterInstance.Value}" : ""));

                // Child parts already processed above


                // 3. Iterate through each distinct instance found for the PARENT part
                // 3. Iterate through the relevant instances for this part
                foreach (var currentInstance in instancesToProcess)
                {
                    Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Processing instance {currentInstance}.");
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
                                } else {
                                     // Fallback if somehow the source line isn't found (shouldn't happen)
                                     parentDitm["FileLineNumber"] = 0;
                                     parentDitm["Instance"] = currentInstance;
                                     parentDitm["Section"] = sectionName;
                                     Console.WriteLine($"[OCR WARNING] SetPartLineValues: Could not find source line for first value in instance {currentInstance}, section {sectionName}."); // Use Console.WriteLine
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
                                    fieldSourceSections.Add(fieldName, sectionName); // Record that this field was populated and from which section
                                    Console.WriteLine($"[OCR DEBUG] SetPartLineValues: Instance {currentInstance}, Field '{fieldName}' set from section '{sectionName}'. Value: '{parentDitm[fieldName]}'");
                                }
                                else
                                {
                                     // Field already set from a higher precedence section, ignore this one.
                                     Console.WriteLine($"[OCR DEBUG] SetPartLineValues: Instance {currentInstance}, Field '{fieldName}' already set from section '{fieldSourceSections[fieldName]}'. Ignoring value '{fieldKvp.Value}' from section '{sectionName}'.");
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

                        Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}, Instance {currentInstance}: Child Part ID {childPartId} - Raw items: {rawChildItems.Count}, Deduplicated items: {deduplicatedChildItems.Count} (using positional index).");

                        var fieldname = childPart.AllLines.FirstOrDefault()?.OCR_Lines?.Fields?.FirstOrDefault()?.EntityType ?? $"ChildPart_{childPartId}";

                        // Attach the deduplicated list (always treat as a list, even if non-recurring child results in 0 or 1 item after deduplication)
                        Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}, Instance {currentInstance}: Attaching {deduplicatedChildItems.Count} deduplicated child items for Child Part ID {childPartId} under field '{fieldname}'.");
                        if (parentDitm.ContainsKey(fieldname))
                        {
                            if (parentDitm[fieldname] is List<IDictionary<string, object>> existingList) {
                                existingList.AddRange(deduplicatedChildItems);
                            } else {
                                Console.WriteLine($"[OCR WARNING] Invoice.SetPartLineValues: Field '{fieldname}' exists but is not a list for child {childPartId}. Overwriting.");
                                parentDitm[fieldname] = deduplicatedChildItems;
                            }
                        }
                        else
                        {
                            parentDitm[fieldname] = deduplicatedChildItems;
                        }
                        if(deduplicatedChildItems.Any()) parentDataFound = true;

                    } // End foreach childPart

                    // 6. Add the assembled parent item (with its associated child data) to the final list
                    // Only add if we actually found data for this parent instance OR its children attached data
                    if (parentDataFound) // Add if any data (parent or child) was found for this instance
                    {
                         Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Adding assembled item for instance {currentInstance} to results.");
                         finalPartItems.Add(parentItem);
                    } else {
                         Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Part ID {part.OCR_Part.Id}: Skipping empty or incomplete item for instance {currentInstance}.");
                    }
                }


                Console.WriteLine($"[OCR DEBUG] Invoice.SetPartLineValues: Finished processing Part ID {part.OCR_Part.Id}. Returning {finalPartItems.Count} items.");
                return finalPartItems;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[OCR ERROR] Invoice.SetPartLineValues: Exception processing Part ID {part.OCR_Part.Id}: {e}");
                throw;
            }
        }

        private static BetterExpando CreateOrGetDitm(Part part, Line line, int i, BetterExpando itm,
            ref IDictionary<string, object> ditm, List<IDictionary<string, object>> lst)
        {
            if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
            {
                if (line.OCR_Lines?.IsColumn == true)
                {
                    if (i > table[line.OCR_Lines.Fields.First().EntityType]
                            .Count - 1)
                    {
                        itm = new BetterExpando();
                        if (line.OCR_Lines.IsColumn == true)
                        {
                            table[line.OCR_Lines.Fields.First().EntityType].Add(itm);
                        }
                    }
                    else
                    {
                        itm = table[line.OCR_Lines.Fields.First().EntityType][i];
                    }
                }
                else
                {
                    itm = (BetterExpando)lst.ElementAtOrDefault(i) ?? new BetterExpando();
                }
                

                ditm = ((IDictionary<string, object>)itm);
            }

            return itm;
        }

        private Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> DistinctValues(Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> lineValues)
        {
            var res = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
            foreach (var val in lineValues.Where(val => !res.Values.Any(z => z.Values.Any(q => val.Value.ContainsValue(q)))))
            {
                res.Add((val.Key.lineNumber, val.Key.section), val.Value);
            }
            return res;
        }

        private void ImportByDataType(KeyValuePair<(Fields fields, int instance), string> field,
            IDictionary<string, object> ditm,
            KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> value)
        {
            try
            {


                switch (field.Key.fields.DataType)
                {
                    case "String":      
                        ditm[field.Key.fields.Field] =
                            (ditm[field.Key.fields.Field] + " " + GetValueByKey(value, field.Key.fields.Key)).Trim();
                        break;
                    case "Number":
                    case "Numeric":

                        if (field.Key.fields.AppendValues == true)
                        {
                            var val = GetValueByKey(value, field.Key.fields.Key);
                            if (ditm[field.Key.fields.Field].ToString() != val.ToString())
                                ditm[field.Key.fields.Field] =
                                    Convert.ToDouble(ditm[field.Key.fields.Field] ?? "0") +
                                    Convert.ToDouble(GetValueByKey(value, field.Key.fields.Key));
                        }
                        else
                        {
                            ditm[field.Key.fields.Field] =
                                Convert.ToDouble(ditm[field.Key.fields.Field] ?? "0") +
                                Convert.ToDouble(GetValueByKey(value, field.Key.fields.Key));
                        }

                        break;
                    default:
                        ditm[field.Key.fields.Field] = GetValueByKey(value, field.Key.fields.Key);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private dynamic GetValue(KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, (Fields fields, int instance) field)
        {
            try
            {
                var f = z.Value.FirstOrDefault(q => q.Key == field);
                return f.Key.fields == null ? null : GetValue(f);
            }
            catch (Exception e)
            {
                var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
                Console.WriteLine(ex);
                throw ex;
            }

        }
        private dynamic GetValueByKey(KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, string key)
        {
            try
            {
                var f = z.Value.FirstOrDefault(q => q.Key.fields.Key == key);
                return f.Key.fields == null ? null : GetValue(f);
            }
            catch (Exception e)
            {
                var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
                Console.WriteLine(ex);
                throw ex;
            }

        }

        private dynamic GetValue(string field)
        {
            var f = Lines.Where(x => x.OCR_Lines.Parts.RecuringPart == null).SelectMany(x => x.Values.Values)
                .SelectMany(x => x).FirstOrDefault(x => x.Key.fields.Field == field);
            return f.Key.fields == null ? null : GetValue(f);
        }

        // Helper to get a grouping key (REMOVED - using positional index instead)
        // private static object GetItemGroupKey(IDictionary<string, object> item) { ... }

        // Helper to assign precedence to sections
        private static int GetSectionPrecedence(string sectionName)
        {
            switch (sectionName)
            {
                case "Single": return 1;
                case "Ripped": return 2;
                case "Sparse": return 3;
                default: return 99;
            }
        }

        private static dynamic GetValue(KeyValuePair<(Fields fields, int instance), string> f)
        {
            try
            {


                if (f.Key.fields == null) return null;
                switch (f.Key.fields.DataType)
                {
                    case "String":
                        return f.Value;
                    case "Numeric":
                    case "Number":
                        var val = f.Value.Replace("$", "");
                        if (val == "") val = "0";
                        if (double.TryParse(val, out double num))
                            return num;
                        else
                            throw new ApplicationException(
                                $"{f.Key.fields.Field} can not convert to {f.Key.fields.DataType} for Value:{f.Value}");

                    case "Date":
                        if (DateTime.TryParse(f.Value, out DateTime date))
                            return date;
                        else
                            //throw new ApplicationException(
                            //    $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                            return DateTime.MinValue;
                    case "English Date":
                        var formatStrings = new List<string>()
                        {
                            "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy", "M/yyyy", "MMMM d, yyyy", "dd.MM.yyyy", "yyyy-mm-dd"
                        };
                        foreach (String formatString in formatStrings)
                        {
                            if (DateTime.TryParseExact(f.Value, formatString, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime edate))
                                return edate;
                        }

                        throw new ApplicationException(
                            $"{f.Key.fields.Field} can not convert to {f.Key.fields.DataType} for Value:{f.Value}");
                    default:
                        return f.Value;
                }
            }
            catch (Exception e)
            {
                    Console.WriteLine(e);
                throw;
            }
        }

        public string Format(string pdftxt)
        {
            try
            {
                foreach (var reg in OcrInvoices.RegEx.OrderBy(x => x.Id))
                {
                    pdftxt = Regex.Replace(pdftxt, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                }

                return pdftxt;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
Understood. This is a very detailed request and will result in a lengthy set of instructions. I will specify code changes by indicating:
File to Modify.
Method/Class to Remove: Complete removal of the specified code block.
Method/Code Block to Move:
== MOVE START == [SourceFile.cs::MethodOrClassSignature]
[Code Block Content]
== MOVE END ==
And then: == PASTE INTO == [TargetFile.cs] (Optional: specific location hint, e.g., "within class X", "as private method")
Line/Block to Change:
-- [Line content to remove]
++ [Line content to add]
Method Signature Change: Indicated as needed.
Call Site Update: Update call in [File.cs::CallingMethod] from [OldCall] to [NewCall].
Pre-computation/Analysis:
Logger: _logger is assumed to be an instance field available in all parts of the partial class OCRCorrectionService.
using statements: Will not be explicitly managed here but are critical. Assume using WaterNut.DataSpace; is present where needed, or standard System, System.Collections.Generic, System.Linq, System.Threading.Tasks, System.Data.Entity, Serilog etc.
Detailed Code Change Instructions
Phase 1: Data Model Consolidation
Goal: OCRDataModels.cs is the single source of truth for all shared data models.
File: OCRRegexManagement.cs
== REMOVE START == OCRRegexManagement.cs::RegexCreationResponse class
        public class RegexCreationResponse
        {
            public string Strategy { get; set; }  // "modify_existing_line" or "create_new_line"
            public string RegexPattern { get; set; }
            public string CompleteLineRegex { get; set; }  // Full regex for line updates
            public bool IsMultiline { get; set; }
            public int MaxLines { get; set; }
            public string TestMatch { get; set; }
            public double Confidence { get; set; }
            public string Reasoning { get; set; }
            public bool PreservesExistingGroups { get; set; }  // Safety check
        }
== REMOVE END ==
Use code with caution.
File: OCRDatabaseStrategies.cs
== REMOVE START == OCRDatabaseStrategies.cs::RegexUpdateRequest class
        public class RegexUpdateRequest
        {
            public string FieldName { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public int LineNumber { get; set; }
            public string LineText { get; set; }
            public string WindowText { get; set; }
            public string CorrectionType { get; set; }
            public double Confidence { get; set; }
            public string DeepSeekReasoning { get; set; }
            public string FilePath { get; set; }
            public string InvoiceType { get; set; }
            public DatabaseUpdateStrategy Strategy { get; set; } // Enum from OCRDataModels.cs
            
            // NEW: Enhanced properties for omission handling
            public int? LineId { get; set; }
            public int? PartId { get; set; }
            public int? RegexId { get; set; }
            public string ExistingRegex { get; set; }
            public string ContextBefore { get; set; } // Should be List<string>
            public string ContextAfter { get; set; } // Should be List<string>
            // public bool RequiresMultilineRegex { get; set; } // Add if missing in OCRDataModels.cs version
        }
== REMOVE END ==
Use code with caution.
Note: The ContextBefore and ContextAfter in the removed RegexUpdateRequest were strings. The one in OCRDataModels.cs (presumably, based on CorrectionResult) should ideally use List<string>. If OCRDataModels.cs::RegexUpdateRequest needs RequiresMultilineRegex, ensure it's added there. The type of Strategy property should be the enum DatabaseUpdateStrategy from OCRDataModels.cs.
File: OCRMetadataExtractor.cs
== REMOVE START == OCRMetadataExtractor.cs::LineContext class
        public class LineContext
        {
            public int? LineId { get; set; }
            public int LineNumber { get; set; }
            public string LineText { get; set; }
            public string RegexPattern { get; set; }
            public string WindowText { get; set; }
            public string LineName { get; set; }
            public string LineRegex { get; set; }
            public int? RegexId { get; set; }
            public List<FieldInfo> FieldsInLine { get; set; } = new List<FieldInfo>();
            public List<OCRFieldMetadata> ExistingFields { get; set; } = new List<OCRFieldMetadata>();
            public List<string> ContextLinesBefore { get; set; } = new List<string>();
            public List<string> ContextLinesAfter { get; set; } = new List<string>();
            public bool RequiresMultilineRegex { get; set; }
            public bool IsOrphaned { get; set; }
            public bool RequiresNewLineCreation { get; set; }

            // Enhanced PartId support for omission handling
            public int? PartId { get; set; }
            public string PartName { get; set; }
            public int? PartTypeId { get; set; }

            public string FullContextWithLineNumbers => string.Join("\n",
                ContextLinesBefore.Concat(new[] { $">>> Line {LineNumber}: {LineText} <<<" }).Concat(ContextLinesAfter));
        }
== REMOVE END ==
Use code with caution.
== REMOVE START == OCRMetadataExtractor.cs::FieldInfo class
        public class FieldInfo
        {
            public int FieldId { get; set; }
            public string Key { get; set; }        // Maps to regex named group
            public string Field { get; set; }      // Database field name
            public string EntityType { get; set; }
            public string DataType { get; set; }
            public bool? IsRequired { get; set; }
        }
== REMOVE END ==
Use code with caution.
Ensure OCRDataModels.cs::LineContext and OCRDataModels.cs::FieldInfo have all properties from the removed versions. Specifically, LineContext needs PartId, PartName, PartTypeId, FullContextWithLineNumbers. FieldInfo needs IsRequired.
Phase 2: Utility Method Consolidation (Target: OCRUtilities.cs)
CleanJsonResponse Method:
The version in OCRUtilities.cs is primary.
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::CleanJsonResponse method
        private string CleanJsonResponse(string jsonResponse)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRDatabaseUpdates.cs::ParseRegexCreationResponse from CleanJsonResponse(response) to this.CleanJsonResponse(response).
File: OCRDeepSeekIntegration.cs
== REMOVE START == OCRDeepSeekIntegration.cs::CleanDeepSeekResponse method
        private string CleanDeepSeekResponse(string response)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update calls in OCRDeepSeekIntegration.cs::ParseDeepSeekResponse and OCRDeepSeekIntegration.cs::ParseRegexCreationResponse from CleanDeepSeekResponse(response) to this.CleanJsonResponse(response).
General JSON GetValue Helpers:
GetStringValue, GetDoubleValue, GetIntValue, GetBooleanValue (from OCRDeepSeekIntegration.cs, OCRRegexManagement.cs, OCRDatabaseUpdates.cs)
The versions GetStringValueWithLogging, GetDoubleValueWithLogging, GetIntValueWithLogging, GetBooleanValueWithLogging, ParseContextLinesArray in OCRUtilities.cs are more robust and should be the only ones.
Remove all simpler GetStringValue, GetDoubleValue, etc., from OCRDeepSeekIntegration.cs, OCRRegexManagement.cs, OCRDatabaseUpdates.cs.
Update call sites:
In OCRDeepSeekIntegration.cs::CreateCorrectionFromElement:
Change fieldProp.GetString() to this.GetStringValueWithLogging(element, "field", 0 /* or pass actual errorIndex if available */). Apply similarly for all property extractions.
In OCRDeepSeekIntegration.cs::CreateCorrectionFromFieldObject:
Change origProp.GetString() to this.GetStringValueWithLogging(fieldObject, "original", 0). Apply similarly.
In OCRDeepSeekIntegration.cs::ParseRegexCreationResponse:
Change GetStringValue(root, "strategy") to this.GetStringValueWithLogging(root, "strategy", 0). Apply similarly for GetBooleanValue, GetIntValue, GetDoubleValue.
In OCRRegexManagement.cs::ParseRegexCreationResponse (before its removal):
Change GetStringValue(root, "strategy") to this.GetStringValueWithLogging(root, "strategy", 0). Apply similarly.
In OCRDatabaseUpdates.cs::ParseRegexCreationResponse:
Change GetStringValue(root, "strategy") to this.GetStringValueWithLogging(root, "strategy", 0). Apply similarly.
GetOriginalLineText (was ExtractLineText)
The method internal string GetOriginalLineText(string text, int lineNumber) from OCRUtilities.cs is primary.
File: OCRCorrectionApplication.cs
== REMOVE START == OCRCorrectionApplication.cs::GetOriginalLineText method
        private string GetOriginalLineText(int lineNumber, string fileText)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update calls in OCRCorrectionApplication.cs::FindLineContextForOmission and OCRCorrectionApplication.cs::CreateLineContextFromMetadata from GetOriginalLineText(...) to this.GetOriginalLineText(...).
File: OCRMetadataExtractor.cs
In OCRMetadataExtractor.cs::FindLineContext:
--- a/OCRMetadataExtractor.cs
+++ b/OCRMetadataExtractor.cs
@@ -221,16 +221,6 @@
             return lineNumberMatch != null ? CreateLineContextFromMetadata(lineMetadata, originalText) : null;
         }

-        // Helper method to get original line text
-        string GetOriginalLineText(int lineNumber, string originalText)
-        {
-            if (string.IsNullOrEmpty(originalText) || lineNumber <= 0)
-                return "";
-
-            var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
-            return lineNumber <= lines.Length ? lines[lineNumber - 1] : "";
-        }
-
         // Helper method to create orphaned line context
         LineContext CreateOrphanedLineContext(CorrectionResult correction, string originalText)
         {
Use code with caution.
Diff
Update calls in local CreateLineContextFromMetadata and CreateOrphanedLineContext within OCRMetadataExtractor.cs::FindLineContext to this.GetOriginalLineText(...).
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::ExtractLineText method
        private string ExtractLineText(string text, int lineNumber)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
ExtractWindowText
The method internal string ExtractWindowText(string text, int lineNumber, int windowSize) from OCRUtilities.cs is primary.
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::ExtractWindowText method
        private string ExtractWindowText(string text, int lineNumber, int windowSize)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRDatabaseUpdates.cs::CreateEnhancedUpdateRequest to this.ExtractWindowText(...).
File: OCRMetadataExtractor.cs
== REMOVE START == OCRMetadataExtractor.cs::ExtractWindowTextEnhanced method
        private string ExtractWindowTextEnhanced(string text, int lineNumber, int windowSize)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
File: OCREnhancedIntegration.cs
In OCREnhancedIntegration.cs::CreateLineContextFromCorrection:
--- a/OCREnhancedIntegration.cs
+++ b/OCREnhancedIntegration.cs
@@ -111,7 +111,7 @@
     ContextLinesAfter = correction.ContextLinesAfter ?? new List<string>(),
     RequiresMultilineRegex = correction.RequiresMultilineRegex,
     IsOrphaned = fieldMetadata?.LineId == null,
-    RequiresNewLineCreation = fieldMetadata?.LineId == null,
-    WindowText = GetLineWindow(correction.LineNumber, fileText, 5)
+    RequiresNewLineCreation = fieldMetadata?.LineId == null
+    // WindowText = this.ExtractWindowText(fileText, correction.LineNumber, 5) // Add if needed from OCRUtilities.cs
 };
Use code with caution.
Diff
}
```
* Add WindowText = this.ExtractWindowText(fileText, correction.LineNumber, 5) if this property is indeed used and needed on `LineContext`.
DetermineInvoiceType
The method internal string DetermineInvoiceType(string filePath) from OCRUtilities.cs is primary.
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::DetermineInvoiceType method
        private string DetermineInvoiceType(string filePath)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRDatabaseUpdates.cs::CreateEnhancedUpdateRequest to this.DetermineInvoiceType(...).
File: OCRMetadataExtractor.cs
== REMOVE START == OCRMetadataExtractor.cs::DetermineInvoiceTypeEnhanced method
        private string DetermineInvoiceTypeEnhanced(string filePath)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
IsMetadataField
Change OCRUtilities.cs::IsMetadataField to internal static bool IsMetadataField(string fieldName).
File: OCRLegacySupport.cs
== REMOVE START == OCRLegacySupport.cs::IsMetadataField method
        private static bool IsMetadataField(string fieldName)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRLegacySupport.cs::ExtractEnhancedOCRMetadata to OCRUtilities.IsMetadataField(...).
File: OCRMetadataExtractor.cs
== REMOVE START == OCRMetadataExtractor.cs::IsMetadataFieldInternal method
        private bool IsMetadataFieldInternal(string fieldName)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRMetadataExtractor.cs::ExtractEnhancedOCRMetadata to OCRUtilities.IsMetadataField(...).
ExtractNamedGroupsFromRegex
== MOVE START == OCRFieldMapping.cs::ExtractNamedGroupsFromRegex method
        public List<string> ExtractNamedGroupsFromRegex(string regexPattern)
        {
            var namedGroups = new List<string>();

            if (string.IsNullOrEmpty(regexPattern)) return namedGroups;

            try
            {
                // Pattern to match named groups: (?<groupName>...) or (?'groupName'...)
                var namedGroupPattern = @"\(\?<([^>]+)>|\(\?'([^']+)'";
                var matches = Regex.Matches(regexPattern, namedGroupPattern);

                foreach (Match match in matches)
                {
                    // Group 1 is for (?<name>) format, Group 2 is for (?'name') format
                    var groupName = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        namedGroups.Add(groupName);
                    }
                }

                _logger?.Debug("Extracted named groups from regex: {Groups}", string.Join(", ", namedGroups));
                return namedGroups;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting named groups from regex pattern: {Pattern}", regexPattern);
                return namedGroups;
            }
        }
== MOVE END ==
== PASTE INTO == OCRUtilities.cs (as public instance method)
Use code with caution.
File: OCRFieldMapping.cs
Update call in OCRFieldMapping.cs::GetFieldsByRegexNamedGroups from ExtractNamedGroupsFromRegex(regexPattern) to this.ExtractNamedGroupsFromRegex(regexPattern).
File: OCRMetadataExtractor.cs
== REMOVE START == OCRMetadataExtractor.cs::IsFieldExistingInLineEnhanced (the first one that just extracts groups)
        public bool IsFieldExistingInLineEnhanced(string deepSeekFieldName, LineContext lineContext)
        {
            var namedGroups = new List<string>();

            if (string.IsNullOrEmpty(regexPattern)) return namedGroups; // regexPattern is not defined here, this method is flawed

            try
            {
                // ... existing flawed implementation ...
                _logger?.Debug("Extracted named groups from regex: {Groups}", string.Join(", ", namedGroups));
                return namedGroups; // This returns List<string>, not bool. Highly flawed.
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting named groups from regex pattern: {Pattern}", regexPattern);
                return namedGroups; // This returns List<string>, not bool. Highly flawed.
            }
        }
== REMOVE END ==
Use code with caution.
MapTemplateFieldToPropertyName (was MapFieldNameToProperty)
== MOVE START == OCRMetadataExtractor.cs::MapFieldNameToProperty method
        private string MapFieldNameToProperty(string fieldName)
        {
            return fieldName?.ToLowerInvariant() switch
            {
                "invoicetotal" or "total" or "invoice_total" => "InvoiceTotal",
                // ... other cases ...
                _ => null
            };
        }
== MOVE END ==
== PASTE INTO == OCRUtilities.cs (as internal instance method, rename to `MapTemplateFieldToPropertyName`)
Use code with caution.
File: OCRLegacySupport.cs
== REMOVE START == OCRLegacySupport.cs::MapFieldNameToProperty method
        private static string MapFieldNameToProperty(string fieldName)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update calls in OCRLegacySupport.cs::GetEnhancedTemplateFieldMappings and OCRLegacySupport.cs::GetTemplateFieldMappings to this.MapTemplateFieldToPropertyName(...) (Note: these are static methods in OCRLegacySupport, they will need an instance of OCRCorrectionService passed in, or the utility method made static). Correction to plan: Make MapTemplateFieldToPropertyName internal static in OCRUtilities.cs. Update calls to OCRUtilities.MapTemplateFieldToPropertyName(...).
Phase 3: Prompt Creation Consolidation (Target: OCRPromptCreation.cs)
CreateOmissionDetectionPrompt
== MOVE START == OCRErrorDetection.cs::CreateOmissionDetectionPrompt method
        private string CreateOmissionDetectionPrompt(
            ShipmentInvoice invoice, 
            string fileText, 
            Dictionary<string, OCRFieldMetadata> metadata)
        {
            // ... implementation ...
        }
== MOVE END ==
== PASTE INTO == OCRPromptCreation.cs (as private instance method)
Use code with caution.
Update call in OCRErrorDetection.cs::DetectFieldOmissionsAsync to this.CreateOmissionDetectionPrompt(...).
CreateHeaderErrorDetectionPrompt
The version in OCRPromptCreation.cs is primary.
File: OCRErrorDetection.cs
--- a/OCRErrorDetection.cs
+++ b/OCRErrorDetection.cs
@@ -50,10 +50,7 @@
     string fileText, 
     Dictionary<string, OCRFieldMetadata> metadata = null)
 {
-    var prompt = CreateHeaderErrorDetectionPrompt(invoice, fileText);
+    var prompt = this.CreateHeaderErrorDetectionPrompt(invoice, fileText);
     var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

     if (string.IsNullOrWhiteSpace(response))
Use code with caution.
Diff
CreateProductErrorDetectionPrompt
The version in OCRPromptCreation.cs is primary.
File: OCRErrorDetection.cs
--- a/OCRErrorDetection.cs
+++ b/OCRErrorDetection.cs
@@ -76,7 +76,7 @@
         return new List<InvoiceError>();
     }

-    var prompt = CreateProductErrorDetectionPrompt(invoice, fileText);
+    var prompt = this.CreateProductErrorDetectionPrompt(invoice, fileText);
     var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

     if (string.IsNullOrWhiteSpace(response))
Use code with caution.
Diff
Consolidate CreateRegexCreationPrompt
The primary version is public string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext) in OCRPromptCreation.cs.
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::CreateRegexCreationPrompt method
        private string CreateRegexCreationPrompt(RegexUpdateRequest request, string currentLineRegex, List<string> existingNamedGroups)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
In OCRDatabaseUpdates.cs::RequestNewRegexFromDeepSeekAsync:
--- a/OCRDatabaseUpdates.cs
+++ b/OCRDatabaseUpdates.cs
@@ -268,7 +268,22 @@
         }
     }

-    var prompt = CreateRegexCreationPrompt(request, currentLineRegex, existingNamedGroups);
+    // Adapt RegexUpdateRequest to CorrectionResult and LineContext for the new prompt
+    var tempCorrectionResult = new CorrectionResult {
+        FieldName = request.FieldName,
+        NewValue = request.NewValue,
+        OldValue = request.OldValue,
+        LineText = request.LineText,
+        LineNumber = request.LineNumber,
+        ContextLinesBefore = request.ContextLinesBefore.Split('\n').ToList(), // Assuming string, convert to List<string>
+        ContextLinesAfter = request.ContextLinesAfter.Split('\n').ToList(), // Assuming string, convert to List<string>
+        RequiresMultilineRegex = request.RequiresMultilineRegex,
+        CorrectionType = request.CorrectionType
+    };
+    var tempLineContext = new LineContext {
+        RegexPattern = currentLineRegex,
+        FieldsInLine = existingNamedGroups.Select(g => new FieldInfo { Key = g }).ToList() // Simplified, actual FieldInfo might need more data
+    };
+    var prompt = this.CreateRegexCreationPrompt(tempCorrectionResult, tempLineContext);
     var response = await _deepSeekApi.GetResponseAsync(prompt);
     
     return ParseRegexCreationResponse(response);
Use code with caution.
Diff
File: OCRDeepSeekIntegration.cs
== REMOVE START == OCRDeepSeekIntegration.cs::CreateRegexCreationPrompt method
        private string CreateRegexCreationPrompt(CorrectionResult correction, string existingLineRegex, List<string> existingNamedGroups)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
In OCRDeepSeekIntegration.cs::RequestNewRegexFromDeepSeek:
--- a/OCRDeepSeekIntegration.cs
+++ b/OCRDeepSeekIntegration.cs
@@ -310,7 +310,11 @@
Use code with caution.
Diff
{
try
{
- var prompt = CreateRegexCreationPrompt(correction, existingLineRegex, existingNamedGroups);
+ var tempLineContext = new LineContext { // Create LineContext based on available params
+ RegexPattern = existingLineRegex,
+ FieldsInLine = (existingNamedGroups ?? new List<string>()).Select(g => new FieldInfo { Key = g }).ToList()
+ };
+ var prompt = this.CreateRegexCreationPrompt(correction, tempLineContext);
var response = await _deepSeekApi.GetResponseAsync(prompt);
return ParseRegexCreationResponse(response);
}
```
File: OCREnhancedIntegration.cs
== REMOVE START == OCREnhancedIntegration.cs::CreateRegexCreationPrompt method
        private string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext,
            string existingPattern, List<string> existingGroups)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
In OCREnhancedIntegration.cs::RequestNewRegexFromDeepSeekAsync:
--- a/OCREnhancedIntegration.cs
+++ b/OCREnhancedIntegration.cs
@@ -237,7 +237,7 @@
         }
     }

-    var prompt = CreateRegexCreationPrompt(correction, lineContext, existingPattern, existingGroups);
+    var prompt = this.CreateRegexCreationPrompt(correction, lineContext); // lineContext should already have existingPattern and existingGroups (via FieldsInLine)
     var response = await _deepSeekApi.GetResponseAsync(prompt);

     return ParseRegexCreationResponse(response);
Use code with caution.
Diff
File: OCRRegexManagement.cs
Its CreateRegexCreationPrompt (which is similar to the target in OCRPromptCreation.cs) will be removed when the whole method RequestNewRegexFromDeepSeek is removed from OCRRegexManagement.cs.
CreateDirectDataCorrectionPrompt
== MOVE START == OCRLegacySupport.cs::RequestDirectCorrectionsFromDeepSeek's prompt string
// The large string starting with "$@DIRECT DATA CORRECTION - BYPASS REGEX:"
== MOVE END ==
== PASTE AND WRAP INTO METHOD == OCRPromptCreation.cs (as `public string CreateDirectDataCorrectionPrompt(List<dynamic> invoiceData, string originalText)`)
// The method body will serialize invoiceData and use this.CleanTextForAnalysis(originalText)
Use code with caution.
Update call in OCRLegacySupport.cs::RequestDirectCorrectionsFromDeepSeek to correctionService.CreateDirectDataCorrectionPrompt(...).
Phase 4: DeepSeek API Interaction & Parsing (Target: OCRDeepSeekIntegration.cs)
RequestNewRegexFromDeepSeek
The method public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(CorrectionResult correction, string existingLineRegex, List<string> existingNamedGroups) in OCRDeepSeekIntegration.cs is primary. (Its call to prompt creation was updated above).
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::RequestNewRegexFromDeepSeekAsync method
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeekAsync(RegexUpdateRequest request)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
File: OCREnhancedIntegration.cs
== REMOVE START == OCREnhancedIntegration.cs::RequestNewRegexFromDeepSeekAsync method
        private async Task<RegexCreationResponse> RequestNewRegexFromDeepSeekAsync(
            CorrectionResult correction, LineContext lineContext)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
File: OCRRegexManagement.cs
== REMOVE START == OCRRegexManagement.cs::RequestNewRegexFromDeepSeek method
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(
            CorrectionResult correction,
            LineContext lineContext)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
ParseRegexCreationResponse
The method private RegexCreationResponse ParseRegexCreationResponse(string response) in OCRDeepSeekIntegration.cs is primary. (Its calls to utilities were updated above).
File: OCRDatabaseUpdates.cs
== REMOVE START == OCRDatabaseUpdates.cs::ParseRegexCreationResponse method
        private RegexCreationResponse ParseRegexCreationResponse(string response)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
File: OCREnhancedIntegration.cs
This file calls ParseRegexCreationResponse from OCRDeepSeekIntegration.cs. If it had its own, remove it. (It seems to be calling the one from _deepSeekApi implicitly, which is fine after consolidation if _deepSeekApi instance provides it or it calls this.ParseRegexCreationResponse now located in OCRDeepSeekIntegration.cs).
File: OCRRegexManagement.cs
== REMOVE START == OCRRegexManagement.cs::ParseRegexCreationResponse method
        private RegexCreationResponse ParseRegexCreationResponse(string response)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
GetFieldExtractionPatterns from OCRDeepSeekIntegration.cs:
== MOVE START == OCRDeepSeekIntegration.cs::GetFieldExtractionPatterns method
        private List<string> GetFieldExtractionPatterns(string fieldName)
        {
            // ... implementation ...
        }
== MOVE END ==
== PASTE AND MERGE INTO == OCRPatternCreation.cs (as helper(s) for `public List<string> CreateFieldExtractionPatterns(string fieldName, IEnumerable<string> sampleValues)`)
// The logic from GetFieldExtractionPatterns (switch case) should be integrated into CreateMonetaryPatterns, CreateDatePatterns, CreateTextPatterns in OCRPatternCreation.cs.
Use code with caution.
Phase 5: Regex Pattern Logic (Target: OCRPatternCreation.cs)
CreateFormatCorrectionPattern logic from FieldFormatUpdateStrategy:
File: OCRDatabaseStrategies.cs
== MOVE START == OCRDatabaseStrategies.cs::FieldFormatUpdateStrategy.CreateFormatCorrectionPattern method
            private (string Pattern, string Replacement)? CreateFormatCorrectionPattern(string oldValue, string newValue)
            {
                // ... implementation ...
            }
== MOVE END ==
== PASTE INTO == OCRPatternCreation.cs (as a private helper method, e.g., `private (string Pattern, string Replacement)? CreateSpecificFormatCorrectionPatterns(string oldValue, string newValue)`)
Use code with caution.
Modify OCRPatternCreation.cs::CreateAdvancedFormatCorrectionPatterns to call this new helper as one of its strategies.
ValidateRegexPattern:
OCRPatternCreation.cs::ValidateRegexPattern(string pattern) is the generic one.
== MOVE START == OCRRegexManagement.cs::ValidateRegexPattern method
        public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)
        {
            // ... implementation ...
        }
== MOVE END ==
== PASTE INTO == OCRPatternCreation.cs (as `public bool ValidateRegexPattern(RegexCreationResponse regexResponse, CorrectionResult correction)`)
Use code with caution.
Phase 6 & 7: Core Service Logic (OCRCorrectionService.cs) and Database Strategies (OCRDatabaseStrategies.cs)
Goal: OCRCorrectionService.cs for high-level orchestration, OCRDatabaseStrategies.cs for detailed DB interaction per strategy. OCREnhancedIntegration.cs will be dismantled.
Methods to make primary in OCRCorrectionService.cs (and remove from OCREnhancedIntegration.cs):
ProcessCorrectionsWithEnhancedMetadataAsync
ExecuteEnhancedDatabaseUpdateAsync
GetFieldMetadata
UpdateRegexPatternWithMetadataAsync
CreateNewPatternWithMetadataAsync
UpdateFieldFormatWithMetadataAsync
LogCorrectionWithMetadataAsync
For each of these, verify the version in OCRCorrectionService.cs is complete or merge any unique aspects from the OCREnhancedIntegration.cs version before deleting the OCREnhancedIntegration.cs one.
OCREnhancedIntegration.cs::ProcessSingleCorrectionWithMetadataAsync dismantling:
The core logic of deciding strategy (omission vs format) and calling the appropriate processing. This decision logic should be in OCRCorrectionService.cs::ProcessSingleCorrectionWithMetadataAsync.
File: OCRCorrectionService.cs (inside ProcessSingleCorrectionWithMetadataAsync)
--- a/OCRCorrectionService.cs
+++ b/OCRCorrectionService.cs
@@ -196,30 +196,31 @@
         detail.HasMetadata = fieldMetadata != null;
         detail.OCRMetadata = fieldMetadata;

-        // Get enhanced database update context
-        var updateContext = GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
-        detail.UpdateContext = updateContext;
-
-        if (!updateContext.IsValid)
-        {
-            detail.SkipReason = updateContext.ErrorMessage;
-            _logger?.Warning("Skipping correction for field {FieldName}: {Reason}",
-                correction.FieldName, updateContext.ErrorMessage);
-            return detail;
-        }
-
-        // Execute database update based on strategy
-        detail.DatabaseUpdate = await ExecuteEnhancedDatabaseUpdateAsync(
-            context, correction, updateContext, fileText, filePath);
+        // Create line context from correction and metadata (method moved from OCREnhancedIntegration)
+        var lineContext = this.CreateLineContextFromCorrection(correction, fieldMetadata, fileText);
+
+        // Determine correction strategy based on type
+        if (correction.CorrectionType == "omission")
+        {
+            // Call logic that eventually uses OmissionUpdateStrategy
+            // This was ProcessOmissionCorrectionAsync in OCREnhancedIntegration.cs
+            // The OmissionUpdateStrategy itself will handle DeepSeek calls for regex and DB updates.
+            var omissionStrategy = _strategyFactory.GetStrategy(correction); // Assuming _strategyFactory is initialized
+            var request = CreateUpdateRequestForStrategy(correction, lineContext, filePath, fileText); // You'll need this helper
+            detail.DatabaseUpdate = await omissionStrategy.ExecuteAsync(context, request);
+        }
+        else
+        {
+            var updateContext = GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
+            detail.UpdateContext = updateContext;
+            if (!updateContext.IsValid) { /* ... existing skip logic ... */ return detail; }
+            detail.DatabaseUpdate = await ExecuteEnhancedDatabaseUpdateAsync(context, correction, updateContext, fileText, filePath);
+        }

         _logger?.Debug("Processed correction for field {FieldName} with strategy {Strategy}: {Success}",
-            correction.FieldName, updateContext.UpdateStrategy, detail.DatabaseUpdate?.IsSuccess);
+            correction.FieldName, correction.CorrectionType, detail.DatabaseUpdate?.IsSuccess); // Update logging if strategy isn't on updateContext for omissions
Use code with caution.
Diff
Helper CreateUpdateRequestForStrategy needs to be created in OCRCorrectionService.cs or OCRDatabaseUpdates.cs to map CorrectionResult + LineContext to RegexUpdateRequest for the strategy.
Move Omission and Format Correction DB logic from OCREnhancedIntegration.cs to OCRDatabaseStrategies.cs:
OCREnhancedIntegration.cs::ProcessOmissionCorrectionAsync logic:
The part calling RequestNewRegexFromDeepSeekAsync and ValidateRegexPattern and then CreateDatabaseEntriesForOmissionAsync becomes the core of OCRDatabaseStrategies.cs::OmissionUpdateStrategy.ExecuteAsync.
RequestNewRegexFromDeepSeekAsync itself will be called from OmissionUpdateStrategy (it's now in OCRDeepSeekIntegration.cs).
ValidateRegexPattern (the specific one for RegexCreationResponse) will be called from OmissionUpdateStrategy (it's now in OCRPatternCreation.cs).
Move the following methods from OCREnhancedIntegration.cs to become private async helpers within OCRDatabaseStrategies.cs::OmissionUpdateStrategy:
CheckFieldExistsInLineAsync
CreateDatabaseEntriesForOmissionAsync
ModifyExistingLineForOmissionAsync
CreateNewLineForOmissionAsync
DeterminePartIdForFieldAsync
Move OCREnhancedIntegration.cs::CreateFieldFormatCorrectionAsync to become the core DB update part of OCRDatabaseStrategies.cs::FieldFormatUpdateStrategy.ExecuteAsync.
It will call CreateAdvancedFormatCorrectionPatterns (from OCRPatternCreation.cs).
It will use a helper like GetOrCreateFieldFormatRegexAsync (see below).
Move OCREnhancedIntegration.cs::GetOrCreateFieldFormatRegexAsync(OCRContext context, int fieldId, string pattern, string replacement) to OCRDatabaseStrategies.cs as a private async Task<FieldFormatRegEx> GetOrCreateFieldFormatRegexAsync(...) helper, likely used by FieldFormatUpdateStrategy.
Remove the simpler GetOrCreateFieldFormatRegexAsync from OCRCorrectionService.cs.
Move OCREnhancedIntegration.cs::CreateLineContextFromCorrection:
== MOVE START == OCREnhancedIntegration.cs::CreateLineContextFromCorrection method
        private LineContext CreateLineContextFromCorrection(CorrectionResult correction,
            OCRFieldMetadata fieldMetadata, string fileText)
        {
            // ... implementation ...
        }
== MOVE END ==
== PASTE INTO == OCRCorrectionService.cs (as a private instance method)
Use code with caution.
File: OCREnhancedIntegration.cs should now be significantly smaller. If it's empty or only contains very high-level calls already present in OCRCorrectionService.cs, it can be deleted.
Phase 8: Metadata Extraction (OCRMetadataExtractor.cs)
GetLineRegexPattern:
The version in OCRMetadataExtractor.cs is primary.
File: OCRCorrectionApplication.cs
== REMOVE START == OCRCorrectionApplication.cs::GetLineRegexPattern method
        private string GetLineRegexPattern(int? lineId)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRCorrectionApplication.cs::CreateLineContextFromMetadata to this.GetLineRegexPattern(...).
IsFieldExistingInLineEnhanced (in OCRMetadataExtractor.cs):
The second version (that checks FieldsInLine) should be renamed or its logic merged if OCRFieldMapping.cs::IsFieldExistingInLine is not sufficient. For now, assume OCRFieldMapping.cs::IsFieldExistingInLine is the canonical for this check. Remove the second IsFieldExistingInLineEnhanced from OCRMetadataExtractor.cs.
Phase 9: Field Mapping (OCRFieldMapping.cs)
IsFieldExistingInLine:
The version public bool IsFieldExistingInLine(string deepSeekFieldName, LineContext lineContext) in OCRFieldMapping.cs is primary.
File: OCRCorrectionApplication.cs
== REMOVE START == OCRCorrectionApplication.cs::IsFieldExistingInLine method
        private bool IsFieldExistingInLine(string fieldName, LineContext lineContext)
        {
            // ... implementation ...
        }
== REMOVE END ==
Use code with caution.
Update call in OCRCorrectionApplication.cs::ProcessOmissionCorrectionAsync to this.IsFieldExistingInLine(...).
Phase 10: Cleanup OCRRegexManagement.cs
Methods like RequestNewRegexFromDeepSeek, ParseRegexCreationResponse, ValidateRegexPattern were moved.
CreateDatabaseEntriesForOmission: The core logic is now in OmissionUpdateStrategy.
== REMOVE START == OCRRegexManagement.cs::CreateDatabaseEntriesForOmission method
        public async Task<bool> CreateDatabaseEntriesForOmission(
            RegexCreationResponse regexResponse,
            LineContext lineContext,
            CorrectionResult correction)
        {
            // ... implementation calling strategy ...
        }
== REMOVE END ==
Use code with caution.
GetRegexIdFromLineContext:
== MOVE START == OCRRegexManagement.cs::GetRegexIdFromLineContext method
        private int? GetRegexIdFromLineContext(LineContext lineContext)
        {
            // ... implementation ...
        }
== MOVE END ==
== PASTE INTO == OCRDatabaseStrategies.cs (as a private static helper if no instance members needed, or private instance if used by instance methods of strategies)
Use code with caution.
File OCRRegexManagement.cs is now likely empty or nearly empty. It can be deleted from the project.
Phase 11: Legacy Support (OCRLegacySupport.cs)
ExtractEnhancedOCRMetadata (static method in OCRLegacySupport.cs):
This static method calls an instance method. It should create an instance of OCRCorrectionService to call the (now consolidated) instance version of ExtractEnhancedOCRMetadata.
--- a/OCRLegacySupport.cs
+++ b/OCRLegacySupport.cs
@@ -209,11 +209,13 @@
     {
         var metadata = new Dictionary<string, OCRFieldMetadata>();
         try
         {
+            // Assuming ExtractEnhancedOCRMetadata is now an instance method on OCRCorrectionService (likely via OCRMetadataExtractor part)
+            // This static context needs an instance.
+            // For simplicity, if this static method is the only caller, consider making the target instance method also static if feasible.
+            // Otherwise, this static method needs to instantiate OCRCorrectionService or be refactored.
+            // For this diff, we'll assume it can call a static version or has an instance.
+            // This specific call site may need more involved refactoring depending on final location of ExtractEnhancedOCRMetadata
             var invoiceContext = new // ... (GetInvoiceContext logic can be a static helper or duplicated)
Use code with caution.
Diff
The method ExtractEnhancedOCRMetadata (the one that takes IDictionary<string, object> invoiceDict, Invoice template, Dictionary<string, (int LineId, int FieldId)> fieldMappings)) is in OCRMetadataExtractor.cs. Make it public if it isn't. OCRLegacySupport.cs needs to call this. If OCRLegacySupport.cs is creating OCRCorrectionService instance, it can call instance.ExtractEnhancedOCRMetadata(...).
RequestDirectCorrectionsFromDeepSeek (static method in OCRLegacySupport.cs):
The prompt creation part was moved.
--- a/OCRLegacySupport.cs
+++ b/OCRLegacySupport.cs
@@ -159,45 +159,10 @@
         ILogger logger)
     {
         try
         {
-            var invoiceJson = JsonSerializer.Serialize(invoiceDict, new JsonSerializerOptions { WriteIndented = true });
-
-            var prompt = $@"DIRECT DATA CORRECTION - BYPASS REGEX:
-
-The OCR import patterns could not be fixed. Provide direct value corrections to make the invoice math balance correctly.
-
-EXTRACTED DATA:
-{invoiceJson}
-
-ORIGINAL TEXT:
-{correctionService.CleanTextForAnalysis(originalText)}
-
-REQUIREMENTS:
-1. Provide correct values that make the math balance: SubTotal + Freight + Other + Insurance - Deduction = InvoiceTotal
-2. Focus on critical fields that affect TotalsZero calculation
-3. Use exact values from the original text
-
-RESPONSE FORMAT:
-{{
-  ""corrections"": [
-    {{
-      ""field"": ""InvoiceTotal"",
-      ""current_value"": ""wrong_value"",
-      ""correct_value"": ""right_value"",
-      ""reasoning"": ""explanation""
-    }}
-  ]
-}}";
-
-            // This would need to be implemented as part of the correctionService
-            // For now, return empty list as placeholder
-            logger.Debug("Direct correction prompt created, but DeepSeek call not implemented in this method");
+            var prompt = correctionService.CreateDirectDataCorrectionPrompt(new List<dynamic>{invoiceDict}, originalText); // Adapt to new prompt signature
+            // var response = await correctionService._deepSeekApi.GetResponseAsync(prompt); // Actual API call
+            // return ParseDirectCorrectionsResponse(response, logger); // Need a parser for this
             return new List<DirectCorrection>();
         }
         catch (Exception ex)
Use code with caution.
Diff
Final Review Steps:
Compile the project. Address any errors related to missing methods, incorrect signatures, or namespace issues.
Review each TODO or NOTE in these instructions to ensure the intent was captured.
Run existing tests or manually test key workflows.
This is a very complex refactoring. Each step needs to be done carefully. The diffs aim to be precise but the context of partial classes means method visibility (private, internal, public) also plays a role in whether this. is needed or if methods are directly callable. I've assumed instance methods are generally callable within the same partial class instance.
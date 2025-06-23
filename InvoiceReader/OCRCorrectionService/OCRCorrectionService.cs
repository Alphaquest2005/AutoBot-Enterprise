// File: OCRCorrectionService/OCRCorrectionService.cs
using EntryDataDS.Business.Entities;
using InvoiceReader.OCRCorrectionService;
using OCR.Business.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    using System.Text.Json.Serialization;
    using WaterNut.Business.Services.Utils;

    public partial class OCRCorrectionService : IDisposable
    {
        #region Fields and Properties

        private readonly DeepSeekInvoiceApi _deepSeekApi;

        private readonly ILogger _logger;

        private bool _disposed = false;

        private DatabaseUpdateStrategyFactory _strategyFactory;

        public double DefaultTemperature { get; set; } = 0.1;

        public int DefaultMaxTokens { get; set; } = 4096;

        #endregion

        #region Constructor

        public OCRCorrectionService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            try
            {
                _deepSeekApi = new DeepSeekInvoiceApi(_logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CRITICAL FAILURE: The DeepSeekInvoiceApi constructor threw an exception.");
                throw;
            }
            _strategyFactory = new DatabaseUpdateStrategyFactory(_logger);
        }

        #endregion

        #region Public Orchestration Methods

        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
        {
            _logger.Error("🚀 **ORCHESTRATION_START**: Starting CorrectInvoiceAsync for Invoice '{InvoiceNo}'", invoice?.InvoiceNo ?? "NULL");
            if (invoice == null || string.IsNullOrEmpty(fileText))
            {
                _logger.Error("   - ❌ **VALIDATION_FAIL**: Invoice or fileText is null/empty. Aborting.");
                return false;
            }

            try
            {
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

                _logger.Error("   - **STEP 1: METADATA_EXTRACTION**: Extracting OCR metadata from the current invoice state.");
                var metadata = this.ExtractFullOCRMetadata(invoice, fileText);

                _logger.Error("   - **STEP 2: ERROR_DETECTION**: Detecting errors and omissions.");
                var allDetectedErrors = await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);

                if (!allDetectedErrors.Any())
                {
                    _logger.Error("   - ✅ **NO_ERRORS_FOUND**: No errors detected. Checking final balance.");
                    return OCRCorrectionService.TotalsZero(invoice, _logger);
                }
                _logger.Error("   - Found {Count} unique, actionable errors.", allDetectedErrors.Count);

                _logger.Error("   - **STEP 3: APPLY_CORRECTIONS**: Applying corrections to in-memory invoice object.");
                var appliedCorrections = await this.ApplyCorrectionsAsync(invoice, allDetectedErrors, fileText, metadata)
                        .ConfigureAwait(false);
                var successfulValueApplications = appliedCorrections.Count(c => c.Success);
                _logger.Error("   - Successfully applied {Count} corrections.", successfulValueApplications);

                _logger.Error("   - **STEP 4: CUSTOMS_RULES (DISABLED)**: Caribbean-specific rules are now handled by the AI prompt. Skipping C# post-processing.");

                _logger.Error("   - **STEP 5: DB_LEARNING_PREP**: Preparing successful detections for database learning.");

                var successfulDetectionsForDB = allDetectedErrors.Select(
                    e => new CorrectionResult
                    {
                        FieldName = e.Field,
                        OldValue = e.ExtractedValue,
                        NewValue = e.CorrectValue,
                        CorrectionType = e.ErrorType,
                        Confidence = e.Confidence,
                        Reasoning = e.Reasoning,
                        LineText = e.LineText,
                        LineNumber = e.LineNumber,
                        Success = true,
                        ContextLinesBefore = e.ContextLinesBefore,
                        ContextLinesAfter = e.ContextLinesAfter,
                        RequiresMultilineRegex = e.RequiresMultilineRegex,
                        SuggestedRegex = e.SuggestedRegex,
                        Pattern = e.Pattern,
                        Replacement = e.Replacement
                    }).ToList();

                // =================================== LOGGING ENHANCEMENT ===================================
                _logger.Error("   - **DATA_DUMP (successfulDetectionsForDB)**: Object state before creating RegexUpdateRequests: {Data}",
                    JsonSerializer.Serialize(successfulDetectionsForDB, jsonOptions));
                // =================================== END ENHANCEMENT =====================================

                if (successfulDetectionsForDB.Any())
                {
                    _logger.Error("   - **STEP 6: REGEX_UPDATE_REQUEST**: Creating {Count} requests for regex pattern updates in the database.", successfulDetectionsForDB.Count);
                    var regexUpdateRequests = successfulDetectionsForDB
                        .Select(c => this.CreateRegexUpdateRequest(c, fileText, metadata, invoice.Id)).ToList();

                    // =================================== LOGGING ENHANCEMENT ===================================
                    _logger.Error("   - **DATA_DUMP (regexUpdateRequests)**: Object state before sending to UpdateRegexPatternsAsync: {Data}",
                        JsonSerializer.Serialize(regexUpdateRequests, jsonOptions));
                    // =================================== END ENHANCEMENT =====================================

                    await this.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false);
                }

                bool isBalanced = OCRCorrectionService.TotalsZero(invoice, _logger);
                _logger.Error("🏁 **ORCHESTRATION_COMPLETE**: Finished for Invoice '{InvoiceNo}'. Final balance state: {IsBalanced}. Corrections applied: {CorrectionsApplied}",
                    invoice.InvoiceNo, isBalanced, successfulValueApplications > 0);

                return successfulValueApplications > 0 || isBalanced;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **ORCHESTRATION_EXCEPTION**: Error during CorrectInvoiceAsync for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        #endregion

        #region Internal and Public Helpers

        private Dictionary<string, OCRFieldMetadata> ExtractFullOCRMetadata(ShipmentInvoice shipmentInvoice, string fileText)
        {
            var metadataDict = new Dictionary<string, OCRFieldMetadata>();
            if (shipmentInvoice == null) return metadataDict;

            Action<string, object> addMetaIfValuePresent = (propName, value) =>
            {
                if (value == null || (value is string s && string.IsNullOrEmpty(s))) return;
                var mappedInfo = this.MapDeepSeekFieldToDatabase(propName);
                if (mappedInfo == null) return;

                int lineNumberInText = this.FindLineNumberInTextByFieldName(mappedInfo.DisplayName, fileText);
                string lineTextFromDoc = lineNumberInText > 0 ? this.GetOriginalLineText(fileText, lineNumberInText) : null;

                metadataDict[mappedInfo.DatabaseFieldName] = new OCRFieldMetadata
                {
                    FieldName = mappedInfo.DatabaseFieldName,
                    Value = value.ToString(),
                    RawValue = value.ToString(),
                    LineNumber = lineNumberInText,
                    LineText = lineTextFromDoc,
                    Key = mappedInfo.DisplayName,
                    Field = mappedInfo.DatabaseFieldName,
                    EntityType = mappedInfo.EntityType,
                    DataType = mappedInfo.DataType,
                    IsRequired = mappedInfo.IsRequired
                };
            };

            addMetaIfValuePresent("InvoiceNo", shipmentInvoice.InvoiceNo);
            addMetaIfValuePresent("InvoiceDate", shipmentInvoice.InvoiceDate);
            addMetaIfValuePresent("InvoiceTotal", shipmentInvoice.InvoiceTotal);
            addMetaIfValuePresent("SubTotal", shipmentInvoice.SubTotal);
            addMetaIfValuePresent("TotalInternalFreight", shipmentInvoice.TotalInternalFreight);
            addMetaIfValuePresent("TotalOtherCost", shipmentInvoice.TotalOtherCost);
            addMetaIfValuePresent("TotalInsurance", shipmentInvoice.TotalInsurance);
            addMetaIfValuePresent("TotalDeduction", shipmentInvoice.TotalDeduction);
            addMetaIfValuePresent("Currency", shipmentInvoice.Currency);
            addMetaIfValuePresent("SupplierName", shipmentInvoice.SupplierName);
            addMetaIfValuePresent("SupplierAddress", shipmentInvoice.SupplierAddress);

            return metadataDict;
        }

        /// <summary>
        /// CRITICAL FIX v3: This method now ensures that the granular, accurate line-level context
        /// from the CorrectionResult is passed directly into the RegexUpdateRequest, preventing
        /// context-passing bugs that led to validation failures.
        /// </summary>
        public RegexUpdateRequest CreateRegexUpdateRequest(
            CorrectionResult correction,
            string fileText, // fileText is now mainly for fallback.
            Dictionary<string, OCRFieldMetadata> metadata,
            int? templateId)
        {
            // =================================== LOGGING ENHANCEMENT ===================================
            _logger.Error("   - **CreateRegexUpdateRequest_ENTRY**: Creating request from CorrectionResult: {Data}",
                JsonSerializer.Serialize(correction, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
            // =================================== END ENHANCEMENT =====================================

            var request = new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,
                SuggestedRegex = correction.SuggestedRegex,
                ExistingRegex = correction.ExistingRegex,
                LineId = correction.LineId,
                PartId = correction.PartId,
                RegexId = correction.RegexId,
                InvoiceId = templateId,

                // Directly inherit the accurate line number and text from the correction object.
                LineNumber = correction.LineNumber,
                LineText = correction.LineText,
                WindowText = correction.WindowText,
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,

                // Pass through the new format correction properties
                Pattern = correction.Pattern,
                Replacement = correction.Replacement
            };

            // This block is now a safety fallback. The primary source of LineText should be the correction object.
            if (!string.IsNullOrEmpty(fileText) && string.IsNullOrEmpty(request.LineText) && request.LineNumber > 0)
            {
                _logger.Warning("CreateRegexUpdateRequest: LineText was missing from CorrectionResult for line {LineNum}. Falling back to extracting from full text.", request.LineNumber);
                var lines = fileText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (request.LineNumber <= lines.Length)
                {
                    request.LineText = lines[request.LineNumber - 1];
                }
            }

            return request;
        }


        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
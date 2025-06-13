// File: OCRCorrectionService/OCRCorrectionService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using Serilog;
using InvoiceReader.OCRCorrectionService;

namespace WaterNut.DataSpace
{
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

        public OCRCorrectionService(ILogger logger = null)
        {
            _logger = logger ?? Log.Logger.ForContext<OCRCorrectionService>();
            try
            {
                _logger.Error("🔍 **OCR_CONSTRUCTOR**: Attempting to initialize DeepSeekInvoiceApi...");
                _deepSeekApi = new DeepSeekInvoiceApi(_logger);
                _logger.Error("✅ **OCR_CONSTRUCTOR**: DeepSeekInvoiceApi initialized SUCCESSFULLY.");
            }
            catch (Exception ex)
            {
                // This will log the REAL error to your console/file
                _logger.Error(ex, "🚨🚨🚨 CRITICAL FAILURE: The DeepSeekInvoiceApi constructor threw an exception. This is the root cause of the NullReferenceException. 🚨🚨🚨");
                // Re-throw the exception to ensure the application still behaves as it did before,
                // but now you have the log of the true cause.
                throw;
            }
            _strategyFactory = new DatabaseUpdateStrategyFactory(_logger);
        }

        #endregion

        #region Public Orchestration Methods

        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
        {
            if (invoice == null || string.IsNullOrEmpty(fileText)) return false;

            try
            {
                var metadata = this.ExtractFullOCRMetadata(invoice, fileText);
                var allDetectedErrors =
                    await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                if (!allDetectedErrors.Any())
                {
                    return OCRCorrectionService.TotalsZero(invoice, _logger);
                }

                var appliedCorrections =
                    await this.ApplyCorrectionsAsync(invoice, allDetectedErrors, fileText, metadata)
                        .ConfigureAwait(false);
                var successfulValueApplications = appliedCorrections.Count(c => c.Success);

                var customsCorrections = this.ApplyCaribbeanCustomsRules(
                    invoice,
                    appliedCorrections.Where(c => c.Success).ToList());
                if (customsCorrections.Any())
                {
                    this.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);
                }

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
                                 Success = true
                             }).ToList();

                if (successfulDetectionsForDB.Any())
                {
                    var regexUpdateRequests = successfulDetectionsForDB
                        .Select(c => this.CreateRegexUpdateRequest(c, fileText, metadata, invoice.Id)).ToList();
                    await this.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false);
                }

                return successfulValueApplications > 0 || OCRCorrectionService.TotalsZero(invoice, _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during CorrectInvoiceAsync for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        #endregion

        #region Internal and Public Helpers

        private Dictionary<string, OCRFieldMetadata> ExtractFullOCRMetadata(
            ShipmentInvoice shipmentInvoice,
            string fileText)
        {
            var metadataDict = new Dictionary<string, OCRFieldMetadata>();
            if (shipmentInvoice == null) return metadataDict;

            Action<string, object> addMetaIfValuePresent = (propName, value) =>
                {
                    if (value == null || (value is string s && string.IsNullOrEmpty(s))) return;
                    var mappedInfo = this.MapDeepSeekFieldToDatabase(propName);
                    if (mappedInfo == null) return;

                    int lineNumberInText = this.FindLineNumberInTextByFieldName(mappedInfo.DisplayName, fileText);
                    string lineTextFromDoc =
                        lineNumberInText > 0 ? this.GetOriginalLineText(fileText, lineNumberInText) : null;

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

        public RegexUpdateRequest CreateRegexUpdateRequest(
            CorrectionResult correction,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata,
            int? invoiceId)
        {
            var request = new RegexUpdateRequest
                              {
                                  FieldName = correction.FieldName,
                                  OldValue = correction.OldValue,
                                  NewValue = correction.NewValue,
                                  CorrectionType = correction.CorrectionType,
                                  Confidence = correction.Confidence,
                                  DeepSeekReasoning = correction.Reasoning,
                                  LineNumber = correction.LineNumber,
                                  RequiresMultilineRegex = correction.RequiresMultilineRegex,
                                  ExistingRegex = correction.ExistingRegex,
                                  SuggestedRegex = correction.SuggestedRegex,
                                  LineId = correction.LineId,
                                  PartId = correction.PartId,
                                  RegexId = correction.RegexId,
                                  InvoiceId = invoiceId
                              };

            if (!string.IsNullOrEmpty(fileText))
            {
                var lines = fileText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (correction.LineNumber > 0 && correction.LineNumber <= lines.Length)
                {
                    request.LineText = lines[correction.LineNumber - 1];
                }

                request.WindowText = correction.WindowText;
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
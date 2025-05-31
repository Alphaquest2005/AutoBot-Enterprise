// File: OCRCorrectionService/OCRCorrectionService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    using System.IO;

    /// <summary>
    /// Service for handling OCR error corrections using DeepSeek LLM analysis
    /// Enhanced with comprehensive product validation and regex learning
    /// </summary>
    public partial class OCRCorrectionService : IDisposable
    {
        #region Fields and Properties

        private readonly DeepSeekInvoiceApi _deepSeekApi;
        private readonly ILogger _logger;
        private bool _disposed = false;

        // Configuration properties
        public double DefaultTemperature { get; set; } = 0.1; // Lower temp for corrections
        public int DefaultMaxTokens { get; set; } = 4096;

        #endregion

        #region Constructor

        public OCRCorrectionService(ILogger logger = null)
        {
            _deepSeekApi = new DeepSeekInvoiceApi();
            _logger = logger ?? Log.Logger.ForContext<OCRCorrectionService>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Corrects a single invoice using comprehensive DeepSeek analysis
        /// </summary>
        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                if (invoice == null || string.IsNullOrEmpty(fileText))
                {
                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
                    return false;
                }

                _logger.Information("Starting OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText).ConfigureAwait(false);
                if (!errors.Any())
                {
                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
                    return false;
                }

                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}", errors.Count, invoice.InvoiceNo);

                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
                var successfulCorrections = corrections.Count(c => c.Success);

                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
                    successfulCorrections, corrections.Count, invoice.InvoiceNo);

                return successfulCorrections > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error correcting invoice {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        /// <summary>
        /// Corrects multiple invoices with comprehensive validation and regex updates
        /// </summary>
        public async Task<List<ShipmentInvoice>> CorrectInvoicesAsync(List<ShipmentInvoice> invoices, string droppedFilePath)
        {
            var invoicesWithIssues = invoices.Where(x => x.TotalsZero != 0).ToList();
            _logger.Information("Found {Count} invoices with TotalsZero != 0", invoicesWithIssues.Count);

            foreach (var invoice in invoicesWithIssues)
            {
                try
                {
                    _logger.Information("Processing invoice {InvoiceNo} with TotalsZero: {TotalsZero}",
                        invoice.InvoiceNo, invoice.TotalsZero);

                    var txtFile = droppedFilePath + ".txt";
                    if (!System.IO.File.Exists(txtFile))
                    {
                        _logger.Warning("Text file not found: {FilePath}", txtFile);
                        continue;
                    }

                    var fileTxt = System.IO.File.ReadAllText(txtFile);
                    await this.CorrectInvoiceWithRegexUpdatesAsync(invoice, fileTxt).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error processing invoice {InvoiceNo}", invoice.InvoiceNo);
                }
            }

            return invoices;
        }

        /// <summary>
        /// Corrects a single invoice with comprehensive validation and regex updates
        /// </summary>
        public async Task<bool> CorrectInvoiceWithRegexUpdatesAsync(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                if (invoice == null || string.IsNullOrEmpty(fileText))
                {
                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
                    return false;
                }

                _logger.Information("Starting comprehensive OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

                // 1. Detect all types of errors
                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText).ConfigureAwait(false);
                if (!errors.Any())
                {
                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
                    return false;
                }

                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}: {ErrorSummary}",
                    errors.Count, invoice.InvoiceNo, string.Join(", ", errors.Select(e => $"{e.Field}({e.ErrorType})")));

                // 2. Apply corrections
                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
                var successfulCorrections = corrections.Where(c => c.Success).ToList();

                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
                    successfulCorrections.Count, corrections.Count, invoice.InvoiceNo);

                // 3. Validate post-correction totals
                var postCorrectionValid = TotalsZero(invoice, _logger);
                _logger.Information("Post-correction TotalsZero validation: {IsValid} for invoice {InvoiceNo}",
                    postCorrectionValid, invoice.InvoiceNo);

                // 4. Update regex patterns based on successful corrections
                if (successfulCorrections.Any())
                {
                    await this.UpdateRegexPatternsAsync(successfulCorrections, fileText).ConfigureAwait(false);
                }

                return successfulCorrections.Any();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in comprehensive invoice correction for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _deepSeekApi?.Dispose();
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
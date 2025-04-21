using System.Collections.Generic;
using System.IO;
using System.Text;
using CoreEntities.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Serilog; // Added Serilog using
using System.Linq; // Added System.Linq using
using WaterNut.DataSpace; // Added WaterNut.DataSpace using for Invoice type

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class InvoiceProcessingContext
    {
        private static readonly ILogger _logger = Log.ForContext<InvoiceProcessingContext>();

        public string FilePath { get; set; }

        private int _fileTypeId;
        public int FileTypeId
        {
            get => _fileTypeId;
            set
            {
                if (_fileTypeId != value)
                {
                    _fileTypeId = value;
                    _logger.Debug("Context Property Changed: FileTypeId = {NewValue}", value);
                }
            }
        }

        public string EmailId { get; set; }
        public bool OverWriteExisting { get; set; }
        public List<AsycudaDocumentSet> DocSet { get; set; }

        private FileTypes _fileType;
        public FileTypes FileType
        {
            get => _fileType;
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    _logger.Debug("Context Property Changed: FileType = {NewValue}", value?.Description ?? "null");
                }
            }
        }

        public EmailDownloader.Client Client { get; set; }
        public StringBuilder PdfText { get; set; }

        private Invoice _template;
        public Invoice Template // Reverted Template property type to WaterNut.DataSpace.Invoice
        {
            get => _template;
            set
            {
                if (_template != value)
                {
                    _template = value;
                    _logger.Debug("Context Property Changed: Template = {NewValue}", value?.OcrInvoices?.Name ?? "null"); // Access Name via OcrInvoices
                }
            }
        }

        private string _formattedPdfText;
        public string FormattedPdfText // Added FormattedPdfText property
        {
            get => _formattedPdfText;
            set
            {
                if (_formattedPdfText != value)
                {
                    _formattedPdfText = value;
                    _logger.Debug("Context Property Changed: FormattedPdfText. Length = {NewValueLength}", value?.Length ?? 0);
                }
            }
        }

        private List<dynamic> _csvLines;
        public List<dynamic> CsvLines // Added CsvLines property
        {
            get => _csvLines;
            set
            {
                if (_csvLines != value)
                {
                    _csvLines = value;
                    _logger.Debug("Context Property Changed: CsvLines. Count = {NewValueCount}", value?.Count ?? 0);
                }
            }
        }

        private ImportStatus _importStatus;
        public ImportStatus ImportStatus // Added ImportStatus property
        {
            get => _importStatus;
            set
            {
                if (_importStatus != value)
                {
                    _importStatus = value;
                    _logger.Debug("Context Property Changed: ImportStatus = {NewValue}", value);
                }
            }
        }

        public IEnumerable<Invoice> Templates { get; set; }

        private IEnumerable<Invoice> _possibleInvoices;
        public IEnumerable<Invoice> PossibleInvoices
        {
            get => _possibleInvoices;
            set
            {
                if (_possibleInvoices != value)
                {
                    _possibleInvoices = value;
                    _logger.Debug("Context Property Changed: PossibleInvoices. Count = {NewValueCount}", value?.Count() ?? 0); // Use Count() for IEnumerable
                }
            }
        }

        public Dictionary<string, (string file, string, ImportStatus Success)> Imports { get; set; } = new Dictionary<string, (string file, string, ImportStatus Success)>();

        private string _error;
        public string Error // Added Error property
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    _logger.Debug("Context Property Changed: Error = {NewValue}", value ?? "null");
                }
            }
        }

        private List<Line> _failedLines;
        public List<Line> FailedLines // Added FailedLines property
        {
            get => _failedLines;
            set
            {
                if (_failedLines != value)
                {
                    _failedLines = value;
                    _logger.Debug("Context Property Changed: FailedLines. Count = {NewValueCount}", value?.Count ?? 0);
                }
            }
        }

        public FileInfo FileInfo { get; set; } // Added FileInfo property
        public string TxtFile { get; set; } // Added TxtFile property
        public string EmailBody { get; set; } // Added EmailBody property
        // Add properties to hold results from each step
    }
}
using System.Collections.Generic;
using System.Diagnostics;
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








        public IEnumerable<Invoice> Templates { get; set; } = Enumerable.Empty<Invoice>();



        public Dictionary<string, (string file, string, ImportStatus Success)> Imports { get; set; } = new Dictionary<string, (string file, string, ImportStatus Success)>();

        private List<string> _errors = new List<string>();
        public List<string> Errors // Changed from string to List<string> for accumulation
        {
            get => _errors;
            // Setter is removed; use AddError method to append errors
        }

        // Method to add errors consistently and log them
        public void AddError(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _errors.Add(errorMessage);
                _logger.Error("Error added to context: {ErrorMessage}", errorMessage); // Log as Error for visibility
                _logger.Debug("Context Property Changed: Errors count = {ErrorCount}", _errors.Count);
            }
        }



        public FileInfo FileInfo { get; set; } // Added FileInfo property
        public string TextFilePath { get; set; } // Added TextFilePath property
        public string EmailBody { get; set; } // Added EmailBody property

        public List<Line> FailedLines
        {
            get { return this.Templates.SelectMany(x => x.FailedLines ?? x.Parts.SelectMany(z => z.FailedLines).ToList()).ToList(); }
        }

        public ImportStatus ImportStatus
        {
            get
            {
                var res = this.Templates.Select(x => x.ImportStatus).ToList();
                if (res.Count == 0)
                {
                    return ImportStatus.Failed;
                }

                var status = res.FirstOrDefault();
                foreach (var item in res)
                {
                    if (item != status)
                    {
                        return ImportStatus.Failed;
                    }
                }
                return status;
            }
        }

        public List<AsycudaDocumentSet> DocSet { get; set; }
        // Add properties to hold results from each step
    }
}
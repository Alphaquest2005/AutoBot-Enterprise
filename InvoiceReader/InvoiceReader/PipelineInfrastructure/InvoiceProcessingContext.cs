using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CoreEntities.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Serilog; // Added Serilog using
using System.Linq; // Added System.Linq using
using WaterNut.DataSpace; // Added WaterNut.DataSpace using for Template type

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class InvoiceProcessingContext(ILogger logger)
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<InvoiceProcessingContext>();

        public ILogger Logger { get; set; } = logger; // Add ILogger property

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
                    Logger?.Debug("Context Property Changed: FileTypeId = {NewValue}", value); // Use the new Logger property
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
                    Logger?.Debug("Context Property Changed: FileType = {NewValue}", value?.Description ?? "null"); // Use the new Logger property
                }
            }
        }

        public EmailDownloader.Client Client { get; set; }
        public StringBuilder PdfText { get; set; }






        public IEnumerable<Template> Templates { get; set; } = Enumerable.Empty<Template>();

        // Added to store templates identified as matching the document
        public IEnumerable<Template> MatchedTemplates { get; set; } = Enumerable.Empty<Template>();


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
                Logger?.Error("Error added to context: {ErrorMessage}", errorMessage); // Use the new Logger property
                Logger?.Debug("Context Property Changed: Errors count = {ErrorCount}", _errors.Count); // Use the new Logger property
            }
        }


        public FileInfo FileInfo { get; set; } // Added FileInfo property
        public string TextFilePath { get; set; } // Added TextFilePath property
        public string EmailBody { get; set; } // Added EmailBody property

        public List<Line> FailedLines
        {
            get { return this.MatchedTemplates.SelectMany(x => x.FailedLines ?? x.Parts.SelectMany(z => z.FailedLines).ToList()).ToList(); }
        }

        public ImportStatus ImportStatus
        {
            get
            {
                var res = this.MatchedTemplates.Select(x => x.ImportStatus).ToList();
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
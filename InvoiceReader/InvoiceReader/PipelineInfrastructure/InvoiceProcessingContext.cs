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
        // Add properties to hold results from each step
    }
}
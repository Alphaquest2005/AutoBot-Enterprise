using System.Collections.Generic;
using System.IO;
using System.Text;
using CoreEntities.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class InvoiceProcessingContext
    {
        public string FilePath { get; set; }
        public int FileTypeId { get; set; }
        public string EmailId { get; set; }
        public bool OverWriteExisting { get; set; }
        public List<AsycudaDocumentSet> DocSet { get; set; }
        public FileTypes FileType { get; set; }
        public EmailDownloader.Client Client { get; set; }
        public StringBuilder PdfText { get; set; }
        public Invoice Template { get; set; } // Added Template property
        public string FormattedPdfText { get; set; } // Added FormattedPdfText property
        public List<dynamic> CsvLines { get; set; } // Added CsvLines property
        public ImportStatus ImportStatus { get; set; } // Added ImportStatus property
        public IEnumerable<Invoice> Templates { get; set; }
        public IEnumerable<Invoice> PossibleInvoices { get; set; }
        public Dictionary<string, (string file, string, ImportStatus Success)> Imports { get; set; } = new Dictionary<string, (string file, string, ImportStatus Success)>();
        public string Error { get; set; } // Added Error property
        public List<Line> FailedLines { get; set; } // Added FailedLines property
        public FileInfo FileInfo { get; set; } // Added FileInfo property
        public string TxtFile { get; set; } // Added TxtFile property
        public string EmailBody { get; set; } // Added EmailBody property
        // Add properties to hold results from each step
    }
}
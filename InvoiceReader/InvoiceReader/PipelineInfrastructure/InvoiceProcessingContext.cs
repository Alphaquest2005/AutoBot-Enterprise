using System.Collections.Generic;
using System.Text;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
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
        public IEnumerable<Invoice> Templates { get; set; }
        public IEnumerable<Invoice> PossibleInvoices { get; set; }
        public Dictionary<string, (string file, string, ImportStatus Success)> Imports { get; set; } = new Dictionary<string, (string file, string, ImportStatus Success)>();
        // Add properties to hold results from each step
    }
}
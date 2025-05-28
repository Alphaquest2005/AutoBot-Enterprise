using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Invoices = OCR.Business.Entities.Invoices;

namespace WaterNut.Business.Services.Importers
{
    public class ImportSettings
    {
        public FileTypes FileType { get; }
        public List<AsycudaDocumentSet> DocSet { get; }
        public bool OverWrite { get; }
        public string DroppedFilePath { get; set; }
        public string EmailId { get; set; }
        public Invoices Template { get; set; }

        public ImportSettings(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite, string fileName, string emailId)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWrite = overWrite;
            DroppedFilePath = fileName;
            EmailId = emailId;
        }

        public ImportSettings(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite, string fileName, string emailId, Invoices template)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWrite = overWrite;
            DroppedFilePath = fileName;
            EmailId = emailId;
            Template = template;
        }


    }
}
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;

namespace WaterNut.Business.Services.Importers
{
    public class ImportSettings 
    {
        public FileTypes FileType { get; }
        public List<AsycudaDocumentSet> DocSet { get; }
        public bool OverWrite { get; }
        public string DroppedFilePath { get; set; }
        public string EmailId { get; set; }

        public ImportSettings(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite, string fileName, string emailId)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWrite = overWrite;
            DroppedFilePath = fileName;
            EmailId = emailId;
        }


    }
}
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace
{
    public class DataFile
    {
        public FileTypes FileType { get; }
        public List<AsycudaDocumentSet> DocSet { get; }
        public bool OverWriteExisting { get; }
        public string EmailId { get; }
        public string DroppedFilePath { get; }
        public List<dynamic> Data { get; }

        public DataFile(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> data)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWriteExisting = overWriteExisting;
            EmailId = emailId;
            DroppedFilePath = droppedFilePath;
            Data = data;
        }
    }
}
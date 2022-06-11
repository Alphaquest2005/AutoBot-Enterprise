using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class RawDataFile
    {
        public FileTypes FileType { get; }
        public string[] Lines { get; }
        public string[] Headings { get; }
        public List<AsycudaDocumentSet> DocSet { get; }
        public bool OverWriteExisting { get; }
        public string EmailId { get; }
        public string DroppedFilePath { get; }

        public RawDataFile(FileTypes fileType, string[] lines, string[] headings, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath)
        {
            FileType = fileType;
            Lines = lines;
            Headings = headings;
            DocSet = docSet;
            OverWriteExisting = overWriteExisting;
            EmailId = emailId;
            DroppedFilePath = droppedFilePath;
            
        }
    }
}
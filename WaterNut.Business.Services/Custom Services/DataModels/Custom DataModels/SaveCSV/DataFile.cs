using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using OCR.Business.Entities;
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
        public OCR.Business.Entities.Templates Template { get; }
        public Dictionary<(int lineNumber, string section), Dictionary<(OCR.Business.Entities.Fields Fields, string Instance), string>> LineValues { get; }

        public DataFile(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> data, OCR.Business.Entities.Templates template)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWriteExisting = overWriteExisting;
            EmailId = emailId;
            DroppedFilePath = droppedFilePath;
            Data = data;
            Template = template;
            LineValues = new Dictionary<(int lineNumber, string section), Dictionary<(OCR.Business.Entities.Fields Fields, string Instance), string>>();
        }

        public DataFile(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> data, (OCR.Business.Entities.Templates template, Dictionary<(int lineNumber, string section), Dictionary<(OCR.Business.Entities.Fields Fields, string Instance), string>> lineValues) templateData)
        {
            FileType = fileType;
            DocSet = docSet;
            OverWriteExisting = overWriteExisting;
            EmailId = emailId;
            DroppedFilePath = droppedFilePath;
            Data = data;
            Template = templateData.template;
            LineValues = templateData.lineValues;
        }
    }
}
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;

namespace WaterNut.Business.Services.Importers
{
    public class DataFile
    {
        public string FileName { get; }
        public FileTypes FileType { get; }
        public List<AsycudaDocumentSet> DocSet { get; }
        public bool OverWrite { get; }
        public List<dynamic> Data { get; }

        public DataFile(string fileName, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite, List<dynamic> data)
        {
            FileName = fileName;
            FileType = fileType;
            DocSet = docSet;
            OverWrite = overWrite;
            Data = data;
            
        }
    }
}
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class POProcessor : IDocumentProcessor
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;
        private readonly bool _overWrite;

        public POProcessor()
        {

        }

        public POProcessor(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWrite)
        {
            _fileType = fileType;
            _docSet = docSet;
            _overWrite = overWrite;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            return new List<dynamic>();

        }
    }
}
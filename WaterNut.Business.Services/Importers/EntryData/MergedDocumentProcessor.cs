using System.Collections.Generic;
using System.Linq;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class MergedDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentProcessor _first;
        private readonly IDocumentProcessor _second;

        public MergedDocumentProcessor(IDocumentProcessor first, IDocumentProcessor second)
        {
            _first = first;
            _second = second;
        }

        public List<dynamic> Execute(List<dynamic> list)
        {
            return _first.Execute(list).Union(_second.Execute(list)).ToList();
        }
    }

    public class MergedInventoryProcessor : IInventoryProcessor
    {
        private readonly IInventoryProcessor _first;
        private readonly IInventoryProcessor _second;

        public MergedInventoryProcessor(IInventoryProcessor first, IInventoryProcessor second)
        {
            _first = first;
            _second = second;
        }

        public List<InventoryDataItem> Execute(List<InventoryDataItem> list)
        {
            return _first.Execute(list).Union(_second.Execute(list)).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers.EntryData
{
   
    public class ChainedDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentProcessor _first;
        private readonly IDocumentProcessor _second;

        public ChainedDocumentProcessor(IDocumentProcessor first, IDocumentProcessor second)
        {
            _first = first;
            _second = second;
        }
        
        public List<dynamic> Execute(List<dynamic> list)
        {
            return _second.Execute(_first.Execute(list));
        }
    }

    public static class ChainedDocumentProcessorConstruction
    {
        public static IDocumentProcessor Then(this IDocumentProcessor first, IDocumentProcessor next) =>
            new ChainedDocumentProcessor(first, next);
    }

    public class ChainedInventoryProcessor : IInventoryProcessor
    {
        private readonly IInventoryProcessor _first;
        private readonly IInventoryProcessor _second;

        public ChainedInventoryProcessor(IInventoryProcessor first, IInventoryProcessor second)
        {
            _first = first;
            _second = second;
        }

        public List<InventoryDataItem> Execute(List<InventoryDataItem> list)
        {
            return _second.Execute(_first.Execute(list));
        }
    }

    public static class ChainedInventoryProcessorConstruction
    {
        public static IInventoryProcessor Then(this IInventoryProcessor first, IInventoryProcessor next) =>
            new ChainedInventoryProcessor(first, next);
    }

}
using System.Collections.Generic;
using CoreEntities.Business.Entities;

namespace WaterNut.Business.Services.Importers
{
    public class ChainedDataExtractor : IDataExtractor
    {
        private readonly IDataExtractor _first;
        private readonly IDataExtractor _second;

        public ChainedDataExtractor(IDataExtractor first, IDataExtractor second)
        {
            _first = first;
            _second = second;
        }
        public List<dynamic> Execute()
        {
            return _second.Execute(_first.Execute());
        }

        public List<dynamic> Execute(List<dynamic> list)
        {
            return _second.Execute(_first.Execute(list));
        }
    }

    public static class ChainDataExtractorConstruction
    {
        public static IDataExtractor Then(this IDataExtractor first, IDataExtractor next) =>
            new ChainedDataExtractor(first, next);
    }
}
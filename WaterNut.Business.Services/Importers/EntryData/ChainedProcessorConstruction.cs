namespace WaterNut.Business.Services.Importers.EntryData
{
    public static class ChainedProcessorConstruction
    {
        public static IProcessor<T> Then<T>(this IProcessor<T> first, IProcessor<T> next) =>
            new ChainedProcessor<T>(first, next);
    }
}
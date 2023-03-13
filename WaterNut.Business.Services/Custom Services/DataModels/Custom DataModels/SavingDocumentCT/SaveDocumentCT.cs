using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Services;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace
{
    public class SaveDocumentCT : ISaveDocumentCT
    {
      
        public async Task Execute(DocumentCT cdoc)
        {
            if (cdoc == null) return;

            using (var ctx = new xcuda_ASYCUDAService())
            {
                cdoc.Document = await ctx.CleanAndUpdateXcuda_ASYCUDA(cdoc.Document).ConfigureAwait(false);
            }


            DataSpace.PreProcessDocumentItems.Execute(cdoc);

            //Parallel.ForEach(cdoc.DocumentItems, new ParallelOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, item =>
            //{

            using (var ctx = new xcuda_ItemService())
            {
                foreach (var t in cdoc.DocumentItems)
                {
                    var exceptions = new ConcurrentQueue<Exception>();
                    //cdoc.DocumentItems.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll((t) =>
                    //{
                    try
                    {
                        if (t.ChangeTracker != null)
                            ctx.Updatexcuda_Item(t.ChangeTracker.FirstOrDefault()).Wait(); //.ChangeTracker.GetChanges().FirstOrDefault()
                    }
                    catch (Exception ex)
                    {
                        exceptions.Enqueue(ex);
                    }

                    //});
                    if (exceptions.Count > 0) throw new AggregateException(exceptions);
                    //    await ctx.Updatexcuda_Item(cdoc.DocumentItems).ConfigureAwait(false);
                }
            }

            //});
        }
    }
}
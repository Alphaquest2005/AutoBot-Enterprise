using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace
{
    public class SaveDocumentCTBulk : ISaveDocumentCT
    {

        public Task Execute(DocumentCT cdoc)
        {
            if (cdoc == null) return Task.CompletedTask;

            try
            {
                CleanAsycudaDocument.Execute(cdoc.Document);
                if (cdoc.Document.ASYCUDA_Id == 0)
                {
                    new DocumentDSContext().BulkInsert(new List<xcuda_ASYCUDA>() { cdoc.Document },
                        x => x.IncludeGraph = true);
                }
                else
                {
                    new DocumentDSContext().BulkUpdate(new List<xcuda_ASYCUDA>() { cdoc.Document },
                        x => x.IncludeGraph = true);
                }

                DataSpace.PreProcessDocumentItems.Execute(cdoc);
                var newitems = cdoc.DocumentItems.Where(x => x.Item_Id == 0).ToList();
                var olditems = cdoc.DocumentItems.Where(x => x.Item_Id != 0).ToList();

                new DocumentItemDSContext().BulkMerge(newitems, x => x.IncludeGraph = true);
                //new DocumentItemDSContext().BulkUpdate(olditems, x => x.IncludeGraph = true);
                //new DocumentItemDSContext().BulkInsert(newitems, x => x.IncludeGraph = true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
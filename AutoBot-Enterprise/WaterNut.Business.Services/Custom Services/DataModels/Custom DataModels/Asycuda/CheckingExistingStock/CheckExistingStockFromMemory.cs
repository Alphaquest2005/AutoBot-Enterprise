using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock
{
    public class CheckExistingStockMemory : ICheckExistingStock
    {


        static CheckExistingStockMemory()
        {
        }

        public CheckExistingStockMemory()
        {
          
        }

        private ConcurrentDictionary<int, xcuda_Item> AsycudaItems { get; }

        public CheckExistingStockMemory(ConcurrentDictionary<int, xcuda_Item> asycudaItems)
        {
            AsycudaItems = asycudaItems;

        }

        public string Execute(string itemNumber, DateTime salesEntryDataDate)
        {
            try
            {
                var itm = AsycudaItems.Values.FirstOrDefault(x =>
                    x.ItemNumber == itemNumber && x.AsycudaDocument.AssessmentDate <= salesEntryDataDate &&
                    (x.xRemainingBalance) > 0);
                return itm == null
                    ? ""
                    : GetExistingStockTextProcessor.GetExistingStockText((itm.AsycudaDocument.CNumber, itm.LineNumber,
                        itm.AsycudaDocument.RegistrationDate, itm.AsycudaDocument.AssessmentDate, itm.ItemQuantity,
                        itm.QtyAllocated, itm.PiQuantity));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
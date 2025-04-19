using System;
using System.Linq;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock
{
    public class CheckExistingStockDB : ICheckExistingStock
    {
        public string Execute(string itemNumber, DateTime salesEntryDataDate)
        {
            var itm = new AllocationDSContext().AsycudaItemRemainingQuantities.FirstOrDefault(x =>
                x.ItemNumber == itemNumber && x.AssessmentDate <= salesEntryDataDate && x.xRemainingBalance > 0);
            return itm == null? "" : GetExistingStockTextProcessor.GetExistingStockText((itm.CNumber, Convert.ToInt32(itm.LineNumber), itm.RegistrationDate, itm.AssessmentDate, Convert.ToDouble(itm.ItemQuantity), Convert.ToDouble(itm.QtyAllocated), Convert.ToDouble(itm.PiQuantity)));
            
        }
    }
}
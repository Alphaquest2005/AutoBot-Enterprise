using System;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock
{
    public class GetExistingStockTextProcessor
    {
        public static string GetExistingStockText((string CNumber,int LineNumber, DateTime? RegistrationDate, DateTime? AssessmentDate, double ItemQuantity, double QtyAllocated, double PiQuantity) itm)
        {
            return  $": Last Available Qty on C#{itm.CNumber}-{itm.LineNumber} RegDate:{itm.RegistrationDate.GetValueOrDefault().ToShortDateString()} AstDate:{itm.AssessmentDate.GetValueOrDefault().ToShortDateString()} ItemQty:{itm.ItemQuantity}, AlloQty:{itm.QtyAllocated}, piQty:{itm.PiQuantity}";
        }
    }
}
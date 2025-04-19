using System;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.CheckingExistingStock
{
    public interface ICheckExistingStock
    {
        string Execute(string itemNumber, DateTime salesEntryDataDate);
    }
}
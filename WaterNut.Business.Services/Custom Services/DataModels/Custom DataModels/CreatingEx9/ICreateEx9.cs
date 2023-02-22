using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    public interface ICreateEx9
    {
        bool PerIM7 { get; set; }
        bool Process7100 { get; set; }

        Task<List<DocumentCT>> Execute(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks,
            AsycudaDocumentSet docSet, string documentType, string ex9BucketType, bool isGrouped,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket, bool applyHistoricChecks,  bool perInvoice, bool autoAssess, bool overPIcheck, bool universalPIcheck, bool itemPIcheck);

        List<ItemSalesPiSummary> GetItemSalesPiSummary(int applicationSettingsId, DateTime startDate,
            DateTime endDate, string dfp, string entryDataType);

        Task<List<DocumentCT>> CreateDutyFreePaidDocument(string dfp,
            IEnumerable<AllocationDataBlock> slst,
            AsycudaDocumentSet docSet, string documentType, bool isGrouped, List<ItemSalesPiSummary> itemSalesPiSummaryLst,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, 
            bool applyEx9Bucket, string ex9BucketType, bool applyHistoricChecks, bool applyCurrentChecks,
            bool autoAssess, bool perInvoice, bool overPIcheck, bool universalPIcheck, bool itemPIcheck, string prefix = null);
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

using Serilog;

public partial class BaseDataModel
{
    public async Task CalculateDocumentSetFreight(int asycudaDocumentSetId, ILogger log)
    {
        var currency = "";
        double totalfob = 0;
        double totalItemQuantity = 0;
        double totalFreight = 0;
        decimal totalWeight = 0;
        List<int> doclst = null;
        var CIFValues = new Dictionary<int, double>();
        var ItemQuantities = new Dictionary<int, double>();
        AsycudaDocumentSet asycudaDocumentSet;
        using (var ctx = new DocumentDSContext())
        {
            asycudaDocumentSet =
                ctx.AsycudaDocumentSets.Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                    .FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
            if (asycudaDocumentSet != null)
            {
                if (asycudaDocumentSet.TotalFreight != null)
                    totalFreight = asycudaDocumentSet.TotalFreight.Value;
                if (asycudaDocumentSet.TotalWeight != null) totalWeight = (decimal)asycudaDocumentSet.TotalWeight.Value;
                if (totalWeight <= 0 && asycudaDocumentSet.Documents.Count() > 0)
                    throw new ApplicationException(
                        $"DocSet:{asycudaDocumentSet.Declarant_Reference_Number} Weight is Zero");

                currency = asycudaDocumentSet.FreightCurrencyCode ?? asycudaDocumentSet.Currency_Code;
            }

            doclst =
                ctx.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Where(x => x.IsManuallyAssessed != true &&
                                x.ImportComplete ==
                                false) // prevent recalculating weights of assessed entries
                    .Select(x => x.ASYCUDA_Id)
                    .ToList();
            if (!doclst.Any()) return;
        }

        using (var ctx = new CoreEntitiesContext())
        {
            foreach (var doc in doclst)
            {
                var t = ctx.AsycudaDocuments.Where(x => x.ASYCUDA_Id == doc)
                    .Select(y =>
                        y.TotalCIF + y.TotalInternalFreight + y.TotalInsurance + y.TotalOtherCost -
                        y.TotalDeduction).DefaultIfEmpty(0).Sum();
                var f = ctx.AsycudaDocuments.Where(x => x.ASYCUDA_Id == doc)
                    .Select(y => y.TotalFreight).DefaultIfEmpty(0)
                    .Sum(); // should be zero if new existing has value take away existing value
                var totalItems = ctx.AsycudaItemBasicInfo.Where(x => x.ASYCUDA_Id == doc)
                    .Select(x => x.ItemQuantity).DefaultIfEmpty(0).Sum(); //* 0.01
                ////////// added total items to prevent over weight due to minimum 0.01 requirement
                var val = f > t ? t.GetValueOrDefault() : t.GetValueOrDefault() - f.GetValueOrDefault(); // + ; 
                CIFValues.Add(doc, val);
                ItemQuantities.Add(doc, totalItems.GetValueOrDefault());
                totalfob += val;
                totalItemQuantity += totalItems.GetValueOrDefault();
            }
        }

        totalWeight -= WeightAsycudaNormallyOffBy;

        var freightRate = totalFreight != 0 && totalfob != 0 ? totalFreight / totalfob : 0;

     
        var weightRate = totalWeight != 0  && totalItemQuantity != 0 ? totalWeight / (decimal)totalItemQuantity : 0;
        decimal weightUsed = 0;

        using (var ctx = new DocumentDSContext { StartTracking = true })
        {
            var weightmsgSent = false;
            foreach (var doc in doclst)
            {
                //calulate frieght based on value, calculate weight based on quantity to prevent the minimm weight per value issue
                var cif = CIFValues.FirstOrDefault(x => x.Value > 0 && x.Key == doc);
                var totalItems = ItemQuantities.FirstOrDefault(x => x.Value > 0 && x.Key == doc);
                // refactor this in to a sub
                if (weightUsed > totalWeight && !weightmsgSent)
                {
                    //throw new ApplicationException("Weight Used Exceed Total Weight!");
                    if (!string.IsNullOrEmpty(BaseDataModel.GetClient().Email))
                        await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null,
                            $"Bug Found",
                            EmailDownloader.EmailDownloader.GetContacts("Developer", log),
                            $"Weight Used Exceed Total Weight! - DocSet:{asycudaDocumentSet?.Declarant_Reference_Number} TotalWeight:{totalWeight}",
                            Array.Empty<string>(), log).ConfigureAwait(false);
                    weightmsgSent = true;
                }


                if (asycudaDocumentSet.ApportionMethod == "Equal")
                {
                    if (cif.Value != 0) UpdateFreight(ctx, cif, totalFreight / doclst.Count(), currency);
                    if (totalItems.Value != 0)
                        weightUsed += UpdateWeight(ctx, totalItems, totalWeight / doclst.Count());
                }
                else
                {
                    if (cif.Value != 0) UpdateFreight(ctx, cif, cif.Value * freightRate, currency);
                    if (totalItems.Value != 0)
                        weightUsed += UpdateWeight(ctx, totalItems, (decimal)(totalItems.Value * (double)weightRate));
                }
            }

            ctx.SaveChanges();
        }
    }
}
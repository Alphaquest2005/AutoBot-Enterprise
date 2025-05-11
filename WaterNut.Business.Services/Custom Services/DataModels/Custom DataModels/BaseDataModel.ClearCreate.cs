using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;
using WaterNut.Business.Entities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using EntryPreviousItems = CoreEntities.Business.Entities.EntryPreviousItems;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    internal async Task Clear(int AsycudaDocumentSetId)
    {
        AsycudaDocumentSet docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);

        await Clear(docset).ConfigureAwait(false);
    }

    internal async Task Clear(AsycudaDocumentSet currentAsycudaDocumentSet)
    {
        await ClearAsycudaDocumentSet(currentAsycudaDocumentSet).ConfigureAwait(false);
    }

    public async Task ClearAsycudaDocumentSet(int AsycudaDocumentSetId)
    {
        var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
        await ClearAsycudaDocumentSet(docset).ConfigureAwait(false);
    }

    public async Task ClearAsycudaDocumentSet(AsycudaDocumentSet docset)
    {
        PreventDeletingFromSystemDocSet(docset);

        StatusModel.StartStatusUpdate($"Deleting Documents from '{docset.Declarant_Reference_Number}' Document Set",
            docset.xcuda_ASYCUDA_ExtendedProperties.Count());

        ParalellDeleteDocSetDocuments(docset);

        await CalculateDocumentSetFreight(docset.AsycudaDocumentSetId).ConfigureAwait(false);

        StatusModel.StopStatusUpdate();
    }

    public async Task<DocumentCT> CreateDocumentCt(AsycudaDocumentSet currentAsycudaDocumentSet)
    {
        var cdoc = new DocumentCT
        {
            Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet),
            DocumentItems = new List<xcuda_Item>()
        };

        return cdoc;
    }

    public xcuda_ASYCUDA CreateNewAsycudaDocument(AsycudaDocumentSet CurrentAsycudaDocumentSet)
    {
        var ndoc = new xcuda_ASYCUDA(true) { TrackingState = TrackingState.Added };
        ndoc.SetupProperties();

        if (CurrentAsycudaDocumentSet != null)
        {
            CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(ndoc.xcuda_ASYCUDA_ExtendedProperties);
            ndoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = CurrentAsycudaDocumentSet;
            ndoc.xcuda_ASYCUDA_ExtendedProperties.FileNumber =
                CurrentAsycudaDocumentSet.LastFileNumber
                    .GetValueOrDefault(); // the number is forward looking
            CurrentAsycudaDocumentSet.LastFileNumber = UpdateAsycudaDocumentSetLastNumber(
                CurrentAsycudaDocumentSet.AsycudaDocumentSetId,
                CurrentAsycudaDocumentSet.LastFileNumber.GetValueOrDefault());
        }

        return ndoc;
    }

    public string CleanText(string p)
    {
        return p?.Replace(",", "");
    }

    private xcuda_Item CreateNewDocumentItem()
    {
        var item = new xcuda_Item(true) { TrackingState = TrackingState.Added };
        item.SetupProperties();
        return item; //
    }

    public async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(int applicationSettingsId)
    {
        using (var ctx = new AsycudaDocumentSetService())
        {
            var doc = await ctx
                .CreateAsycudaDocumentSet(new AsycudaDocumentSet
                {
                    ApplicationSettingsId = applicationSettingsId,
                    Currency_Code = "USD",
                    FreightCurrencyCode = "USD"
                })
                .ConfigureAwait(false);
            return doc;
        }
    }

    public async Task SaveEntryPreviousItems(List<EntryPreviousItems> epi)
    {
        using (var ctx = new CoreEntitiesContext())
        {
            ctx.ApplyChanges(epi);
            ctx.SaveChanges();
        }
    }
}
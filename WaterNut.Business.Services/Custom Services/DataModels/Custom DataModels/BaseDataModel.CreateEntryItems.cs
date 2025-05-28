using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace;

using Serilog;

public partial class BaseDataModel
{
    public async Task<List<DocumentCT>> CreateEntryItems(List<EntryDataDetails> slstSource,
        AsycudaDocumentSet currentAsycudaDocumentSet, bool perInvoice, bool autoUpdate, bool autoAssess,
        bool combineEntryDataInSameFile, bool groupItems, bool checkPackages, ILogger log, string prefix = null)
    {
        var docList = new List<DocumentCT>();
        var itmcount = 0;
        var slst = groupItems || Instance.CurrentApplicationSettings.GroupIM4ByCategory == true
            ? CreateGroupEntryLineData(slstSource)
            : CreateSingleEntryLineData(slstSource);

        var cdoc = new DocumentCT { Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet) };


        IntCdoc(cdoc, currentAsycudaDocumentSet, prefix);

        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
        if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
        AttachCustomProcedure(cdoc, currentAsycudaDocumentSet.Customs_Procedure);
        var entryLineDatas = slst as IList<BaseDataModel.EntryLineData> ?? slst.ToList();
        StatusModel.StartStatusUpdate("Adding Entries to New Asycuda Document", entryLineDatas.Count());


        //////////////////////////////////////////////////////
        /// 
        /// If per invoice is choosen and sorted by tariff code it will alternate between invoice no and create too much entries
        /// left as is because this is not a problem
        /// 
        switch (Instance.CurrentApplicationSettings.OrderEntriesBy)
        {
            case "TariffCode":
                if (combineEntryDataInSameFile)
                    entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.SourceFile)
                        .ThenBy(p => p.EntryData.EntryDataId)
                        .ThenBy(p => p.InventoryItem?.TariffCode ?? p.TariffCode).ToList();
                else
                    entryLineDatas = entryLineDatas.OrderBy(p => p.InventoryItem?.TariffCode ?? p.TariffCode)
                        .ToList();

                break;
            case "Template":
                if (combineEntryDataInSameFile)
                    entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.SourceFile)
                        .ThenBy(p => p.EntryData.EntryDataId).ToList();
                else
                    entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.EntryDataId).ToList();

                break;
        }


        var oldentryData = new EntryData();
        foreach (var pod in entryLineDatas) //
        {
            var remainingPackages = pod.EntryData.Packages.GetValueOrDefault();
            var possibleEntries =
                Math.Ceiling(pod.EntryDataDetails.Count /
                             (double)(currentAsycudaDocumentSet.MaxLines ??
                                      Instance.CurrentApplicationSettings.MaxEntryLines));


            if (checkPackages)
                if (combineEntryDataInSameFile == false && remainingPackages < possibleEntries)
                    throw new ApplicationException("Entry data lines need more packages");

            if (pod.EntryData.DocumentType?.DocumentType != null && cdoc.DocumentItems.Count == 0 &&
                string.IsNullOrEmpty(oldentryData.EntryDataId))
            {
                var cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
            }

            if (oldentryData.EntryDataId != pod.EntryData.EntryDataId)
                if (perInvoice)
                    if (cdoc.DocumentItems.Any() && oldentryData.EntryDataId != pod.EntryData.EntryDataId)
                        if (combineEntryDataInSameFile && pod.EntryData.SourceFile != oldentryData.SourceFile ||
                            perInvoice && combineEntryDataInSameFile == false)
                        {
                            SetEffectiveAssessmentDate(cdoc);


                            LinkPreviousDocuments(pod, cdoc);

                            await SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
                            docList.Add(cdoc);
                            cdoc = new DocumentCT { Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet) };


                            var cp = currentAsycudaDocumentSet.Customs_Procedure;
                            IntCdoc(cdoc, currentAsycudaDocumentSet);
                            if (pod.EntryData.DocumentType?.DocumentType != null)
                                cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
                            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
                            if (autoAssess)
                                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                            AttachCustomProcedure(cdoc, cp);
                            itmcount = 0;
                        }
                        else
                        {
                            await this.AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, pod, cdoc).ConfigureAwait(false);
                            SetPackages(ref remainingPackages, ref possibleEntries, pod, cdoc);
                        }


            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code =
                pod.EntryData.Currency ?? currentAsycudaDocumentSet.Currency_Code;


            var itm = CreateItemFromEntryDataDetail(pod, cdoc);

            if (pod.EntryData is PurchaseOrders p)
            {
                if (itmcount == 0 && p.FinancialInformation != null)
                    cdoc.Document.xcuda_Traders.xcuda_Traders_Financial = new xcuda_Traders_Financial
                    {
                        Financial_name = p.FinancialInformation,
                        TrackingState = TrackingState.Added,
                        xcuda_Traders = cdoc.Document.xcuda_Traders
                    };

                cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name =
                    $"INV# {p.SupplierInvoiceNo}\r\n" +
                    $"{pod.EntryData.Suppliers?.SupplierName}\r\n" +
                    $"{pod.EntryData.Suppliers?.Street}\r\n";
            }

            if (pod.EntryData is Adjustments a)
            {
                cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name =
                    $"{pod.EntryData.Suppliers?.SupplierName ?? a.Vendor}\r\n" +
                    $"{pod.EntryData.Suppliers?.Street}\r\n";
            }


            if (itm == null) continue;
            itmcount += 1;

            if (itmcount == 1 && cdoc.DocumentItems.Any() && !cdoc.DocumentItems.First().xcuda_Packages.Any())
            {
                SetPackages(ref remainingPackages, ref possibleEntries, pod, cdoc);
                await this.AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, pod, cdoc).ConfigureAwait(false);
            }


            if (oldentryData.EntryDataId != pod.EntryData.EntryDataId)
            {
                await this.AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, pod, cdoc).ConfigureAwait(false);
                cdoc.Document.xcuda_Valuation.xcuda_Gs_internal_freight.Amount_foreign_currency +=
                    pod.EntryData.TotalInternalFreight.GetValueOrDefault();
                cdoc.Document.xcuda_Valuation.xcuda_Gs_internal_freight.Currency_code =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                cdoc.Document.xcuda_Valuation.xcuda_Gs_deduction.Amount_foreign_currency +=
                    pod.EntryData.TotalDeduction.GetValueOrDefault();
                cdoc.Document.xcuda_Valuation.xcuda_Gs_deduction.Currency_code =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                cdoc.Document.xcuda_Valuation.xcuda_Gs_insurance.Amount_foreign_currency +=
                    pod.EntryData.TotalInsurance.GetValueOrDefault();
                cdoc.Document.xcuda_Valuation.xcuda_Gs_insurance.Currency_code =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                cdoc.Document.xcuda_Valuation.xcuda_Gs_other_cost.Amount_foreign_currency +=
                    pod.EntryData.TotalOtherCost.GetValueOrDefault();
                cdoc.Document.xcuda_Valuation.xcuda_Gs_other_cost.Currency_code =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;

                if (Instance.CurrentApplicationSettings.AssessIM7 == true &&
                    pod.EntryData.Suppliers == null)
                    throw new ApplicationException($"Supplier not found for InvoiceNo {pod.EntryData.EntryDataId}");


                oldentryData = pod.EntryData;
            }


            if (itmcount % (currentAsycudaDocumentSet.MaxLines ?? Instance.CurrentApplicationSettings.MaxEntryLines) ==
                0)
                if (cdoc.DocumentItems.Any())
                {
                    await this.AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, pod, cdoc).ConfigureAwait(false);
                    SetEffectiveAssessmentDate(cdoc);
                    LinkPreviousDocuments(pod, cdoc);
                    await SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
                    docList.Add(cdoc);
                    cdoc = new DocumentCT { Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet) };
                    var cp = currentAsycudaDocumentSet.Customs_Procedure;
                    IntCdoc(cdoc, currentAsycudaDocumentSet);
                    if (pod.EntryData.DocumentType?.DocumentType != null)
                        cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
                    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
                    if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                    AttachCustomProcedure(cdoc, cp);
                    itmcount = 0;
                }

            LinkPreviousDocuments(pod, cdoc);

            StatusModel.StatusUpdate();
        }

        StatusModel.Timer("Saving To Database");
        if (cdoc.DocumentItems.Any())
        {
            await this.AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, entryLineDatas.Last(), cdoc).ConfigureAwait(false);
            SetEffectiveAssessmentDate(cdoc);
            await SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
            docList.Add(cdoc);
        }

        await CalculateDocumentSetFreight(currentAsycudaDocumentSet.AsycudaDocumentSetId, log).ConfigureAwait(false);
        StatusModel.StopStatusUpdate();

        await this.AttachToExistingDocuments(currentAsycudaDocumentSet.AsycudaDocumentSetId).ConfigureAwait(false);
        SetInvoicePerline(docList.Select(x => x.Document.ASYCUDA_Id).ToList());
        RenameDuplicateDocumentCodes(docList.Select(x => x.Document.ASYCUDA_Id).ToList());

        return docList;
    }
}
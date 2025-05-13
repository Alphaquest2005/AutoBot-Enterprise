using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace;

using System.Threading.Tasks;

public partial class BaseDataModel
{
    private async Task AttachDocSetDocumentsToDocuments(AsycudaDocumentSet currentAsycudaDocumentSet,
        BaseDataModel.EntryLineData pod,
        DocumentCT cdoc)
    {
        var alst = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
            .Where(x => x.Attachment.FilePath.Contains(pod.EntryData.EntryDataId) &&
                        (x.FileType.DocumentCode == "IV05" || x.FileType.DocumentCode == "DO02")
            )
            .Select(x => x.Attachment)
            .DistinctBy(x => x.Id)
            .ToList();
        if ((pod.EntryData is PurchaseOrders p))
        {
            var ialst = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
                .Where(x => x.Attachment.FilePath.Contains(p.SupplierInvoiceNo) &&
                            (x.FileType.DocumentCode == "IV05" || x.FileType.DocumentCode == "DO02")
                )
                .Select(x => x.Attachment)
                .DistinctBy(x => x.Id)
                .ToList();
            alst.AddRange(ialst);
            if (p.PreviousCNumber != null)
            {
                AddPreviousDocument(currentAsycudaDocumentSet, cdoc, p, alst);
            }

                await AttachToDocument(alst.GroupBy(x => new FileInfo(x.FilePath).Name).Select(x => x.Last()).ToList(),
                    cdoc.Document, cdoc.DocumentItems).ConfigureAwait(false);
        }

        if ((pod.EntryData is Adjustments a))
        {
            var itm = cdoc.DocumentItems.FirstOrDefault();
            if (itm != null && !itm.xcuda_Attached_documents.Any())
            {
                var file = new FileInfo(a.SourceFile);
                itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                {
                    Attached_document_code =
                        DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                            x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "IV05",
                    Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                    Attached_document_reference = file.Name,
                    xcuda_Attachments = new List<xcuda_Attachments>()
                    {
                        new xcuda_Attachments(true)
                        {
                            Attachments = new global::DocumentItemDS.Business.Entities.Attachments(true)
                            {
                                FilePath = Path.Combine(file.FullName + ".pdf"),
                                TrackingState = TrackingState.Added,
                                DocumentCode = DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                                    x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                        .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "IV05",
                                EmailId = a.EmailId,
                                Reference = file.Name,
                            },

                            TrackingState = TrackingState.Added
                        }
                    },
                    TrackingState = TrackingState.Added
                });

                itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                {
                    Attached_document_code =
                        DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                            x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "IV05",
                    Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                    Attached_document_reference = cdoc.Document.ReferenceNumber,
                    xcuda_Attachments = new List<xcuda_Attachments>()
                    {
                        new xcuda_Attachments(true)
                        {
                            Attachments = new global::DocumentItemDS.Business.Entities.Attachments(true)
                            {
                                FilePath =
                                    (DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.DataFolder == null
                                        ? cdoc.Document.ReferenceNumber + ".pdf"
                                        : $"{DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.DataFolder}\\{cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number}\\{cdoc.Document.ReferenceNumber}.pdf"),
                                TrackingState = TrackingState.Added,
                                DocumentCode = DataSpace.BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x =>
                                    x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                                        .Customs_Procedure.CustomsProcedure)?.AttachedDocumentCode ?? "IV05",
                                EmailId = a.EmailId,
                                Reference = file.Name,
                            },
                            TrackingState = TrackingState.Added
                        }
                    },
                    TrackingState = TrackingState.Added
                });
            }
        }
    }
}
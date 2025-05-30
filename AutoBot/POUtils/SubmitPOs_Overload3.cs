using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters; // Assuming ExportToCSV is here
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitPOInfo, AsycudaDocumentSetEx, AsycudaDocuments, AsycudaDocument_Attachments, Attachments, AttachmentLog are here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here (for Tuple in CurrentPOInfo call)
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here
using WaterNut.Business.Entities; // Assuming AssessedEntryInfo is here

namespace AutoBot
{
    public partial class POUtils
    {
        private static void SubmitPOs(AsycudaDocumentSetEx docSet, List<TODO_SubmitPOInfo> pOs, string[] contacts,
            string[] poContacts)
        {
            if (!pOs.Any())
            {
                // Assuming Utils.Client exists
                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Document Package for {docSet.Declarant_Reference_Number}",
                    contacts, "No Entries imported", Array.Empty<string>());

                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                    $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                    poContacts, "No Entries imported", Array.Empty<string>());
                return;
            }

            using (var ctx = new CoreEntitiesContext())
            {
                try
                {
                    //var pdfs = Enumerable
                    //    .Select<AsycudaDocumentSet_Attachments, string>(docSet.AsycudaDocumentSet_Attachments,
                    //        x => x.Attachments.FilePath)
                    //    .Where(x => x.ToLower().EndsWith(".pdf"))
                    //    .ToList();

                    var Assessedpdfs = new List<string>();

                    var pdfs = new List<string>();

                    var poInfo =
                        Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(
                            CurrentPOInfo(docSet.AsycudaDocumentSetId)); // Calls method in another partial class
                    if (!Directory.Exists(poInfo.Item2)) return;
                    foreach (var itm in pOs)
                    {
                        List<string> ndp = new List<string>();
                        var newEntry = ctx.AsycudaDocuments.FirstOrDefault(x =>
                            x.ReferenceNumber == itm.Reference &&
                            (x.ImportComplete == null || x.ImportComplete == false));
                        if (newEntry != null)
                        {
                            ndp = ctx.AsycudaDocument_Attachments
                                .Where(x => x.AsycudaDocumentId == newEntry.ASYCUDA_Id)
                                .GroupBy(x => x.Attachments.Reference)
                                .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                                .Select(x => x.Attachments.FilePath)
                                .Where(x => x.ToLower().EndsWith(".pdf"))
                                .ToList();
                        }


                        var adp = ctx.AsycudaDocument_Attachments
                            .Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                            .GroupBy(x => x.Attachments.Reference)
                            .Select(x => x.OrderByDescending(z => z.AttachmentId).FirstOrDefault())
                            .Select(x => x.Attachments.FilePath)
                            .Where(x => x.ToLower().EndsWith(".pdf"))
                            .ToList();

                        if (!adp.Any())
                        {
                            BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id }); // Assuming LinkPDFs exists
                            adp = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                                .Select(x => x.Attachments.FilePath).ToList();
                        }

                        pdfs.AddRange(ndp);
                        pdfs.AddRange(adp);
                        Assessedpdfs.AddRange(adp);
                    }

                    pdfs = pdfs.Distinct().ToList();
                    Assessedpdfs = Assessedpdfs.Distinct().ToList();

                    var body =
                        $"Please see attached documents entries for {docSet.Declarant_Reference_Number}.\r\n" +
                        $"\r\n" +
                        $"Please open the attached email to view Email Thread.\r\n" +
                        $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                        $"\r\n" +
                        $"Regards,\r\n" +
                        $"AutoBot";


                    var xRes = Enumerable.Select<TODO_SubmitPOInfo, AssessedEntryInfo>(pOs, x =>
                        new AssessedEntryInfo()
                        {
                            DocumentType = x.DocumentType,
                            CNumber = x.CNumber,
                            Reference = x.Reference,
                            Date = x.Date.ToString(),
                            PONumber = x.PONumber,
                            Invoice = x.SupplierInvoiceNo,
                            Taxes = x.Totals_taxes.GetValueOrDefault().ToString("C"),
                            CIF = x.Total_CIF.ToString("C"),
                            BillingLine = x.BillingLine
                        }).ToList();


                    var summaryFile = Path.Combine(poInfo.Item2, "Summary.csv");
                    if (File.Exists(summaryFile)) File.Delete(summaryFile);
                    var errRes =
                        new ExportToCSV<AssessedEntryInfo, List<AssessedEntryInfo>>
                        {
                            dataToPrint = xRes
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => errRes.SaveReport(summaryFile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    pdfs.Add(summaryFile);
                    Assessedpdfs.Add(summaryFile);


                    var emailIds = pOs.FirstOrDefault().EmailId;

                    if (emailIds == null)
                    {
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Document Package for {docSet.Declarant_Reference_Number}",
                            contacts, body, pdfs.ToArray());

                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}",
                            poContacts, body, Assessedpdfs.ToArray());
                    }
                    else
                    {
                        EmailDownloader.EmailDownloader.ForwardMsg(emailIds, Utils.Client,
                            $"Document Package for {docSet.Declarant_Reference_Number}", body, contacts,
                            pdfs.ToArray());

                        EmailDownloader.EmailDownloader.ForwardMsg(emailIds, Utils.Client,
                            $"Assessed Entries for {docSet.Declarant_Reference_Number}", body, poContacts,
                            Assessedpdfs.ToArray());
                    }

                    foreach (var item in pOs)
                    {
                        var sfile = Queryable.FirstOrDefault(ctx.AsycudaDocuments, x =>
                            x.ASYCUDA_Id == item.ASYCUDA_Id &&
                            x.ApplicationSettingsId == item.ApplicationSettingsId);
                        var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == sfile.SourceFileName); // Potential NullReferenceException if sfile is null
                        if (eAtt == null)
                        {
                            var att = ctx.Attachments.OrderByDescending(x => x.Id)
                                .FirstOrDefault(x => x.FilePath == sfile.SourceFileName); // Potential NullReferenceException if sfile is null
                            eAtt = new AsycudaDocumentSet_Attachments(true)
                            {
                                AsycudaDocumentSetId = item.AsycudaDocumentSetId,
                                AttachmentId = att.Id, // Potential NullReferenceException if att is null
                                DocumentSpecific = true,
                                FileDate = DateTime.Now,
                                EmailId = att.EmailId, // Potential NullReferenceException if att is null
                            };
                            ctx.AsycudaDocumentSet_Attachments.Add(eAtt);
                        }

                        ctx.AttachmentLog.Add(new AttachmentLog(true)
                        {
                            DocSetAttachment = eAtt.Id,
                            AsycudaDocumentSet_Attachments = eAtt,
                            Status = "Submit PO Entries",
                            TrackingState = TrackingState.Added
                        });
                    }

                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
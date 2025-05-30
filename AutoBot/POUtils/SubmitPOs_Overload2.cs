using System;
using System.Collections.Generic;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, Contacts, AsycudaDocumentSetExs, TODO_SubmitPOInfo, AsycudaDocuments are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static void SubmitPOs(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                Console.WriteLine("Submit POs");

                // var saleInfo = CurrentSalesInfo(); // Assuming CurrentSalesInfo exists if needed

                using (var ctx = new CoreEntitiesContext())
                {
                    var poList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();

                    var contacts = ctx.Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();

                    var poContacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Developer")
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).Distinct().ToArray();
                    var docSet = ctx.AsycudaDocumentSetExs.Include("AsycudaDocumentSet_Attachments.Attachments")
                        .FirstOrDefault(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);
                    if (docSet == null)
                    {
                        throw new ApplicationException($"Asycuda Document Set not Found: {ft.AsycudaDocumentSetId}");
                    }

                    var rlst = ctx.TODO_SubmitPOInfo
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number) || poList.Contains(x.CNumber))
                        .ToList();
                    List<TODO_SubmitPOInfo> lst;
                    if (rlst.Any())
                    {
                        lst = poList.Any()
                            ? rlst.Where(x => poList.Contains(x.CNumber)).ToList()
                            : rlst.Where(x => x.Reference.Contains(docSet.Declarant_Reference_Number)).ToList();

                    }
                    else
                    {
                        lst = ctx.AsycudaDocuments.Where(x =>
                                x.ReferenceNumber.Contains(docSet.Declarant_Reference_Number)
                                || poList.Contains(x.CNumber))
                            .Where(x => x.ImportComplete == true)
                            .Select(x => new
                            {
                                docSet.ApplicationSettingsId,
                                AssessedAsycuda_Id = x.ASYCUDA_Id,
                                x.CNumber
                            }).ToList()
                            .Select(x => new TODO_SubmitPOInfo()
                            {
                                ApplicationSettingsId = docSet.ApplicationSettingsId,
                                ASYCUDA_Id = x.AssessedAsycuda_Id,
                                CNumber = x.CNumber
                            }).ToList();
                    }

                    if (poList.Any())
                    {
                        lst = lst.Where(x => poList.Contains(x.CNumber))
                            .ToList();
                    }

                    // This calls the other SubmitPOs overload, which needs to be in its own partial class
                    SubmitPOs(docSet, lst, contacts, poContacts);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
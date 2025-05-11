using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocumentItemDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void SetInvoicePerline(List<int> docList)
    {
        try
        {
            using (var ctx = new DocumentItemDSContext { StartTracking = true })
            {
                foreach (var doc in docList)
                {
                    var mItms = ctx.xcuda_Item
                        .Include(x => x.xcuda_Attached_documents)
                        .Where(x => x.ASYCUDA_Id == doc &&
                                    x.xcuda_Attached_documents.Count(z => z.Attached_document_code == "IV05") > 1)
                        .SelectMany(x => x.xcuda_Attached_documents)
                        .Where(x => x.Attached_document_code == "IV05")
                        .ToList()
                        .GroupBy(x => x.Item_Id);

                    var sItms = ctx.xcuda_Item
                        .Include(x => x.xcuda_Attached_documents)
                        .Where(x => x.ASYCUDA_Id == doc
                        )
                        .ToList();


                    var i = 0;
                    foreach (var item in mItms)
                    foreach (var att in item)
                    {
                        if (i >= sItms.Count()) break;
                        var sitm = sItms[i];
                        att.Item_Id = sitm.Item_Id;

                        ctx.Database.ExecuteSqlCommand($@"update xcuda_Attached_documents
                                                                    set Item_Id = {sitm.Item_Id}
                                                                    where Attached_documents_Id = {
                                                                        att.Attached_documents_Id
                                                                    } ");
                        i += 1;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
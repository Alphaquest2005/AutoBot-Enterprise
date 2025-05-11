using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DocumentItemDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void RenameDuplicateDocumentCodes(List<int> docList)
    {
        try
        {
            var codeLst = new List<string> { "IV05", "IV02", "IV03", "IV06", "IV07", "IV08" };
            using (var ctx = new DocumentItemDSContext { StartTracking = true })
            {
                foreach (var doc in docList)
                {
                    var itms = ctx.xcuda_Item
                        .Include(x => x.xcuda_Attached_documents)
                        .Where(x => x.ASYCUDA_Id == doc &&
                                    x.xcuda_Attached_documents.Count(z => z.Attached_document_code == "IV05") > 1)
                        .SelectMany(x => x.xcuda_Attached_documents)
                        .Where(x => x.Attached_document_code == "IV05")
                        .ToList()
                        .GroupBy(x => x.Item_Id);
                    foreach (var item in itms)
                    {
                        var i = 0;

                        foreach (var att in item)
                        {
                            if (i == codeLst.Count() - 1) break;
                            att.Attached_document_code = codeLst[i];
                            i += 1;
                        }
                    }


                    ctx.SaveChanges();
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
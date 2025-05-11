using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot;

public partial class EX9Utils
{
    public static async Task relinkAllPreviousItems()
    {
        try
        {
            Console.WriteLine("ReLink All Previous Items");

            //////// all what i fucking try aint work just can't load the navigation properties fucking ef shit
            List<xcuda_ASYCUDA> docLst;
            List<xcuda_Identification> idlst;
            using (var ctx = new DocumentDSContext() { StartTracking = true })
            {
                idlst = ctx.xcuda_Identification
                    .Include(x => x.xcuda_Registration)
                    .Include(x => x.xcuda_Type)
                    .Where(x => x.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                        .Declaration_gen_procedure_code == "7")
                    .ToList();
            }

            using (var ctx = new DocumentDSContext() { StartTracking = true })
            {
                docLst = ctx.xcuda_ASYCUDA
                    //.Include(x => x.xcuda_Identification.xcuda_Registration)
                    //.Include(x => x.xcuda_Identification.xcuda_Type)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type
                        .Declaration_gen_procedure_code == "7")
                    .ToList();
            }

            List<xcuda_Tarification> itmCodes;
            using (var ctx = new DocumentItemDSContext() { StartTracking = true })
            {
                itmCodes = ctx.xcuda_Tarification
                    .Include(x => x.xcuda_HScode)
                    .ToList();
            }

            using (var ctx = new DocumentItemDSContext() { StartTracking = true })
            {
                foreach (var doc in docLst)
                {
                    ctx.Database.ExecuteSqlCommand($@"DELETE FROM EntryPreviousItems
                    FROM    EntryPreviousItems INNER JOIN
                    xcuda_Item ON EntryPreviousItems.Item_Id = xcuda_Item.Item_Id
                    WHERE(xcuda_Item.ASYCUDA_Id = {doc.ASYCUDA_Id})");

                    var itms = ctx.xcuda_Item
                        //.Include(x => x.xcuda_Tarification.xcuda_HScode)
                        .Where(x => x.ASYCUDA_Id == doc.ASYCUDA_Id)
                        .ToList();

                    doc.xcuda_Identification = idlst.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id);
                    foreach (var itm in itms)
                    {
                        itm.xcuda_Tarification = itmCodes.First(x => x.Item_Id == itm.Item_Id);
                    }

                    await BaseDataModel.Instance.LinkExistingPreviousItems(doc, itms, true).ConfigureAwait(false);
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
using System;
using System.Collections.Generic;
using System.Data.Entity; // For Include
using System.Linq;
using DocumentDS.Business.Entities; // Assuming DocumentDSContext, xcuda_ASYCUDA, xcuda_Identification, xcuda_Registration, xcuda_Type are here
using DocumentItemDS.Business.Entities; // Assuming DocumentItemDSContext, xcuda_Tarification, xcuda_HScode, xcuda_Item are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        // Note: This method seems complex and might benefit from refactoring.
        // It loads large lists into memory (idlst, docLst, itmCodes) which could be inefficient.
        // The commented-out includes suggest potential issues with navigation properties.
        public static void relinkAllPreviousItems()
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
                        .Where(x => x.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type // Potential NullReferenceExceptions
                            .Declaration_gen_procedure_code == "7")
                        .ToList();
                }

                using (var ctx = new DocumentDSContext() { StartTracking = true })
                {
                    docLst = ctx.xcuda_ASYCUDA
                        //.Include(x => x.xcuda_Identification.xcuda_Registration)
                        //.Include(x => x.xcuda_Identification.xcuda_Type)
                        .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type // Potential NullReferenceExceptions
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

                        doc.xcuda_Identification = idlst.First(x => x.ASYCUDA_Id == doc.ASYCUDA_Id); // InvalidOperationException if no match
                        foreach (var itm in itms)
                        {
                            itm.xcuda_Tarification = itmCodes.First(x => x.Item_Id == itm.Item_Id); // InvalidOperationException if no match
                        }

                        BaseDataModel.Instance.LinkExistingPreviousItems(doc, itms, true).Wait(); // Assuming LinkExistingPreviousItems exists
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
}
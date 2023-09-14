using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentItemDS.Business.Entities;
using WaterNut.Business.Entities;

namespace AllocationQS.Business.Services
{
    public class BondStressTest
    {
        public async Task<List<DocumentCT>> Execute(List<DocumentCT> documentCts)
        {
            AddErrorItm(documentCts);
            return documentCts;
        }

        private void AddErrorItm(List<DocumentCT> documentCts)
        {
            using (var ctx = new DocumentItemDSContext(){StartTracking = true})
            {
                foreach (var doc in documentCts)
                {
                    var itemId = doc.DocumentItems.Last().Item_Id;
                    var itm = ctx.xcuda_Item
                                        .Include(x => x.xcuda_Valuation_item.xcuda_Weight_itm)
                                        .Include(x => x.xcuda_PreviousItem)
                                        .Include(x => x.xcuda_Tarification.Unordered_xcuda_Supplementary_unit)
                                        .FirstOrDefault(x => x.Item_Id == itemId);
                    var pitm = itm?.xcuda_PreviousItem;
                    if (pitm == null) continue;

                    itm.Net_weight = pitm.Prev_net_weight;
                    itm.Gross_weight = itm.Net_weight;
                    itm.ItemQuantity = pitm.Preveious_suplementary_quantity - 1;
                    pitm.Suplementary_Quantity = (decimal)(pitm.Preveious_suplementary_quantity - 1);
                    pitm.Net_weight = pitm.Prev_net_weight;

                    ctx.SaveChanges();
                }
            }

        }
    }
}

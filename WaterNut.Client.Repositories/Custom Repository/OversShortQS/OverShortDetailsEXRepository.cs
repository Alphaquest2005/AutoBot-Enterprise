
//using Core.Common.Client.Repositories;
//using AdjustmentQS.Client.Entities;
//using AdjustmentQS.Client.DTO;


//using System.Threading.Tasks;
//using AdjustmentQS.Client.Services;
//using AsycudaDocumentItem = CoreEntities.Client.Entities.AsycudaDocumentItem;
//using OverShortDetailsEX = AdjustmentQS.Client.Entities.OverShortDetailsEX;

//namespace AdjustmentQS.Client.Repositories 
//{
   
//    public partial class OverShortDetailsEXRepository : BaseRepository<OverShortDetailsEXRepository>
//    {
//        public async Task MatchToCurrentItem(int currentDocumentItem, OverShortDetailsEX osd)
//        {
//            using (var t = new OverShortDetailsEXClient())
//            {
//                await t.MatchToCurrentItem(currentDocumentItem, osd.DTO).ConfigureAwait(false);
//            }
//        }

//        public async Task RemoveOverShortDetail(int osd)
//        {
//            using (var t = new OverShortDetailsEXClient())
//            {
//                await t.RemoveOverShortDetail(osd).ConfigureAwait(false);
//            }
//        }
        
//    }
//}


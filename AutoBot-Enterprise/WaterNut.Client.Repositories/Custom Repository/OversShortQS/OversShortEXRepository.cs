




//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CoreEntities.Client.Entities;
//using AdjustmentQS.Client.Entities;
//using AdjustmentQS.Client.Services;

//namespace AdjustmentQS.Client.Repositories 
//{

//    public partial class OversShortEXRepository
//    {

//        public async Task Import(string fileName, string fileType, int docSetId, bool overWriteExisting)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.Import(fileName, fileType, docSetId, overWriteExisting).ConfigureAwait(false);
//            }
//        }

//        public async Task SaveReferenceNumber(IEnumerable<int> slst, string refTxt)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.SaveReferenceNumber(slst, refTxt).ConfigureAwait(false);
//            }

//        }

//        public async Task SaveCNumber(IEnumerable<int> slst, string cntxt)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.SaveCNumber(slst, cntxt).ConfigureAwait(false);
//            }
//        }

//        public async Task MatchEntries(IEnumerable<int> olst)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.MatchEntries(olst).ConfigureAwait(false);
//            }
//        }

//        public async Task RemoveSelectedOverShorts(IEnumerable<int> lst)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.RemoveSelectedOverShorts(lst).ConfigureAwait(false);
//            }
//        }

//        public async Task AutoMatch(IEnumerable<int> slst)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.AutoMatch(slst).ConfigureAwait(false);
//            }
//        }

//        public async Task CreateOversOps(IEnumerable<int> selOS, int docSetId)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.CreateOversOps(selOS, docSetId).ConfigureAwait(false);
//            }
//        }

//        public async Task CreateShortsEx9(IEnumerable<int> selos, int docSetId,
//            bool BreakOnMonthYear, bool ApplyEX9Bucket)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                await t.CreateShortsEx9(selos, docSetId,BreakOnMonthYear, ApplyEX9Bucket).ConfigureAwait(false);
//            }
//        }

//        public async Task<StringBuilder> BuildOSLst(List<int> lst)
//        {
//            using (var t = new OversShortEXClient())
//            {
//                return await t.BuildOSLst(lst).ConfigureAwait(false);
//            }
//        }
//    }
//}


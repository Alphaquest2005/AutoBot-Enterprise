

//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using CoreEntities.Business.Entities;
//using AdjustmentQS.Business.Entities;

//namespace AdjustmentQS.Business.Services
//{


//    public partial class OversShortEXService
//    {
//        public async Task Import(string fileName, string fileType, int docSetId, bool overWriteExisting)
//        {
//            var docSet =
//                await
//                    WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId, null)
//                        .ConfigureAwait(false);
//            await
//                OverShortModelDS.Instance.Import(fileName, fileType, docSet, overWriteExisting)
//                    .ConfigureAwait(false);
//        }

//        public async Task SaveReferenceNumber(IEnumerable<int> slst, string refTxt)
//        {
//            await OverShortModelDS.Instance.SaveReferenceNumber(slst, refTxt).ConfigureAwait(false);
//        }

//        public async Task SaveCNumber(IEnumerable<int> slst, string cntxt)
//        {
//            await OverShortModelDS.Instance.SaveCNumber(slst, cntxt).ConfigureAwait(false);
//        }

//        public async Task MatchEntries(IEnumerable<int> olst)
//        {
//            await OverShortModelDS.Instance.MatchEntries(olst).ConfigureAwait(false);
//        }

//        public async Task RemoveSelectedOverShorts(IEnumerable<int> lst)
//        {
//            await OverShortModelDS.Instance.RemoveSelectedOverShorts(lst).ConfigureAwait(false);
//        }

//        public async Task AutoMatch(IEnumerable<int> slst)
//        {
//            await OverShortModelDS.Instance.AutoMatch(slst).ConfigureAwait(false);
//        }

//        public async Task CreateOversOps(IEnumerable<int> selOS, int docSetId)
//        {
//            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId, null).ConfigureAwait(false);
//            var slst = await OverShortModelDS.Instance.GetOverShortEx(selOS).ConfigureAwait(false);
//            await OverShortModelDS.Instance.CreateOversOps(slst, docSet).ConfigureAwait(false);
//        }

//        public async Task CreateShortsEx9(IEnumerable<int> selos, int docSetId, bool BreakOnMonthYear, bool ApplyEX9Bucket)
//        {
//            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId, null).ConfigureAwait(false);
//            var slst = await OverShortModelDS.Instance.GetOverShortEx(selos).ConfigureAwait(false);
//            await OverShortModelDS.Instance.CreateShortsEx9(slst, docSet, BreakOnMonthYear, ApplyEX9Bucket).ConfigureAwait(false);
//        }

//        public async Task<StringBuilder> BuildOSLst(List<int> lst)
//        {
//            return await OverShortModelDS.Instance.BuildOSLst(lst).ConfigureAwait(false);
//        }
//    }
//}




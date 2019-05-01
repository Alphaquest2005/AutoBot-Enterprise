
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.ServiceModel;
//using System.Text;
//using System.Threading.Tasks;
//using CoreEntities.Client.DTO;
//using AdjustmentQS.Client.DTO;
//using AdjustmentQS.Client.Contracts;
//using Core.Common.Client.Services;

//using Core.Common.Contracts;
//using System.ComponentModel.Composition;


//namespace AdjustmentQS.Client.Services
//{

//    public partial class OversShortEXClient
//    {
//        public async Task Import(string fileName, string fileType, int docSetId, bool overWriteExisting)
//        {
//            await Channel.Import(fileName, fileType,docSetId, overWriteExisting).ConfigureAwait(false);
//        }

//        public async Task SaveReferenceNumber(IEnumerable<int> slst, string refTxt)
//        {
//            await Channel.SaveReferenceNumber(slst, refTxt).ConfigureAwait(false);
//        }

//        public async Task SaveCNumber(IEnumerable<int> slst, string cntxt)
//        {
//            await Channel.SaveCNumber(slst, cntxt).ConfigureAwait(false);
//        }

//        public async Task MatchEntries(IEnumerable<int> olst)
//        {
//            await Channel.MatchEntries(olst).ConfigureAwait(false);
//        }

//        public async Task RemoveSelectedOverShorts(IEnumerable<int> lst)
//        {
//            await Channel.RemoveSelectedOverShorts(lst).ConfigureAwait(false);
//        }

//        public async Task AutoMatch(IEnumerable<int> slst)
//        {
//            await Channel.AutoMatch(slst).ConfigureAwait(false);
//        }

//        public async Task CreateOversOps(IEnumerable<int> selOS, int docSetId)
//        {
//            await Channel.CreateOversOps(selOS, docSetId).ConfigureAwait(false);
//        }

//        public async Task CreateShortsEx9(IEnumerable<int> selos, int docSetId,
//            bool applyCurrentChecks, bool process7100)
//        {
//            await Channel.CreateShortsEx9(selos, docSetId, applyCurrentChecks, process7100).ConfigureAwait(false);
//        }

//        public async Task<StringBuilder> BuildOSLst(List<int> lst)
//        {
//            return await Channel.BuildOSLst(lst).ConfigureAwait(false);
//        }
//    }
//}


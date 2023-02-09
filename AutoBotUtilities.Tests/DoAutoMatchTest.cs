using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using NUnit.Framework;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DoAutoMatchTest
    {
        [SetUp]
        public void SetUp()
        {
            
                Infrastructure.Utils.ClearDataBase();

                ImportTestFile();
                ImportTestIM7();
            
            Infrastructure.Utils.SetTestApplicationSettings(2);
            return;

        }

        [Test]
        public async Task CanDoAutoMatch()
        {

            if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
            
            var lst = new AdjustmentQSContext().AdjustmentDetails
                .Where(x => x.ItemNumber == @"DAI/JB4U80-3000IB" || x.ItemNumber == "DAI/JB4U80-300IB")
                .ToList();


            AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.ItemNumber}'").Aggregate((o, n) => $"{o},{n}")).Wait();

            AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);

            var entryDataDetailsIdLst = lst.Select(x => x.EntryDataDetailsId).ToList();


            var elapsed = Infrastructure.Utils.Time(async () => await AutoMatchProcessor.DoAutoMatch(2, lst).ConfigureAwait(false));


            var res = new AdjustmentQSContext().EntryDataDetails
                .Where(x => entryDataDetailsIdLst.Contains((int)x.EntryDataDetailsId))
                .Count(x => x.EffectiveDate != null);

            var ares = new AdjustmentQSContext().AsycudaSalesAllocations
                .Where(x => entryDataDetailsIdLst.Contains((int)x.EntryDataDetailsId))
                .Count(x => x.EntryDataDetail.EffectiveDate != null);

            Assert.Multiple(() =>
            {

                Assert.IsTrue(res == 2 && ares == 1);
                Assert.That(elapsed, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(1)));
            });
           
        }

        [Test]
        public async Task CanDoAutoMatchAllPerformance()
        {

            if (Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
            List<AdjustmentDetail> lst = null;
            var elapsedGetAdjustmentDetails = Infrastructure.Utils.Time(() => lst = new AdjustmentQSContext().AdjustmentDetails.Take(1000).ToList());


            var elapsedClearDocSetAllocations = Infrastructure.Utils.Time(() => AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.ItemNumber}'").Aggregate((o, n) => $"{o},{n}")).ConfigureAwait(false));

            var elapsedPrepareDataForAllocation = Infrastructure.Utils.Time(() => AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings));


            var elapsedAutoMatch = Infrastructure.Utils.Time(async () => await AutoMatchProcessor.DoAutoMatch(2, lst).ConfigureAwait(false));


         

            Assert.Multiple(() =>
            {
                Assert.That(elapsedGetAdjustmentDetails, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(elapsedClearDocSetAllocations, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(elapsedPrepareDataForAllocation, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(7)));
                Assert.That(elapsedAutoMatch, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(60)));
            });

        }

    




        private static void ImportTestIM7()
        {
            if (!Infrastructure.Utils.IsTestApplicationSettings()) return;

            var im7File = Infrastructure.Utils.GetTestSalesFile(new List<string>()
                { "Discrepancy", "IM7-GDSGO-19810.xml" });

            var docSet = WaterNut.DataSpace.EntryDocSetUtils.GetDocSet("Imports");
            Infrastructure.Utils.ImportDocuments(docSet, new List<string>() { im7File });
        }

        private static void ImportTestFile()
        {
            if (!Infrastructure.Utils.IsTestApplicationSettings()) return;

            var disFile = Infrastructure.Utils.GetTestSalesFile(new List<string>()
                { "Discrepancy", "CALA MARINE - Customs-test.csv" });

            Infrastructure.Utils.ImportEntryDataOldWay(new List<string>() { disFile }, FileTypeManager.EntryTypes.Unknown,
                FileTypeManager.FileFormats.Csv);
        }
    }
}
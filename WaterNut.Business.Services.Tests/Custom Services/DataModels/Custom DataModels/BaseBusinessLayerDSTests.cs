using AutoBot;

namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BaseDataModelTests
    {
        private BaseDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new BaseDataModel();
        }

        [Test]
        public void CanCallRenameDuplicateDocuments()
        {
            try
            {
                var docSet = EntryDocSetUtils.GetLatestDocSet();
                BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }

        }

        [Test]
        public void CanCallImportDocuments()
        {
            try
            {
                DocumentDS.Business.Entities.AsycudaDocumentSet docSet = new DocumentDS.Business.Entities.AsycudaDocumentSet()
                {
                    ApplicationSettingsId = 6,
                    AsycudaDocumentSetEntryDatas = new System.Collections.Generic.List<DocumentDS.Business.Entities.AsycudaDocumentSetEntryData>() { },
                    AsycudaDocumentSetId = 1,
                    AsycudaDocumentSet_Attachments = new System.Collections.Generic.List<DocumentDS.Business.Entities.AsycudaDocumentSet_Attachments>() { },
                    Country_of_origin_code = "US",
                    Currency_Code = "USD",
                    Customs_Procedure = new DocumentDS.Business.Entities.Customs_Procedure()
                    {
                    },
                    Customs_ProcedureId = 132,
                    Declarant_Reference_Number = "Imports",
                    Document_Type = new DocumentDS.Business.Entities.Document_Type()
                    {
                    },
                    Document_TypeId = 1,
                    Exchange_Rate = 0,
                    ExpectedEntries = null,
                    FileTypes = new System.Collections.Generic.List<DocumentDS.Business.Entities.FileType>() { },
                    FreightCurrencyCode = "USD",
                    LastFileNumber = 305,
                    LocationOfGoods = null,
                    Manifest_Number = null,
                    MaxLines = null,
                    ModifiedProperties = null,
                    Office = null,
                    StartingFileCount = null,
                    SystemDocumentSet = null,
                    TotalFreight = 0,
                    TotalInvoices = null,
                    TotalPackages = null,
                    TotalWeight = 0,
                    TrackingState = TrackableEntities.TrackingState.Unchanged,
                    xcuda_ASYCUDA_ExtendedProperties = new System.Collections.Generic.List<DocumentDS.Business.Entities.xcuda_ASYCUDA_ExtendedProperties>() { }
                };

                System.Collections.Generic.List<string> fileNames = new System.Collections.Generic.List<string>()
{
    @"C:\Users\josep\OneDrive\Clients\Columbian\Emails\Imports\IM4-GDSGO-17482.xml"
};

                bool importOnlyRegisteredDocument = true;

                bool importTariffCodes = true;

                bool noMessages = false;

                bool overwriteExisting = true;

                bool linkPi = true;

                BaseDataModel.Instance.ImportDocuments(docSet, fileNames, importOnlyRegisteredDocument,
                    importTariffCodes, noMessages, overwriteExisting, linkPi).Wait();

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }

        }

    }
}
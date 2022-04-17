namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class DISUtilsTests
    {
        private DISUtils _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new DISUtils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new DISUtils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public void CanCallAllocateDocSetDiscrepancies()
        {
            var fileType = new FileTypes()
            {
                ApplicationSettings = null,
                ApplicationSettingsId = 2,
                AsycudaDocumentSetEx = new AsycudaDocumentSetEx()
                {
                    ApplicationSettings = null,
                    ApplicationSettingsId = 2,
                    ApportionMethod = null,
                    AsycudaDocumentSetEntryDataEx = new List<AsycudaDocumentSetEntryDataEx>() { },
                    AsycudaDocumentSetId = 8,
                    AsycudaDocumentSet_Attachments = new List<AsycudaDocumentSet_Attachments>() { },
                    AsycudaDocuments = new List<AsycudaDocument>() { },
                    BLNumber = null,
                    ClassifiedLines = 4597,
                    Country_of_origin_code = "GD",
                    CurrencyRate = 2.7169,
                    Currency_Code = "USD",
                    Customs_ProcedureId = 1,
                    Declarant_Reference_Number = "Discrepancies",
                    Description = null,
                    Document_TypeId = 1,
                    DocumentsCount = 0,
                    EntityId = "8",
                    EntityName = null,
                    EntryPackages = null,
                    EntryTimeStamp = null,
                    Exchange_Rate = 0,
                    ExpectedEntries = null,
                    FileTypes = new List<FileTypes>()
                    {

                    },
                    FreightCurrencyCode = "USD",
                    FreightCurrencyRate = 2.7169,
                    ImportedInvoices = 552,
                    InvoiceTotal = 73917.069588789251,
                    LastFileNumber = 54,
                    LicenceSummary = new List<LicenceSummary>() { },
                    LicenseLines = 176,
                    LocationOfGoods = null,
                    Manifest_Number = null,
                    MaxLines = null,
                    QtyLicensesRequired = 51,
                    StartingFileCount = null,
                    TotalCIF = null,
                    TotalFreight = 0,
                    TotalInvoices = null,
                    TotalLines = 4603,
                    TotalPackages = null,
                    TotalWeight = null,

                },
                AsycudaDocumentSetId = 8064,
                AsycudaDocumentSet_Attachments = new List<AsycudaDocumentSet_Attachments>() { },
                ChildFileTypes = new List<FileTypes>()
                {
                    new FileTypes()
                    {
                        ApplicationSettings = null,
                        ApplicationSettingsId = 2,
                        AsycudaDocumentSetEx = new AsycudaDocumentSetEx()
                        {
                        },
                        AsycudaDocumentSetId = 8064,
                        AsycudaDocumentSet_Attachments = new List<AsycudaDocumentSet_Attachments>() { },
                        ChildFileTypes = new List<FileTypes>() { },
                        CopyEntryData = true,
                        CreateDocumentSet = true,
                        Data = new List<KeyValuePair<string, string>>() { },
                        DocReference = null,
                        DocumentCode = "IV05",
                        DocumentSpecific = false,
                        EmailFileTypes = new List<EmailFileTypes>() { },
                        EmailId = "Star Brite Shipment Discrepancy--2022-03-28-20:17:59",
                        EmailInfoMappings = new List<EmailInfoMappings>() { },
                        EntityId = "33",
                        EntityName = null,
                        FileGroupId = 2,
                        FileGroups = null,
                        FilePath = null,
                        FilePattern = ".*-Fixed.csv",
                        FileTypeActions = new List<FileTypeActions>() { },
                        FileTypeContacts = new List<FileTypeContacts>() { },
                        FileTypeMappings = new List<FileTypeMappings>() { },
                        FileTypeReplaceRegex = new List<FileTypeReplaceRegex>() { },
                        HasFiles = null,
                        Id = 33,
                        ImportActions = new List<ImportActions>() { },
                        IsImportable = null,
                        MaxFileSizeInMB = 4,
                        MergeEmails = false,
                        OldFileTypeId = null,
                        OverwriteFiles = null,
                        ParentFileTypeId = 32,
                        ParentFileTypes = new FileTypes()
                        {
                        },
                        ProcessNextStep = new List<string>() { },
                        ReplicateHeaderRow = null,
                        ReplyToMail = false,
                        Type = "DIS",

                    }
                },
                CopyEntryData = false,
                CreateDocumentSet = true,
                Data = new List<KeyValuePair<string, string>>()
                    { new KeyValuePair<string, string>("AsycudaDocumentSetId", "8064") },
                DocReference = null,
                DocumentCode = "IV05",
                DocumentSpecific = true,
                EmailFileTypes = new List<EmailFileTypes>() { },
                EmailId = "Star Brite Shipment Discrepancy--2022-03-28-20:17:59",
                EmailInfoMappings = new List<EmailInfoMappings>() { },
                EntityId = "32",
                EntityName = null,
                FileGroupId = 2,
                FileGroups = null,
                FilePath = null,
                FilePattern = "(.*(?<!UnAttachedSummary)).xlsx",
                FileTypeActions = new List<FileTypeActions>()
                {
                    new FileTypeActions()
                    {
                        ActionId = 20,
                        Actions = new Actions()
                        {
                        },
                        AssessEX = null,
                        AssessIM7 = null,

                        EntityId = "1318",
                        EntityName = null,
                        FileTypeId = 32,
                        FileTypes = new FileTypes()
                        {
                        },
                        Id = 1318,

                    },
                    new FileTypeActions()
                    {
                        ActionId = 71,
                        Actions = new Actions()
                        {
                        },
                        AssessEX = null,
                        AssessIM7 = null,
                    },

                },
                FileTypeReplaceRegex = new List<FileTypeReplaceRegex>() { },
                HasFiles = null,
                Id = 32,
                ImportActions = new List<ImportActions>() { },
                IsImportable = null,
                MaxFileSizeInMB = 4,
                MergeEmails = false,
                OldFileTypeId = null,
                OverwriteFiles = true,
                ParentFileTypeId = null,
                ParentFileTypes = null,
                ProcessNextStep = new List<string>() { },
                ReplicateHeaderRow = null,
                ReplyToMail = false,
                Type = "XLSX",

            };



            try
            {

                DISUtils.AllocateDocSetDiscrepancies(fileType);
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
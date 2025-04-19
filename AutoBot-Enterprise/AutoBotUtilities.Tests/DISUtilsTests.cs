using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using EntryDocSetUtils = WaterNut.DataSpace.EntryDocSetUtils;

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
        private readonly DoAutoMatchTest _doAutoMatchTest = new DoAutoMatchTest();

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.ClearDataBase();
            Infrastructure.Utils.SetTestApplicationSettings(2);
            _testClass = new DISUtils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new DISUtils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public void CanCallGetSubmitEntryData()
        {
            var fileType = GetFileType();


            try
            {

                DISUtils.GetSubmitEntryData(fileType);
                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        private static FileTypes GetFileType()
        {
            var fileType = new FileTypes()
            {
                ApplicationSettings = null,
                ApplicationSettingsId = 2,
               
                AsycudaDocumentSetId = 8064,
                AsycudaDocumentSet_Attachments = new List<AsycudaDocumentSet_Attachments>() { },
                ChildFileTypes = new List<FileTypes>()
                {
                    new FileTypes()
                    {
                        ApplicationSettings = null,
                        ApplicationSettingsId = 2,
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
                        FileImporterInfos = new FileImporterInfo(){EntryType =  FileTypeManager.EntryTypes.Dis},
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
                FileImporterInfos = new FileImporterInfo() { EntryType = FileTypeManager.FileFormats.Xlsx },
                
            };
            return fileType;
        }
    }
}
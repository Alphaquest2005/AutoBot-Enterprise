namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using DocumentDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class SaveCSVModelTests
    {
        private SaveCSVModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SaveCSVModel();
        }

        [Test]
        public async Task CanCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndDocSetAndOverWriteExisting()
        {
            var droppedFilePath = "TestValue1069455611";
            var fileType = new FileTypes();
            var docSet = new List<AsycudaDocumentSet>();
            var overWriteExisting = false;
            await _testClass.ProcessDroppedFile(droppedFilePath, fileType, docSet, overWriteExisting);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndDocSetAndOverWriteExistingWithNullFileType()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessDroppedFile("TestValue722765619", default(FileTypes), new List<AsycudaDocumentSet>(), false));
        }

        [Test]
        public void CannotCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndDocSetAndOverWriteExistingWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessDroppedFile("TestValue1390348351", new FileTypes(), default(List<AsycudaDocumentSet>), true));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndDocSetAndOverWriteExistingWithInvalidDroppedFilePath(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessDroppedFile(value, new FileTypes(), new List<AsycudaDocumentSet>(), false));
        }

        [Test]
        public async Task CanCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndOverWriteExisting()
        {
            var droppedFilePath = "TestValue768278973";
            var fileType = new FileTypes();
            var overWriteExisting = false;
            await _testClass.ProcessDroppedFile(droppedFilePath, fileType, overWriteExisting);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndOverWriteExistingWithNullFileType()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessDroppedFile("TestValue745274893", default(FileTypes), false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallProcessDroppedFileWithDroppedFilePathAndFileTypeAndOverWriteExistingWithInvalidDroppedFilePath(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessDroppedFile(value, new FileTypes(), false));
        }

        [Test]
        public void CanCallGetDocSets()
        {
            var fileType = new FileTypes();
            var result = _testClass.GetDocSets(fileType);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetDocSetsWithNullFileType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetDocSets(default(FileTypes)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(SaveCSVModel.Instance, Is.InstanceOf<SaveCSVModel>());
            Assert.Fail("Create or modify test");
        }
    }
}
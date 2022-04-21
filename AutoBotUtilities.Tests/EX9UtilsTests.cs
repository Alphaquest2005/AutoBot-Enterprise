using System.IO;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class EX9UtilsTests
    {
        private EX9Utils _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EX9Utils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EX9Utils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public void CanGetXSalesFileType()
        {
            var type = "XSales";
            var fileType = Utils.GetFileType(type);
            Assert.AreEqual(type, fileType.Type);
        }

        [Test]
        public void CanImportXSalesFile()
        {
            try
            {
                var testFile = getTestSalesFile();
                EX9Utils.ImportXSalesFiles(testFile);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        private string getTestSalesFile()
        {
            var testDirectory = GetTestDirectory();
            var testSalesFile = GetTestSalesFile(testDirectory, "TestXSalesFile.csv");
            return testSalesFile;
        }

        private static string GetTestSalesFile(string testDirectory, string testXSalesfile)
        {
            var testSalesFile = Path.Combine(testDirectory, testXSalesfile);
            if (!File.Exists(testSalesFile)) throw new ApplicationException($"TestFile Dose not Exists: '{testSalesFile}'");
            return testSalesFile;
        }

        private static string GetTestDirectory()
        {

            var testDirectory = GetDirectory(new List<string>(){"Imports", "Test Folder"});
            

            return testDirectory;
        }

        private static void EnsureDirectoryExists(string testDirectory)
        {
            if (!Directory.Exists(testDirectory)) Directory.CreateDirectory(testDirectory);
        }

        private static string GetDirectory(List<string> folderPath)
        {
            folderPath.Insert(0,BaseDataModel.Instance.CurrentApplicationSettings.DataFolder);
            var directory = Path.Combine(folderPath.ToArray());
            EnsureDirectoryExists(directory);
            return directory;
        }
    }
}
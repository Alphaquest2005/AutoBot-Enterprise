using Xunit;
using WaterNut.DataSpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WaterNut.DataSpace.Tests
{
    public class InvoiceReaderTests
    {
 
        [Fact()]
        public void ImportTest()
        {
            var testCasesList = GetTestCasesList();

            foreach (var testCase in testCasesList)
            {
                var pdfTxt = File.ReadAllText(Path.Combine(testCase.PdfFile, txtFileExtension));
                var expectedRes = new List<dynamic>();
                string supplier = testCase.Supplier;
                var tmp = InvoiceReader.GetTemplates(x => x.Name == supplier).First();
                var res = tmp.Read(tmp.Format(pdfTxt));
                Assert.Equal(expectedRes, res);

            }
        }
        private static readonly string _unitTestFolder = "UnitTests";
        const string txtFileExtension = ".txt";

        private static IEnumerable<dynamic> GetTestCasesList()
        {
            var testCasesPaths
                = GetCurrentTestCasesPaths();
            foreach (var log in testCasesPaths)
            {
                yield return GetTestCasesList(log.LogPath);
            }
           
        }

        private static List<dynamic> GetTestCasesList(string path)
        {
            var file = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, _unitTestFolder,
                path, "UnitTests.yaml");
            var deserializer = GetYamlDeserializer(file, out string yaml);
            return deserializer.Deserialize<List<dynamic>>(yaml);
        }

        private static List<UnitTestLogger.Logs> GetCurrentTestCasesPaths()
        {
            
            var fileName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, _unitTestFolder,
                "InvoiceReader", "UnitTests.yaml");
            var deserializer = GetYamlDeserializer(fileName, out var yaml);
            //yml contains a string containing your YAML
            return deserializer.Deserialize<List<UnitTestLogger.Logs>>(yaml);
        }

        private static IDeserializer GetYamlDeserializer(string fileName, out string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // see height_in_inches in sample yml 
                .Build();
            yaml = File.ReadAllText(fileName);
            return deserializer;
        }
    }


}
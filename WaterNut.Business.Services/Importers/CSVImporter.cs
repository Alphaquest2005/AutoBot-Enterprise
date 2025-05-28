using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.CSV;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers.EntryData;
using WaterNut.Business.Services.Utils;
using System.Threading.Tasks;
using Serilog; // Added Serilog using

namespace WaterNut.Business.Services.Importers
{
    public class CSVImporter : IImporter
    {
        public FileTypes FileType { get; private set; }
        private readonly ILogger _logger; // Added private ILogger field

        public CSVImporter(FileTypes fileType, ILogger logger) // Added ILogger parameter to constructor
        {
            FileType = fileType;
            _logger = logger; // Assign logger to private field
        }

        public async Task Import(string fileName, bool overWrite, ILogger log) // Removed ILogger parameter
        {
            try
            {
                var docSet = await DataSpace.Utils.GetDocSets(FileType, _logger).ConfigureAwait(false); // Use private logger field
                var lines = GetFileLines(fileName);
                var header = GetHeadings(lines);
                var emailId = DataSpace.Utils.GetExistingEmailId(fileName, FileType);

                var fileType = FileTypeManager.GetHeadingFileType(header, FileType);

                var data = new CSVDataExtractor(fileType, lines, header, emailId)
                    //.Then(new EntryDataExtractor(fileType, docSet, emailId,fileName))
                    .Execute();

                fileType.EmailId = emailId;
                var importSettings = new ImportSettings(fileType, docSet, overWrite, fileName, emailId);

                await EntryDataManager.CSVDocumentProcessors(importSettings)[fileType.FileImporterInfos.EntryType]
                    .Execute(data, log).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }



        public IEnumerable<string> GetFileLines(string droppedFilePath)
        {
            var rawFileTxt = GetRawFileText(droppedFilePath);

            var fixedFileTxt = ApplyFileTypeTextReplacements(FileType, rawFileTxt);


            var lines = GetLinesFromText(fixedFileTxt);
            return lines;
        }

        private static IEnumerable<string> GetLinesFromText(string fixedFileTxt) => new StringTextSplitter(new string[] { "\n", "\r\n" }).Execute(fixedFileTxt);

        private static string ApplyFileTypeTextReplacements(FileTypes fileType, string rawFileTxt)
        {
            var pTxt = rawFileTxt;
            foreach (var r in fileType.FileTypeReplaceRegex)
            {
                var reg = new Regex(r.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                pTxt = reg.Replace(pTxt, r.ReplacementRegEx, Int16.MaxValue);
            }

            return pTxt;
        }

        private static string GetRawFileText(string droppedFilePath) => File.ReadAllText(droppedFilePath).Replace("", " ");

        public  IEnumerable<string> GetHeadings(IEnumerable<string> lines)
        {
            var headerline = lines.FirstOrDefault();

            if (headerline == null) return Array.Empty<string>();

            var headings = headerline.CsvSplit();

            return RemoveCarriageReturnFromHeadings(headings);
        }

        private string[] RemoveCarriageReturnFromHeadings(string[] headings)
        {
            return headings.Select(x => x.EndsWith("\r\n") ? x.Replace("\r\n", "") : x).ToArray();

        }




    }
}
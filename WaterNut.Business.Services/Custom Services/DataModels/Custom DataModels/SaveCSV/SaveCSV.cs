using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.CSV;
using System.IO;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using MoreLinq.Extensions;


namespace WaterNut.DataSpace
{
    public partial class SaveCSVModel
    {
        private static readonly SaveCSVModel instance;
        static SaveCSVModel()
        {
            instance = new SaveCSVModel();
        }

        public static SaveCSVModel Instance
        {
            get { return instance; }
        }
        internal  async Task GetFile(string filetype)
        {

            ////import asycuda xml id and details
            //var od = new OpenFileDialog();
            //od.Title = "Import Sales";
            //od.DefaultExt = ".csv";
            //od.Filter = "CSV Files (.csv)|*.csv";
            //od.Multiselect = true;
            //var result = od.ShowDialog();
            //if (result == true)
            //{
            //    foreach (var f in od.FileNames)
            //    {
                   // await ProcessDroppedFile(f, filetype).ConfigureAwait(false);
            //    }
            //}
            //MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting)
        {
            try
            {
                
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }
        public  async Task ProcessDroppedFile(string droppedFilePath, FileTypes fileType, bool overWriteExisting)
        {
            try
            {
                var docSet = GetDocSets(fileType);
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }

        public List<AsycudaDocumentSet> GetDocSets(FileTypes fileType)
        {
            List<AsycudaDocumentSet> docSet;
            using (var ctx = new DocumentDSContext())
            {
                docSet = new List<AsycudaDocumentSet>()
                {
                    ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                };
                var ddocset = ctx.FileTypes.First(x => x.Id == fileType.Id).AsycudaDocumentSetId;
                if (fileType.CopyEntryData)
                    docSet.Add(ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == ddocset));
                else
                {
                    if (ctx.SystemDocumentSets.FirstOrDefault(x => x.Id == ddocset) != null)
                    {
                        docSet.Clear();
                        docSet.Add(ctx.AsycudaDocumentSets.Include(x => x.SystemDocumentSet).FirstOrDefault(x => x.AsycudaDocumentSetId == ddocset));

                    }
                    
                }
                if (!docSet.Any()) throw new ApplicationException("Document Set with reference not found");
            }

            return docSet;
        }

        private async Task SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting)
        {
            var emailId = GetExistingEmailId(droppedFilePath, fileType);

            var lines = GetFileLines(droppedFilePath, fileType);
           
            var fixedHeadings = GetHeadings(lines);

            if (fileType.Type == "SI")
            {
                await SaveCsvSubItems.Instance.ExtractSubItems(fileType.Type, lines, fixedHeadings)
                    .ConfigureAwait(false);
            }
            else
            {
                await SaveCsvEntryData.Instance
                    .ExtractEntryData(fileType, lines, fixedHeadings, docSet, overWriteExisting, emailId,
                        droppedFilePath).ConfigureAwait(false);
            }

        }

        private static string[] GetFileLines(string droppedFilePath, FileTypes fileType)
        {
            var rawFileTxt = GetRawFileText(droppedFilePath);

            var fixedFileTxt = ApplyFileTypeTextReplacements(fileType, rawFileTxt);


            var lines = GetLinesFromText(fixedFileTxt);
            return lines;
        }

        private string[] GetHeadings(string[] lines)
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

        private static string[] GetLinesFromText(string fixedFileTxt)
        {
            return fixedFileTxt.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

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

        private static string GetRawFileText(string droppedFilePath)
        {
            return File.ReadAllText(droppedFilePath).Replace("�", " ");
        }

        private static string GetExistingEmailId(string droppedFilePath, FileTypes fileType)
        {
            string emailId = null;

            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.AsycudaDocumentSet_Attachments.Where(x =>
                        x.Attachments.FilePath.Replace(".xlsx", "-Fixed.csv") == droppedFilePath
                        || x.Attachments.FilePath == droppedFilePath)
                    .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                emailId = res?.EmailId ?? (fileType.EmailId == "0" || fileType.EmailId == null ? null : fileType.EmailId);
            }

            return emailId;
        }
    }
}

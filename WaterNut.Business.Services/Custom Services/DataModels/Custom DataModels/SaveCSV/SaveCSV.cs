using System;
using System.Collections.Generic;
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
                using (var ctx = new DocumentDSContext())
                {
                    var docSet = new List<AsycudaDocumentSet>() {
                        ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)};
                    var ddocset = ctx.FileTypes.First(x => x.Id == fileType.Id).AsycudaDocumentSetId;
                    if (fileType.CopyEntryData) docSet.Add(ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == ddocset));
                    if (!docSet.Any()) throw new ApplicationException("Document Set with reference not found");
                    await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }

        private  async Task SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting)
        {
            try
            {
                int? emailId = 0;
                int? fileTypeId = 0;
                using (var ctx = new CoreEntitiesContext())
                {
                    
                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath.Replace(".xlsx", "-Fixed.csv") == droppedFilePath 
                                                                            || x.Attachments.FilePath == droppedFilePath) 
                        .Select(x => new{x.EmailUniqueId, x.FileTypeId}).FirstOrDefault();
                    emailId = res?.EmailUniqueId;
                    fileTypeId = res?.FileTypeId;
                }

                var fileTxt = File.ReadAllText(droppedFilePath).Replace("�", " ");

                var fixedtxt = Regex.Replace(fileTxt, ",\"[^\"\n]*\n", "");
                    var lines = fixedtxt.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                // identify header
                var headerline = lines.FirstOrDefault();

                var csvType = GetFileType(headerline);

                
                if (headerline != null)
                {
                    var headings = headerline.CsvSplit();


                    if (fileType.Type == "SI")
                    {
                        await SaveCsvSubItems.Instance.ExtractSubItems(fileType.Type, lines, headings, csvType).ConfigureAwait(false);
                        return;
                    }

                    if (await SaveCsvEntryData.Instance.ExtractEntryData(fileType.Type, lines, headings, csvType, docSet, overWriteExisting, emailId, fileTypeId, droppedFilePath).ConfigureAwait(false)) return;
                }
            }
            catch (Exception Ex)
            {
                throw;
            }
        }
     
        private  string GetFileType(string headerline)
        {
            if (headerline == "INVNO,SEQ,ITEM-#,DESCRIPTION,QUANTITY,UNIT, PRICE , Amount ,DATE,TYPE")
                return "Standard";

            if (headerline == ",,,,Type,,Date,,Num,,Memo,,Item,,Qty,,Sales Price,,Amount,,Balance")
                return "QB9";

            if (headerline == "Type,,Date,,Num,,Memo,,Item,,Qty,,Sales Price,,Amount,,Balance")
                return "QB9";

            if (headerline == "Precision_4,Number,Date,ItemNumber,ItemDescription,Quantity,QtyAllocated")
                return "SubItems";

            return null;
        }

    }
}

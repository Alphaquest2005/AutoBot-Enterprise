using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.CSV;
using System.IO;
using DocumentDS.Business.Entities;



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
            //MessageBox.Show("Complete");
        }

        internal  async Task ProcessDroppedFile(string droppedFilePath, string fileType, AsycudaDocumentSet docSet,bool overWriteExisting)
        {
            try
            {
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException(string.Format("Problem importing File '{0}'. - Error: {1}", droppedFilePath, Ex.Message));
            }

        }

        private  async Task SaveCSV(string droppedFilePath, string fileType, AsycudaDocumentSet docSet, bool overWriteExisting)
        {
            try
            {
                var lines = File.ReadAllLines(droppedFilePath);
                // identify header
                var headerline = lines.FirstOrDefault();

                var csvType = GetFileType(headerline);

                
                if (headerline != null)
                {
                    var headings = headerline.CsvSplit();


                    if (fileType == "SI")
                    {
                        await SaveCsvSubItems.Instance.ExtractSubItems(fileType, lines, headings, csvType).ConfigureAwait(false);
                        return;
                    }

                    if (await SaveCsvEntryData.Instance.ExtractEntryData(fileType, lines, headings, csvType, docSet, overWriteExisting).ConfigureAwait(false)) return;
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

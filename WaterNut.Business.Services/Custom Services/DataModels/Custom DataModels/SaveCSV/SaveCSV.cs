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
using WaterNut.Business.Services.Importers;


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
                var docSet = Utils.GetDocSets(fileType);
                await SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException($"Problem importing File '{droppedFilePath}'. - Error: {Ex.Message}");
            }

        }

        

        private async Task SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet,
            bool overWriteExisting)
        {
            var csvImporter = new CSVImporter(fileType);
            var emailId = Utils.GetExistingEmailId(droppedFilePath, fileType);

            var lines = csvImporter.GetFileLines(droppedFilePath).ToArray();
           
            var fixedHeadings = csvImporter.GetHeadings(lines).ToArray();

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

        


    }
}

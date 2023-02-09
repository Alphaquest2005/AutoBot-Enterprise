using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using MoreLinq;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class EntryDataFileImporter
    {
      
        public async Task ImportEntryDataFile(DataFile dataFile)
        {
            EntryDataFileProcessor(dataFile);
        }

        private static void EntryDataFileProcessor(DataFile dataFile)
        {
            try
            {
                var eslst = SetDefaults(dataFile.Data);

                SaveDataFile(eslst, dataFile.FileType, dataFile.EmailId, dataFile.DroppedFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SaveDataFile(IEnumerable<dynamic> eslst, FileTypes fileType, string emailId, string droppedFilePath)
        {
            using (var ctx = new EntryDataDSContext())
            {
                ctx.Database.ExecuteSqlCommand(
                    $@"delete from EntryDataFiles where SourceFile = '{droppedFilePath}'");
                var rows = new List<EntryDataFiles>();
                foreach (var line in eslst)
                {
                    var drow = new EntryDataFiles(true)
                    {
                        TrackingState = TrackingState.Added,
                        SourceFile = droppedFilePath,
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        SourceRow = line.SourceRow,
                        FileType = fileType.FileImporterInfos.EntryType,
                        EmailId = emailId,
                        FileTypeId = fileType.Id,
                        EntryDataId = line.EntryDataId,
                        EntryDataDate = line.EntryDataDate,
                        Cost = line.Cost,
                        InvoiceQty = line.InvoiceQuantity,
                        ItemDescription = line.ItemDescription,
                        ItemNumber = ((string)Convert.ToString(line.ItemNumber)).Truncate(20),
                        LineNumber = line.LineNumber,
                        Quantity = line.Quantity,
                        ReceivedQty = line.ReceivedQuantity,
                        TaxAmount = line.Tax,
                        TotalCost = line.TotalCost,
                        Units = line.Units,
                        
                    };
                    rows.Add(drow);
                }

                ctx.BulkInsert(rows);
            }
        }

        public static IEnumerable<dynamic> SetDefaults(List<dynamic> data)
        {
            var eslst = data.Where(x => !string.IsNullOrEmpty(x.SourceRow));
            eslst.Select(x =>
            {
                if (!((IDictionary<string, object>)x).ContainsKey("InvoiceQuantity")) x.InvoiceQuantity = 0;
                if (!((IDictionary<string, object>)x).ContainsKey("ReceivedQuantity")) x.ReceivedQuantity = 0;
                if (!((IDictionary<string, object>)x).ContainsKey("Tax")) x.Tax = 0.0;
                if (!((IDictionary<string, object>)x).ContainsKey("TotalCost")) x.TotalCost = 0.0;
                return x;
            });
            return eslst;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class EntryDataFileImporter
    {
        static EntryDataFileImporter()
        {
        }

        public EntryDataFileImporter()
        {
        }

        public async Task ImportEntryDataFile(FileTypes fileType, List<dynamic> eslst, string emailId,
            int? fileTypeId, string droppedFilePath, int applicationSettingsId)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    eslst.ForEach(x =>
                    {
                        if (!((IDictionary<string, object>)x).ContainsKey("InvoiceQuantity")) x.InvoiceQuantity = 0;
                        if (!((IDictionary<string, object>)x).ContainsKey("ReceivedQuantity")) x.ReceivedQuantity = 0;
                        if (!((IDictionary<string, object>)x).ContainsKey("Tax")) x.Tax = 0.0;
                        if (!((IDictionary<string, object>)x).ContainsKey("TotalCost")) x.TotalCost = 0.0;
                       
                    });

                    ctx.Database.ExecuteSqlCommand($@"delete from EntryDataFiles where SourceFile = '{droppedFilePath}'");
                    foreach (var line in eslst)
                    {
                        var drow = new EntryDataFiles(true)
                        {
                            TrackingState = TrackingState.Added,
                            SourceFile = droppedFilePath,
                            ApplicationSettingsId = applicationSettingsId,
                            SourceRow = line.SourceRow,
                            FileType = fileType.FileImporterInfos.EntryType,
                            EmailId = emailId,
                            FileTypeId = fileTypeId,
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
                            Units = line.Units
                        };
                        ctx.EntryDataFiles.Add(drow);
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
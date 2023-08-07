using EntryDataDS.Business.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Common.Utils;
using TrackableEntities;
using Z.EntityFramework.Extensions;

namespace WaterNut.DataSpace
{
    internal class SaveItemHistory 
    {


        public void Process(DataFile dataFile)
        {
            try
            {
                var lstHistories = CreateItemHistories(dataFile);
                DeleteData(lstHistories);
                SaveItemHistories(GetItemHistories(dataFile, lstHistories));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<ItemHistory> CreateItemHistories(DataFile dataFile)
        {
            var lstHistories = dataFile.Data
                .Select(x => ((IDictionary<string, dynamic>)x))
                .Select(x =>
                {
                    try
                    {
                        return new ItemHistory()
                        {
                            TransactionId = x[nameof(ItemHistory.TransactionId)]?.ToString().Trim(),
                            Date = DateTime.Parse(x[nameof(ItemHistory.Date)]?.ToString().Trim()),
                            ItemNumber = x[nameof(ItemHistory.ItemNumber)]?.ToString().Trim(),
                            ItemDescription = x[nameof(ItemHistory.ItemDescription)]?.ToString().Trim(),
                            Quantity = Convert.ToDouble((string)x[nameof(ItemHistory.Quantity)]?.ToString().Trim()),
                            Cost = Convert.ToDouble((string)x[nameof(ItemHistory.Cost)]?.ToString().Trim()),
                            Comments = x[nameof(ItemHistory.Comments)]?.ToString().Trim(),
                            TransactionType = x[nameof(ItemHistory.TransactionType)]?.ToString().Trim(),
                        };
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return null;
                    }
                   
                }).ToList();
            return lstHistories;
        }

        private static void SaveItemHistories(List<ItemHistory> entities)
        {
            using (var ctx = new EntryDataDSContext())
            {
                ctx.ItemHistory.AddRange(entities);
                ctx.SaveChanges();
            }
        }

        private static List<ItemHistory> GetItemHistories(DataFile dataFile, List<ItemHistory> lstHistories)
        {
            var entities = lstHistories.Select(history => new ItemHistory()
            {
                ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings
                    .ApplicationSettingsId,

                FileTypeId = dataFile.FileType.Id,
                EmailId = dataFile.EmailId,
                SourceFile = dataFile.DroppedFilePath,
                TrackingState = TrackingState.Added,

                TransactionId = history.TransactionId,
                Date = history.Date,
                ItemNumber = history.ItemNumber,
                ItemDescription = history.ItemDescription,
                Quantity = history.Quantity,
                Cost = history.Cost,
                Comments = history.Comments,
                TransactionType = history.TransactionType,
            }).ToList();
            return entities;
        }

        private void DeleteData(List<ItemHistory> lstHistories)
        {
            //var existingHistory = lstHistories.Select(x => x.TransactionId).ToList();
            using (var ctx = new EntryDataDSContext())
            {
                ctx.ItemHistory.BulkDelete(lstHistories);
               
                ctx.SaveChanges();
            }
               
        }
    }
}
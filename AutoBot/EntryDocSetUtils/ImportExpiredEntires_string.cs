using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version
using AutoBot.Services;

namespace AutoBot
{
    public partial class EntryDocSetUtils
    {
        // NOTE: Original method name has a typo "Entires" instead of "Entries"
        public static void ImportExpiredEntires(string expFile)
        {
            var fileType = new CoreEntitiesContext()
                .FileTypes
                .Include(x => x.FileImporterInfos)
                .First(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                        x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ExpiredEntries);
            CSVUtils.SaveCsv(new FileInfo[] { new FileInfo(expFile) }, fileType);
        }
    }
}
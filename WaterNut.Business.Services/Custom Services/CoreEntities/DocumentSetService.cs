using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllocationQS.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using Omu.ValueInjecter;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;


namespace CoreEntities.Business.Services
{
    using Serilog;

    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IBusinessService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class DocumentSetService : IDocumentSetService, IDisposable
    {
        public async Task DeleteDocuments(int docSetId)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.ClearAsycudaDocumentSet(docSetId).ConfigureAwait(false);
        }

        public async Task DeleteDocumentSet(int docSetId)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.DeleteAsycudaDocumentSet(docSetId).ConfigureAwait(false);
        }

        public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages, bool overwriteExisting, bool linkPi)
        {
            var docset =
                await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                    .ConfigureAwait(false);
            await
                WaterNut.DataSpace.BaseDataModel.Instance.ImportDocuments(docset, fileNames, onlyRegisteredDocuments,
                    importTariffCodes, noMessages, overwriteExisting, linkPi).ConfigureAwait(false);
        }

        public async Task ExportDocument(string fileName, int docId)
        {
            var doc = await WaterNut.DataSpace.BaseDataModel.Instance.GetDocument(docId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.ExportDocument(fileName, doc).ConfigureAwait(false);
        }

        public async Task ExportDocSet(int docSetId, string directoryName)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.ExportDocSet(docSetId, directoryName, true).ConfigureAwait(false);
        }

        public async Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx, ILogger log)
        {
            try
            {
                var docset = new AsycudaDocumentSet();
                asycudaDocumentSetEx.ModifiedProperties = null;
                docset.InjectFrom(asycudaDocumentSetEx);
                //docset.ApplicationSettingsId = asycudaDocumentSetEx.ApplicationSettingsId;
                //docset.ApportionMethod = asycudaDocumentSetEx.ApportionMethod;
                //docset.AsycudaDocumentSetId = asycudaDocumentSetEx.AsycudaDocumentSetId;
                //docset.BLNumber = asycudaDocumentSetEx.BLNumber;
                //docset.Country_of_origin_code = asycudaDocumentSetEx.Country_of_origin_code;
                //docset.Currency_Code = asycudaDocumentSetEx.Currency_Code;
                //docset.Customs_ProcedureId = asycudaDocumentSetEx.Customs_ProcedureId;
                //docset.Declarant_Reference_Number = asycudaDocumentSetEx.Declarant_Reference_Number;
                //docset.Description = asycudaDocumentSetEx.Description;
                //docset.Document_TypeId = asycudaDocumentSetEx.Document_TypeId;
                //docset.Exchange_Rate = asycudaDocumentSetEx.Exchange_Rate.GetValueOrDefault();
                //docset.EntryTimeStamp = asycudaDocumentSetEx.EntryTimeStamp;
                //docset.LastFileNumber = asycudaDocumentSetEx.LastFileNumber;
                //docset.Manifest_Number = asycudaDocumentSetEx.Manifest_Number;
                //docset.StartingFileCount = asycudaDocumentSetEx.StartingFileCount;
                //docset.TotalFreight = asycudaDocumentSetEx.TotalFreight;
                //docset.TotalInvoices = asycudaDocumentSetEx.TotalInvoices;
                //docset.TotalPackages = asycudaDocumentSetEx.TotalPackages;
                docset.TotalWeight = (double?)asycudaDocumentSetEx.TotalWeight;
                
                await WaterNut.DataSpace.DocumentDS.DataModels.BaseDataModel.Instance.SaveAsycudaDocumentSet(docset, log)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId)
        {
            try
            {
                var docset = await WaterNut.DataSpace.BaseDataModel.Instance
                    .CreateAsycudaDocumentSet(applicationSettingsId).ConfigureAwait(false);
                using (var ctx = new AsycudaDocumentSetExService())
                {
                    return await ctx.GetAsycudaDocumentSetExByKey(docset.AsycudaDocumentSetId.ToString())
                        .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task BaseDataModelInitialize()
        {
          
          await WaterNut.DataSpace.BaseDataModel.Initialization.ConfigureAwait(false);

        }

        public async Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanEntries(docSet,lst, perIM7).ConfigureAwait(false);
        }

        public async Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanLines(docSet, lst, perIM7).ConfigureAwait(false);
        }

        public async Task AttachDocuments(int asycudaDocumentSetId, List<string> files)
        {
            var fileTypes = new CoreEntitiesContext().FileTypes
                .Where(x => x.FileImporterInfos != null)
                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .ToList();
            foreach (var fileType in fileTypes)
            {
                var csvFiles = files.Where(x => Regex.IsMatch(x, fileType.FilePattern, RegexOptions.IgnoreCase))
                    .Select(x => new FileInfo(x)).ToArray();
                if (csvFiles.Length == 0) continue;



                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Lic)
                    await BaseDataModel.Instance.ImportLicense(fileType.AsycudaDocumentSetId,
                        csvFiles.Select(x => x.FullName).ToList()).ConfigureAwait(false);

                if (fileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71)
                    await BaseDataModel.Instance.ImportC71(fileType.AsycudaDocumentSetId,
                        csvFiles.Select(x => x.FullName).ToList()).ConfigureAwait(false);

                fileType.AsycudaDocumentSetId = asycudaDocumentSetId;
                await BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, fileType).ConfigureAwait(false);
                
            }

            await BaseDataModel.Instance.AttachToExistingDocuments(asycudaDocumentSetId).ConfigureAwait(false);
        }

        public async Task CleanBond(int docSetId, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanBond(docSet, perIM7).ConfigureAwait(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion
    }
}


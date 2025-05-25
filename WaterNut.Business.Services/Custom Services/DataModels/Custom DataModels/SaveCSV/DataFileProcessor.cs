using Serilog;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class DataFileProcessor
    {
        private static readonly ILogger _logger = Log.ForContext<DataFileProcessor>();
        private Dictionary<string, Dictionary<string, Func<DataFile, Task<bool>>>> _dataFileActions = new Dictionary<string, Dictionary<string, Func<DataFile, Task<bool>>>>()
            {
                {FileTypeManager.FileFormats.Csv, CSVDataFileActions.Actions},
                {FileTypeManager.FileFormats.PDF, PDFDataFileActions.Actions}
            };

        public async Task<bool> Process(DataFile dataFile)
        {
            _logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                            nameof(Process), "Process data file based on format and entry type", $"FileType: {dataFile?.FileType?.Description}, DocSetCount: {dataFile?.DocSet?.Count()}");
            try
            {
                if (dataFile.DocSet.Any(x =>
                        x.ApplicationSettingsId !=
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                    throw new ApplicationException("Doc Set not belonging to Current Company");

               _logger.Debug("INTERNAL_STEP: {MethodName} - Invoking Action. Intention: Look up and invoke action based on file format and entry type. CurrentState: {{FileFormat}}, {{EntryType}}",
                   nameof(Process), dataFile.FileType.FileImporterInfos.Format, dataFile.FileType.FileImporterInfos.EntryType);
              var res =  await _dataFileActions
                       [dataFile.FileType.FileImporterInfos.Format]
                   [dataFile.FileType.FileImporterInfos.EntryType]
                   .Invoke(dataFile).ConfigureAwait(false);

               _logger.Information("METHOD_EXIT: {MethodName}. Result: {Result}", nameof(Process), true);
                return res; // Assuming Invoke returns true on success
            }
            catch (Exception e)
            {
                _logger.Error(e, "METHOD_EXIT_EXCEPTION: {MethodName}. An error occurred during data file processing.", nameof(Process));
                throw;
            }
        }
    }
}
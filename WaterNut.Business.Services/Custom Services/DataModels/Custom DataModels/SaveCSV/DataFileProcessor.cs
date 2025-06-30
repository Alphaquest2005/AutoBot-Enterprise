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

               // **CRITICAL_DEBUG**: Add comprehensive dictionary lookup logging for OCR template debugging
               _logger.Error("🔍 **DATAFILE_PROCESSOR_DEBUG**: About to perform dictionary lookup");
               _logger.Error("   - **FORMAT_KEY**: '{FormatKey}'", dataFile.FileType.FileImporterInfos.Format);
               _logger.Error("   - **ENTRY_TYPE_KEY**: '{EntryTypeKey}'", dataFile.FileType.FileImporterInfos.EntryType);
               _logger.Error("   - **AVAILABLE_FORMAT_KEYS**: {AvailableFormats}", string.Join(", ", _dataFileActions.Keys));
               
               if (_dataFileActions.ContainsKey(dataFile.FileType.FileImporterInfos.Format))
               {
                   var formatActions = _dataFileActions[dataFile.FileType.FileImporterInfos.Format];
                   _logger.Error("   - **AVAILABLE_ENTRY_TYPE_KEYS_FOR_FORMAT**: {AvailableEntryTypes}", string.Join(", ", formatActions.Keys));
                   
                   if (formatActions.ContainsKey(dataFile.FileType.FileImporterInfos.EntryType))
                   {
                       _logger.Error("✅ **DICTIONARY_LOOKUP_SUCCESS**: Both format and entry type keys found");
                   }
                   else
                   {
                       _logger.Error("❌ **ENTRY_TYPE_KEY_NOT_FOUND**: Entry type '{EntryType}' not found in format '{Format}' actions", 
                           dataFile.FileType.FileImporterInfos.EntryType, dataFile.FileType.FileImporterInfos.Format);
                   }
               }
               else
               {
                   _logger.Error("❌ **FORMAT_KEY_NOT_FOUND**: Format '{Format}' not found in _dataFileActions", 
                       dataFile.FileType.FileImporterInfos.Format);
               }

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
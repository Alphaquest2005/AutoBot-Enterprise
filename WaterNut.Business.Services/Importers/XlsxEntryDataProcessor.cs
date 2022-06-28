using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers.EntryData;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers
{
    public class XlsxEntryDataProcessor : IProcessor<DataTable>
    {
        private readonly FileTypes _fileType;
        private readonly string _fileName;
        private readonly bool _overWrite;

        public XlsxEntryDataProcessor(FileTypes fileType, string fileName, bool overWrite)
        {
            _fileType = fileType;
            _fileName = fileName;
            _overWrite = overWrite;
        }



        public Result<List<DataTable>> Execute(List<DataTable> data)
        {
            try
            {


                var mainTable = data.First();
                var rows = XLSXUtils.FixupDataSet(mainTable);

                var fileType = _fileType.ChildFileTypes.First().FileImporterInfos.EntryType ==
                               FileTypeManager.EntryTypes.Unknown
                    ? XLSXUtils.DetectFileType(_fileType, new FileInfo(_fileName), rows)
                    : FileTypeManager.GetHeadingFileType(rows, _fileType);

                var res = XLSXUtils.DataRowToBetterExpando(fileType, rows, mainTable).Select(x => (dynamic)x).ToList();
                //var header = ((IDictionary<string, object>)res.First()).Values.Select(x => x.ToString()).ToList();

                var docSet = DataSpace.Utils.GetDocSets(fileType);

                var emailId = DataSpace.Utils.GetExistingEmailId(_fileName, fileType);



                fileType.EmailId = emailId;
                var importSettings = new ImportSettings(fileType, docSet, _overWrite, _fileName, emailId);

                var preEntryDatapipline = new DocumentProcessorPipline(new List<IDocumentProcessor>()
                {
                    new FilterOutHeaders(fileType),
                    new GetItemInfo(),
                    new FilterOutBlankLines(),
                    new FillEntryId(),
                    new ApplyCalcualatedFileTypeMappings(fileType),
                    EntryDataManager.CSVDocumentProcessors(importSettings)[fileType.FileImporterInfos.EntryType]

                });

                preEntryDatapipline.Execute(res);



                return new Result<List<DataTable>>(data, true, "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }

    public class ApplyCalcualatedFileTypeMappings: IDocumentProcessor
    {
        private readonly FileTypes _fileType;

        public ApplyCalcualatedFileTypeMappings(FileTypes fileType)
        {
            _fileType = fileType;
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            var dic = FileTypeManager.PostCalculatedFileTypeMappings(_fileType);
            var res = new List<dynamic>();

            foreach (IDictionary<string, object> line in lines.ToList())
            {
                foreach (var map in _fileType.FileTypeMappings.Where(x => x.OriginalName.Contains("{")).ToList())
                {
                    if (map.OriginalName.Contains("{") &&
                        dic.ContainsKey(map.OriginalName.Replace("{", "").Replace("}", "")))
                    {
                        line[map.DestinationName] = XLSXUtils.GetMappingValue(map,dic[map.OriginalName.Replace("{", "").Replace("}", "")]
                            .Invoke(line, line, null));
                    }
                }

                res.Add(line);
            }

            return res;
        }
    }

    public class FilterOutHeaders :IDocumentProcessor
    {
        private readonly FileTypes _fileType;

        public FilterOutHeaders(FileTypes fileType)
        {
            _fileType = fileType;
            
        }

        public List<dynamic> Execute(List<dynamic> lines)
        {
            var result = new List<dynamic>();
            foreach (IDictionary<string, object> line in lines.ToList())
            {
                if (line.Values.Select(x => x.ToString()).All(x => !_fileType.FileTypeMappings.Select(z => z.OriginalName).Contains(x)))
                    result.Add((dynamic)line);
            }

            return result;
        }
    }

    public class FilterOutBlankLines : IDocumentProcessor
    {
        public List<dynamic> Execute(List<dynamic> lines)
        {
           return lines.Where(x => !string.IsNullOrEmpty(x.ItemNumber)).ToList();
        }
    }

    public class FillEntryId : IDocumentProcessor
    {
        public List<dynamic> Execute(List<dynamic> lines)
        {
            dynamic lastLine = null;
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line.EntryDataId))
                {
                    lastLine = line;
                }
                else
                {
                    line.EntryDataId = lastLine?.EntryDataId;
                    line.EntryDataDate = lastLine?.EntryDataDate;
                    line.CustomerName = lastLine?.CustomerName;
                }
            }
            return lines;
        }
    }

    public class GetItemInfo: IDocumentProcessor
    {
        public List<dynamic> Execute(List<dynamic> lines)
        {
            lines.Select(x =>
            {
                x.ItemNumber = x.ItemNumber ?? x.POItemNumber ?? x.SupplierItemNumber;
                x.ItemDescription = x.ItemDescription ?? x.POItemDescription ?? x.SupplierItemDescription;
                return x;
            }).ToList();
            return lines;
        }
    }
}
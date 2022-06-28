using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers.EntryData;

namespace WaterNut.Business.Services.Importers
{
    public class MisMatchesProcessor : IProcessor<DataTable>
    {
        private readonly FileTypes _fileType;

        public MisMatchesProcessor(FileTypes fileType)
        {
            _fileType = fileType;
            
        }
        
        public Result<List<DataTable>> Execute(List<DataTable> data)
        {
             if (data.Any(x => x.TableName == "MisMatches") && data.Any(x => x.TableName == "POTemplate"))
                XLSXUtils.ReadMISMatches(data.First(x => x.TableName == "MisMatches"), data.First(x => x.TableName == "POTemplate"));
             return new Result<List<DataTable>>(data, true, "");
        }
    }
}
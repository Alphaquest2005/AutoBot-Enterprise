using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers.EntryData;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers
{
    using Serilog;

    public class MisMatchesProcessor : IProcessor<DataTable>
    {
        private readonly FileTypes _fileType;
 
        public MisMatchesProcessor(FileTypes fileType)
        {
            _fileType = fileType;
            
        }
        
        public async Task<Result<List<DataTable>>> Execute(List<DataTable> data, ILogger log)
        {
             if (data.Any(x => x.TableName == "MisMatches") && data.Any(x => x.TableName == "POTemplate"))
                await XLSXUtils.ReadMISMatches(data.First(x => x.TableName == "MisMatches"), data.First(x => x.TableName == "POTemplate"), log).ConfigureAwait(false);
            return await Task.FromResult(new Result<List<DataTable>>(data, true, "")).ConfigureAwait(false); // Wrap in Task.FromResult
        }
    }
}
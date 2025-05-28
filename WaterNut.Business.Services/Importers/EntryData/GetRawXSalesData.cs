using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.DataSpace;
using System.Threading.Tasks;
 
namespace WaterNut.Business.Services.Importers.EntryData
{
    using Serilog;

    public class GetRawXSalesData : IProcessor<BetterExpando>
    {
        private readonly ImportSettings _importSettings;
        private readonly List<dynamic> _lines;
 
        public GetRawXSalesData(ImportSettings importSettings, List<dynamic> lines)
        {
            _importSettings = importSettings;
            _lines = lines;
        }
 
        public Task<Result<List<BetterExpando>>> Execute(List<BetterExpando> data, ILogger log)
        {
            try
            {
 
 
            dynamic xsale = new BetterExpando();
            xsale.EmailId = _importSettings.EmailId;
            xsale.StartDate = _lines.Min(x => x.Date);
            xsale.EndDate = _lines.Max(x => x.Date);
            xsale.SourceFile = _importSettings.DroppedFilePath;
            xsale.ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
            xsale.IsImported = true;
 
            var details = _lines.Select(x =>
            {
                dynamic itm = new BetterExpando();
                try
                {
                    itm.FileLineNumber = (int)x.LineNumber;
                    itm.Line = (int)x.Line;
                    itm.Date = x.Date;
                    itm.InvoiceNo = x.InvoiceNo;
                    itm.CustomerName = x.CustomerName;
                    itm.ItemNumber = x.ItemNumber;
                    itm.ItemDescription = x.ItemDescription;
                    itm.TariffCode = x.TariffCode;
                    itm.SalesQuantity = x.SalesQuantity;
                    itm.SalesFactor = x.SalesFactor;
                    itm.xQuantity = x.xQuantity;
                    itm.Price = x.Price;
                    itm.DutyFreePaid = x.DutyFreePaid;
                    itm.pCNumber = x.pCNumber;
                    itm.pLineNumber = (int)x.pLineNumber;
                    itm.pRegDate = x.pRegDate;
                    itm.CIFValue = x.CIFValue;
                    itm.DutyLiablity = x.DutyLiablity;
                    itm.Comment = x.Comment;
                    return itm;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }).ToList();
 
            xsale.xSalesDetails = details;
 
            return Task.FromResult(Task.FromResult(new Result<List<BetterExpando>>(new List<BetterExpando>(){xsale}, true,"")).Result); // Wrap in Task.FromResult
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
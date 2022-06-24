using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetRawXSalesData : IProcessor<BetterExpando>
    {
        private readonly ImportSettings _importSettings;
        private readonly List<dynamic> _lines;

        public GetRawXSalesData(ImportSettings importSettings, List<dynamic> lines)
        {
            _importSettings = importSettings;
            _lines = lines;
        }

        public Result<List<BetterExpando>> Execute(List<BetterExpando> data)
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
                itm.DutyFreePaid = x.SalesType;
                itm.pCNumber = x.PreviousCNumber;
                itm.pLineNumber = (int)x.PreviousLineNumber;
                itm.pRegDate = x.PreviousRegDate;
                itm.CIFValue = x.CIFValue;
                itm.DutyLiablity = x.DutyLiablity;
                itm.Comment = x.Comment;
                return itm;
            }).ToList();

            xsale.xSalesDetails = details;

            return new Result<List<BetterExpando>>(new List<BetterExpando>(){xsale}, true,"");
        }
    }
}
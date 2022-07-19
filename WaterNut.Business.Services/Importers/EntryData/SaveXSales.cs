using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class SaveXSales : IProcessor<BetterExpando>
    {
        private readonly ImportSettings _importSettings;

        public SaveXSales(ImportSettings importSettings)
        {
            _importSettings = importSettings;

        }

        public Result<List<BetterExpando>> Execute(List<BetterExpando> data)
        {
            try
            {


                using (var ctx = new EntryDataDSContext())
                {
                    foreach (dynamic itm in data)
                    {
                        var xSale = new xSalesFiles(true)
                        {
                            SourceFile = itm.SourceFile,
                            StartDate = itm.StartDate,
                            EndDate = itm.EndDate,
                            ApplicationSettingsId = itm.ApplicationSettingsId,
                            EmailId = itm.EmailId,
                            IsImported = itm.IsImported,
                            TrackingState = TrackingState.Added,
                            xSalesDetails = ((List<dynamic>)itm.xSalesDetails)
                                .Select(x =>
                                {
                                    var t = new xSalesDetails(true)
                                    {
                                        FileLine = x.FileLineNumber,
                                        Line = x.Line,
                                        Date = x.Date,
                                        InvoiceNo = x.InvoiceNo,
                                        CustomerName = x.CustomerName,
                                        ItemNumber = x.ItemNumber,
                                        ItemDescription = ((string)x.ItemDescription).Truncate(50),
                                        TariffCode = x.TariffCode,
                                        SalesQuantity = x.SalesQuantity,
                                        SalesFactor = x.SalesFactor,
                                        xQuantity = x.xQuantity,
                                        Price = x.Price,
                                        DutyFreePaid = x.DutyFreePaid,
                                        pCNumber = x.pCNumber,
                                        pLineNumber = x.pLineNumber,
                                        pRegDate = x.pRegDate,
                                        CIFValue = x.CIFValue,
                                        DutyLiablity = x.DutyLiablity,
                                        Comment = x.Comment,
                                        TrackingState = TrackingState.Added,
                                    };
                                    return t;
                                }).ToList()
                        };
                        ctx.xSalesFiles.Add(xSale);
                    }

                    ctx.SaveChanges();
                }

                return new Result<List<BetterExpando>>(data, true, "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
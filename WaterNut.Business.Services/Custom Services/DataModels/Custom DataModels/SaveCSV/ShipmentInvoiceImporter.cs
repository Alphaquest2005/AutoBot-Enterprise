using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class ShipmentInvoiceImporter
    {
        static ShipmentInvoiceImporter()
        {
        }

        public ShipmentInvoiceImporter()
        {
        }

        public void ProcessShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst, Dictionary<string, string> invoicePOs)
        {
            try
            {
                var itms = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).ToList();
                var lst = itms.Select(x => (IDictionary<string, object>)x)
                    .Select(x => new ShipmentInvoice()
                    {

                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        InvoiceNo = x.ContainsKey("InvoiceNo") ?  x["InvoiceNo"].ToString() : "Unknown",
                        InvoiceDate = x.ContainsKey("InvoiceDate") ?  DateTime.Parse(x["InvoiceDate"].ToString()) : DateTime.MinValue,
                        InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null,//Because of MPI not 
                        SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null,
                        ImportedLines = !x.ContainsKey("InvoiceDetails") ? 0 : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).Count,
                        SupplierCode = x.ContainsKey("SupplierCode") ? x["SupplierCode"].ToString() : null,
                        FileLineNumber = itms.IndexOf(x) + 1,
                        Currency = x.ContainsKey("Currency") ? x["Currency"].ToString() : null,
                        TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null,
                        TotalOtherCost = x.ContainsKey("TotalOtherCost")? Convert.ToDouble(x["TotalOtherCost"].ToString()): (double?) null,
                        TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null,
                        TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null,
                        InvoiceDetails = !x.ContainsKey("InvoiceDetails") ? new List<InvoiceDetails>() : ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                            .Where(z => z.ContainsKey("ItemDescription"))
                                              
                            .Select(z => new InvoiceDetails()
                            {
                                Quantity = Convert.ToDouble(z["Quantity"].ToString()),
                                ItemNumber = z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20): null,
                                ItemDescription = z["ItemDescription"].ToString().Truncate(255),
                                Units = z.ContainsKey("Units") ? z["Units"].ToString() : null,
                                Cost = z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) : Convert.ToDouble(z["TotalCost"].ToString()) / (Convert.ToDouble(z["Quantity"].ToString()) == 0 ? 1 : Convert.ToDouble(z["Quantity"].ToString())),
                                TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) : Convert.ToDouble(z["Cost"].ToString()) * Convert.ToDouble(z["Quantity"].ToString()),
                                Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0,
                                Volume = z.ContainsKey("Gallons") ? new InvoiceDetailsVolume() {Quantity = Convert.ToDouble(z["Gallons"].ToString()), Units = "Gallons", TrackingState = TrackingState.Added, } : null,
                                SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA") || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units")) ? Convert.ToInt32(z["SalesFactor"].ToString()) /* * (z.ContainsKey("Multiplier")  ? Convert.ToInt32(z["Multiplier"].ToString()) : 1) */ : 1,
                                LineNumber = z.ContainsKey("Instance") ? Convert.ToInt32(z["Instance"].ToString()) :((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1,
                                FileLineNumber = Convert.ToInt32(z["FileLineNumber"].ToString()),
                                Section = z.ContainsKey("Section") ? z["Section"].ToString( ): null,
                                InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"]: (int?)null,
                                TrackingState = TrackingState.Added,

                            }).ToList(),
                        InvoiceExtraInfo = !x.ContainsKey("ExtraInfo")? new List<InvoiceExtraInfo>(): ((List<IDictionary<string, object>>)x["ExtraInfo"])
                            .Where(z => z.Keys.Any())
                            .SelectMany(z => z)
                            .Where(z => z.Value != null)
                            .Select(z => new InvoiceExtraInfo()
                            {
                                Info = z.Key.ToString(),
                                Value = z.Value.ToString(),
                                TrackingState = TrackingState.Added,
                            }).ToList(),

                        EmailId = emailId,
                        SourceFile = droppedFilePath,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,

                    }).ToList();

               
                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var invoice in lst)
                    {
                       
                        var existingManifest =
                            ctx.ShipmentInvoice.FirstOrDefault(
                                x => x.InvoiceNo == invoice.InvoiceNo);
                        if (existingManifest != null)
                            ctx.ShipmentInvoice.Remove(existingManifest);

                        invoice.InvoiceDetails = AutoFixImportErrors(invoice);
                        invoice.ImportedLines = invoice.InvoiceDetails.Count;

                        if (Math.Abs(invoice.SubTotal.GetValueOrDefault()) < 0.01)
                        {
                            invoice.SubTotal = invoice.ImportedSubTotal;
                        }

                        if (!invoice.InvoiceDetails.Any())
                            throw new ApplicationException(
                                $"No Invoice Details");

                        if (invoicePOs != null && lst.Count > 1 && invoice != lst.First())
                        {
                            invoice.SourceFile = invoice.SourceFile.Replace($"{invoicePOs[lst.First().InvoiceNo]}",
                                invoicePOs[invoice.InvoiceNo]);
                        }

                        ctx.ShipmentInvoice.Add(invoice);
                        
                        ctx.SaveChanges();

                        //----------ALLOW IMPORTS AND CHANGE THE XLSX TO HIGHLIGHT ERRORS
                        //if (invoice.ImportedTotalDifference > 0.001 )
                        //    throw new ApplicationException(
                        //        $"Imported Total Difference for Invoice > 0: {invoice.ImportedTotalDifference}");
                    }

                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static List<InvoiceDetails> AutoFixImportErrors(ShipmentInvoice invoice)
        {
            var details = invoice.InvoiceDetails;

            foreach (var err in details.Where(x =>x.TotalCost.GetValueOrDefault() > 0 && Math.Abs(((x.Quantity * x.Cost) - x.Discount.GetValueOrDefault()) - x.TotalCost.GetValueOrDefault()) > 0.01))
            {
                
            }

            //if (invoice.SubTotal > 0
            //    && Math.Abs((double) (invoice.SubTotal - details.Sum(x => x.Cost * x.Quantity))) > 0.01)
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<InvoiceDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.TotalCost == detail.TotalCost && x.ItemNumber == detail.ItemNumber &&
                            x.Quantity == detail.Quantity && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

            //details = details.DistinctBy(x => new { ItemNumber = x.ItemNumber.ToUpper(), x.Quantity, x.TotalCost})
            //    .ToList();


            //  var invoiceInvoiceDetails = lst ;
            return lst;
        }
    }
}
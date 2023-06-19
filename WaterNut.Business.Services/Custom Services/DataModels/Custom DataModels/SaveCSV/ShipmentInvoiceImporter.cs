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
                var lstdata = itms.Select(x => (IDictionary<string, object>)x)
                    .Where(x => x != null && x.Any())
                    .Select(x =>
                    {
                        var invoice = new ShipmentInvoice();
                        invoice.ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
                        invoice.InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ?  x["InvoiceNo"].ToString().Truncate(50) : "Unknown";
                        invoice.PONumber = x.ContainsKey("PONumber") && x["PONumber"] != null ? x["PONumber"].ToString() : null;
                        invoice.InvoiceDate = x.ContainsKey("InvoiceDate") ?  DateTime.Parse(x["InvoiceDate"].ToString()) : DateTime.MinValue;
                        invoice.InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null; //Because of MPI not 
                        invoice.SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null;
                        invoice.ImportedLines = !x.ContainsKey("InvoiceDetails") ? 0 : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).Count;
                        invoice.SupplierCode = x.ContainsKey("SupplierCode") ? x["SupplierCode"]?.ToString() : null;
                        invoice.FileLineNumber = itms.IndexOf(x) + 1;
                        invoice.Currency = x.ContainsKey("Currency") ? x["Currency"].ToString() : null;
                        invoice.TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null;
                        invoice.TotalOtherCost = x.ContainsKey("TotalOtherCost")? Convert.ToDouble(x["TotalOtherCost"].ToString()): (double?) null;
                        invoice.TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null;
                        invoice.TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null;
                        invoice.InvoiceDetails = !x.ContainsKey("InvoiceDetails") ? new List<InvoiceDetails>() : ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                            .Where(z => z != null)
                            .Where(z => z.ContainsKey("ItemDescription"))
                                              
                            .Select(z =>
                            {
                                var details = new InvoiceDetails();
                                details.Quantity = z.ContainsKey("Quantity")
                                    ? Convert.ToDouble(z["Quantity"].ToString())
                                    : 0;
                                details.ItemNumber = z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20): null;
                                details.ItemDescription = z["ItemDescription"].ToString().Truncate(255);
                                details.Units = z.ContainsKey("Units") ? z["Units"].ToString() : null;
                                details.Cost = z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) : Convert.ToDouble(z["TotalCost"].ToString()) / (Convert.ToDouble(z["Quantity"].ToString()) == 0 ? 1 : Convert.ToDouble(z["Quantity"].ToString()));
                                details.TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) : Convert.ToDouble(z["Cost"].ToString()) * Convert.ToDouble(z["Quantity"].ToString());
                                details.Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0;
                                details.Volume = z.ContainsKey("Gallons") ? new InvoiceDetailsVolume() {Quantity = Convert.ToDouble(z["Gallons"].ToString()), Units = "Gallons", TrackingState = TrackingState.Added, } : null;
                                details.SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA") || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units")) ? Convert.ToInt32(z["SalesFactor"].ToString()) /* * (z.ContainsKey("Multiplier")  ? Convert.ToInt32(z["Multiplier"].ToString()) : 1) */ : 1;
                                details.LineNumber = z.ContainsKey("Instance") ? Convert.ToInt32(z["Instance"].ToString()) :((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1;
                                details.FileLineNumber = Convert.ToInt32(z["FileLineNumber"].ToString());
                                details.Section = z.ContainsKey("Section") ? z["Section"].ToString( ): null;
                                details.InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"]: (int?)null;
                                details.TrackingState = TrackingState.Added;
                                return details;
                            }).ToList();
                        invoice.InvoiceExtraInfo = !x.ContainsKey("ExtraInfo")? new List<InvoiceExtraInfo>(): ((List<IDictionary<string, object>>)x["ExtraInfo"])
                            .Where(z => z.Keys.Any())
                            .SelectMany(z => z)
                            .Where(z => z.Value != null)
                            .Select(z =>
                            {
                                var info = new InvoiceExtraInfo();
                                info.Info = z.Key.ToString();
                                info.Value = z.Value.ToString();
                                info.TrackingState = TrackingState.Added;
                                return info;
                            }).ToList();
                        invoice.EmailId = emailId;
                        invoice.SourceFile = droppedFilePath;
                        invoice.FileTypeId = fileType.Id;
                        invoice.TrackingState = TrackingState.Added;
                        return invoice;
                    }).ToList();

                var lst = ReduceLstByInvoiceNo(lstdata);
               
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
                            continue;

                        if (invoicePOs != null && lst.Count > 1 && invoice != lst.First())
                        {
                            invoice.SourceFile = invoice.SourceFile.Replace($"{invoicePOs[lst.First().InvoiceNo]}",
                                invoicePOs[invoice.InvoiceNo]);
                        }
                        //Todo: figure out how to merge the invoice details
                        //if (overWriteExisting)
                        //{
                            var existing = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == invoice.InvoiceNo);
                            if (existing != null)
                            {
                                ctx.ShipmentInvoice.Remove(existing);
                            }
                            ctx.ShipmentInvoice.Add(invoice);
                        //}
                        //else
                        //{

                        //}
                        
                        
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

        private List<ShipmentInvoice> ReduceLstByInvoiceNo(List<ShipmentInvoice> lstData)
        {
            var res = new List<ShipmentInvoice>();

            foreach (var grp in lstData.GroupBy(x => x.InvoiceNo))
            {
                var rinvoice = grp.FirstOrDefault(x => grp.Count() <= 1 || x.InvoiceNo == x.PONumber);
                if(rinvoice == null) continue;
                foreach (var invoice in grp.Where(x => grp.Count() > 1 && x.InvoiceNo != x.PONumber))
                {
                    rinvoice.InvoiceDetails.AddRange(invoice.InvoiceDetails);
                    rinvoice.PONumber = invoice.PONumber; // don't think this makes a difference
                }
                res.Add(rinvoice);
            }

            return res;
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
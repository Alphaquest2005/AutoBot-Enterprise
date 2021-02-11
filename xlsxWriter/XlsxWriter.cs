using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using WaterNut.Interfaces;
using PicoXLSX;
using WaterNut.DataSpace;

namespace xlsxWriter
{
    public class XlsxWriter
    {
        public static List<(string reference, string filepath)> CreatCSV(ShipmentInvoice shipmentInvoice)
        {
           
            //workbook.WS.Value("Some Data");                                        // Add cell A1
            //workbook.WS.Formula("=A1");                                            // Add formula to cell B1
            //workbook.WS.Down();                                                    // Go to row 2
            //workbook.WS.Value(DateTime.Now, Style.BasicStyles.Bold);               // Add formatted value to cell A2
            //workbook.Save();

            //return "shit";
            try
            {

                var pdfFile = new FileInfo(shipmentInvoice.SourceFile);
                var pdfFilePath = Path.Combine(pdfFile.DirectoryName, $"{shipmentInvoice.InvoiceNo}.pdf");
                var csvFilePath = Path.Combine(pdfFile.DirectoryName, $"{shipmentInvoice.InvoiceNo}.xlsx");
                var poTemplate = new CoreEntitiesContext().FileTypes
                    .Include(x => x.FileTypeMappings)
                    .First(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.Type == "POTemplate");
                Workbook workbook = new Workbook(csvFilePath, "POTemplate");         // Create new workbook with a worksheet called Sheet1
               
                var header = poTemplate.FileTypeMappings.OrderBy(x => x.Id).Select((x) => new { value = x, index = poTemplate.FileTypeMappings.IndexOf(x) })
                    .ToDictionary(x => (Column: x.value.OriginalName,Index: x.index ), v => v.value);
                var headerRow = 0;

                header.ForEach(x => { SetValue(workbook, headerRow, x.Key.Index, x.Key.Column); });

                var invoiceRow = 1;

                //SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceTotal)));
                //SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.SupplierCode)));
                //SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.Currency)));
                 //var po = shipmentInvoice.InvoiceExtraInfo.FirstOrDefault(x => x.Info == "PONumber");
                 //               if (!string.IsNullOrEmpty(po?.Value))
                 //   SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == po.Info).Key.Index, po.Value);

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceTotal)).Key.Index, shipmentInvoice.InvoiceTotal.GetValueOrDefault());

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.SupplierCode)).Key.Index, shipmentInvoice.SupplierCode);

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.Currency)).Key.Index, shipmentInvoice.Currency);

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "SupplierInvoice").Key.Index, shipmentInvoice.InvoiceNo);

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Total Internal Freight").Key.Index, shipmentInvoice.TotalInternalFreight);
                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Total Deductions").Key.Index, shipmentInvoice.TotalDeduction);
                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Total Other Cost").Key.Index, shipmentInvoice.TotalOtherCost);

                var riderdetails = shipmentInvoice.ShipmentRiderInvoice.Where(x => x.ShipmentRiderDetails != null).ToList();
                var doRider = false;
                if(riderdetails.Any())
                    if (shipmentInvoice.ShipmentInvoicePOs.Sum(x => x.PurchaseOrders.EntryDataDetails.Count()) > riderdetails.Count())
                    {
                        doRider = false;
                        SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index, riderdetails.Sum(x => x.ShipmentRiderDetails.Pieces));
                        SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index, riderdetails.Select(x => x.ShipmentRiderDetails.WarehouseCode).DefaultIfEmpty("").Aggregate((o,c) => o + "," + c));
                    }
                    else
                    {
                        doRider = true;
                        SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index, riderdetails.First().ShipmentRiderDetails.Pieces);
                        SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index, riderdetails.First().ShipmentRiderDetails.WarehouseCode);
                    }


                if (shipmentInvoice.ShipmentInvoicePOs.Any())
                {
                    var i = 1;
                    foreach (var pO in shipmentInvoice.ShipmentInvoicePOs)
                    {
                        foreach (var itm in pO.PurchaseOrders.EntryDataDetails)
                        {

                            SetValue(workbook, i, header.First(x => x.Key.Column == "PONumber").Key.Index,
                                pO.PurchaseOrders.PONumber);
                            SetValue(workbook, i,
                                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                                pO.PurchaseOrders.EntryDataDate.ToString("yyyy-MM-dd"));
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                                itm.Cost);
                            SetValue(workbook, i,
                                header.First(x => x.Key.Column == "Description").Key.Index,
                                itm.ItemDescription);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                                itm.Quantity);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "ProductCode").Key.Index,
                                itm.ItemNumber);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                                itm.TotalCost);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index,
                                itm.Units);
                            if (doRider && i < riderdetails.Count)
                            {
                                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index,
                                    riderdetails[i].ShipmentRiderDetails.Pieces);
                                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                                    riderdetails[i].ShipmentRiderDetails.WarehouseCode);
                            }

                            i++;
                        }
                    }
                }
                else
                {
                    var i = 1;
                    foreach (var itm in shipmentInvoice.InvoiceDetails)
                    {
                        SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierInvoice").Key.Index, shipmentInvoice.InvoiceNo);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index, shipmentInvoice.InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index, itm.Cost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.ItemDescription)).Key.Index, itm.ItemDescription);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index, itm.Quantity);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.ItemNumber)).Key.Index, itm.ItemNumber);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index, itm.TotalCost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index, itm.Units);

                        if (doRider && i < riderdetails.Count)
                        {
                            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index, riderdetails[i].ShipmentRiderDetails.Pieces);
                            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index, riderdetails[i].ShipmentRiderDetails.WarehouseCode);
                        }

                        i++;
                    }

                }
                
                


                workbook.Save();
                if(!File.Exists(pdfFilePath)) File.Copy(pdfFile.FullName, pdfFilePath);
                return new List<(string reference,string filepath)>(){(shipmentInvoice.InvoiceNo, pdfFilePath),(shipmentInvoice.InvoiceNo, csvFilePath) };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      
        private static void SetValue(Workbook workbook, int row, int col, dynamic value)
        {
            
            workbook.CurrentWorksheet.AddCell(value ?? "",col, row);
        }
    }
}

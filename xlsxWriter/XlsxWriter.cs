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
        public static List<(string reference, string filepath)> CreatCSV(ShipmentInvoice shipmentInvoice,
            int riderId)
        {
           
            try
            {

                var pdfFile = new FileInfo(shipmentInvoice.SourceFile);
                var pdfFilePath = "";
                var csvFilePath = "";
                var csvs = new List<(string reference, string filepath)>();
                var poTemplate = new CoreEntitiesContext().FileTypes
                    .Include(x => x.FileTypeMappings)
                    .First(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.Type == "POTemplate");

               
                var header = poTemplate.FileTypeMappings.OrderBy(x => x.Id).Select((x) => new { value = x, index = poTemplate.FileTypeMappings.IndexOf(x) })
                    .ToDictionary(x => (Column: x.value.OriginalName,Index: x.index ), v => v.value);
              

                

               


                if (shipmentInvoice.ShipmentInvoicePOs.Any())
                {
                     
                    foreach (var pO in shipmentInvoice.ShipmentInvoicePOs)
                    {
                        pdfFilePath = Path.Combine(pdfFile.DirectoryName, $"{pO.PurchaseOrders.PONumber}.pdf");
                        csvFilePath = Path.Combine(pdfFile.DirectoryName, $"{pO.PurchaseOrders.PONumber}.xlsx");
                        var workbook = CreateWorkbook(riderId, shipmentInvoice, csvFilePath, header, out var invoiceRow, out var riderdetails, out var doRider);
                        var i = 1;
                        foreach (var itm in pO.PurchaseOrders.EntryDataDetails)
                        {

                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(pO.PurchaseOrders.PONumber)).Key.Index,
                                pO.PurchaseOrders.PONumber);
                            SetValue(workbook, i,
                                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                                pO.PurchaseOrders.EntryDataDate.ToString("yyyy-MM-dd"));
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                                itm.Cost);
                            SetValue(workbook, i,
                                header.First(x => x.Key.Column == "POItemDescription").Key.Index,
                                itm.ItemDescription);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                                itm.Quantity);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index,
                                itm.ItemNumber);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                                itm.TotalCost);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index,
                                itm.Units);
                            if (doRider && i < riderdetails.Count)
                            {
                                SetValue(workbook, i + invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index,
                                    riderdetails[i].ShipmentRiderDetails.Pieces);
                                SetValue(workbook, i + invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                                    riderdetails[i].ShipmentRiderDetails.WarehouseCode);
                            }

                            i++;
                        }
                        workbook.Save();
                        if (!File.Exists(pdfFilePath)) File.Copy(pdfFile.FullName, pdfFilePath);
                        csvs.Add((pO.PurchaseOrders.PONumber, pdfFilePath));
                        csvs.Add((pO.PurchaseOrders.PONumber, csvFilePath));
                    }
                }
                else
                {
                    pdfFilePath = Path.Combine(pdfFile.DirectoryName, $"{shipmentInvoice.InvoiceNo}.pdf");
                    csvFilePath = Path.Combine(pdfFile.DirectoryName, $"{shipmentInvoice.InvoiceNo}.xlsx");
                    var workbook = CreateWorkbook(riderId, shipmentInvoice, csvFilePath, header, out var invoiceRow, out var riderdetails, out var doRider);
                    var i = 1;
                    foreach (var itm in shipmentInvoice.InvoiceDetails)
                    {
                        SetValue(workbook, i, header.First(x => x.Key.Column == "InvoiceNo").Key.Index, shipmentInvoice.InvoiceNo);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index, shipmentInvoice.InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index, itm.Cost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemDescription").Key.Index, itm.ItemDescription);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index, itm.Quantity);
                        SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index, itm.ItemNumber);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index, itm.TotalCost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index, itm.Units);

                        if (doRider && i < riderdetails.Count)
                        {
                            SetValue(workbook, i, header.First(x => x.Key.Column == "Packages").Key.Index, riderdetails[i].Packages);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "Warehouse").Key.Index, riderdetails[i].WarehouseCode);
                        }

                        i++;
                    }
                    workbook.Save();
                    if (!File.Exists(pdfFilePath)) File.Copy(pdfFile.FullName, pdfFilePath);
                    csvs.Add((shipmentInvoice.InvoiceNo, pdfFilePath));
                    csvs.Add((shipmentInvoice.InvoiceNo, csvFilePath));
                }

                
                return csvs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static Workbook CreateWorkbook(int riderId, ShipmentInvoice shipmentInvoice, string csvFilePath,
            Dictionary<(string Column, int Index), FileTypeMappings> header,
            out int invoiceRow, out List<ShipmentRiderInvoice> riderdetails, out bool doRider)
        {
            Workbook workbook = new Workbook(csvFilePath, "POTemplate"); // Create new workbook with a worksheet called Sheet1
            var headerRow = 0;

            header.ForEach(x => { SetValue(workbook, headerRow, x.Key.Index, x.Value.DestinationName); });
            invoiceRow = 1;
            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceTotal)).Key.Index,
                shipmentInvoice.InvoiceTotal.GetValueOrDefault());

            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.SupplierCode)).Key.Index,
                shipmentInvoice.SupplierCode);

            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.Currency)).Key.Index,
                shipmentInvoice.Currency);

            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceNo)).Key.Index,
                shipmentInvoice.InvoiceNo);

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalInternalFreight)).Key.Index,
                shipmentInvoice.TotalInternalFreight);
            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalDeduction)).Key.Index,
                shipmentInvoice.TotalDeduction);
            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalOtherCost)).Key.Index,
                shipmentInvoice.TotalOtherCost);

            riderdetails = shipmentInvoice.ShipmentRiderInvoice.Where(x => x.ShipmentRiderDetails != null && x.RiderID == riderId).ToList();
            doRider = false;
            if (riderdetails.Any())
                if (shipmentInvoice.ShipmentInvoicePOs.Sum(x => x.PurchaseOrders.EntryDataDetails.Count()) <
                    riderdetails.Count())
                {
                    doRider = false;
                    SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index,
                        riderdetails.Sum(x => x.Packages));
                    SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                        riderdetails.Select(x => x.WarehouseCode).DefaultIfEmpty("")
                            .Aggregate((o, c) => o + "," + c));
                }
                else
                {
                    doRider = true;
                    SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index,
                        riderdetails.First().Packages);
                    SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                        riderdetails.First().WarehouseCode);
                }

            return workbook;
        }


        private static void SetValue(Workbook workbook, int row, int col, dynamic value)
        {
            
            workbook.CurrentWorksheet.AddCell(value ?? "",col, row);
        }
    }
}

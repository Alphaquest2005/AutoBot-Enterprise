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
using NanoXLSX;
using TrackableEntities;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using Workbook = PicoXLSX.Workbook;

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
                        var workbook = CreateShipmentWorkBook(riderId, shipmentInvoice, csvFilePath, header, out var invoiceRow, out var riderdetails, out var doRider);
                        var i = 1;
                        foreach (var itm in pO.PurchaseOrders.EntryDataDetails.Where(x => pO.POMISMatches.All(m => m.PODetailsId != x.EntryDataDetailsId)))
                        {
                            var pOItem = itm.INVItems.FirstOrDefault();
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(pO.PurchaseOrders.PONumber)).Key.Index,
                                pO.PurchaseOrders.PONumber);
                            SetValue(workbook, i,
                                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                                pO.PurchaseOrders.EntryDataDate.ToString("yyyy-MM-dd"));
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                                itm.Cost);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemDescription").Key.Index, itm.ItemDescription);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemDescription").Key.Index, pOItem?.INVDescription);
                            
                            SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index, pOItem.INVItemCode);

                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                                itm.Quantity);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index, itm.ItemNumber);
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
                        DoMisMatches(shipmentInvoice, workbook);
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
                    var workbook = CreateShipmentWorkBook(riderId, shipmentInvoice, csvFilePath, header, out var invoiceRow, out var riderdetails, out var doRider);
                    var i = 1;
                    foreach (var itm in shipmentInvoice.InvoiceDetails)
                    {
                        

                        SetValue(workbook, i, header.First(x => x.Key.Column == "InvoiceNo").Key.Index, shipmentInvoice.InvoiceNo);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index, shipmentInvoice.InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index, itm.Cost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemDescription").Key.Index, itm.ItemDescription);
                        SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index, itm.ItemNumber);
                        if (itm.ItemAlias != null)
                        {
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index,
                                itm.ItemAlias.POItemCode);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemDescription").Key.Index,
                                itm.ItemAlias.POItemDescription);
                        }


                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index, itm.Quantity);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index, itm.TotalCost);
                        SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index, itm.Units);

                        if (doRider && i < riderdetails.Count)
                        {
                            SetValue(workbook, i, header.First(x => x.Key.Column == "Packages").Key.Index, riderdetails[i].Packages);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "Warehouse").Key.Index, riderdetails[i].WarehouseCode);
                        }

                        i++;
                    }

                    DoMisMatches(shipmentInvoice, workbook);
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

        private static void DoMisMatches(ShipmentInvoice shipmentInvoice, Workbook workbook)
        {
            if (shipmentInvoice.ShipmentInvoicePOs.Select(x => x.POMISMatches).Any())
            {
                
                workbook.AddWorksheet("MisMatches");
                var header = "PONumber,InvoiceNo,POItemCode,INVItemCode,PODescription,INVDescription,POCost,INVCost,POQuantity,INVQuantity,POTotalCost,INVTotalCost".Split(',').ToList();
                header.ForEach(x => SetValue(workbook, 0, header.IndexOf(x), x));
                var i = 1;
                foreach (var mis in shipmentInvoice.ShipmentInvoicePOs.SelectMany(x => x.POMISMatches).ToList())
                {
                    SetValue(workbook, i, header.IndexOf("PONumber"), mis.PONumber);
                    SetValue(workbook, i, header.IndexOf("InvoiceNo"), mis.InvoiceNo);
                    SetValue(workbook, i, header.IndexOf("POItemCode"), mis.POItemCode);
                    SetValue(workbook, i, header.IndexOf("INVItemCode"), mis.INVItemCode);
                    SetValue(workbook, i, header.IndexOf("PODescription"), mis.PODescription);
                    SetValue(workbook, i, header.IndexOf("INVDescription"), mis.INVDescription);
                    SetValue(workbook, i, header.IndexOf("POCost"), mis.POCost);
                    SetValue(workbook, i, header.IndexOf("INVCost"), mis.INVCost);
                    SetValue(workbook, i, header.IndexOf("POQuantity"), mis.POQuantity);
                    SetValue(workbook, i, header.IndexOf("INVQuantity"), mis.INVQuantity);
                    SetValue(workbook, i, header.IndexOf("POTotalCost"), mis.POTotalCost);
                    SetValue(workbook, i, header.IndexOf("INVTotalCost"), mis.INVTotalCost);
                    i++;
                }
               
            }
        }

        public static Workbook CreateUnattachedShipmentWorkBook(string csvFilePath, UnAttachedWorkBookPkg summaryPkg )
        {
            Workbook workbook = new Workbook(csvFilePath, "Summary");
            SetValue(workbook,0,0,"Reference:");
            SetValue(workbook,0,1,summaryPkg.Reference);
            CreateUnMatchedWorkSheet(workbook, summaryPkg.UnMatchedPOs, summaryPkg.UnMatchedInvoices);
           
            workbook.Save();
            return workbook;
        }

        private static void CreateUnMatchedWorkSheet(Workbook workbook, List<ShipmentMIS_POs> unMatchedPOs, List<ShipmentMIS_Invoices> unMatchedInvoices)
        {
            if (!unMatchedInvoices.Any() && !unMatchedPOs.Any()) return;
            workbook.AddWorksheet("UnMatchedInvoicePOs");
            var superHeader = ",Match POs/Invoice List,,,Invoice List,,,,,,,,,PO List".Split(',').ToList();
            superHeader.ForEach(x => SetValue(workbook, 0, superHeader.IndexOf(x), x));
            var header = ",PONumber,InvoiceNo".Split(',').ToList();
            var invHeader =
                ",,,,InvoiceNo, InvoiceDate, ImportedLines, SubTotal, InvoiceTotal, SupplierCode, SourceFile"
                    .Split(',').Select(x => x.Trim()).ToList();
            var poheader = ",,,,,,,,,,,,,InvoiceNo,InvoiceDate,ImportedLines,SubTotal,SupplierCode,SourceFile".Split(',').Select(x => x.Trim()).ToList();
            header.ForEach(x => SetValue(workbook, 1, header.IndexOf(x), x));
            invHeader.ForEach(x => SetValue(workbook, 1, invHeader.IndexOf(x), x));
            poheader.ForEach(x => SetValue(workbook, 1, poheader.IndexOf(x), x));
            var isEnd = false;
            var i = 0;
            while (!isEnd)
            {
                if (i <= unMatchedInvoices.Count() - 1)
                {
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceNo)), unMatchedInvoices[i].InvoiceNo);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceDate)), unMatchedInvoices[i].InvoiceDate);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.ImportedLines)), unMatchedInvoices[i].ImportedLines);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SubTotal)), unMatchedInvoices[i].SubTotal);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceTotal)), unMatchedInvoices[i].InvoiceTotal);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SupplierCode)), unMatchedInvoices[i].SupplierCode);
                    SetValue(workbook, i+2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SourceFile)), new FileInfo(unMatchedInvoices[i].SourceFile).Name);
                }
                if (i <= unMatchedPOs.Count() - 1)
                {
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.InvoiceNo)), unMatchedPOs[i].InvoiceNo);
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.InvoiceDate)), unMatchedPOs[i].InvoiceDate);
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.ImportedLines)), unMatchedPOs[i].ImportedLines);
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.SubTotal)), unMatchedPOs[i].SubTotal);
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.SupplierCode)), unMatchedPOs[i].SupplierCode);
                    SetValue(workbook, i+2, poheader.IndexOf(nameof(ShipmentMIS_POs.SourceFile)), new FileInfo(unMatchedPOs[i].SourceFile).Name);
                }
                i++;
                if (i >= unMatchedInvoices.Count() && i >= unMatchedInvoices.Count()) isEnd = true;
            }
        }

        private static Workbook CreateShipmentWorkBook(int riderId, ShipmentInvoice shipmentInvoice, string csvFilePath,
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

        public static string SaveUnAttachedSummary(FileInfo file)
        {

            var workBook = NanoXLSX.Workbook.Load(file.FullName);
            workBook.SetCurrentWorksheet("Summary");
            var reference = workBook.CurrentWorksheet.GetCell(1, 0).Value.ToString();

            ImportUnMatchedInvoicePOs(workBook);

            return reference;
        }

        private static void ImportUnMatchedInvoicePOs(NanoXLSX.Workbook workBook)
        {
            workBook.SetCurrentWorksheet("UnMatchedInvoicePOs");
            var isEnd = false;
            var matches = new List<ShipmentInvoicePOManualMatches>();
            var row = 2;
            while (true)
            {
                if (!workBook.CurrentWorksheet.HasCell(1, row))
                {
                   break;
                }
                var po = workBook.CurrentWorksheet.GetCell(1, row).Value.ToString();
                var inv = workBook.CurrentWorksheet.GetCell(2, row).Value.ToString();
                matches.Add(new ShipmentInvoicePOManualMatches(){InvoiceNo = inv, PONumber = po, TrackingState = TrackingState.Added});
                row++;
            }

            if (!matches.Any()) return;
            using (var ctx = new EntryDataDSContext())
            {
                ctx.ShipmentInvoicePOManualMatches.AddRange(matches);
                ctx.SaveChanges();
            }
        }
    }

    public class UnAttachedWorkBookPkg
    {
        public List<ShipmentMIS_Invoices> UnMatchedInvoices { get; set; }
        public List<ShipmentMIS_POs> UnMatchedPOs { get; set; }
        public string Reference { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;
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
        private static string RideManualMatchesHeader = "WarehouseCode, RiderInvoiceNumber, InvoiceNo, Packages";
        private static string RiderManualMatchesTag = "Manual Matches";

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
                        foreach (var itm in pO.PurchaseOrders.EntryDataDetails.OrderBy(x => x.INVItems.FirstOrDefault()?.InvoiceDetails?.FileLineNumber??x.FileLineNumber).Where(x => pO.POMISMatches.All(m => m.POItemCode != x.ItemNumber && m.PODescription != x.ItemDescription /* m.PODetailsId != x.EntryDataDetailsId ---- Took this out because it Allowed the grouped po items to still show*/)))
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
                            
                            SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index, pOItem?.INVItemCode);

                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                                itm.Quantity);
                            SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index, itm.ItemNumber);
                            SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                                itm.TotalCost == 0 ? pOItem?.INVTotalCost??0 : itm.TotalCost);
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
                    foreach (var itm in shipmentInvoice.InvoiceDetails.OrderBy(x => x.FileLineNumber))
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
            var shipmentInvoicePoItemMisMatchesList = shipmentInvoice.ShipmentInvoicePOs.SelectMany(x => x.POMISMatches).ToList();
            if (!shipmentInvoicePoItemMisMatchesList.Any()) return;
            
                workbook.AddWorksheet("MisMatches");
                var header = "PONumber,InvoiceNo,POItemCode,INVItemCode,PODescription,INVDescription,POCost,INVCost,POQuantity,INVQuantity,POTotalCost,INVTotalCost,INVDetailsId,PODetailsId".Split(',').ToList();
                header.ForEach(x => SetValue(workbook, 0, header.IndexOf(x), x));
                var i = 1;
                
                foreach (var mis in shipmentInvoicePoItemMisMatchesList)
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
                    SetValue(workbook, i, header.IndexOf("INVDetailsId"), mis.INVDetailsId);
                    SetValue(workbook, i, header.IndexOf("PODetailsId"), mis.PODetailsId);
                    i++;
                }
            
        }

        public static string CreateUnattachedShipmentWorkBook(
            Tuple<string, int, string> client, UnAttachedWorkBookPkg summaryPkg)
        {
            var summaryWorkBook = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, "Imports",
                $"Summary-{client.Item3}-{client.Item2}.xlsx");
            if (File.Exists(summaryWorkBook)) File.Delete(summaryWorkBook);
            Workbook workbook = new Workbook(summaryWorkBook, "Summary");
            CreateSummarySheet(summaryPkg, workbook);
            CreateRiderInvoiceSheet(summaryPkg, workbook);
            CreateUnMatchedWorkSheet(workbook, summaryPkg.UnMatchedPOs, summaryPkg.UnMatchedInvoices);
            CreateClassificationsSheet(workbook, summaryPkg.Classifications);
            workbook.Save();
            return summaryWorkBook;
        }

        private static void CreateClassificationsSheet(Workbook workbook, List<ShipmentInvoicePOItemData> summaryPkgClassifications)
        {
            workbook.AddWorksheet("Classifications");
            var currentline = 0;

            WriteTable( summaryPkgClassifications.Select(x => (dynamic)x).ToList() , workbook, currentline, "PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, TariffCode, POCost, INVCost, POQuantity, INVQuantity, POTotalCost, INVTotalCost", "Classifications:");
        }

        private static void CreateRiderInvoiceSheet(UnAttachedWorkBookPkg summaryPkg, Workbook workbook)
        {
            workbook.AddWorksheet("RiderInvoices");
            var currentline = 0;

            WriteTable(new List<dynamic>(){summaryPkg.RiderSummary}, workbook, currentline, "ETA, DocumentDate, Packages, WarehouseCode, InvoiceNumber, GrossWeightKg, CubicFeet, Code", "Rider Summary:");

            currentline += 3;

            WriteTable(new List<dynamic>(){ summaryPkg.PackagesSummary}, workbook, currentline, "BLPackages, RiderPackages, InvoicePackages, Diff", "Packages Summary");

            currentline += 3;
            WriteTable(summaryPkg.RiderManualMatches.Select(x => (dynamic)x).ToList(), workbook, currentline, RideManualMatchesHeader, "Manual Matches");

            currentline += 2 + summaryPkg.RiderManualMatches.Count;
            WriteTable(summaryPkg.UnAttachedRiderDetails.Select(x => (dynamic)x).ToList(), workbook, currentline, "Shipper, WarehouseCode, InvoiceNumber, Pieces", "Rider Details with No Invoices");

            currentline += 2 + summaryPkg.UnAttachedRiderDetails.Count;
            WriteTable(summaryPkg.UnAttachedInvoices.Select(x => (dynamic)x).ToList(), workbook, currentline, "InvoiceNo, InvoiceTotal, ImportedLines, SupplierCode", "Invoices Not Linked To Rider");

            currentline += 2 + summaryPkg.UnAttachedInvoices.Count;
            WriteTable(summaryPkg.RiderDetails.Select(x => (dynamic)x).ToList(), workbook, currentline, "Shipper, WarehouseCode, InvoiceNumber, Pieces", "All Rider Details");

            var pkgInvoices = summaryPkg.Invoices.Select(z => new{ InvoiceNo = z.InvoiceNo,PONumber = z.ShipmentInvoicePOs.FirstOrDefault()?.PurchaseOrders?.PONumber, z.InvoiceTotal, z.ImportedLines, z.SupplierCode, Packages = z.ShipmentRiderInvoice.Where(r => r.RiderID == summaryPkg.RiderSummary.Id).Sum(w => w.Packages)});

            currentline += 2 + summaryPkg.RiderDetails.Count;
            WriteTable(pkgInvoices.Select(x => (dynamic)x).ToList(), workbook, currentline, "InvoiceNo, PONumber, InvoiceTotal, ImportedLines, SupplierCode, Packages", "All Invoices");

            
        }

        private static void WriteTable(List<dynamic> table, Workbook workbook, int currentline, string headerString,
            string tableName)
        {
            try
            {
                SetValue(workbook, currentline, 0, tableName);
                var header = headerString.Split(',').Select(x => x.Trim()).ToList();

                header.ForEach(x =>
                {
                    SetValue(workbook, currentline, header.IndexOf(x) + 1, x);
                    for (int i = 0; i < table.Count; i++)
                    {
                        var itm = table[i];
                        SetValue(workbook, currentline + i + 1, header.IndexOf(x) + 1,
                            itm.GetType().GetProperty(x)?.GetValue(itm)?.ToString());
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void ReadTable<T>(NanoXLSX.Workbook workbook, string sheetName, string headerString,
            string tableTag) where T : class, ITrackable, new()
        {
            try
            {

                workbook.SetCurrentWorksheet(sheetName);
                var header = headerString.Split(',').Select(x => x.Trim()).ToList();
                var currentRow = 0;
                var currentColumn = 0;
                var lst = new List<T>();
                var readStarted = false;
                while (true)
                {
                    if (currentRow > 100) break;

                    if (workbook.CurrentWorksheet.HasCell(currentColumn, currentRow) &&
                        workbook.CurrentWorksheet.GetCell(currentColumn, currentRow).Value.ToString() == tableTag)
                    {
                        currentColumn++;
                        if (header.All(x =>
                            workbook.CurrentWorksheet.GetCell(currentColumn + header.IndexOf(x), currentRow).Value
                                .ToString() == x))
                        {
                            readStarted = true;
                            currentRow++;
                        }
                    }

                    if (readStarted)
                    {
                        
                        
                            
                            if (!workbook.CurrentWorksheet.HasCell(currentColumn, currentRow)) break;
                            var itm = new T() {TrackingState = TrackingState.Added};
                            header.ForEach(x =>
                            {
                                var prop = itm.GetType().GetProperty(x);
                                prop?.SetValue(itm,
                                    Convert.ChangeType(workbook.CurrentWorksheet
                                        .GetCell(currentColumn + header.IndexOf(x), currentRow)
                                        .Value, prop.PropertyType));
                            });
                            lst.Add(itm);

                        
                    }

                    currentRow++;
                }

                if (!lst.Any()) return;
                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Set<T>().AddRange(lst);
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateSummarySheet(UnAttachedWorkBookPkg summaryPkg, Workbook workbook)
        {
            try
            {

           
            workbook.SetCurrentWorksheet("Summary");
            var currentline = 0;
            SetValue(workbook, currentline, 0, "Reference:");
            SetValue(workbook, currentline, 1, summaryPkg.Reference);

            currentline += 2;
            SetValue(workbook, currentline, 0, "List of Invoices");
            
            var invHeader =
                "InvoiceNo,PONumber, InvoiceDate, ImportedLines, SubTotal, InvoiceTotal, ImportedTotalDifference, PO/Inv Total Diff, PO/Inv MisMatches, SupplierCode, SourceFile"
                    .Split(',').Select(x => x.Trim()).ToList();
            currentline++;
            invHeader.ForEach(x => SetValue(workbook, currentline, invHeader.IndexOf(x), x));

            currentline++;

            var i = 0;
            while (true)
            {
                if (i > summaryPkg.Invoices.Count() - 1) break;
               
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceNo)),
                        summaryPkg.Invoices[i].InvoiceNo);
                    SetValue(workbook, currentline, invHeader.IndexOf("PONumber"),
                        summaryPkg.Invoices[i].ShipmentInvoicePOs.FirstOrDefault()?.PurchaseOrders?.PONumber??"");
                    SetValue(workbook, currentline , invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceDate)),
                        summaryPkg.Invoices[i].InvoiceDate);
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.ImportedLines)),
                        summaryPkg.Invoices[i].ImportedLines);
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SubTotal)),
                        summaryPkg.Invoices[i].SubTotal);
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceTotal)),
                        summaryPkg.Invoices[i].InvoiceTotal);
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.ImportedTotalDifference)),
                        summaryPkg.Invoices[i].ImportedTotalDifference);


                    SetValue(workbook, currentline, invHeader.IndexOf("PO/Inv Total Diff"),
                        summaryPkg.Invoices[i].SubTotal - summaryPkg.Invoices[i].ShipmentInvoicePOs.FirstOrDefault()?.PurchaseOrders.EntryDataDetails.Sum(x => x.Cost * x.Quantity));

                    SetValue(workbook, currentline, invHeader.IndexOf("PO/Inv MisMatches"),
                         summaryPkg.Invoices[i].ShipmentInvoicePOs.FirstOrDefault()?.POMISMatches.Count());


                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SupplierCode)),
                        summaryPkg.Invoices[i].SupplierCode);
                    SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SourceFile)),
                        new FileInfo(summaryPkg.Invoices[i].SourceFile).Name);
                

                i++;
                currentline++;
            }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateUnMatchedWorkSheet(Workbook workbook, List<ShipmentMIS_POs> unMatchedPOs, List<ShipmentMIS_Invoices> unMatchedInvoices)
        {
            try
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
                if (i >= unMatchedInvoices.Count() && i >= unMatchedPOs.Count()) isEnd = true;
            }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static Workbook CreateShipmentWorkBook(int riderId, ShipmentInvoice shipmentInvoice, string csvFilePath,
            Dictionary<(string Column, int Index), FileTypeMappings> header,
            out int invoiceRow, out List<ShipmentRiderInvoice> riderdetails, out bool doRider)
        {
            try
            {


            Workbook workbook = new Workbook(csvFilePath, "POTemplate"); // Create new workbook with a worksheet called Sheet1
            var headerRow = 0;

            header.ForEach(x => { SetValue(workbook, headerRow, x.Key.Index, x.Value.DestinationName); });
            invoiceRow = 1;
            SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceTotal)).Key.Index,
                shipmentInvoice.InvoiceTotal.GetValueOrDefault());

                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                    shipmentInvoice.InvoiceDate.GetValueOrDefault());

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

            riderdetails = shipmentInvoice.ShipmentRiderInvoice.Where(x => x.ShipmentRiderDetails != null && x.RiderID == riderId && x.Packages > 0).ToList();
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

        public static string SaveUnAttachedSummary(FileInfo file)
        {

            var workBook = NanoXLSX.Workbook.Load(file.FullName);
            workBook.SetCurrentWorksheet("Summary");
            var reference = workBook.CurrentWorksheet.GetCell(1, 0).Value.ToString();

            ImportUnMatchedInvoicePOs(workBook);
            ReadTable<ShipmentInvoiceRiderManualMatches>(workBook, "RiderInvoices", RideManualMatchesHeader, RiderManualMatchesTag);

            return reference;
        }



        private static void ImportUnMatchedInvoicePOs(NanoXLSX.Workbook workBook)
        {
            try
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
                    matches.Add(new ShipmentInvoicePOManualMatches()
                    {
                        InvoiceNo = inv,
                        PONumber = po,
                        TrackingState = TrackingState.Added
                    });
                    row++;
                }

                if (!matches.Any()) return;
                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var mat in matches)
                    {
                        if(ctx.ShipmentInvoicePOManualMatches.FirstOrDefault(x => x.InvoiceNo == mat.InvoiceNo && x.PONumber == mat.PONumber) != null) continue;
                        ctx.ShipmentInvoicePOManualMatches.Add(mat);
                    }
                    
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }

    public class UnAttachedWorkBookPkg
    {
        public List<ShipmentMIS_Invoices> UnMatchedInvoices { get; set; }
        public List<ShipmentMIS_POs> UnMatchedPOs { get; set; }
        public string Reference { get; set; }
        public List<ShipmentInvoice> Invoices { get; set; }
        public List<ShipmentInvoice> UnAttachedInvoices { get; set; }
        public List<ShipmentRiderDetails> UnAttachedRiderDetails { get; set; }
        public List<ShipmentRiderDetails> RiderDetails { get; set; }
        public List<ShipmentInvoiceRiderManualMatches> RiderManualMatches { get; set; }
        public ShipmentRiderEx RiderSummary { get; set; }
        public List<ShipmentInvoicePOItemData> Classifications { get; set; }
        public PackagesSummary PackagesSummary { get; set; }


    }
    public class PackagesSummary
        {
            public int BLPackages { get; set; }
            public int RiderPackages { get; set; }
            public int InvoicePackages { get; set; }

            public int Diff => (BLPackages == 0 ? RiderPackages : BLPackages) - InvoicePackages;
        }
}

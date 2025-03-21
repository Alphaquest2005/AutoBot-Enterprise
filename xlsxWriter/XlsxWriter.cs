using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using MoreLinq.Extensions;
using PicoXLSX;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using FormatException = PicoXLSX.FormatException;

namespace xlsxWriter
{
    public class XlsxWriter
    {
        private static readonly string RideManualMatchesHeader =
            "WarehouseCode, RiderInvoiceNumber, InvoiceNo, Packages";

        private static readonly string RiderManualMatchesTag = "Manual Matches";

        public static List<(string reference, string filepath)> CreateCSV(string shipmentInvoiceKey,
            List<ShipmentInvoice> shipmentInvoices, string emailId,
            List<(string WarehouseCode, int Packages, string InvoiceNo)> packingDetails)
        {
            var riderId = new CoreEntitiesContext().Emails.FirstOrDefault(x => x.EmailId == emailId && x.MachineName == Environment.MachineName)?.EmailUniqueId ?? 0;
            return CreateCSV(shipmentInvoices, riderId,packingDetails);
        }

        public static List<(string reference, string filepath)> CreateCSV(
            List<ShipmentInvoice> shipmentInvoices, int riderId,
            List<(string WarehouseCode, int Packages, string InvoiceNo)> packingDetails)
        {
            try
            {
                var pdfFilePath = "";
                var csvFilePath = "";
                var csvs = new List<(string reference, string filepath)>();
                var poTemplate = new CoreEntitiesContext().FileTypes
                    .Include(x => x.FileTypeMappings)
                    .First(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.POTemplate);




                var header = poTemplate.FileTypeMappings.OrderBy(x => x.Id)
                    .Select(x => new { value = x, index = poTemplate.FileTypeMappings.IndexOf(x) })
                    .ToDictionary(x => (Column: x.value.OriginalName, Index: x.index), v => v.value);


                ShipmentInvoice parent = null;
                Workbook workbook = null;
                var invoiceRow = 0;
                var doRider = false;
                foreach (var shipmentInvoice in shipmentInvoices)
                {


                    try
                    {

                        var pdfFile = new FileInfo(shipmentInvoice.SourceFile);

                        var packingLst = packingDetails.Where(x =>
                                (shipmentInvoice.ShipmentInvoicePOs.Any() && shipmentInvoice.ShipmentInvoicePOs.Any(z => z.ShipmentInvoice.InvoiceNo == x.InvoiceNo)
                                || (!shipmentInvoice.ShipmentInvoicePOs.Any() && x.InvoiceNo == shipmentInvoice.InvoiceNo)))
                            .ToList();

                        var isCombined = !packingLst.Any() 
                                         ||  packingLst.FirstOrDefault(x => x.InvoiceNo == shipmentInvoice.InvoiceNo).Packages == 0 
                                         || (shipmentInvoice.ShipmentInvoicePOs.Count == 1 
                                                && shipmentInvoice.ShipmentInvoicePOs.First().PurchaseOrders.ShipmentInvoicePOs.Count > 1 && shipmentInvoices.Count > 1
                                                && parent != null);


                        
                            if (!isCombined)
                            {
                                parent = shipmentInvoice;


                                if (parent.ShipmentInvoicePOs.Where(x => x.EntryData_Id != 0).Any(x => !String.Equals(x.PurchaseOrders.PONumber, "Null",
                                        StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    csvFilePath = Path.Combine(pdfFile.DirectoryName,
                                        $"{parent.ShipmentInvoicePOs.First().PurchaseOrders.PONumber.Replace("/", "-")}.xlsx");
                                    csvs.Add((parent.ShipmentInvoicePOs.First().PurchaseOrders.PONumber, csvFilePath));
                                }
                                else
                                {
                                    csvFilePath = Path.Combine(pdfFile.DirectoryName,
                                        $"{parent.InvoiceNo.Replace("/", "-")}.xlsx");
                                    csvs.Add((parent.InvoiceNo, csvFilePath));
                                }

                                workbook = CreateShipmentWorkBook(riderId, parent, csvFilePath, header,
                                    out invoiceRow, packingLst, out doRider);
                            }
                       

                        if (workbook == null) continue;

                        WriteInvHeader(shipmentInvoice, header, workbook, isCombined);

                        if (shipmentInvoice.ShipmentInvoicePOs.Where(x => x.EntryData_Id != 0).Any(x => !String.Equals(x.PurchaseOrders.PONumber,
                                "Null", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            foreach (var pO in shipmentInvoice.ShipmentInvoicePOs)
                            {
                                pdfFilePath = !isCombined || shipmentInvoice.ShipmentInvoicePOs.First().PurchaseOrders.ShipmentInvoicePOs.Count == 1 
                                            ? Path.Combine(pdfFile.DirectoryName, $"{pO.PurchaseOrders.PONumber.Replace("/", "-")}.pdf")
                                            : Path.Combine(pdfFile.DirectoryName, $"{shipmentInvoice.InvoiceNo.Replace("/", "-")}.pdf");


                                WritePOToFile(pO, workbook, header, doRider, packingLst, invoiceRow,
                                    (shipmentInvoice.ShipmentInvoicePOs.Count > 1));//


                                if (pdfFile.FullName != pdfFilePath)
                                {
                                    //compare pdfFile and pdfFilePath ignore case
                                    if (String.Compare(pdfFile.FullName, pdfFilePath, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        File.Move(pdfFile.FullName, pdfFile.FullName + "1");
                                        File.Move(pdfFile.FullName +"1", pdfFilePath);
                                    }
                                    else
                                    {
                                         File.Copy(pdfFile.FullName, pdfFilePath, true);
                                    }


                                   
                                }
                                    
                                csvs.Add((pO.PurchaseOrders.PONumber, pdfFilePath));
                            }

                            DoMisMatches(shipmentInvoice, workbook);
                            workbook.Save();
                        }
                        else
                        {
                            pdfFilePath = Path.Combine(pdfFile.DirectoryName,
                                $"{shipmentInvoice.InvoiceNo.Replace("/", "-")}.pdf");


                            WriteInvToFile(shipmentInvoice, workbook, header, doRider, packingLst);

                            DoMisMatches(shipmentInvoice, workbook);
                            workbook.Save();
                            if (pdfFile.FullName.ToUpper() != pdfFilePath.ToUpper())
                                File.Copy(pdfFile.FullName, pdfFilePath, true);
                            csvs.Add((shipmentInvoice.InvoiceNo, pdfFilePath));
                        }
                    }
                    catch (Exception)
                    {

                    }
                }


                return csvs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static void WriteInvToFile(ShipmentInvoice shipmentInvoice, Workbook workbook,
            Dictionary<(string Column, int Index), FileTypeMappings> header,
            bool doRider, List<(string WarehouseCode, int Packages, string InvoiceNo)> packageDetails)
        {
            workbook.SetCurrentWorksheet("POTemplate");
            var i = workbook.CurrentWorksheet
                .GetLastRowNumber(); // == 1 ? 1 : workbook.CurrentWorksheet.GetLastRowNumber() + 1;
            var starti = i;
            foreach (var itm in shipmentInvoice.InvoiceDetails.OrderBy(x => x.FileLineNumber))
            {
                SetValue(workbook, i, header.First(x => x.Key.Column == "InvoiceNo").Key.Index,
                    shipmentInvoice.InvoiceNo);
                SetValue(workbook, i,
                    header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                    shipmentInvoice.InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));

                SetValue(workbook, i,
                    header.First(x => x.Key.Column == nameof(shipmentInvoice.PONumber)).Key.Index,
                    shipmentInvoice.PONumber);

                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                    itm.Cost);
                SetValue(workbook, i,
                    header.First(x => x.Key.Column == "SupplierItemDescription").Key.Index,
                    itm.ItemDescription);
                SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index,
                    itm.ItemNumber);
                if (itm.ItemAlias != null)
                {
                    SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index,
                        itm.ItemAlias.POItemCode);
                    SetValue(workbook, i, header.First(x => x.Key.Column == "POItemDescription").Key.Index,
                        itm.ItemAlias.POItemDescription);
                }

                if (itm.Volume != null && itm.Volume.Units == "Gallons")
                    SetValue(workbook, i, header.First(x => x.Key.Column == "Gallons").Key.Index,
                        itm.Quantity * itm.Volume.Quantity);

                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                    itm.Quantity);
                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                    itm.TotalCost);
                SetFormula(workbook, i, header.First(x => x.Key.Column == "Total").Key.Index,
                    $"=O{i + 1}*K{i + 1}");
                SetFormula(workbook, i, header.First(x => x.Key.Column == "TotalCost Vs Total").Key.Index,
                    $"=P{i + 1}-Q{i + 1}");

                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index,
                    itm.Units);

                SetValue(workbook, i, header.First(x => x.Key.Column == "TariffCode").Key.Index, itm.TariffCode);

                if (doRider && i < packageDetails.Count)
                    foreach (var riderdetail in packageDetails)
                    {
                        SetValue(workbook, i, header.First(x => x.Key.Column == "Packages").Key.Index,
                            riderdetail.Packages);
                        SetValue(workbook, i, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                            riderdetail.WarehouseCode);
                    }

                i++;
            }

            WriteSummary(workbook, header, i-1, starti, "TotalCost");
            WriteSummary(workbook, header, i-1, starti, "Total");
        }

        private static void WritePOToFile(ShipmentInvoicePOs pO, Workbook workbook,
            Dictionary<(string Column, int Index), FileTypeMappings> header, bool doRider,
            List<(string WarehouseCode, int Packages, string InvoiceNo)> packageDetails, int invoiceRow, bool combineedFile)
        {
            workbook.SetCurrentWorksheet("POTemplate");
            var i = workbook.CurrentWorksheet
                .GetLastRowNumber(); //== 1? 1: workbook.CurrentWorksheet.GetLastRowNumber() + 1;
            if (combineedFile
                //&& workbook.CurrentWorksheet.HasCell(header.First(x => x.Key.Column == nameof(pO.PurchaseOrders.PONumber)).Key.Index, i)
                //&& !string.IsNullOrEmpty(workbook.CurrentWorksheet.GetCell(header.First(x => x.Key.Column == nameof(pO.PurchaseOrders.PONumber)).Key.Index, i).Value.ToString())
                && i > 1)
            {
                i += 1;
            }

            //var isPOTotalCostZero = Math.Round(pO.PurchaseOrders.EntryDataDetails.Sum(x => x.TotalCost) - pO.ShipmentInvoice.SubTotal??0,2) == 0;
            var isPOTotalZero = Math.Round(pO.PurchaseOrders.EntryDataDetails.Sum(x => x.Cost * x.Quantity) - pO.ShipmentInvoice.SubTotal ?? 0, 2) == 0;

            var isINVTotalCostZero = Math.Round(pO.PurchaseOrders.EntryDataDetails.Sum(x => x.INVItems.Sum(z => z.INVTotalCost)) - pO.ShipmentInvoice.SubTotal ?? 0, 2) == 0;
            //var isINVTotalZero = Math.Round(pO.PurchaseOrders.EntryDataDetails.Sum(x => x.INVItems.Sum(z => z.INVCost * z.INVQuantity)) - pO.ShipmentInvoice.SubTotal ?? 0, 2) == 0;


            var starti = i;
            var goodDetails = pO.PurchaseOrders.EntryDataDetails
                .OrderBy(x =>
                    x.INVItems.FirstOrDefault()?.InvoiceDetails?.FileLineNumber ?? x.FileLineNumber)
                .Where(x => x.INVItems.Any(z => packageDetails.Any(p => p.InvoiceNo == z.InvoiceNo)))
                .Where(x => x.INVItems.Any() || (!x.INVItems.Any() &&
                                                 pO.POMISMatches.All(m => m.POItemCode != x.ItemNumber &&
                                                                          m.PODescription != x.ItemDescription /* m.PODetailsId != x.EntryDataDetailsId ---- Took this out because it Allowed the grouped po items to still show*/)));
            foreach (var itm in goodDetails)
            {
                var pOItem = itm.INVItems.OrderByDescending(x => x.RankNo).FirstOrDefault();

                var invTotalCost = itm.INVItems.Select(x => x.INVTotalCost?? x.INVQuantity * x.INVCost).Sum();

                SetValue(workbook, i,
                    header.First(x => x.Key.Column == nameof(pO.PurchaseOrders.PONumber)).Key.Index,
                    pO.PurchaseOrders.PONumber);
                SetValue(workbook, i,
                    header.First(x => x.Key.Column == nameof(ShipmentInvoice.InvoiceDate)).Key.Index,
                    pO.PurchaseOrders.EntryDataDate.ToString("yyyy-MM-dd"));

               

                //if (isPOTotalCostZero)
                //{
                //    SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                //        pOItem?.INVQuantity == itm.Quantity && pOItem?.INVCost != null ? pOItem?.POTotalCost/pOItem.POQuantity : itm.Cost);

                //    SetValue(workbook, i,
                //        header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                //        pOItem.POTotalCost);
                //}


                if (isINVTotalCostZero)
                {
                    
                    SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                    pOItem?.INVQuantity == itm.Quantity && pOItem?.INVCost != null ? pOItem.INVTotalCost / pOItem.POQuantity : itm.Cost);

                    SetValue(workbook, i,
                        header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                        invTotalCost);
                }
                else
                //if (isPOTotalZero)
                {
                    SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                        pOItem?.INVQuantity == itm.Quantity && pOItem?.INVCost != null ? itm.Cost : itm.Cost);

                    SetValue(workbook, i,
                        header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                        itm.Cost * itm.Quantity);
                }

                //if (isINVTotalZero)
                //{
                //    SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Cost)).Key.Index,
                //        pOItem?.INVQuantity == itm.Quantity && pOItem?.INVCost != null ? pOItem?.INVCost : itm.Cost);

                //    SetValue(workbook, i,
                //        header.First(x => x.Key.Column == nameof(itm.TotalCost)).Key.Index,
                //        pOItem.INVCost * pOItem.INVQuantity);
                //}
                SetValue(workbook, i,
                    header.First(x => x.Key.Column == "INVTotalCost").Key.Index,
                    invTotalCost);

                SetValue(workbook, i,
                    header.First(x => x.Key.Column == "POTotalCost").Key.Index,
                    pOItem?.POTotalCost ?? 0);

                SetValue(workbook, i, header.First(x => x.Key.Column == "POItemDescription").Key.Index,
                    itm.ItemDescription);
                SetValue(workbook, i,
                    header.First(x => x.Key.Column == "SupplierItemDescription").Key.Index,
                    pOItem?.INVDescription);

                SetValue(workbook, i, header.First(x => x.Key.Column == "SupplierItemNumber").Key.Index,
                    pOItem?.INVItemCode);

                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Quantity)).Key.Index,
                    itm.Quantity);

                if (pOItem != null && pOItem.Gallons > 0)
                    SetValue(workbook, i, header.First(x => x.Key.Column == "Gallons").Key.Index,
                        itm.Quantity * pOItem.Gallons);

                SetValue(workbook, i, header.First(x => x.Key.Column == "POItemNumber").Key.Index,
                    itm.ItemNumber);
                

                SetFormula(workbook, i, header.First(x => x.Key.Column == "Total").Key.Index,
                    $"=O{i + 1}*K{i + 1}");
                SetFormula(workbook, i, header.First(x => x.Key.Column == "TotalCost Vs Total").Key.Index,
                    $"=P{i + 1}-Q{i + 1}");

                SetValue(workbook, i, header.First(x => x.Key.Column == nameof(itm.Units)).Key.Index,
                    itm.Units);
                if (doRider && i < packageDetails.Count) //
                    foreach (var riderdetail in packageDetails)
                    {
                        SetValue(workbook, i + invoiceRow,
                            header.First(x => x.Key.Column == "Packages").Key.Index,
                            riderdetail.Packages);
                        SetValue(workbook, i + invoiceRow,
                            header.First(x => x.Key.Column == "Warehouse").Key.Index,
                            riderdetail.WarehouseCode);
                    }

                i++;
            }

            /// write out summary
            WriteSummary(workbook, header, i-1, starti, "TotalCost");
            WriteSummary(workbook, header, i-1, starti, "Total");
        }

        private static void WriteSummary(Workbook workbook,
            Dictionary<(string Column, int Index), FileTypeMappings> header, int i, int starti, string colHeader)
        {
            try
            {


                var r = i + 3;

              
                SetFormula(workbook, r, header.First(x => x.Key.Column == colHeader).Key.Index,
                    $"=Sum({GetOrAddCell(workbook, header, starti, colHeader).CellAddress}:{GetOrAddCell(workbook, header, i, colHeader).CellAddress})");
                SetFormula(workbook, r + 1, header.First(x => x.Key.Column == colHeader).Key.Index,
                    $"=({GetOrAddCell(workbook, header, starti, "TotalInternalFreight").CellAddress} + " +
                    $"{GetOrAddCell(workbook, header, starti, "TotalInsurance").CellAddress} + " +
                    $"{GetOrAddCell(workbook, header, starti, "TotalOtherCost").CellAddress} - " +
                    $"{GetOrAddCell(workbook, header, starti, "TotalDeduction").CellAddress}" +
                    $")");

                SetFormula(workbook, r + 2, header.First(x => x.Key.Column == colHeader).Key.Index,
                    $"=({GetOrAddCell(workbook, header, r, colHeader).CellAddress} + " +
                    $"{GetOrAddCell(workbook, header, r + 1, colHeader).CellAddress}" +
                    $")");

                SetFormula(workbook, r + 4, header.First(x => x.Key.Column == colHeader).Key.Index,
                    $"=({GetOrAddCell(workbook, header, starti, "InvoiceTotal").CellAddress} - " +
                    $"{GetOrAddCell(workbook, header, r + 2, colHeader).CellAddress}" +
                    $")");

                //if (GetOrAddCell(workbook, header, r + 4, colHeader).Value.ToString() != "0")
                //    GetOrAddCell(workbook, header, r + 4, colHeader).CellStyle.CurrentFill =
                //        new Style.Fill("Black", "Red");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static Cell GetOrAddCell(Workbook workbook, Dictionary<(string Column, int Index), FileTypeMappings> header, int row, string ColName)
        {
            if (!workbook.CurrentWorksheet.HasCell(header.First(x => x.Key.Column == ColName).Key.Index, row))
                workbook.CurrentWorksheet.AddCell("", header.First(x => x.Key.Column == ColName).Key.Index, row);
            
            return workbook.CurrentWorksheet.GetCell(header.First(x => x.Key.Column == ColName).Key.Index, row);
        }

        private static Cell GetOrAddCell(Workbook workbook, List<string> header, int row, string ColName)
        {
            if (!workbook.CurrentWorksheet.HasCell(header.IndexOf(ColName), row))
                workbook.CurrentWorksheet.AddCell("", header.IndexOf(ColName), row);

            return workbook.CurrentWorksheet.GetCell(header.IndexOf(ColName), row);
        }

        private static void DoMisMatches(ShipmentInvoice shipmentInvoice, Workbook workbook)
        { 
            var pOs = shipmentInvoice.ShipmentInvoicePOs.Select(x => x.EntryData_Id).ToList();

            var shipmentInvoicePoItemMisMatchesEnumerable = GetMisMatches();
            var matchesList = shipmentInvoicePoItemMisMatchesEnumerable
                .Where(x => x.INVId == shipmentInvoice.Id && pOs.Contains(x.POId ?? 0)).ToList();


            var preMisMatchesList = shipmentInvoicePoItemMisMatchesEnumerable
                .Where(x => x != null && ((x.INVId == shipmentInvoice.Id && (x.ShipmentInvoicePOs == null || (x.ShipmentInvoicePOs != null && x.ShipmentInvoicePOs.ShipmentInvoice.InvoiceDetails.Any(z => /*!z.POItems.Any() &&*/ (x.INVDetailsId == null || z.Id == x.INVDetailsId))))))
                            ||
                             (pOs.Contains(x.POId ?? 0) && (x.ShipmentInvoicePOs == null || (x.ShipmentInvoicePOs != null && x.ShipmentInvoicePOs.PurchaseOrders.EntryDataDetails.Any(z => /*!z.INVItems.Any() &&*/ (x.PODetailsId == null || z.EntryDataDetailsId == x.PODetailsId))))
                                 /* ------put back because it suggest bad matches be imported ..*/)).ToList();

            var shipmentInvoicePoItemMisMatchesList = preMisMatchesList.Where(x =>
                matchesList.All(z => z.PODetailsId != x.PODetailsId || z.INVDetailsId != x.INVDetailsId)).ToList();
            
            ///////////////// replace this because the keys not working properly with nulls

            //var shipmentInvoicePoItemMisMatchesList = shipmentInvoice
            //    .ShipmentInvoicePOs
            //    .SelectMany(x => x.POMISMatches)
            //    //.Where(x => (x.INVQuantity != 0 && x.INVQuantity != null) && (x.POQuantity != 0 && x.POQuantity != null))
            //    .OrderBy(x => x.INVTotalCost).ThenBy(x => x.POTotalCost)
            //    .DistinctBy(x => new {x.INVDetailsId, x.PODetailsId})// re intro this because multiple pos to one invoice
            //    .ToList();
            

           


            if (!shipmentInvoicePoItemMisMatchesList.Any()) return;

            var rematched = ReMatchOnItemDescription(shipmentInvoicePoItemMisMatchesList, shipmentInvoice);

            //rematched = ReMatchOnPrice(rematched);

            var filteredMatches = rematched
                .Where(x => //(x.PODetailsId == null || x.PODetailsId != null)  &&
                            x.InvoiceNo == null ||(x.InvoiceNo != null && x.InvoiceNo == shipmentInvoice.InvoiceNo)).ToList();

            var header =
                "PONumber,POItemCode,PODescription,POCost,POQuantity,POTotalCost,PODetailsId,InvoiceNo,INVItemCode,INVDescription,INVCost,INVQuantity,INVSalesFactor,INVTotalCost,INVDetailsId"
                    .Split(',').ToList();

            if (!workbook.Worksheets.Exists(x => x.SheetName == "MisMatches"))
            {
                workbook.AddWorksheet("MisMatches");
                header.ForEach(x => SetValue(workbook, 0, header.IndexOf(x), x));
            }
            else
            {
                workbook.SetCurrentWorksheet("MisMatches");
            }

            var i = workbook.CurrentWorksheet.GetLastRowNumber() + 1;
            var startRow = i;
            foreach (var mis in filteredMatches)
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
                SetValue(workbook, i, header.IndexOf("INVSalesFactor"), mis.INVSalesFactor);
                SetValue(workbook, i, header.IndexOf("POTotalCost"), mis.POTotalCost);
                SetValue(workbook, i, header.IndexOf("INVTotalCost"), mis.INVTotalCost);
                SetValue(workbook, i, header.IndexOf("INVDetailsId"), mis.INVDetailsId);
                SetValue(workbook, i, header.IndexOf("PODetailsId"), mis.PODetailsId);

                SetFormula(workbook, i, header.Count + 1,
                    $"={GetOrAddCell(workbook, header, i, "POTotalCost").CellAddress}-{GetOrAddCell(workbook, header, i, "INVTotalCost").CellAddress}");

                i++;
            }

            SetFormula(workbook, i + 2, header.IndexOf("POTotalCost"),
                $"=Sum({GetOrAddCell(workbook, header, startRow, "POTotalCost").CellAddress}:{GetOrAddCell(workbook, header, i, "POTotalCost").CellAddress})");

            SetFormula(workbook, i + 2, header.IndexOf("INVTotalCost"),
                $"=Sum({GetOrAddCell(workbook, header, startRow, "INVTotalCost").CellAddress}:{GetOrAddCell(workbook, header, i, "INVTotalCost").CellAddress})");
        }

        private static List<ShipmentInvoicePOItemMISMatches> ReMatchOnPrice(List<ShipmentInvoicePOItemMISMatches> shipmentInvoicePoItemMisMatchesList)
        {
            var POItems = shipmentInvoicePoItemMisMatchesList
                                                .Where(x => !string.IsNullOrEmpty(x.PONumber) && string.IsNullOrEmpty(x.InvoiceNo))
                                                .Select(x => (x.PONumber, x.POItemCode, x.PODescription, x.POCost, x.POQuantity, x.POTotalCost, x.PODetailsId, WordLst: GetWords(x.PODescription)))
                                                .ToList();

            var INVItems = shipmentInvoicePoItemMisMatchesList
                                                    .Where(x => string.IsNullOrEmpty(x.PONumber) && !string.IsNullOrEmpty(x.InvoiceNo))
                                                    .Select(x => (x.InvoiceNo, x.INVItemCode, x.INVDescription, x.INVCost, x.INVQuantity, x.INVTotalCost, x.INVDetailsId, WordLst: GetWords(x.INVDescription)))
                                                    .ToList();
            foreach (var poItem in POItems)
            {
                var match = INVItems.Select(x => new { itm = x, matches = poItem.WordLst.Intersect(x.WordLst).ToList() })
                    .Where(x => x.matches.Count >= 1)
                    .OrderBy(x => Math.Abs(poItem.POCost.GetValueOrDefault() - x.itm.INVCost.GetValueOrDefault()))
                    .OrderBy(x => Math.Abs(poItem.POQuantity.GetValueOrDefault() - x.itm.INVQuantity.GetValueOrDefault()))
                    //.OrderBy(x => Math.Abs(poItem.POTotalCost.GetValueOrDefault() - x.itm.INVTotalCost.GetValueOrDefault()))
                    .ThenByDescending(x => x.matches.Count).ToList();
                if (!match.Any()) continue;

                var po = shipmentInvoicePoItemMisMatchesList.First(x => x.PODetailsId == poItem.PODetailsId);
                var inv = shipmentInvoicePoItemMisMatchesList.First(x => x.INVDetailsId == match.First().itm.INVDetailsId);

                if (shipmentInvoicePoItemMisMatchesList.Any(x => x.INVDetailsId == inv.INVDetailsId && x.PODetailsId == po.PODetailsId)) continue;


                po.InvoiceNo = inv.InvoiceNo;
                po.INVTotalCost = inv.INVTotalCost;
                po.INVQuantity = inv.INVQuantity;
                po.INVDetailsId = inv.INVDetailsId;
                po.INVItemCode = inv.INVItemCode;
                po.INVDescription = inv.INVDescription;
                po.INVCost = inv.INVCost;
                po.INVSalesFactor = po.POQuantity / inv.INVQuantity;
                INVItems.Remove(match.First().itm);
                shipmentInvoicePoItemMisMatchesList.Remove(inv);

            }


            return shipmentInvoicePoItemMisMatchesList;
        }

        private static List<ShipmentInvoicePOItemMISMatches> misMatches = null;
        private static List<ShipmentInvoicePOItemMISMatches> GetMisMatches()
        {
            return misMatches ?? (misMatches = new EntryDataDSContext().ShipmentInvoicePOItemMISMatches.ToList());
        }

        private static List<ShipmentInvoicePOItemMISMatches> ReMatchOnItemDescription(
            List<ShipmentInvoicePOItemMISMatches> shipmentInvoicePoItemMisMatchesList, ShipmentInvoice shipmentInvoice)
        {
            var POItems = shipmentInvoicePoItemMisMatchesList
                                                .Where(x => !string.IsNullOrEmpty(x.PONumber) /*&& string.IsNullOrEmpty(x.InvoiceNo)*/)
                                                .Select(x => (x.PONumber, x.POItemCode, x.PODescription, x.POCost, x.POQuantity, x.POTotalCost, x.PODetailsId, WordLst: GetWords(x.PODescription)))
                                                .ToList();

            var INVItems = shipmentInvoicePoItemMisMatchesList
                                                    .Where(x => /*string.IsNullOrEmpty(x.PONumber) &&*/ !string.IsNullOrEmpty(x.InvoiceNo))
                                                    .Where(x => x.InvoiceNo == shipmentInvoice.InvoiceNo)
                                                    .Select(x => ( x.InvoiceNo, x.INVItemCode, x.INVDescription, x.INVCost, x.INVQuantity, x.INVTotalCost, x.INVDetailsId, WordLst: GetWords(x.INVDescription)))
                                                    .ToList();
            foreach (var poItem in POItems)
            {
                var match = INVItems. Select(x => new {itm = x, matches = poItem.WordLst.Intersect(x.WordLst).ToList() })
                    .Where(x => x.matches.Count >= 1)
                    .OrderBy(x => Math.Abs(poItem.POCost.GetValueOrDefault() - x.itm.INVCost.GetValueOrDefault()))
                    .OrderBy(x => Math.Abs(poItem.POQuantity.GetValueOrDefault() - x.itm.INVQuantity.GetValueOrDefault()))
                    //.OrderBy(x => Math.Abs(poItem.POTotalCost.GetValueOrDefault() - x.itm.INVTotalCost.GetValueOrDefault()))
                    .ThenByDescending(x => x.matches.Count).ToList();
                if (!match.Any()) continue;

                var po = shipmentInvoicePoItemMisMatchesList.First(x => x.PODetailsId == poItem.PODetailsId);
                var inv = shipmentInvoicePoItemMisMatchesList
                    .Where(x => POItems.All(z => z.PODetailsId != x.PODetailsId))
                    .FirstOrDefault(x => x.INVDetailsId == match.OrderByDescending(z => z.matches.Count()).First().itm.INVDetailsId);

                if(inv == null || shipmentInvoicePoItemMisMatchesList.Any(x => (x.INVDetailsId == inv?.INVDetailsId && x.PODetailsId == inv?.PODetailsId) && (x.PODetailsId == po.PODetailsId && x.INVDetailsId == po.INVDetailsId))) continue;

                
                po.InvoiceNo = inv.InvoiceNo;
                po.INVTotalCost = inv.INVTotalCost;
                po.INVQuantity = inv.INVQuantity;
                po.INVDetailsId = inv.INVDetailsId;
                po.INVItemCode = inv.INVItemCode;
                po.INVDescription = inv.INVDescription;
                po.INVCost = inv.INVCost;
                po.INVSalesFactor = po.POQuantity / inv.INVQuantity;
                INVItems.Remove(match.First().itm);
                shipmentInvoicePoItemMisMatchesList.Remove(inv);

            }


            return shipmentInvoicePoItemMisMatchesList;
        }

        private static List<string> GetWords(string description) => Regex.Matches(description, @"([A-Z]+)|([\d/]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Cast<Match>().Select(match => match.Value.ToUpper()).ToList();

        public static string CreateUnattachedShipmentWorkBook(
            (string Code, int RiderId, string BLNumber) client, UnAttachedWorkBookPkg summaryPkg)
        {
            try
            {


                var summaryWorkBook = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"),
                    $"Summary-{client.BLNumber}-{client.RiderId}.xlsx");
                if (File.Exists(summaryWorkBook)) File.Delete(summaryWorkBook);
                var workbook = new Workbook(summaryWorkBook, "Summary");
                CreateSummarySheet(summaryPkg, workbook);
                CreateRiderInvoiceSheet(summaryPkg, workbook);
                CreateUnMatchedWorkSheet(workbook, summaryPkg.UnMatchedPOs, summaryPkg.UnMatchedInvoices);
                CreateClassificationsSheet(workbook, summaryPkg.Classifications);
                CreateErrorDetails(workbook, summaryPkg);

                workbook.Save();
                return summaryWorkBook;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateErrorDetails(Workbook workbook, UnAttachedWorkBookPkg summaryPkg)
        {
            try
            {


                workbook.AddWorksheet("Error Details");
                var currentline = 0;

                WriteTable( summaryPkg.RepeatInvoices.Select(x => (dynamic)x).ToList(), workbook, currentline,
                    "WarehouseCode, InvoiceNumber, Packages",
                    "Repeat Invoices:");

                currentline += 2 + summaryPkg.RepeatInvoices.Count; ;

                WriteTable(summaryPkg.RepeatMarks.Select(x => (dynamic)x).ToList(), workbook, currentline,
                    "BLNumber, ETA, Marks, WarehouseCode, Quantity",
                    "Repeat Marks:");

                //currentline += 2 + summaryPkg.RepeatMarks.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateClassificationsSheet(Workbook workbook,
            List<ShipmentInvoicePOItemData> summaryPkgClassifications)
        {
            workbook.AddWorksheet("Classifications");
            var currentline = 0;

            WriteTable(summaryPkgClassifications.Select(x => (dynamic) x).ToList(), workbook, currentline,
                "PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, TariffCode, POCost, INVCost, POQuantity, INVQuantity, POTotalCost, INVTotalCost",
                "Classifications:");
        }

        private static void CreateRiderInvoiceSheet(UnAttachedWorkBookPkg summaryPkg, Workbook workbook)
        {
            workbook.AddWorksheet("RiderInvoices");
            var currentline = 0;

            WriteTable(new List<dynamic> {summaryPkg.RiderSummary}, workbook, currentline,
                "ETA, DocumentDate, Packages, WarehouseCode, InvoiceNumber, GrossWeightKg, CubicFeet, Code",
                "Rider Summary:");

            currentline += 3;

            WriteTable(new List<dynamic> {summaryPkg.PackagesSummary}, workbook, currentline,
                "BLPackages, RiderPackages, InvoicePackages, Diff", "Packages Summary");

            currentline += 3;
            WriteTable(summaryPkg.UnMatchedBLDetails.Select(x => (dynamic)x).ToList(), workbook, currentline,
                "Marks, PackageType, Quantity", "Unmatched BL Details");

            currentline += 3;
            WriteTable(summaryPkg.UnMatchedRiderDetails.Select(x => (dynamic)x).ToList(), workbook, currentline,
                "WarehouseCode, InvoiceNumber, Pieces", "Unmatched Rider Details");

            currentline += 2 + summaryPkg.UnMatchedRiderDetails.Count;
            
            WriteTable(summaryPkg.RiderManualMatches.Select(x => (dynamic) x).ToList(), workbook, currentline,
                RideManualMatchesHeader, "Manual Matches");

            currentline += 2 + summaryPkg.RiderManualMatches.Count;
            WriteTable(summaryPkg.UnAttachedRiderDetails.Select(x => (dynamic) x).ToList(), workbook, currentline,
                "Shipper, WarehouseCode, InvoiceNumber, Pieces", "Rider Details with No Invoices");

            currentline += 2 + summaryPkg.UnAttachedRiderDetails.Count;
            WriteTable(summaryPkg.UnAttachedInvoices.Select(x => (dynamic) x).ToList(), workbook, currentline,
                "InvoiceNo, InvoiceTotal, ImportedLines, SupplierCode", "Invoices Not Linked To Rider");

            currentline += 2 + summaryPkg.UnAttachedInvoices.Count;
            WriteTable(summaryPkg.RiderDetails.Select(x => (dynamic) x).ToList(), workbook, currentline,
                "Shipper, WarehouseCode, InvoiceNumber, Pieces", "All Rider Details");

            var pkgInvoices = summaryPkg.Invoices.Select(z => new
            {
                z.InvoiceNo, z.ShipmentInvoicePOs.FirstOrDefault()?.PurchaseOrders?.PONumber, z.InvoiceTotal,
                z.ImportedLines, z.SupplierCode,
                Packages = summaryPkg.PackingDetails.Where(r => r.InvoiceNumber == z.InvoiceNo).Select(r => r.Packages).DefaultIfEmpty(0).Sum(),//z.ShipmentRiderInvoice.Where(r => r.RiderID == summaryPkg.RiderSummary?.Id).Sum(w => w.Packages)
                WarehouseCode = summaryPkg.PackingDetails.Where(r => r.InvoiceNumber == z.InvoiceNo).Select(x => x.Marks).DefaultIfEmpty("SAME").Aggregate((o,n) => $"{o},{n}")
            });

            currentline += 2 + summaryPkg.RiderDetails.Count;
            WriteTable(pkgInvoices.Select(x => (dynamic) x).ToList(), workbook, currentline,
                "InvoiceNo, PONumber, InvoiceTotal, ImportedLines, SupplierCode, Packages, WarehouseCode", "All Invoices");
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
                    for (var i = 0; i < table.Count; i++)
                    {
                        var itm = table[i];
                        if (itm != null)
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
                using (var ctx = new EntryDataDSContext())
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


                            var itm = new T {TrackingState = TrackingState.Added};
                            var search = "";
                            header.ForEach(x =>
                            {
                                var prop = itm.GetType().GetProperty(x);
                                prop?.SetValue(itm,
                                    Convert.ChangeType(workbook.CurrentWorksheet
                                        .GetCell(currentColumn + header.IndexOf(x), currentRow)
                                        .Value, prop.PropertyType));
                                search += $@" and {prop.Name} = '{prop.GetValue(itm)}'";
                            });

                            ctx.Database.ExecuteSqlCommand($"delete from {itm.GetType().Name} where {search.Substring(4)}");
                            
                            lst.Add(itm);
                        }

                        currentRow++;
                    }

                    if (!lst.Any()) return;

                    
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
                    "InvoiceNo,PONumber, InvoiceDate, ImportedLines, SubTotal, InvoiceTotal, ImportedTotalDifference,ImportedVSExpectedTotal, PO/Inv Total Diff, PO/Inv MisMatches, SupplierCode, SourceFile"
                        .Split(',').Select(x => x.Trim()).ToList();
                currentline++;
                invHeader.ForEach(x => SetValue(workbook, currentline, invHeader.IndexOf(x), x));

                currentline++;
               
                Style duplicateStyle = new Style();                                                                           // Create new style
                duplicateStyle.CurrentFill.SetColor("FFFFFF00", Style.Fill.FillType.fillColor);

                Style errStyle = new Style();                                                                           // Create new style
                errStyle.CurrentFill.SetColor("FFFFC0CB", Style.Fill.FillType.fillColor);


                Style repeatStyle = new Style();                                                                           // Create new style
                repeatStyle.CurrentFill.SetColor("FFFFA500", Style.Fill.FillType.fillColor);

                var i = 0;
                while (true)
                {
                    if (i > summaryPkg.Invoices.Count() - 1) break;

                    foreach (var po in summaryPkg.Invoices[i].ShipmentInvoicePOs)
                    {
                       

                        
                        
                        
                        var importedVSExpectedTotal = summaryPkg.Invoices[i].InvoiceTotal.GetValueOrDefault()
                                                      - (summaryPkg.Invoices[i].SubTotal.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalInternalFreight.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalOtherCost.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalInsurance.GetValueOrDefault()
                                                         - summaryPkg.Invoices[i].TotalDeduction.GetValueOrDefault());

                        var subTotal = Math.Round(summaryPkg.Invoices[i].SubTotal.GetValueOrDefault(), 2) - Math.Round(po.PurchaseOrders.EntryDataDetails.Sum(x => x.Cost * x.Quantity), 2);
                        var totalCost = Math.Round(summaryPkg.Invoices[i].InvoiceDetails.Sum(x => x.TotalCost??0), 2) - Math.Round(po.PurchaseOrders.EntryDataDetails.Sum(x => x.Cost * x.Quantity), 2);
                        var importedTotalDifference = summaryPkg.Invoices[i].ImportedTotalDifference;
                        var poInvMismatch = po?.POMISMatches.Count();


                        if (summaryPkg.Invoices[i].ShipmentInvoicePOs.Count > 1)
                            workbook.CurrentWorksheet.SetActiveStyle(duplicateStyle);
                        else if (summaryPkg.Invoices[i].ShipmentInvoicePOs.Any(x => x.PurchaseOrders.ShipmentInvoicePOs.Count > 1))
                            workbook.CurrentWorksheet.SetActiveStyle(repeatStyle);
                        else if(Math.Round(importedVSExpectedTotal,2) != 0 || Math.Round(importedTotalDifference,2) != 0 || (poInvMismatch??0) != 0 || totalCost != 0)
                            workbook.CurrentWorksheet.SetActiveStyle(errStyle);
                        else
                        {
                            workbook.CurrentWorksheet.ClearActiveStyle();
                        }

                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceNo)),
                            summaryPkg.Invoices[i].InvoiceNo);
                        SetValue(workbook, currentline, invHeader.IndexOf("PONumber"),
                            po.PurchaseOrders?.PONumber ?? "");
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceDate)),
                            summaryPkg.Invoices[i].InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.ImportedLines)),
                            summaryPkg.Invoices[i].ImportedLines);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SubTotal)),
                            summaryPkg.Invoices[i].SubTotal);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceTotal)),
                            summaryPkg.Invoices[i].InvoiceTotal);
                        
                        SetValue(workbook, currentline,
                            invHeader.IndexOf(nameof(ShipmentInvoice.ImportedTotalDifference)),
                            importedTotalDifference);

                        
                        SetValue(workbook, currentline, invHeader.IndexOf("ImportedVSExpectedTotal"),
                            importedVSExpectedTotal);


                        
                        SetValue(workbook, currentline, invHeader.IndexOf("PO/Inv Total Diff"),
                            subTotal);

                        
                        SetValue(workbook, currentline, invHeader.IndexOf("PO/Inv MisMatches"),
                            poInvMismatch);


                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SupplierCode)),
                            summaryPkg.Invoices[i].SupplierCode);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SourceFile)),
                            new FileInfo(summaryPkg.Invoices[i].SourceFile).Name);

                        

                        currentline++;

                    }
                    
                    if (summaryPkg.Invoices[i].ShipmentInvoicePOs.All(x => String.Equals(x.PurchaseOrders.PONumber, "Null", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var importedVSExpectedTotal = summaryPkg.Invoices[i].InvoiceTotal.GetValueOrDefault()
                                                      - (summaryPkg.Invoices[i].SubTotal.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalInternalFreight.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalOtherCost.GetValueOrDefault()
                                                         + summaryPkg.Invoices[i].TotalInsurance.GetValueOrDefault()
                                                         - summaryPkg.Invoices[i].TotalDeduction.GetValueOrDefault());

                        var subTotal = Math.Round(summaryPkg.Invoices[i].SubTotal.GetValueOrDefault(), 2) - Math.Round(summaryPkg.Invoices[i].InvoiceDetails.Sum(x => x.Cost * x.Quantity), 2);
                        var totalCost = Math.Round(summaryPkg.Invoices[i].InvoiceDetails.Sum(x => x.TotalCost ?? 0), 2) - Math.Round(summaryPkg.Invoices[i].InvoiceDetails.Sum(x => x.Cost * x.Quantity), 2);
                        var importedTotalDifference = summaryPkg.Invoices[i].ImportedTotalDifference;
                        


                        if (summaryPkg.Invoices[i].ShipmentInvoicePOs.Count > 1)
                            workbook.CurrentWorksheet.SetActiveStyle(duplicateStyle);
                        else if (Math.Round(importedVSExpectedTotal, 2) != 0 || Math.Round(importedTotalDifference, 2) != 0 ||  totalCost != 0)
                            workbook.CurrentWorksheet.SetActiveStyle(errStyle);
                        else
                        {
                            workbook.CurrentWorksheet.ClearActiveStyle();
                        }

                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceNo)),
                            summaryPkg.Invoices[i].InvoiceNo);
                        
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceDate)),
                            summaryPkg.Invoices[i].InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.ImportedLines)),
                            summaryPkg.Invoices[i].ImportedLines);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SubTotal)),
                            summaryPkg.Invoices[i].SubTotal);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.InvoiceTotal)),
                            summaryPkg.Invoices[i].InvoiceTotal);

                        SetValue(workbook, currentline,
                            invHeader.IndexOf(nameof(ShipmentInvoice.ImportedTotalDifference)),
                            importedTotalDifference);


                        SetValue(workbook, currentline, invHeader.IndexOf("ImportedVSExpectedTotal"),
                            importedVSExpectedTotal);



                        SetValue(workbook, currentline, invHeader.IndexOf("PO/Inv Total Diff"),
                            subTotal);


                        

                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SupplierCode)),
                            summaryPkg.Invoices[i].SupplierCode);
                        SetValue(workbook, currentline, invHeader.IndexOf(nameof(ShipmentInvoice.SourceFile)),
                            new FileInfo(summaryPkg.Invoices[i].SourceFile).Name);



                        currentline++;
                    }

                    i++;
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CreateUnMatchedWorkSheet(Workbook workbook, List<ShipmentMIS_POs> unMatchedPOs,
            List<ShipmentMIS_Invoices> unMatchedInvoices)
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
                var poheader = ",,,,,,,,,,,,,InvoiceNo,InvoiceDate,ImportedLines,SubTotal,SupplierCode,SourceFile"
                    .Split(',').Select(x => x.Trim()).ToList();
                header.ForEach(x => SetValue(workbook, 1, header.IndexOf(x), x));
                invHeader.ForEach(x => SetValue(workbook, 1, invHeader.IndexOf(x), x));
                poheader.ForEach(x => SetValue(workbook, 1, poheader.IndexOf(x), x));
                var isEnd = false;
                var i = 0;
                while (!isEnd)
                {
                    if (i <= unMatchedInvoices.Count() - 1)
                    {
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceNo)),
                            unMatchedInvoices[i].InvoiceNo);
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceDate)),
                            unMatchedInvoices[i].InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.ImportedLines)),
                            unMatchedInvoices[i].ImportedLines);
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SubTotal)),
                            unMatchedInvoices[i].SubTotal);
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.InvoiceTotal)),
                            unMatchedInvoices[i].InvoiceTotal);
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SupplierCode)),
                            unMatchedInvoices[i].SupplierCode);
                        SetValue(workbook, i + 2, invHeader.IndexOf(nameof(ShipmentMIS_Invoices.SourceFile)),
                            new FileInfo(unMatchedInvoices[i].SourceFile).Name);
                    }

                    if (i <= unMatchedPOs.Count() - 1)
                    {
                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.InvoiceNo)),
                            unMatchedPOs[i].InvoiceNo);
                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.InvoiceDate)),
                            unMatchedPOs[i].InvoiceDate.ToString("yyyy-MM-dd"));
                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.ImportedLines)),
                            unMatchedPOs[i].ImportedLines);
                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.SubTotal)),
                            unMatchedPOs[i].SubTotal);
                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.SupplierCode)),
                            unMatchedPOs[i].SupplierCode);

                        if (unMatchedPOs[i].SourceFile == null)
                        {
                            i++;
                            continue;
                        }

                        var fileInfo = new FileInfo(unMatchedPOs[i].SourceFile);

                        SetValue(workbook, i + 2, poheader.IndexOf(nameof(ShipmentMIS_POs.SourceFile)),
                            fileInfo == null ? unMatchedPOs[i].SourceFile : fileInfo.Name);
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
            out int invoiceRow,  List<(string WarehouseCode, int Packages, string InvoiceNo)> packingDetails, out bool doRider)
        {
            try
            {
                var workbook =
                    new Workbook(csvFilePath, "POTemplate"); // Create new workbook with a worksheet called Sheet1
                var headerRow = 0;

                MoreEnumerable.ForEach(header,
                    x => { SetValue(workbook, headerRow, x.Key.Index, x.Value.DestinationName); });
                invoiceRow = 1;




                doRider = false;

                //if (!packingDetails.Any()) return workbook;
             
               doRider = shipmentInvoice.ShipmentInvoicePOs.Sum(x => x.PurchaseOrders.EntryDataDetails.Count()) >= packingDetails.Count();

              
                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Packages").Key.Index,
                    packingDetails.Select(x => x.Packages).DefaultIfEmpty(0).Sum());
                SetValue(workbook, invoiceRow, header.First(x => x.Key.Column == "Warehouse").Key.Index,
                    packingDetails.Select(x => x.WarehouseCode).DefaultIfEmpty("")
                        .Aggregate((o, c) => o + "," + c));


                return workbook;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void WriteInvHeader(ShipmentInvoice shipmentInvoice,
            Dictionary<(string Column, int Index), FileTypeMappings> header, Workbook workbook, bool IsCombined)
        {
            workbook.SetCurrentWorksheet("POTemplate");
            var invoiceRow = IsCombined
                ? workbook.CurrentWorksheet.GetLastRowNumber() + 1
                : workbook.CurrentWorksheet.GetLastRowNumber();

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceTotal)).Key.Index,
                shipmentInvoice.InvoiceTotal.GetValueOrDefault());

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceDate)).Key.Index,
                shipmentInvoice.InvoiceDate.GetValueOrDefault().ToString("yyyy-MM-dd"));

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.SupplierCode)).Key.Index,
                shipmentInvoice.SupplierCode);

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.Currency)).Key.Index,
                shipmentInvoice.Currency);

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.InvoiceNo)).Key.Index,
                shipmentInvoice.InvoiceNo);

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.PONumber)).Key.Index,
                shipmentInvoice.ShipmentInvoicePOs.FirstOrDefault()?.PurchaseOrders?.PONumber);

            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalInternalFreight)).Key.Index,
                shipmentInvoice.TotalInternalFreight ??0);
            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalDeduction)).Key.Index,
                shipmentInvoice.TotalDeduction ??0);
            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalOtherCost)).Key.Index,
                shipmentInvoice.TotalOtherCost ?? 0);
            SetValue(workbook, invoiceRow,
                header.First(x => x.Key.Column == nameof(shipmentInvoice.TotalInsurance)).Key.Index,
                shipmentInvoice.TotalInsurance ?? 0);
        }


        private static void SetValue(Workbook workbook, int row, int col, dynamic value)
        {
            workbook.CurrentWorksheet.AddCell(value ?? "", col, row);
        }

        private static void SetFormula(Workbook workbook, int row, int col, dynamic value)
        {
            workbook.CurrentWorksheet.AddCellFormula(value ?? "", col, row);
        }


        public static string SaveUnAttachedSummary(FileInfo file)
        {
            var workBook = NanoXLSX.Workbook.Load(file.FullName);
            workBook.SetCurrentWorksheet("Summary");
            var reference = workBook.CurrentWorksheet.GetCell(1, 0).Value.ToString();

            ImportUnMatchedInvoicePOs(workBook);
            ReadTable<ShipmentInvoiceRiderManualMatches>(workBook, "RiderInvoices", RideManualMatchesHeader,
                RiderManualMatchesTag);

            return reference;
        }


        private static void ImportUnMatchedInvoicePOs(NanoXLSX.Workbook workBook)
        {
            try
            {
                if (workBook.Worksheets.All(x => x.SheetName != "UnMatchedInvoicePOs")) return;
                workBook.SetCurrentWorksheet("UnMatchedInvoicePOs");
                var isEnd = false;
                var matches = new List<ShipmentInvoicePOManualMatches>();
                var row = 2;
                while (true)
                {
                    if (!workBook.CurrentWorksheet.HasCell(1, row)) break;

                    var po = workBook.CurrentWorksheet.GetCell(1, row).Value.ToString();
                    var inv = workBook.CurrentWorksheet.GetCell(2, row).Value.ToString();
                    matches.Add(new ShipmentInvoicePOManualMatches
                    {
                        InvoiceNo = inv,
                        PONumber = po,
                        TrackingState = TrackingState.Added
                    });
                    row++;
                }

                if (!matches.Any()) return;
                using (var ctx = new EntryDataDSContext(){StartTracking = true})
                {
                    foreach (var mat in matches)
                    {
                        if (ctx.ShipmentInvoicePOManualMatches.FirstOrDefault(x =>
                            x.InvoiceNo == mat.InvoiceNo && x.PONumber == mat.PONumber) != null) continue;


                        var delitms = ctx.ShipmentInvoicePOs.Where(x => x.PurchaseOrders.EntryDataId == mat.PONumber)
                            .ToList();

                        ctx.ShipmentInvoicePOs.RemoveRange(delitms);

                        var dellst = ctx.ShipmentInvoicePOManualMatches.Where(x => x.InvoiceNo == mat.InvoiceNo || x.PONumber == mat.PONumber)
                            .ToList();
                        ctx.ShipmentInvoicePOManualMatches.RemoveRange(dellst);

                        ctx.SaveChanges();

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
        public List<ShipmentInvoiceBLManualMatches> BLManualMatches;
        public List<ShipmentBLDetails> UnMatchedBLDetails { get; set; }
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
        public List<ShipmentErrors_RepeatMarks> RepeatMarks { get; set; }
        public List<ShipmentErrors_RepeatInvoices> RepeatInvoices { get; set; }
        public List<ShipmentRiderDetails> UnMatchedRiderDetails { get; set; }
        public List<ShipmentBLDetails> BlDetails { get; set; }
        public List<(string Marks, int Packages, string InvoiceNumber)> PackingDetails { get; set; }
    }

    public class PackagesSummary
    {
        public int BLPackages { get; set; }
        public int RiderPackages { get; set; }
        public int InvoicePackages { get; set; }

        public int Diff => (BLPackages == 0 ? RiderPackages : BLPackages) - InvoicePackages;
    }
}
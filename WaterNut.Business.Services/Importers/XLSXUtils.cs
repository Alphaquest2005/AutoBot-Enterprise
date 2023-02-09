using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using ExcelDataReader;
using MoreLinq;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using Z.BulkOperations.Internal.InformationSchema;
//using EntryDataDS.Business.Entities;
using EntryDocSetUtils = WaterNut.DataSpace.EntryDocSetUtils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;


namespace WaterNut.Business.Services.Importers
{
    public static class XLSXUtils
    {
        public static void ReadMISMatches(DataTable misMatches, DataTable poTemplate)
        {
            try
            {

                var misHeaderRow = misMatches.Rows[0].ItemArray.ToList();
                var poHeaderRow = poTemplate.Rows[0].ItemArray.ToList();
                string oldInvoiceNo = "";
                DataRow poTemplateRow = null;
                var addrow = true;
                foreach (DataRow misMatch in misMatches.Rows)
                {
                    if (misMatch == misMatches.Rows[0]) continue;
                    var InvoiceNo = misMatch[misHeaderRow.IndexOf("InvoiceNo")].ToString();
                    var invItemCode = misMatch[misHeaderRow.IndexOf("INVItemCode")].ToString();
                    var poItemCode = misMatch[misHeaderRow.IndexOf("POItemCode")].ToString();
                    var poNumber = misMatch[misHeaderRow.IndexOf("PONumber")].ToString();
                    //var invDetailId = misMatch[misHeaderRow.IndexOf("INVDetailsId")].ToString();
                    //var poDetailId = misMatch[misHeaderRow.IndexOf("PODetailsId")].ToString();
                   




                    if (!string.IsNullOrEmpty(poNumber) &&
                        !string.IsNullOrEmpty(InvoiceNo) &&
                        !string.IsNullOrEmpty(poItemCode) &&
                        !string.IsNullOrEmpty(invItemCode))
                    {

                        
                        foreach (DataRow prow in poTemplate.Rows)
                        {
                            if (prow[poHeaderRow.IndexOf("PO Number")].ToString() == poNumber &&
                                prow[poHeaderRow.IndexOf("Supplier Invoice#")].ToString() == InvoiceNo)
                            {
                                if (prow == poTemplateRow)
                                {
                                    addrow = true;
                                    break;
                                }
                                poTemplateRow = prow;
                                if (string.IsNullOrEmpty(
                                        poTemplateRow[poHeaderRow.IndexOf("PO Item Number")].ToString())
                                    && string.IsNullOrEmpty(poTemplateRow[poHeaderRow.IndexOf("Supplier Item Number")]
                                        .ToString()))
                                {
                                    addrow = false;
                                }
                                else
                                {
                                    addrow = true;
                                }
                                break;
                            }
                        }

                        if (poTemplateRow == null)
                        {
                            BaseDataModel.EmailExceptionHandler(new ApplicationException(
                                $"Mismatch PO:{poNumber} and SupplierNo{InvoiceNo} on template and mismatch sheet."));
                            return;
                        }

                        DataRow row;
                        // changed to false because when importing in portage it doubling the errors because they get imported in importData function
                        row = addrow == false ? poTemplateRow : poTemplate.NewRow();

                       if(oldInvoiceNo != InvoiceNo && !addrow) row[poHeaderRow.IndexOf("Supplier Invoice#")] = InvoiceNo;
                        row[poHeaderRow.IndexOf("PO Number")] = misMatch[misHeaderRow.IndexOf("PONumber")];
                        row[poHeaderRow.IndexOf("Date")] = poTemplateRow[poHeaderRow.IndexOf("Date")];
                        row[poHeaderRow.IndexOf("PO Item Number")] = poItemCode;
                        row[poHeaderRow.IndexOf("Supplier Item Number")] = invItemCode;
                        row[poHeaderRow.IndexOf("PO Item Description")] = misMatch[misHeaderRow.IndexOf("PODescription")];
                        row[poHeaderRow.IndexOf("Supplier Item Description")] = misMatch[misHeaderRow.IndexOf("INVDescription")];
                        row[poHeaderRow.IndexOf("Cost")] = ((double)misMatch[misHeaderRow.IndexOf("INVCost")] /
                                                            ((misHeaderRow.IndexOf("INVSalesFactor") > -1
                                                              && !string.IsNullOrEmpty(misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString()) && 
                                                              misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString() != "Infinity")
                                                                ? Convert.ToDouble(misMatch[misHeaderRow.IndexOf("INVSalesFactor")])
                                                                : 1));
                        row[poHeaderRow.IndexOf("Quantity")] = misMatch[misHeaderRow.IndexOf("POQuantity")];
                        row[poHeaderRow.IndexOf("Total Cost")] = misMatch[misHeaderRow.IndexOf("INVTotalCost")];

                        //if (!string.IsNullOrEmpty(invDetailId) && int.TryParse(invDetailId, out int invId))
                        //{
                        //    InvoiceDetails invDetail;
                        //    invDetail = new EntryDataDSContext().ShipmentInvoiceDetails.FirstOrDefault(x => x.Id == invId);
                        //    if (invDetail != null)
                        //    {
                        //        //row[poHeaderRow.IndexOf("FileLineNumber")] = invDetail.FileLineNumber;
                        //    }
                        //}

                        if (addrow) poTemplate.Rows.Add(row);
                        oldInvoiceNo = InvoiceNo;

                        ImportInventoryMapping(invItemCode, misMatch, misHeaderRow, poItemCode);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void ImportInventoryMapping(string invItemCode, DataRow misMatch, List<object> misHeaderRow,
            string poItemCode)
        {
            try
            {


                using (var ctx = new EntryDataDSContext())
                {
                    InvoiceDetails invRow;
                    EntryDataDetails poRow;

                    if (invItemCode == poItemCode)
                    {
                        return;
                        // this should not happen write code to deal with it
                        Debugger.Break();
                    }

                    var invItm = ctx.InventoryItems.Include(x => x.InventoryItemAlias).FirstOrDefault(x =>
                        x.ItemNumber == invItemCode
                        && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (invItm == null)
                    {
                        invItm = new InventoryItems()
                        {
                            ApplicationSettingsId =
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            Description = misMatch[misHeaderRow.IndexOf("INVDescription")].ToString(),
                            ItemNumber = invItemCode,
                            TrackingState = TrackingState.Added
                        };
                        ctx.InventoryItems.Add(invItm);
                    }

                    var poItm = ctx.InventoryItems.Include(x => x.InventoryItemAlias).FirstOrDefault(x =>
                        x.ItemNumber == poItemCode && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (poItm == null)
                    {
                        poItm = new InventoryItems()
                        {
                            ApplicationSettingsId =
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            Description = misMatch[misHeaderRow.IndexOf("PODescription")].ToString(),
                            ItemNumber = poItemCode,
                            TrackingState = TrackingState.Added
                        };
                        ctx.InventoryItems.Add(poItm);
                    }
                    ctx.SaveChanges();

                    if (poItm.InventoryItemAlias.All(x => x.AliasItemId != invItm.Id) //&& !invItm.AliasItems.Any(x => x.InventoryItemId == poItm.Id)
                       )
                    {
                        ctx.InventoryItemAlias.Add(new InventoryItemAlias(true)
                        {
                            InventoryItem = poItm,
                            AliasItem = invItm,
                            AliasName = invItm.ItemNumber,
                            TrackingState = TrackingState.Added
                        });
                    }

                    if (invItm.InventoryItemAlias.All(x => x.AliasItemId != poItm.Id) //&&!poItm.AliasItems.Any(x => x.InventoryItemId == invItm.Id)
                        )
                    {
                        ctx.InventoryItemAlias.Add(new InventoryItemAlias(true)
                        {
                            InventoryItem = invItm,
                            AliasItem = poItm,
                            AliasName = poItm.ItemNumber,
                            TrackingState = TrackingState.Added
                        });
                    }

                    //var itmAlias = ctx.InventoryItemAlias
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void FixCSVFile(FileTypes fileType, bool? overwrite, string output)
        {
          
            CSVUtils.FixCsv(new FileInfo(output), fileType, overwrite);
        }



        public static string CreateCSVFile(FileInfo file, string fileText)
        {
            string output = Path.ChangeExtension(file.FullName, ".csv");
            StreamWriter csv = new StreamWriter(output, false);

            csv.Write(fileText);
            csv.Close();
            return output;
        }

        public static string GetText(FileTypes fileType, List<DataRow> rows, DataTable dataTable)
        {
            var table = new ConcurrentDictionary<int, string>();
            Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 },
                row =>
                {
                    StringBuilder a = new StringBuilder();

                    if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                            .All(x => row.ItemArray.Contains(x)))
                    {
                        //if(dic.ContainsKey())
                        a.Append("");
                    }

                    for (int i = 0; i < dataTable.Columns.Count - 1; i++)
                    {
                        a.Append(CSVUtils.StringToCSVCell(row[i].ToString()) + ",");
                    }


                    a.Append("\n");
                    table.GetOrAdd(Convert.ToInt32(row["LineNumber"]), a.ToString());
                });
            var res = new StringBuilder();
            //var aggregate = table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => res.Append(x));
            //return aggregate;
            table.OrderBy(x => x.Key).Select(x => x.Value).ForEach(x => res.Append(x));
            return res.ToString();
        }

        public static List<dynamic> DataRowToBetterExpando(FileTypes fileType, List<DataRow> rows, DataTable dataTable)
        {
            try
            {

                var table = new ConcurrentDictionary<int, BetterExpando>();
                var headerRow = new Dictionary<string, FileTypeMappings>();
                var headers = new List<string>();
                //Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 },
                //  row =>

                Importers.ValidateFileType.Execute(fileType);

                foreach (var row in rows)
                {
                    IDictionary<string, object> a = new BetterExpando();
                    if (!headerRow.Any())
                        if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                                .Any(x => row.ItemArray.Contains(x)) && !headerRow.Any())
                        {
                            //if(dic.ContainsKey())
                            headers = row.ItemArray.Select(x => x.ToString()).GetEnumerator().ToList<string>();
                            headerRow = fileType.FileTypeMappings.Where(x => row.ItemArray.Contains(x.OriginalName))
                                .Select(x => new { x.OriginalName, x }).ToDictionary(k => k.OriginalName, d => d.x);

                        }
                        else
                        {
                            continue;
                        }

                    for (int i = 0; i < dataTable.Columns.Count - 1; i++)
                    {
                        if (headerRow.ContainsKey(headers[i]))
                            a[headerRow[headers[i]].DestinationName] =
                                row[i].ToString() == headerRow[headers[i]].OriginalName
                                    ? row[i].ToString()
                                    : GetMappingValue(headerRow[headers[i]], row[i].ToString());



                    }

                    table.GetOrAdd(Convert.ToInt32(row["LineNumber"]), (BetterExpando)a);
                }
                //);

                return table.OrderBy(x => x.Key).Select(x => (dynamic)x.Value).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static object GetMappingValue(FileTypeMappings key, string val) => FileTypeManager.MappingValueToType(key, FileTypeManager.ApplyFileMapRegEx(key, val));

        public static DataSet ExtractTables(FileInfo file) => ReadFile(file);

        public static List<DataRow> FixupDataSet( DataTable Table)
        {
            int row_no = 0;


            Table.Columns.Add("LineNumber", typeof(int));

            var rows = new List<DataRow>();
            ///insert linenumber
            while (row_no < Table.Rows.Count)
            {
                var dataRow = Table.Rows[row_no];
                dataRow["LineNumber"] = row_no;
                rows.Add(dataRow);
                row_no++;
            }

            return rows;
        }

        private static DataSet ReadFile(FileInfo file)
        {
            FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
            var excelReader = ExcelReaderFactory.CreateReader(stream);
            var result = excelReader.AsDataSet();
            excelReader.Close();
            return result;
        }

        public static FileTypes DetectFileType(FileTypes fileType, FileInfo file, List<DataRow> dataRows)
        {
            try
            {
                var originalDocSetId = fileType.AsycudaDocumentSetId;
                FileTypes rfileType = null;
                var potentialsFileTypes = new List<FileTypes>();
                var lastHeaderRow = dataRows[0].ItemArray.ToList();
                int drow_no = 0;
                List<object> headerRow;
                var filetypes = FileTypeManager.GetImportableFileType(fileType.FileImporterInfos.EntryType, fileType.FileImporterInfos.Format, file.Name);
                while (drow_no < dataRows.Take(WaterNut.DataSpace.Utils.maxRowsToFindHeader).ToList().Count)
                {
                    headerRow = dataRows[drow_no].ItemArray.ToList();

                    foreach (var f in filetypes.Where(x => ((x.IsImportable ?? true) != false) && x.FileTypeMappings.Any()))
                    {
                        if (//headerRow.Any(x => f.FileTypeMappings.All(z => z.Required == false) && f.FileTypeMappings.All(z => z.OriginalName == x.ToString())) || // All False && all in header or all required in header
                            headerRow.Any(x => f.FileTypeMappings.Where(z => z.Required == true).Any(z => z.OriginalName.ToUpper().Trim() == x.ToString().ToUpper().Trim() || z.DestinationName.ToUpper().Trim() == x.ToString().ToUpper().Trim())))
                        {
                            potentialsFileTypes.Add(f);
                            lastHeaderRow = headerRow;
                        }
                    }

                    drow_no++;
                }

                fileType.AsycudaDocumentSetId = originalDocSetId; // changing when retriving list - sideeffect
                if (!potentialsFileTypes.Any()) return fileType;
                rfileType = potentialsFileTypes
                    .OrderByDescending(x => x.FileTypeMappings.Where(z =>
                        lastHeaderRow.Select(h => h.ToString().ToUpper().Trim())
                            .Contains(z.OriginalName.ToUpper().Trim())).Count())
                    .ThenByDescending(x => x.FileTypeActions.Count())
                    .FirstOrDefault();
                rfileType.AsycudaDocumentSetId = originalDocSetId;
                rfileType.Data = fileType.Data;
                rfileType.EmailId = fileType.EmailId;
               
                return rfileType;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

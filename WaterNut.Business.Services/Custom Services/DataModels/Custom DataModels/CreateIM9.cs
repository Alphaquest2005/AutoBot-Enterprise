using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using CoreEntities.Business.Enums;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;

namespace WaterNut.DataSpace
{
    public class CreateIM9
    {
        private static readonly CreateIM9 _instance;

        static CreateIM9()
        {
            _instance = new CreateIM9();
        }

        public static CreateIM9 Instance
        {
            get { return _instance; }
        }


        internal async Task CleanBond(AsycudaDocumentSet docSet, bool PerIM7)
        {
            //get document items where itemquantity > pi quantity
            try
            {
                var alst = await GetAllIM9Data().ConfigureAwait(false);
                //var grp = alst.GroupBy(x => x.CNumber);
                //foreach (var item in grp)
                //{
                //   await CreateIM9Entries(docSet, PerIM7, item).ConfigureAwait(false);
                //}

                await CreateIM9Entries(docSet, PerIM7, alst).ConfigureAwait(false);

            }
            catch (Exception)
            {
                throw;
            }
        }

        internal async Task CleanEntries(AsycudaDocumentSet docSet, IEnumerable<int> lst, bool PerIM7)
        {
            //get document items where itemquantity > pi quantity
            try
            {
                var alst = await GetSelectedIM9Data(lst, docSet.ApplicationSettingsId).ConfigureAwait(false);

                await CreateIM9Entries(docSet, PerIM7, alst).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal async Task CleanLines(AsycudaDocumentSet docSet, IEnumerable<int> lst, bool perIM7)
        {
            try
            {
                var alst = await GetSelectedLinesIM9Data(lst).ConfigureAwait(false);

                await CreateIM9Entries(docSet, perIM7, alst).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CreateIM9Entries(AsycudaDocumentSet docSet, bool PerIM7,
            IEnumerable<AsycudaDocumentItemIM9> alst)
        {
            try
            {


                var elst = alst.OrderByDescending(x => x.RegistrationDate).GroupBy(x => x.AsycudaDocumentId);

                var itmcount = 0;

                DocumentCT cdoc = null;
                foreach (var entry in elst)
                {
                    if (PerIM7) cdoc = await SaveAndCreateIM9Doc(docSet, cdoc).ConfigureAwait(false);

                    //create new document
                    foreach (
                            var ditm in
                            entry.OrderByDescending(x => x.RegistrationDate).ThenBy(x => x.LineNumber))
                        //.Where(x => x.pCNumber == "24985" && x.LineNumber == 5)
                    {
                        if (ditm.ItemQuantity == ditm.PiQuantity) continue;
                        if (BaseDataModel.Instance.MaxLineCount(itmcount) || itmcount == 0)
                        {
                            cdoc = await SaveAndCreateIM9Doc(docSet, cdoc).ConfigureAwait(false);
                        }

                        if (itmcount == 0)
                        {
                            cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code =
                                ditm.Country_of_origin_code;
                            cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination
                                    .Destination_country_code
                                = "GD";
                        }

                        var itm = CreateIM9Line(ditm, itmcount, cdoc);

                        cdoc.DocumentItems.Add(itm);
                        itmcount += 1;
                    }
                }

                await SaveAndCreateIM9Doc(docSet, cdoc).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<List<AsycudaDocumentItemIM9>> GetAllIM9Data()
        {
            var alst = new List<AsycudaDocumentItemIM9>();

            using (var ctx = new AllocationDSContext())
            {
                
                alst.AddRange(
                    ctx.xcuda_Item
                                
                                .Where(x => x.AsycudaDocument.RegistrationDate >= BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate)
                                .Where(x => x.AsycudaDocument.CustomsOperationId == (int) CustomsOperations.Warehouse)
                                .Where(x => x.WarehouseError == null)
                                //.Where(x => x.xcuda_Tarification.Item_price > 0)
                                .Where(x => x.xcuda_Tarification.xcuda_Supplementary_unit.Any())
                                .Where(x => x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault()
                                            .Suppplementary_unit_quantity > (double?) x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                    .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
                                    .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum())
                                
                                .Select(x => new AsycudaDocumentItemIM9()
                                {
                                    ItemNumber = x.xcuda_Tarification.xcuda_HScode.Precision_4,
                                    CNumber = x.AsycudaDocument.CNumber,
                                    Customs_clearance_office_code = x.AsycudaDocument.Customs_clearance_office_code,
                                    Country_of_origin_code = x.xcuda_Goods_description.Country_of_origin_code,
                                    PiQuantity = (double) x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                        .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
                                        .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                                    PiWeight =  x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                        .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true)
                                        .Select(z => z.Net_weight).DefaultIfEmpty(0).Sum(),
                                    ItemQuantity = (double)x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault().Suppplementary_unit_quantity,
                                    Suppplementary_unit_code = x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault().Suppplementary_unit_code,
                                    LineNumber = x.LineNumber,
                                    AsycudaDocumentId = x.ASYCUDA_Id,
                                    Commercial_Description = x.xcuda_Goods_description.Commercial_Description,
                                    TariffCode = x.xcuda_Tarification.xcuda_HScode.Commodity_code,
                                    Item_price = x.xcuda_Tarification.Item_price,
                                    Net_weight = x.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm,
                                    ItemId = x.Item_Id,
                                    RegistrationDate = x.AsycudaDocument.RegistrationDate.Value,
                                    Total_CIF_itm = x.xcuda_Valuation_item.Total_CIF_itm
                                }).ToList()
                );
            }
            var ares = alst.OrderBy(x => x.CNumber).ToList();
            AttachPackageInfo(ares);

            return ares;
        }

        private static async Task<IEnumerable<AsycudaDocumentItemIM9>> GetSelectedIM9Data(IEnumerable<int> lst,
            int applicationSettingsId)
        {
            try
            {

                //var alst = new ConcurrentQueue<IEnumerable<AsycudaDocumentItemIM9>>();


                //Parallel.ForEach(lst, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount*4},//
                //    docId =>
                //    {
                var res = new StringBuilder();
                var enumerable = lst as IList<int> ?? lst.ToList();
                foreach (var itm in enumerable)
                {
                    res.Append(itm + ",");
                }



                using (var ctx = new AllocationDSContext())
                {
                    
                    var str = "," + res.ToString();


                    var alst =

                        ctx.xcuda_Item
                            .Where(x => x.xcuda_Tarification.xcuda_Supplementary_unit.Any())
                            .Where(x =>  str.Contains("," + x.AsycudaDocument.ASYCUDA_Id.ToString() + ",") && x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
                            .Select(x => new AsycudaDocumentItemIM9()
                            {
                                ItemNumber = x.xcuda_Tarification.xcuda_HScode.Precision_4,
                                CNumber = x.AsycudaDocument.CNumber,
                                Customs_clearance_office_code = x.AsycudaDocument.Customs_clearance_office_code,
                                Country_of_origin_code = x.xcuda_Goods_description.Country_of_origin_code,
                                PiQuantity = (double) x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                    .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null /*|| y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true*/)// only assessed items allowed especially now as i preassessing entries
                                    .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                                PiWeight =  x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                    .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null /*|| y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true*/)
                                    .Select(z => z.Net_weight).DefaultIfEmpty(0).Sum(),
                                ItemQuantity = (double) x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault(z => z.IsFirstRow == true)
                                    .Suppplementary_unit_quantity,
                                Suppplementary_unit_code = x.xcuda_Tarification.xcuda_Supplementary_unit
                                    .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_code??"",
                                LineNumber = x.LineNumber,
                                AsycudaDocumentId = x.ASYCUDA_Id,
                                Commercial_Description = x.xcuda_Goods_description.Commercial_Description,
                                TariffCode = x.xcuda_Tarification.xcuda_HScode.Commodity_code,
                                Item_price = x.xcuda_Tarification.Item_price,
                                Net_weight = x.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm,
                                ItemId = x.Item_Id,
                                RegistrationDate = x.AsycudaDocument.RegistrationDate.Value,
                                Total_CIF_itm = x.xcuda_Valuation_item.Total_CIF_itm,
                                SalesFactor = x.SalesFactor
                            }).ToList();
                    var ares = alst.OrderBy(x => x.LineNumber).ToList();
                    AttachPackageInfo(ares);

                    return ares;

                }
                //});

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static async Task<IEnumerable<AsycudaDocumentItemIM9>> GetSelectedLinesIM9Data(IEnumerable<int> lst)
        {
            var alst = new ConcurrentQueue<IEnumerable<AsycudaDocumentItemIM9>>();


            //Parallel.ForEach(lst, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount*4},//
            //    docId =>
            //    {
            var res = new StringBuilder();
            var enumerable = lst as IList<int> ?? lst.ToList();
            foreach (var itm in enumerable)
            {
                res.Append(itm + ",");
            }
            var str = res.ToString();


            using (var ctx = new AllocationDSContext())
            {
                
                alst.Enqueue(

                           ctx.xcuda_Item
                               .Where(x => x.xcuda_Tarification.xcuda_Supplementary_unit.Any())
                               .Where(x => x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault()
                                           .Suppplementary_unit_quantity > (double?) x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                   .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null )// only aSSessed entries
                                   .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum())
                               .Where(x => str.Contains(x.Item_Id.ToString()))
                               .Select(x => new AsycudaDocumentItemIM9()
                               {
                                   ItemNumber = x.xcuda_Tarification.xcuda_HScode.Precision_4,
                                   CNumber = x.AsycudaDocument.CNumber,
                                   Customs_clearance_office_code = x.AsycudaDocument.Customs_clearance_office_code,
                                   Country_of_origin_code = x.xcuda_Goods_description.Country_of_origin_code,
                                   PiQuantity = (double) x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                       .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null)
                                       .Select(z => z.Suplementary_Quantity).DefaultIfEmpty(0).Sum(),
                                   PiWeight = x.EntryPreviousItems.Select(y => y.xcuda_PreviousItem)
                                       .Where(y => y.xcuda_Item.AsycudaDocument.CNumber != null )
                                       .Select(z => z.Net_weight).DefaultIfEmpty(0).Sum(),
                                   ItemQuantity = (double)x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault().Suppplementary_unit_quantity,
                                   Suppplementary_unit_code = x.xcuda_Tarification.xcuda_Supplementary_unit.FirstOrDefault().Suppplementary_unit_code,
                                   LineNumber = x.LineNumber,
                                   AsycudaDocumentId = x.ASYCUDA_Id,
                                   Commercial_Description = x.xcuda_Goods_description.Commercial_Description,
                                   TariffCode = x.xcuda_Tarification.xcuda_HScode.Commodity_code,
                                   Item_price = x.xcuda_Tarification.Item_price,
                                   Net_weight = x.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm,
                                   ItemId = x.Item_Id,
                                   RegistrationDate = x.AsycudaDocument.RegistrationDate.Value,
                                   Total_CIF_itm = x.xcuda_Valuation_item.Total_CIF_itm
                               }).ToList());
            }

            var ares = alst.SelectMany(x => x).OrderBy(x => x.LineNumber).ToList();

            AttachPackageInfo(ares);

            return ares;
        }

        private static void AttachPackageInfo(List<AsycudaDocumentItemIM9> ares)
        {
            var str = ares.Select(z => z.AsycudaDocumentId.ToString()).Distinct().DefaultIfEmpty(string.Empty).Aggregate((n, c) => c + "," + n);
            using (var ctx = new DocumentItemDSContext())
            {
                var ps = ctx.xcuda_Packages
                    .Where(x => str.Contains(x.xcuda_Item.ASYCUDA_Id.ToString()))
                    .Where(x => x.Number_of_packages != 0)
                    .Select(x => new {ItemId = x.Item_Id, Packages = x.Number_of_packages}).ToList();

                foreach (var itm in ps)
                {
                    var a = ares.FirstOrDefault(x => x.ItemId == itm.ItemId.Value);
                    if (a != null) a.Number_of_packages = (int) itm.Packages;
                }
            }
        }

        private async Task<DocumentCT> SaveAndCreateIM9Doc(AsycudaDocumentSet docSet, DocumentCT cdoc)
        {
            try
            {
                int itmcount;
                if (cdoc != null && cdoc.Document != null && cdoc.DocumentItems.Any())
                {
                    await BaseDataModel.Instance.SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
                    itmcount = 0;
                }

                var cp = BaseDataModel.Instance.Customs_Procedures.Single(x =>
                    x.CustomsOperationId == (int) CustomsOperations.Exwarehouse && x.Stock == true);

                var Exp = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(y =>
                    y.Customs_Procedure == cp.CustomsProcedure);
                if (Exp.Customs_Procedure == null || string.IsNullOrEmpty(Exp.Customs_Procedure))
                {
                    throw new ApplicationException(
                        $"Export Template default Customs Procedures not Configured for {cp.CustomsProcedure}");
                }

                docSet.Customs_Procedure = cp;

                BaseDataModel.ConfigureDocSet(docSet, Exp);

                cdoc = await BaseDataModel.Instance.CreateDocumentCt(docSet).ConfigureAwait(false);
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
                BaseDataModel.Instance.IntCdoc(cdoc, docSet);



                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = $"{cp.Document_Type.DisplayName} Entries";


                BaseDataModel.Instance.AttachCustomProcedure(cdoc, cp);
                return cdoc;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private xcuda_Item CreateIM9Line(AsycudaDocumentItemIM9 ditm, int itmcount, DocumentCT cdoc)
        {
            try
            {

                var itm = new xcuda_Item(true) { };
                itm.SetupProperties();

                itm.ItemNumber = ditm.ItemNumber;
                itm.ItemQuantity = Convert.ToDouble(ditm.ItemQuantity - ditm.PiQuantity);
                itm.SalesFactor = ditm.SalesFactor;
                itm.xcuda_PreviousItem = new xcuda_PreviousItem(true)
                {
                    TrackingState = TrackingState.Added,
                    Hs_code = ditm.TariffCode,
                    Commodity_code = "00",
                    Current_item_number = (itmcount + 1), // piggy back the previous item count
                    Previous_item_number = ditm.LineNumber,
                    Previous_Packages_number = ditm.Number_of_packages.ToString(),
                    Suplementary_Quantity = Convert.ToDecimal(ditm.ItemQuantity - ditm.PiQuantity),
                    Preveious_suplementary_quantity = Convert.ToDouble(ditm.ItemQuantity),
                    Prev_net_weight = (decimal)ditm.Net_weight,

                    //////////////////////////todo///////////////////////////


                    Net_weight = ditm.Net_weight - ditm.PiWeight,
                    Goods_origin = ditm.Country_of_origin_code,
                    Previous_value =
                        Convert.ToDouble((ditm.Total_CIF_itm / ditm.ItemQuantity)),
                    Current_value =
                        Convert.ToDouble((ditm.Total_CIF_itm) / ditm.ItemQuantity),
                    Prev_reg_ser = "C",
                    Prev_reg_nbr = ditm.CNumber,
                    Prev_reg_year = ditm.RegistrationDate.Year,
                    Prev_reg_cuo = ditm.Customs_clearance_office_code ?? "GDSGO",
                    Prev_decl_HS_spec = ditm.ItemNumber,
                };
                itm.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                itm.TrackingState = TrackingState.Added;
                
                var pitm = itm.xcuda_PreviousItem;
                pitm.xcuda_Item = itm;
                if (ditm.Number_of_packages != 0)
                {
                    if (itm.xcuda_Packages.FirstOrDefault() == null)
                        itm.xcuda_Packages.Add(new xcuda_Packages(true) {TrackingState = TrackingState.Added});
                    itm.xcuda_Packages.FirstOrDefault().Number_of_packages =
                        ditm.Number_of_packages;
                }

                itm.xcuda_Goods_description.Commercial_Description = ditm.Commercial_Description;
                if (cdoc.Document.xcuda_General_information != null)
                    itm.xcuda_Goods_description.Country_of_origin_code =
                        cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination;
                itm.xcuda_Tarification.Item_price = Convert.ToSingle(ditm.Total_CIF_itm/ditm.ItemQuantity)* (double) pitm.Suplementary_Quantity;

                itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pitm.Hs_code;
                itm.xcuda_Tarification.xcuda_HScode.Precision_1 = pitm.Commodity_code;
                itm.xcuda_Tarification.xcuda_HScode.InventoryItems =
                    InventoryItemUtils.GetInventoryItem(x => x.ItemNumber == ditm.ItemNumber);
                itm.xcuda_Goods_description.Country_of_origin_code = pitm.Goods_origin;
           
                itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                {
                    TrackingState = TrackingState.Added,
                    //TODO: Check the weight when creating was making weight zero
                    Gross_weight_itm = pitm.Net_weight,
                    Net_weight_itm =  pitm.Net_weight
                };

                // adjusting because not using real statistical value when calculating
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency =
                    Convert.ToDouble(Math.Round((pitm.Current_value * (double) pitm.Suplementary_Quantity), 2));
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_national_currency =
                    Convert.ToDouble(Math.Round(pitm.Current_value * (double) pitm.Suplementary_Quantity, 2));
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_code = "XCD";
                itm.xcuda_Valuation_item.xcuda_Item_Invoice.Currency_rate = 1;

                itm.xcuda_Valuation_item.Total_CIF_itm =
                    Convert.ToSingle(Math.Round((pitm.Current_value * (double)pitm.Suplementary_Quantity), 4));
                itm.xcuda_Valuation_item.Statistical_value =
                    Convert.ToSingle(Math.Round((pitm.Current_value * (double)pitm.Suplementary_Quantity), 4));

                if (itmcount == 0) //cdoc.DocumentItems.Select(x => x.xcuda_PreviousItem).Count() == 1 ||
                {
                    pitm.Packages_number = "1"; //(i.Packages.Number_of_packages).ToString();
                    pitm.Previous_Packages_number = pitm.Previous_item_number == 1 ? "1" : "0";
                }
                else
                {
                    if (pitm.Packages_number == null)
                    {
                        pitm.Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                        pitm.Previous_Packages_number = (0).ToString(CultureInfo.InvariantCulture);
                    }
                }

                //if (ditm.xcuda_Supplementary_unit.Count > 1)
                //{
                //    var sup = ditm.xcuda_Supplementary_unit[1];
                    itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true)
                    {
                        
                        Suppplementary_unit_code = ditm.Suppplementary_unit_code,
                        Suppplementary_unit_quantity = ditm.ItemQuantity,
                        IsFirstRow = true,
                        TrackingState = TrackingState.Added
                    });
                //}


                itm.ImportComplete = true;
                return itm;

            }
            catch (Exception)
            {
                throw;
            }
        }

       
    }

    internal class AsycudaDocumentItemIM9
    {
        public int LineNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int AsycudaDocumentId { get; set; }
        public string ItemNumber { get; set; }
        public double ItemQuantity { get; set; }
        public double PiQuantity { get; set; }
        public string TariffCode { get; set; }
        public int Number_of_packages { get; set; }
        public decimal Net_weight { get; set; }
        public string CNumber { get; set; }
        public string Customs_clearance_office_code { get; set; }
        public string Commercial_Description { get; set; }
        public double Item_price { get; set; }
        public string Suppplementary_unit_code { get; set; }
        public int ItemId { get; set; }
        public decimal PiWeight { get; set; }
        public string Country_of_origin_code { get; set; }
        public double Total_CIF_itm { get; set; }
        public double SalesFactor { get; set; }
    }
}
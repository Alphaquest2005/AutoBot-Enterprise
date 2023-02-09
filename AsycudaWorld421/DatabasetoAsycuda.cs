using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using WaterNut.DataLayer;

namespace Asycuda421
{
    public partial class ASYCUDA
    {
        private FileInfo _destinatonFile;

        //easier to do this so because of the deep layers rather than calling each one
        private readonly WaterNutDBEntities db = new WaterNutDBEntities();
        private string DocSetPath;

        public void LoadFromDataBase(string docSetPath, int ASYCUDA_Id, ASYCUDA a, FileInfo fileInfo)
        {
            _destinatonFile = fileInfo;
            DocSetPath = docSetPath;
            var doc = db.xcuda_ASYCUDA.FirstOrDefault(x => x.ASYCUDA_Id == ASYCUDA_Id);
            LoadFromDataBase(doc, a);
        }

        public void LoadFromDataBase(xcuda_ASYCUDA da, ASYCUDA a)
        {
            try
            {
                SetupProperties(a, da);
                SaveGeneralInformation(a, da);
                SaveTraders(a, da);
                SaveProperty(a, da);
                SaveDeclarant(da, a);
                SaveContainer(da, a);
                SaveIdentification(da, a);
                SaveItem(da, a);
                SavePreviousItem(da, a);
                SaveValuationItem(da, a);

                // if (a.Valuation.Gs_Invoice.Amount_foreign_currency == null)
                a.Valuation.Gs_Invoice.Amount_foreign_currency = Math.Round(da.xcuda_Item.Where(i =>
                        i.xcuda_Valuation_item.xcuda_Item_Invoice != null
                        && (i.xcuda_PreviousItem == null || !string.IsNullOrEmpty(i.xcuda_PreviousItem.Prev_reg_nbr)))
                    .Sum(i => i.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency), 2).ToString();
                var totalPkgs = a.Item.Select(x => Convert.ToInt32(x.Packages.Number_of_packages)).Sum();
                if (totalPkgs > 1) a.Property.Nbers.Total_number_of_packages = totalPkgs.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveContainer(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Container.Count > 0)
            {
                a.Transport.Container_flagSpecified = true;
                a.Transport.Container_flag = true;
                foreach (var cnt in da.xcuda_Container)
                {
                    a.Container = new ObservableCollection<ASYCUDAContainer>();


                    var ac = new ASYCUDAContainer();
                    a.Container.Add(ac);

                    ac.Item_Number = cnt.Item_Number;
                    ac.Container_identity = cnt.Container_identity;
                    ac.Container_type = cnt.Container_type;
                    ac.Empty_full_indicator = cnt.Empty_full_indicator;
                    if (ac.Gross_weight == null) ac.Gross_weight = new ASYCUDAContainerGross_weight();
                    ac.Gross_weight.Text.Add(cnt.Gross_weight.ToString());

                    ac.Goods_description.Text.Clear();
                    ac.Goods_description.Text.Add(cnt.Goods_description);

                    ac.Packages_number = cnt.Packages_number;
                    ac.Packages_type = cnt.Packages_type;
                    ac.Packages_weight = cnt.Packages_weight.ToString();
                }
            }
        }

        private void SaveProperty(ASYCUDA a, xcuda_ASYCUDA da)
        {
            if (da.xcuda_Property != null)
            {
                if (da.xcuda_Property.Date_of_declaration != null)
                    a.Property.Date_of_declaration = da.xcuda_Property.Date_of_declaration;
                // a.Property.Place_of_declaration. = da.xcuda_Property.Place_of_declaration;
                if (da.xcuda_Property.Sad_flow != null)
                    a.Property.Sad_flow = da.xcuda_Property.Sad_flow;
                if (da.xcuda_Property.Selected_page != null)
                    a.Property.Selected_page = da.xcuda_Property.Selected_page;

                if (da.xcuda_Property.xcuda_Forms != null)
                {
                    if (da.xcuda_Property.xcuda_Forms.Number_of_the_form != null)
                        a.Property.Forms.Number_of_the_form =
                            da.xcuda_Property.xcuda_Forms.Number_of_the_form.ToString();
                    if (da.xcuda_Property.xcuda_Forms.Total_number_of_forms != null)
                        a.Property.Forms.Total_number_of_forms =
                            da.xcuda_Property.xcuda_Forms.Total_number_of_forms.ToString();
                }

                if (da.xcuda_Property.xcuda_Nbers != null)
                {
                    if (da.xcuda_Property.xcuda_Nbers.Number_of_loading_lists != null)
                        a.Property.Nbers.Number_of_loading_lists =
                            da.xcuda_Property.xcuda_Nbers.Number_of_loading_lists;
                    if (da.xcuda_Property.xcuda_Nbers.Total_number_of_items != null)
                        a.Property.Nbers.Total_number_of_items = da.xcuda_Property.xcuda_Nbers.Total_number_of_items;
                    // if(da.xcuda_Property.xcuda_Nbers.Total_number_of_packages != 0)
                    a.Property.Nbers.Total_number_of_packages =
                        da.xcuda_Property.xcuda_Nbers.Total_number_of_packages.ToString();
                }
            }
        }

        private void SaveTraders(ASYCUDA a, xcuda_ASYCUDA da)
        {
            if (da.xcuda_Traders != null)
            {
                var t = new ASYCUDATraders();

                if (da.xcuda_Traders.xcuda_Consignee != null)
                {
                    t.Consignee = new ASYCUDATradersConsignee();
                    if (da.xcuda_Traders.xcuda_Consignee.Consignee_code != null)
                        t.Consignee.Consignee_code.Text.Add(da.xcuda_Traders.xcuda_Consignee.Consignee_code);
                    if (da.xcuda_Traders.xcuda_Consignee.Consignee_name != null)
                        t.Consignee.Consignee_name.Text.Add(da.xcuda_Traders.xcuda_Consignee.Consignee_name);
                }

                if (da.xcuda_Traders.xcuda_Exporter != null)
                {
                    if (da.xcuda_Traders.xcuda_Exporter.Exporter_code != null)
                        t.Exporter.Exporter_code.Text.Add(da.xcuda_Traders.xcuda_Exporter.Exporter_code);
                    if (da.xcuda_Traders.xcuda_Exporter.Exporter_name != null)
                        t.Exporter.Exporter_name.Text.Add(da.xcuda_Traders.xcuda_Exporter.Exporter_name);
                }

                if (da.xcuda_Traders.xcuda_Traders_Financial != null)
                {
                    if (da.xcuda_Traders.xcuda_Traders_Financial.Financial_code != null)
                        t.Financial.Financial_code.Text.Add(da.xcuda_Traders.xcuda_Traders_Financial.Financial_code);
                    if (da.xcuda_Traders.xcuda_Traders_Financial.Financial_name != null)
                        t.Financial.Financial_name.Text.Add(da.xcuda_Traders.xcuda_Traders_Financial.Financial_name);
                }

                a.Traders = t;
            }
        }

        private void SaveValuationItem(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Valuation != null) //&& da.xcuda_Valuation.xcuda_Gs_Invoice.Amount_foreign_currency != 0
            {
                var v = new ASYCUDAValuation();
                if (da.xcuda_Valuation.Calculation_working_mode != null)
                    v.Calculation_working_mode = da.xcuda_Valuation.Calculation_working_mode;
                SaveGSInvoice(v, da.xcuda_Valuation.xcuda_Gs_Invoice);
                SaveGSExternalFreight(v, da.xcuda_Valuation.xcuda_Gs_external_freight);
                SaveGSInternalFreight(v, da.xcuda_Valuation.xcuda_Gs_internal_freight);
                SaveGSOtherCost(v, da.xcuda_Valuation.xcuda_Gs_other_cost);
                SaveGSInsurance(v, da.xcuda_Valuation.xcuda_Gs_insurance);
                SaveGSDeduction(v, da.xcuda_Valuation.xcuda_Gs_deduction);
                SaveWeight(v, da.xcuda_Valuation.xcuda_Weight);
                a.Valuation = v;
            }
        }

        private void SaveGSDeduction(ASYCUDAValuation v, xcuda_Gs_deduction df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_deduction
                {
                    Amount_foreign_currency = df.Amount_foreign_currency.ToString(),
                    Amount_national_currency = df.Amount_national_currency.ToString()
                };
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code.Trim());
                f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_deduction = f;
            }
        }

        private void SaveGSInsurance(ASYCUDAValuation v, xcuda_Gs_insurance df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_insurance
                {
                    Amount_foreign_currency = df.Amount_foreign_currency.ToString(),
                    Amount_national_currency = df.Amount_national_currency.ToString()
                };
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code.Trim());
                f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_insurance = f;
            }
        }

        private void SaveGSOtherCost(ASYCUDAValuation v, xcuda_Gs_other_cost df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_other_cost
                {
                    Amount_foreign_currency = df.Amount_foreign_currency.ToString(),
                    Amount_national_currency = df.Amount_national_currency.ToString()
                };
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code.Trim());
                f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_other_cost = f;
            }
        }

        private void SaveGSInternalFreight(ASYCUDAValuation v, xcuda_Gs_internal_freight df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_internal_freight
                {
                    Amount_foreign_currency = df.Amount_foreign_currency.ToString(),
                    Amount_national_currency = df.Amount_national_currency.ToString()
                };
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code.Trim());
                f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_internal_freight = f;
            }
        }

        private void SaveWeight(ASYCUDAValuation v, xcuda_Weight dw)
        {
            if (dw != null)
            {
                var w = new ASYCUDAValuationWeight
                {
                    Gross_weight = dw.Gross_weight.ToString()
                };
                v.Weight = w;
            }
        }

        private void SaveGSExternalFreight(ASYCUDAValuation v, xcuda_Gs_external_freight df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_external_freight
                {
                    Amount_foreign_currency = df.Amount_foreign_currency.ToString(),
                    Amount_national_currency = df.Amount_national_currency.ToString()
                };
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code.Trim());

                f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_external_freight = f;
            }
        }

        private void SaveGSInvoice(ASYCUDAValuation v, xcuda_Gs_Invoice di)
        {
            if (di != null) //&& di.Amount_foreign_currency != 0
            {
                var inv = new ASYCUDAValuationGs_Invoice();
                if (di.Amount_foreign_currency != 0)
                    inv.Amount_foreign_currency = di.Amount_foreign_currency.ToString();
                if (di.Amount_national_currency != 0)
                    inv.Amount_national_currency = di.Amount_national_currency.ToString();
                if (di.Currency_code != null)
                    inv.Currency_code.Text.Add(di.Currency_code.Trim());
                if (di.Currency_name != null)
                    inv.Currency_name.Text.Add(di.Currency_name);
                if (di.Currency_rate != 0)
                    inv.Currency_rate = di.Currency_rate.ToString();

                v.Gs_Invoice = inv;
            }
        }

        private void SaveGeneralInformation(ASYCUDA a, xcuda_ASYCUDA da)
        {
            if (da.xcuda_General_information != null && da.xcuda_General_information.xcuda_Country != null)
            {
                var gi = new ASYCUDAGeneral_information();

                SaveCountry(gi, da.xcuda_General_information);
                gi.Value_details = da.xcuda_General_information.Value_details;
                gi.Comments_free_text.Text.Add(da.xcuda_General_information.Comments_free_text);
                a.General_information = gi;
            }
        }

        private void SaveCountry(ASYCUDAGeneral_information gi, xcuda_General_information dg)
        {
            if (dg.xcuda_Country != null)
            {
                var c = new ASYCUDAGeneral_informationCountry();
                c.Country_first_destination.Text.Add(dg.xcuda_Country.Country_first_destination);
                c.Country_of_origin_name = dg.xcuda_Country.Country_of_origin_name;

                c.Destination = new ASYCUDAGeneral_informationCountryDestination();
                if (dg.xcuda_Country.xcuda_Destination != null)
                {
                    c.Destination.Destination_country_code.Text.Add(dg.xcuda_Country.xcuda_Destination
                        .Destination_country_code);
                    c.Destination.Destination_country_name.Text.Add(dg.xcuda_Country.xcuda_Destination
                        .Destination_country_name);
                    //c.Destination.Destination_country_region.Text.Add(dg.xcuda_Country.xcuda_Destination.
                }

                if (dg.xcuda_Country.xcuda_Export != null)
                {
                    c.Export.Export_country_code.Text.Add(dg.xcuda_Country.xcuda_Export.Export_country_code);
                    c.Export.Export_country_name.Text.Add(dg.xcuda_Country.xcuda_Export.Export_country_name);
                    c.Export.Export_country_region.Text.Add(dg.xcuda_Country.xcuda_Export.Export_country_region);
                }

                if (dg.xcuda_Country.Trading_country != null)
                    c.Trading_country.Text.Add(dg.xcuda_Country.Trading_country);
                gi.Country = c;
            }
        }

        private void SavePreviousItem(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_PreviousItem != null)
            {
                var lncounter = 0;
                foreach (var item in da.xcuda_PreviousItem.OrderBy(x => Convert.ToInt32(x.Current_item_number)))
                {
                    //if (item.Prev_decl_HS_spec.Length > 17) continue;
                    if (string.IsNullOrEmpty(item.Prev_reg_nbr)) continue;
                    lncounter += 1;
                    var pi = new ASYCUDAPrev_decl
                    {
                        Prev_decl_office_code = item.Prev_reg_cuo,
                        Prev_decl_reg_year = item.Prev_reg_year.ToString(),
                        Prev_decl_reg_serial = new ASYCUDAPrev_declPrev_decl_reg_serial(){Text = new ObservableCollection<string>(){ item.Prev_reg_ser } } ,
                        Prev_decl_reg_number = item.Prev_reg_nbr,
                        Prev_decl_item_number = item.Previous_item_number.GetValueOrDefault().ToString(),
                        Prev_decl_HS_code = new ASYCUDAPrev_declPrev_decl_HS_code() {Text = new ObservableCollection<string>() {item.Hs_code}} ,
                        Prev_decl_HS_prec = new ASYCUDAPrev_declPrev_decl_HS_prec() {Text = new ObservableCollection<string>(){ item.Commodity_code } } ,
                        Prev_decl_country_origin = new ASYCUDAPrev_declPrev_decl_country_origin() {Text = new ObservableCollection<string>() {item.Goods_origin}} ,
                        Prev_decl_number_packages = item.Packages_number,
                        Prev_decl_weight = item.Net_weight.ToString(),
                        Prev_decl_supp_quantity = item.Suplementary_Quantity.ToString(),
                        Prev_decl_ref_value = Math.Round(item.Current_value, 4).ToString(),
                        Prev_decl_current_item = lncounter.ToString(), //item.Current_item_number;
                        Prev_decl_number_packages_written_off = item.Previous_Packages_number,
                        Prev_decl_weight_written_off = item.Prev_net_weight.ToString(),
                        Prev_decl_supp_quantity_written_off = item.Preveious_suplementary_quantity.ToString(),
                        Prev_decl_ref_value_written_off = Math.Round(item.Previous_value, 4).ToString(),
                        Prev_decl_HS_spec = new ASYCUDAPrev_declPrev_decl_HS_spec(){Text = new ObservableCollection<string>(){item.Prev_decl_HS_spec.Length <= 20 ? item.Prev_decl_HS_spec : ""}} 
                    }; //ASYCUDAPreviousItem

                    a.Prev_decl.Add(pi);
                }

                if (a.Prev_decl.Any())
                    a.Prev_decl[0].Prev_decl_number_packages = a.Item[0].Packages.Number_of_packages;
            }
        }

        private void SetupProperties(ASYCUDA a, xcuda_ASYCUDA da)
        {
            ExportTemplate Exp = null;
            if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure != null)
                Exp = db.ExportTemplate
                    .Where(x => x.ApplicationSettingsId ==
                                da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId)
                    .FirstOrDefault(x =>
                        x.Customs_Procedure == da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsProcedure);// x.Description == da.xcuda_ASYCUDA_ExtendedProperties.Document_Type.DisplayName

            //if (Exp == null && da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet != null && da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ExportTemplate != null)
            //{
            //    Exp = da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ExportTemplate;
            //}

            if (Exp == null)
                throw new ApplicationException(
                    $"Export Template is Null for {da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure?.Document_Type.DisplayName}");
            a.Financial = new ASYCUDAFinancial();

            if (Exp.Deffered_payment_reference != null)
                a.Financial.Deffered_payment_reference.Text.Add(Exp.Deffered_payment_reference);

            a.Export_release = new ASYCUDAExport_release();
            a.Identification.Office_segment.Customs_clearance_office_code.Text.Add(
                da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Office ?? Exp.Customs_clearance_office_code);

            a.General_information = new ASYCUDAGeneral_information();
            //a.Property = new ASYCUDAProperty();
            //a.Property.Forms = new ASYCUDAPropertyForms();
            a.Transport.Single_waybill_flag = true;
            a.Transport.Container_flag = false;
            a.Container = null;
            a.Property.Forms.Number_of_the_form = "1";
            a.Property.Nbers.Number_of_loading_lists = "1";
            a.Property.Nbers.Total_number_of_packages = "1";
            a.Property.Forms.Total_number_of_forms = "1";
            a.Property.Selected_page = "1";
            a.Traders = new ASYCUDATraders();
            if (Exp.Exporter_code != null)
                a.Traders.Exporter.Exporter_code.Text.Add(Exp.Exporter_code);
            if (Exp.Exporter_name != null)
                a.Traders.Exporter.Exporter_name.Text.Add(Exp.Exporter_name);
            if (Exp.Financial_code != null)
                a.Traders.Financial.Financial_code.Text.Add(Exp.Financial_code);
            if (Exp.Consignee_code != null)
                a.Traders.Consignee.Consignee_code.Text.Add(Exp.Consignee_code);
            if (Exp.Consignee_name != null)
                a.Traders.Consignee.Consignee_name.Text.Add(Exp.Consignee_name);
            a.Transit = new ASYCUDATransit();

            if (Exp.Country_first_destination != null)
                a.General_information.Country.Country_first_destination.Text.Add(Exp.Country_first_destination);

            if (Exp.Trading_country != null)
                a.General_information.Country.Trading_country.Text.Add(Exp.Trading_country);

            if (Exp.Export_country_code != null)
                a.General_information.Country.Export.Export_country_code.Text.Add(Exp.Export_country_code);

            if (Exp.Destination_country_code != null)
                a.General_information.Country.Destination.Destination_country_code.Text.Add(
                    Exp.Destination_country_code);

            if (Exp.TransportName != null)
                a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text.Add(Exp.TransportName);

            if (Exp.TransportNationality != null)
                a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text.Add(
                    Exp.TransportNationality);

            if (Exp.Location_of_goods != null)
                a.Transport.Location_of_goods.Text.Add(
                    da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.LocationOfGoods ?? Exp.Location_of_goods);

            if (Exp.Border_information_Mode != null)
            {
                a.Transport.Means_of_transport.Border_information.Identity.Text.Add(Exp.TransportName);
                a.Transport.Means_of_transport.Border_information.Nationality.Text.Add(Exp.TransportNationality);
                a.Transport.Means_of_transport.Border_information.Mode.Text.Add(Exp.Border_information_Mode);
            }


            if (Exp.Delivery_terms_Code != null)
                a.Transport.Delivery_terms.Code.Text.Add(Exp.Delivery_terms_Code);

            a.Warehouse = new ASYCUDAWarehouse();
            if (Exp.Warehouse_Delay != null)
                a.Warehouse.Delay = Exp.Warehouse_Delay;

            if (Exp.Warehouse_Identification != null)
                a.Warehouse.Identification.Text.Add(Exp.Warehouse_Identification);


            a.Valuation = new ASYCUDAValuation();

            if (Exp.Gs_Invoice_Currency_code != null)
                a.Valuation.Gs_Invoice.Currency_code.Text.Add(Exp.Gs_Invoice_Currency_code);


            a.Valuation.Gs_Invoice.Amount_foreign_currency = Math
                .Round(
                    da.xcuda_Item.Where(i => i.xcuda_Valuation_item.xcuda_Item_Invoice != null).Sum(i =>
                        i.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency), 2).ToString();
            a.Supplier_documents.Add(new ASYCUDASupplier_documents
            {
                Invoice_supplier_city = new ASYCUDASupplier_documentsInvoice_supplier_city {@null = new object()},

                Invoice_supplier_country = new ASYCUDASupplier_documentsInvoice_supplier_country {@null = new object()},

                Invoice_supplier_fax = new ASYCUDASupplier_documentsInvoice_supplier_fax {@null = new object()},

                Invoice_supplier_name = new ASYCUDASupplier_documentsInvoice_supplier_name {@null = new object()},
                Invoice_supplier_street = new ASYCUDASupplier_documentsInvoice_supplier_street {@null = new object()},
                Invoice_supplier_telephone =
                    new ASYCUDASupplier_documentsInvoice_supplier_telephone {@null = new object()},

                Invoice_supplier_zip_code =
                    new ASYCUDASupplier_documentsInvoice_supplier_zip_code {@null = new object()}
            });
        }

        private void SaveIdentification(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification != null)
            {
                SaveOfficeSegment(da, a);
                SaveManifestReferenceNumber(da, a);
                SaveRegistration(da, a);
                SaveType(da, a);
            }
        }

        private void SaveType(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Type != null)
            {
                if (da.xcuda_Identification.xcuda_Type.Type_of_declaration != null)
                    a.Identification.Type.Type_of_declaration = da.xcuda_Identification.xcuda_Type.Type_of_declaration;
                if (da.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code != null)
                    a.Identification.Type.Declaration_gen_procedure_code =
                        da.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code;
            }
        }

        private void SaveItem(xcuda_ASYCUDA da, ASYCUDA a)
        {
            try
            {
                if (da.xcuda_Item != null)
                {
                    var lnCounter = 0;
                    foreach (var item in da.xcuda_Item.OrderBy(x => x.LineNumber))
                    {
                        //if (item.ItemNumber.Length > 17) continue;

                        if (item.xcuda_PreviousItem != null &&
                            string.IsNullOrEmpty(item.xcuda_PreviousItem.Prev_reg_nbr)) continue;

                        lnCounter += 1;
                        item.LineNumber = lnCounter;
                        var ai = SetupItemProperties(da);

                        if (item.Licence_number != null)
                            ai.Licence_number.Text.Add(item.Licence_number);
                        if (item.Quantity_deducted_from_licence != null)
                            ai.Quantity_deducted_from_licence = item.Quantity_deducted_from_licence;
                        if (item.Free_text_1 != null)
                            ai.Free_text_1.Text.Add(item.Free_text_1);
                        if (item.Free_text_2 != null)
                            ai.Free_text_2.Text.Add(item.Free_text_2);


                        SaveAttachedDocuments(item, ai);
                        if (item.xcuda_Tarification == null)
                            throw new Exception("Null Tarification, for item number: " + item.ItemNumber);

                        SaveTarification(item, ai);
                        SaveGoodsDescription(item, ai);
                        SavePreviousDoc(item, ai);
                        SavePackages(item, ai, lnCounter);
                        SaveValuationItem(item, ai);
                        if (db.TariffCodes.FirstOrDefault(x =>
                                x.TariffCode == item.xcuda_Tarification.xcuda_HScode.Commodity_code &&
                                x.LicenseRequired == true) != null)
                            ai.Quantity_deducted_from_licence =
                                item.ItemQuantity.ToString(CultureInfo.InvariantCulture);

                        a.Item.Add(ai);
                    }

                    if (a.Item.Count != 0 && a.Item[0].Packages.Number_of_packages == "0")
                        a.Item[0].Packages.Number_of_packages = "1";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SavePackages(xcuda_Item item, ASYCUDAItem ai, int lnCounter)
        {
            if (item.xcuda_Packages.Count > 0 && item.xcuda_Packages.First().Number_of_packages > 0)
            {
                var pk = item.xcuda_Packages.First();
                ai.Packages.Number_of_packages = pk.Number_of_packages.ToString(CultureInfo.InvariantCulture);
                ai.Packages.Marks1_of_packages.Text.Clear();
                ai.Packages.Marks1_of_packages.Text.Add(pk.Marks1_of_packages);
                if (!string.IsNullOrEmpty(pk.Marks2_of_packages))
                {
                    ai.Packages.Marks2_of_packages.Text.Clear();
                    ai.Packages.Marks2_of_packages.Text.Add(pk.Marks2_of_packages);
                }

                ai.Packages.Kind_of_packages_code.Text.Clear();
                ai.Packages.Kind_of_packages_code.Text.Add(string.IsNullOrEmpty(pk.Kind_of_packages_code)
                    ? "PK"
                    : pk.Kind_of_packages_code);
            }
            else
            {
                if (lnCounter != 1) return;
                ai.Packages.Number_of_packages = "1";
                ai.Packages.Marks1_of_packages.Text.Clear();
                ai.Packages.Marks1_of_packages.Text.Add("No Marks");
                ai.Packages.Kind_of_packages_code.Text.Clear();
                ai.Packages.Kind_of_packages_code.Text.Add("PK");
            }
        }


        private void SavePreviousDoc(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Previous_doc != null)
            {
                if (item.xcuda_Previous_doc.Summary_declaration != null)
                    ai.Previous_doc.Summary_declaration.Text.Add(item.xcuda_Previous_doc.Summary_declaration);
                if (item.xcuda_Previous_doc.Previous_document_reference != null)
                    ai.Previous_doc.Previous_document_reference.Text.Add(item.xcuda_Previous_doc
                        .Previous_document_reference);
                if (item.xcuda_Previous_doc.Previous_warehouse_code != null)
                    ai.Previous_doc.Previous_warehouse_code.Text.Add(item.xcuda_Previous_doc.Previous_warehouse_code);
            }
            else
            {
                if (item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.BLNumber != null)
                    if (item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.BLNumber != null)
                        ai.Previous_doc.Summary_declaration.Text.Add(item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties
                            .AsycudaDocumentSet.BLNumber);
            }
        }

        private void SaveAttachedDocuments(xcuda_Item item, ASYCUDAItem ai)
        {
            try
            {

           
            foreach (var doc in item.xcuda_Attached_documents)
            {
                var adoc = new ASYCUDAItemAttached_documents();
                if (doc.Attached_document_code != null)
                    adoc.Attached_document_code.Text.Add(doc.Attached_document_code);
                if (doc.Attached_document_date != null)
                    adoc.Attached_document_date = doc.Attached_document_date;
                if (doc.Attached_document_reference != null)
                    adoc.Attached_document_reference.Text.Add(doc.Attached_document_reference.Truncate(30));
                if (doc.Attached_document_from_rule != null)
                    adoc.Attached_document_from_rule.Text.Add(doc.Attached_document_from_rule.ToString());
                if (doc.Attached_document_name != null)
                    adoc.Attached_document_name.Text.Add(doc.Attached_document_name);
                ai.Attached_documents.Add(adoc);
                if (doc.xcuda_Attachments.Any(x => x.Attachments.Reference != "Info"))
                {
                    var att = doc.xcuda_Attachments.FirstOrDefault(x => x.Attachments.Reference != "Info");
                    if (att == null) continue;
                    var filePath = att.Attachments.FilePath;
                    if (string.IsNullOrEmpty(filePath))
                    {
                        File.AppendAllText(Path.Combine(_destinatonFile.DirectoryName, "Instructions.txt"),
                            $"{doc.Attached_documents_Id}\tAttachment\tBlank File\r\n");
                    }
                    else
                    {
                        var fileinfo = new FileInfo(filePath);
                        if (fileinfo.Extension != ".pdf" && File.Exists(filePath)) fileinfo = Change2Pdf(fileinfo);
                        var desFile = DocSetPath != null &&  DocSetPath != _destinatonFile.DirectoryName && fileinfo.DirectoryName + "\\" != AppDomain.CurrentDomain.BaseDirectory
                            ? fileinfo.FullName.Replace($"{DocSetPath}", _destinatonFile.DirectoryName)
                            : Path.Combine(_destinatonFile.DirectoryName, fileinfo.Name);
                        // var desFile = Path.Combine(desPath, fileinfo.Name);
                        if (!File.Exists(desFile) && fileinfo.DirectoryName + "\\" != AppDomain.CurrentDomain.BaseDirectory) desFile = fileinfo.FullName; //create sales files first before entries// took out because of sales file not created yet
                        File.AppendAllText(Path.Combine(_destinatonFile.DirectoryName, "Instructions.txt"),
                            $"{doc.Attached_documents_Id}\tAttachment\t{desFile}\r\n");
                    }
                }
            }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private FileInfo Change2Pdf(FileInfo fileinfo)
        {
            try
            {
                var res = new FileInfo(fileinfo.FullName + ".pdf");
                if (res.Exists) return res;
                File.Copy(fileinfo.FullName, res.FullName);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private ASYCUDAItem SetupItemProperties(xcuda_ASYCUDA da)
        {
            var ai = new ASYCUDAItem
            {
                Suppliers_link =
                {
                    Suppliers_link_code = "1"
                }
            };

            ai.Tarification.HScode.Precision_1.Text.Add("00");

            ai.Packages.Number_of_packages = "0";
            ai.Packages.Kind_of_packages_code.Text.Add("PK");
            ai.Packages.Kind_of_packages_name.Text.Add("Package");
            ai.Packages.Marks1_of_packages.Text.Add("Marks");
            ai.Packages.Marks2_of_packages.Text.Add("SAME");

            ai.Valuation_item.Weight_itm.Gross_weight_itm = "1"; //(Decimal)ops.Quantity;
            ai.Valuation_item.Weight_itm.Net_weight_itm = "1"; //(Decimal)ops.Quantity;

            ////////////////////// new upgrade dose not update based on first line so this is irrelevant
            if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure != null)
            {
                if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure != null)
                    ai.Tarification.Extended_customs_procedure.Text.Add(da.xcuda_ASYCUDA_ExtendedProperties
                        .Customs_Procedure.Extended_customs_procedure);
                if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure != null)
                    ai.Tarification.National_customs_procedure.Text.Add(da.xcuda_ASYCUDA_ExtendedProperties
                        .Customs_Procedure.National_customs_procedure);
            }

            ai.Tarification.Supplementary_unit.Add(new ASYCUDAItemTarificationSupplementary_unit());
            ai.Tarification.Supplementary_unit.Add(new ASYCUDAItemTarificationSupplementary_unit());


            //ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            //ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            //ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            //ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());


            ai.Taxation = new ASYCUDAItemTaxation();
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());
            ai.Taxation.Taxation_line.Add(new ASYCUDAItemTaxationTaxation_line());

            ai.Valuation_item.Market_valuer = new ASYCUDAItemValuation_itemMarket_valuer();


            return ai;
        }

        private void SaveValuationItem(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Valuation_item != null)
            {
                if (item.Statistical_value != 0)
                    ai.Valuation_item.Statistical_value = item.Statistical_value.ToString();
                if (item.xcuda_Valuation_item.Total_CIF_itm != 0)
                    ai.Valuation_item.Total_CIF_itm = item.xcuda_Valuation_item.Total_CIF_itm.ToString();
                if (item.xcuda_Valuation_item.Total_cost_itm != 0)
                    ai.Valuation_item.Total_cost_itm = item.xcuda_Valuation_item.Total_cost_itm.ToString();
                if (item.xcuda_Valuation_item.xcuda_Item_Invoice != null &&
                    item.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency != 0)
                {
                    var ivc = item.xcuda_Valuation_item.xcuda_Item_Invoice;

                    var av = new ASYCUDAItemValuation_itemItem_Invoice
                    {
                        Amount_foreign_currency = Math.Round(ivc.Amount_foreign_currency, 2)
                            .ToString() //Convert.ToDecimal(ivc.Amount_foreign_currency);
                    };
                    if (ivc.Amount_national_currency != 0)
                        av.Amount_national_currency = ivc.Amount_national_currency.ToString();
                    if (ivc.Currency_code != null)
                        av.Currency_code.Text.Add(ivc.Currency_code.Trim());
                    if (ivc.Currency_rate != 0)
                        av.Currency_rate = ivc.Currency_rate.ToString();

                    ai.Valuation_item.Item_Invoice = av;
                }

                if (item.xcuda_Valuation_item.xcuda_item_external_freight != null
                    && item.xcuda_Valuation_item.xcuda_item_external_freight.Amount_foreign_currency != 0)
                {
                    var ief = item.xcuda_Valuation_item.xcuda_item_external_freight;

                    var af = new ASYCUDAItemValuation_itemItem_external_freight();
                    if (ief.Amount_foreign_currency != 0)
                        af.Amount_foreign_currency = ief.Amount_foreign_currency.ToString();
                    if (ief.Amount_national_currency != 0)
                        af.Amount_national_currency = ief.Amount_national_currency.ToString();
                    if (ief.Currency_code != null)
                        af.Currency_code.Text.Add(ief.Currency_code.Trim());
                    if (ief.Currency_rate != 0)
                        af.Currency_rate = ief.Currency_rate.ToString();

                    ai.Valuation_item.item_external_freight = af;
                }

                if (item.xcuda_Valuation_item.xcuda_Weight_itm != null &&
                    item.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm != 0)
                {
                    var wi = new ASYCUDAItemValuation_itemWeight_itm();
                    if (item.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm != 0)
                        wi.Gross_weight_itm = item.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm.ToString();
                    if (item.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm != 0)
                        wi.Net_weight_itm = item.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm.ToString();

                    ai.Valuation_item.Weight_itm = wi;
                }
            }
        }


        private void SaveGoodsDescription(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Goods_description != null && item.xcuda_Goods_description.Country_of_origin_code != null)
            {
                if (item.xcuda_Goods_description.Commercial_Description != null)
                    ai.Goods_description.Commercial_Description.Text.Add(CleanText(item.xcuda_Goods_description
                        .Commercial_Description));
                if (item.xcuda_Goods_description.Country_of_origin_code != null)
                    ai.Goods_description.Country_of_origin_code.Text.Add(item.xcuda_Goods_description
                        .Country_of_origin_code);
                if (item.xcuda_Goods_description.Description_of_goods != null)
                    ai.Goods_description.Description_of_goods.Text.Add(
                        item.xcuda_Goods_description.Description_of_goods);
            }
        }

        private string CleanText(string p)
        {
            var s = p.Replace(",", "");
            var t = Regex.Replace(s, @"[\u0000-\u0008\u000B\u000C\u000E-\u001F]", "");
            var re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(t, re, "");
        }

        private void SaveTarification(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Tarification != null)
            {
                if (item.xcuda_Tarification.Extended_customs_procedure != null)
                {
                    ai.Tarification.Extended_customs_procedure.Text.Clear();
                    ai.Tarification.Extended_customs_procedure.Text.Add(item.xcuda_Tarification
                        .Extended_customs_procedure);
                }

                if (item.xcuda_Tarification.National_customs_procedure != null)
                {
                    ai.Tarification.National_customs_procedure.Text.Clear();
                    ai.Tarification.National_customs_procedure.Text.Add(item.xcuda_Tarification
                        .National_customs_procedure);
                }

                if (item.xcuda_Tarification.Item_price != 0)
                    ai.Tarification.Item_price = Math.Round(item.xcuda_Tarification.Item_price, 2).ToString();
                SaveHSCode(item, ai);
                SaveSupplementaryUnit(item, ai);
            }
        }

        private void SaveHSCode(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Tarification.xcuda_HScode != null)
            {
                if (item.xcuda_Tarification.xcuda_HScode.Commodity_code != null)
                {
                    if (!item.xcuda_Tarification.xcuda_HScode.Commodity_code.EndsWith("00") && item.xcuda_Tarification.xcuda_HScode.Commodity_code.Length == 10)
                    {
                        ai.Tarification.HScode.Precision_1.Text.Clear();
                        ai.Tarification.HScode.Precision_1.Text.Add(item.xcuda_Tarification.xcuda_HScode.Commodity_code.Substring(8,2));
                    }

                    ai.Tarification.HScode.Commodity_code.Text.Add(item.xcuda_Tarification.xcuda_HScode.Commodity_code.Truncate(8));
                } // item.xcuda_Tarification.xcuda_HScode.Commodity_code;
            }

            if (item.xcuda_Tarification.xcuda_HScode.Precision_1 != null)
            {
                ai.Tarification.HScode.Precision_1.Text.Clear();
                ai.Tarification.HScode.Precision_1.Text.Add(item.xcuda_Tarification.xcuda_HScode.Precision_1);
            }

            if (item.xcuda_Tarification.xcuda_HScode.Precision_4 != null && item.ItemNumber.Length <= 20) //
                ai.Tarification.HScode.Precision_4.Text.Add(item.xcuda_Tarification.xcuda_HScode.Precision_4.Trim()
                    .Truncate(20));

        }

        private void SaveSupplementaryUnit(xcuda_Item item, ASYCUDAItem ai)
        {
            for (var i = 0; i < item.xcuda_Tarification.xcuda_Supplementary_unit.Count; i++)
            {
                var supp = item.xcuda_Tarification.xcuda_Supplementary_unit.ElementAt(i);

                if (supp.Suppplementary_unit_quantity != null)
                {
                    var asupp = new ASYCUDAItemTarificationSupplementary_unit();
                    if (supp.Suppplementary_unit_code != null)
                        asupp.Suppplementary_unit_code.Text.Add(supp.Suppplementary_unit_code);
                    if (supp.Suppplementary_unit_name != null)
                        asupp.Suppplementary_unit_name.Text.Add(supp.Suppplementary_unit_name);
                    if (supp.Suppplementary_unit_quantity != null)
                        asupp.Suppplementary_unit_quantity =
                            supp.Suppplementary_unit_quantity.GetValueOrDefault().ToString();
                    ai.Tarification.Supplementary_unit.Insert(i, asupp);
                }
            }
        }

        private void SaveRegistration(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Registration != null)
            {
                if (da.xcuda_Identification.xcuda_Registration.Date != null)
                    a.Identification.Registration.Date = da.xcuda_Identification.xcuda_Registration.Date.ToString();
                if (da.xcuda_Identification.xcuda_Registration.Number != null)
                    a.Identification.Registration.Number = da.xcuda_Identification.xcuda_Registration.Number;
            }
        }

        private void SaveManifestReferenceNumber(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.Manifest_reference_number != null)
                a.Identification.Manifest_reference_number.Text.Add(da.xcuda_Identification.Manifest_reference_number);
        }

        private void SaveOfficeSegment(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Office_segment != null)
            {
                if (da.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code != null)
                    a.Identification.Office_segment.Customs_clearance_office_code.Text.Add(da.xcuda_Identification
                        .xcuda_Office_segment.Customs_clearance_office_code);
                if (da.xcuda_Identification.xcuda_Office_segment.Customs_Clearance_office_name != null)
                    a.Identification.Office_segment.Customs_Clearance_office_name.Text.Add(da.xcuda_Identification
                        .xcuda_Office_segment.Customs_Clearance_office_name);
            }
        }

        private void SaveDeclarant(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Declarant != null)
            {
                if (da.xcuda_Declarant.Declarant_code != null)
                    a.Declarant.Declarant_code.Text.Add(da.xcuda_Declarant.Declarant_code);
                if (da.xcuda_Declarant.Declarant_name != null)
                    a.Declarant.Declarant_name.Text.Add(da.xcuda_Declarant.Declarant_name);
                if (da.xcuda_Declarant.Declarant_representative != null)
                    a.Declarant.Declarant_representative.Text.Add(da.xcuda_Declarant.Declarant_representative);
                if (da.xcuda_Declarant.Number != null)
                    a.Declarant.Reference.Number.Text.Add(da.xcuda_Declarant.Number);
            }
        }
    }
}
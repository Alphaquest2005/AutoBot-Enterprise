using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterNut.DataLayer;



namespace WaterNut.Asycuda
{
    public partial class ASYCUDA
    {
        //easier to do this so because of the deep layers rather than calling each one
         WaterNutDBEntities db = new WaterNutDBEntities();

        public  void LoadFromDataBase(int ASYCUDA_Id, ASYCUDA a)
        {
            var doc = db.xcuda_ASYCUDA.FirstOrDefault(x => x.ASYCUDA_Id == ASYCUDA_Id);
            LoadFromDataBase(doc,a);
        }

        public  void LoadFromDataBase(xcuda_ASYCUDA da, ASYCUDA a)
        {
            try
            {
                SetupProperties(a, da);
                SaveGeneralInformation(a, da);
                SaveTraders(a, da);
                SaveProperty(a, da);
                SaveDeclarant(da, a);
                SaveIdentification(da, a);
                SaveItem(da, a);
                SavePreviousItem(da, a);
                SaveValuationItem(da, a);

                if (a.Valuation.Gs_Invoice.Amount_foreign_currency == null)
                    a.Valuation.Gs_Invoice.Amount_foreign_currency = Math.Round(da.xcuda_Item.Where(i => i.xcuda_Valuation_item.xcuda_Item_Invoice != null).Sum(i => i.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency), 2).ToString();
            }
            catch (Exception Ex)
            {
                throw;
            }


        }

        private  void SaveProperty(ASYCUDA a, xcuda_ASYCUDA da)
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
                        a.Property.Forms.Number_of_the_form = da.xcuda_Property.xcuda_Forms.Number_of_the_form.ToString();
                    if (da.xcuda_Property.xcuda_Forms.Total_number_of_forms != null)
                        a.Property.Forms.Total_number_of_forms = da.xcuda_Property.xcuda_Forms.Total_number_of_forms.ToString();
                }
                if (da.xcuda_Property.xcuda_Nbers != null)
                {
                    if (da.xcuda_Property.xcuda_Nbers.Number_of_loading_lists != null)
                        a.Property.Nbers.Number_of_loading_lists = da.xcuda_Property.xcuda_Nbers.Number_of_loading_lists;
                    if (da.xcuda_Property.xcuda_Nbers.Total_number_of_items != null)
                        a.Property.Nbers.Total_number_of_items = da.xcuda_Property.xcuda_Nbers.Total_number_of_items;
                    // if(da.xcuda_Property.xcuda_Nbers.Total_number_of_packages != 0)
                    a.Property.Nbers.Total_number_of_packages = da.xcuda_Property.xcuda_Nbers.Total_number_of_packages.ToString();
                }
            }
        }

        private  void SaveTraders(ASYCUDA a, xcuda_ASYCUDA da)
        {
            if (da.xcuda_Traders != null)
            {
                var t = new ASYCUDATraders();

                var c = new ASYCUDATradersConsignee();
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

                if (da.xcuda_Traders.xcuda_Traders_Financial != null && da.xcuda_Traders.xcuda_Traders_Financial.Financial_code != null)
                {
                    if (da.xcuda_Traders.xcuda_Traders_Financial.Financial_code != null)
                        t.Financial.Financial_code.Text.Add(da.xcuda_Traders.xcuda_Traders_Financial.Financial_code);
                    if (da.xcuda_Traders.xcuda_Traders_Financial.Financial_name != null)
                        t.Financial.Financial_name.Text.Add(da.xcuda_Traders.xcuda_Traders_Financial.Financial_name);
                }
                a.Traders = t;
            }
        }

        private  void SaveValuationItem(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Valuation != null)//&& da.xcuda_Valuation.xcuda_Gs_Invoice.Amount_foreign_currency != 0
            {
                var v = new ASYCUDAValuation();
                if (da.xcuda_Valuation.Calculation_working_mode != null)
                    v.Calculation_working_mode = da.xcuda_Valuation.Calculation_working_mode;
                SaveGSInvoice(v, da.xcuda_Valuation.xcuda_Gs_Invoice);
                SaveGSExternalFreight(v, da.xcuda_Valuation.xcuda_Gs_external_freight);
                SaveWeight(v, da.xcuda_Valuation.xcuda_Weight);
                a.Valuation = v;
            }
        }

        private  void SaveWeight(ASYCUDAValuation v, xcuda_Weight dw)
        {
            if (dw != null)
            {
                var w = new ASYCUDAValuationWeight();
                w.Gross_weight = dw.Gross_weight.ToString();
                v.Weight = w;
            }
        }

        private  void SaveGSExternalFreight(ASYCUDAValuation v, xcuda_Gs_external_freight df)
        {
            if (df != null && df.Amount_foreign_currency != 0)
            {
                var f = new ASYCUDAValuationGs_external_freight();
                f.Amount_foreign_currency = df.Amount_foreign_currency.ToString();
                f.Amount_national_currency = df.Amount_national_currency.ToString();
                if (df.Currency_code != null)
                    f.Currency_code.Text.Add(df.Currency_code);
                if (df.Currency_rate != null)
                    f.Currency_rate = df.Currency_rate.ToString();
                v.Gs_external_freight = f;
            }
        }

        private  void SaveGSInvoice(ASYCUDAValuation v, xcuda_Gs_Invoice di)
        {
            if (di != null)//&& di.Amount_foreign_currency != 0
            {
                var inv = new ASYCUDAValuationGs_Invoice();
                if (di.Amount_foreign_currency != 0)
                    inv.Amount_foreign_currency = di.Amount_foreign_currency.ToString();
                if (di.Amount_national_currency != 0)
                    inv.Amount_national_currency = di.Amount_national_currency.ToString();
                if (di.Currency_code != null)
                    inv.Currency_code.Text.Add(di.Currency_code);
                if (di.Currency_name != null)
                    inv.Currency_name.Text.Add(di.Currency_name);
                if (di.Currency_rate != 0)
                    inv.Currency_rate = di.Currency_rate.ToString();

                v.Gs_Invoice = inv;
            }
        }

        private  void SaveGeneralInformation(ASYCUDA a, xcuda_ASYCUDA da)
        {
            if (da.xcuda_General_information != null && da.xcuda_General_information.xcuda_Country != null
                        && da.xcuda_General_information.xcuda_Country.Country_first_destination != null
                        && da.xcuda_General_information.xcuda_Country.Trading_country != null)
            {
                var gi = new ASYCUDAGeneral_information();

                SaveCountry(gi, da.xcuda_General_information);
                a.General_information.Value_details = gi.Value_details;
                a.General_information = gi;

            }
        }

        private  void SaveCountry(ASYCUDAGeneral_information gi, xcuda_General_information dg)
        {
            if (dg.xcuda_Country != null)
            {
                var c = new ASYCUDAGeneral_informationCountry();
                c.Country_first_destination.Text.Add(dg.xcuda_Country.Country_first_destination);
                c.Country_of_origin_name = dg.xcuda_Country.Country_of_origin_name;

                c.Destination = new ASYCUDAGeneral_informationCountryDestination();
                if (dg.xcuda_Country.xcuda_Destination != null)
                {
                    c.Destination.Destination_country_code = dg.xcuda_Country.xcuda_Destination.Destination_country_code;
                    c.Destination.Destination_country_name = dg.xcuda_Country.xcuda_Destination.Destination_country_name;
                    //c.Destination.Destination_country_region = dg.xcuda_Country.xcuda_Destination.
                }

                if (dg.xcuda_Country.xcuda_Export != null)
                {
                    c.Export.Export_country_code = dg.xcuda_Country.xcuda_Export.Export_country_code;
                    c.Export.Export_country_name = dg.xcuda_Country.xcuda_Export.Export_country_name;
                }
                if (dg.xcuda_Country.Trading_country != null)
                {
                    c.Trading_country.Text.Add(dg.xcuda_Country.Trading_country);
                }
                gi.Country = c;
            }
        }

        private  void SavePreviousItem(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_PreviousItem != null)
            {
                foreach (var item in da.xcuda_PreviousItem)
                {
                    var pi = new ASYCUDAPreviousItem();
                    pi.Commodity_code = item.Commodity_code;
                    pi.Current_item_number = item.Current_item_number;
                    pi.Current_value = Math.Round(item.Current_value, 2).ToString();
                    pi.Goods_origin = item.Goods_origin;
                    pi.Hs_code = item.Hs_code;
                    pi.Net_weight = item.Net_weight.ToString();
                    pi.Packages_number = item.Packages_number;
                    pi.Prev_net_weight = item.Prev_net_weight.ToString();
                    pi.Prev_reg_cuo = item.Prev_reg_cuo;
                    pi.Prev_reg_dat = item.Prev_reg_dat;
                    pi.Prev_reg_nbr = item.Prev_reg_nbr;
                    pi.Prev_reg_ser = item.Prev_reg_ser;
                    pi.Preveious_suplementary_quantity = item.Preveious_suplementary_quantity.ToString();
                    pi.Previous_item_number = item.Previous_item_number;
                    pi.Previous_Packages_number = item.Previous_Packages_number;
                    pi.Previous_value = Math.Round(item.Previous_value, 2).ToString();
                    pi.Suplementary_Quantity = item.Suplementary_Quantity.ToString();
                    a.PreviousItem.Add(pi);
                }
                if (a.PreviousItem.Any() == true)
                    a.PreviousItem[0].Packages_number = a.Item[0].Packages.Number_of_packages;
            }
        }

        private  void SetupProperties(ASYCUDA a, xcuda_ASYCUDA da)
        {
            ExportTemplate Exp;

            Exp = db.ExportTemplate.AsEnumerable().Where(x => da.xcuda_ASYCUDA_ExtendedProperties.Document_Type != null && x.Description == da.xcuda_ASYCUDA_ExtendedProperties.Document_Type.DisplayName).FirstOrDefault();

            if (Exp == null && da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet != null && da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ExportTemplate != null)
            {
                Exp = da.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ExportTemplate;
            }

            if (Exp == null)
            {
                Exp = db.ExportTemplate.FirstOrDefault();
            }
            a.Financial = new ASYCUDAFinancial();

            if (Exp.Deffered_payment_reference != null)
                a.Financial.Deffered_payment_reference.Text.Add(Exp.Deffered_payment_reference);

            a.Export_release = new ASYCUDAExport_release();
            a.Identification.Office_segment.Customs_clearance_office_code.Text.Add(Exp.Customs_clearance_office_code);

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
                a.General_information.Country.Export.Export_country_code = Exp.Export_country_code;

            if (Exp.Destination_country_code != null)
                a.General_information.Country.Destination.Destination_country_code = Exp.Destination_country_code;

            if (Exp.TransportName != null)
                a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text.Add(Exp.TransportName);

            if (Exp.TransportNationality != null)
                a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text.Add(Exp.TransportNationality);

            if (Exp.Location_of_goods != null)
                a.Transport.Location_of_goods.Text.Add(Exp.Location_of_goods);

            if (Exp.Border_information_Mode != null)
                a.Transport.Means_of_transport.Border_information.Mode.Text.Add(Exp.Border_information_Mode);

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


            a.Valuation.Gs_Invoice.Amount_foreign_currency = Math.Round(da.xcuda_Item.Where(i => i.xcuda_Valuation_item.xcuda_Item_Invoice != null).Sum(i => i.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency), 2).ToString();
            a.Suppliers_documents.Add(new ASYCUDASuppliers_documents()
                {
                    Suppliers_document_invoice_amt = new ASYCUDASuppliers_documentsSuppliers_document_invoice_amt() { @null = new object() },
                    Suppliers_document_city = new ASYCUDASuppliers_documentsSuppliers_document_city() { @null = new object() },
                    Suppliers_document_code = new ASYCUDASuppliers_documentsSuppliers_document_code() { @null = new object() },
                    Suppliers_document_country = new ASYCUDASuppliers_documentsSuppliers_document_country() { @null = new object() },
                    Suppliers_document_invoice_nbr = new ASYCUDASuppliers_documentsSuppliers_document_invoice_nbr() { @null = new object() },
                    Suppliers_document_fax = new ASYCUDASuppliers_documentsSuppliers_document_fax() { @null = new object() },
                    Suppliers_document_itmlink = new ASYCUDASuppliers_documentsSuppliers_document_itmlink() { @null = new object() },
                    Suppliers_document_name = new ASYCUDASuppliers_documentsSuppliers_document_name() { @null = new object() },
                    Suppliers_document_street = new ASYCUDASuppliers_documentsSuppliers_document_street() { @null = new object() },
                    Suppliers_document_telephone = new ASYCUDASuppliers_documentsSuppliers_document_telephone() { @null = new object() },
                    Suppliers_document_type_code = new ASYCUDASuppliers_documentsSuppliers_document_type_code() { @null = new object() },
                    Suppliers_document_zip_code = new ASYCUDASuppliers_documentsSuppliers_document_zip_code() { @null = new object() },
                    Suppliers_document_date = DateTime.Now.ToString("dd/MM/yyyy")
                });




        }

        private  void SaveIdentification(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification != null)
            {


                SaveOfficeSegment(da, a);
                SaveManifestReferenceNumber(da, a);
                SaveRegistration(da, a);
                SaveType(da, a);

            }
        }

        private  void SaveType(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Type != null)
            {
                if (da.xcuda_Identification.xcuda_Type.Type_of_declaration != null)
                    a.Identification.Type.Type_of_declaration = da.xcuda_Identification.xcuda_Type.Type_of_declaration;
                if (da.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code != null)
                    a.Identification.Type.Declaration_gen_procedure_code = da.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code;
            }
        }

        private  void SaveItem(xcuda_ASYCUDA da, ASYCUDA a)
        {

            if (da.xcuda_Item != null)
            {
                foreach (var item in da.xcuda_Item)
                {
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
                    {
                        throw new Exception("Null Tarification, for item number: " + item.ItemNumber);
                    }
                    SaveTarification(item, ai);
                    SaveGoodsDescription(item, ai);
                    SavePreviousDoc(item, ai);

                    SaveValuationItem(item, ai);

                    a.Item.Add(ai);
                }
                if (a.Item.Count != 0)
                    a.Item[0].Packages.Number_of_packages = "1";
            }
        }



        private  void SavePreviousDoc(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Previous_doc != null)
            {
                if (item.xcuda_Previous_doc.Summary_declaration != null)
                    ai.Previous_doc.Summary_declaration.Text.Add(item.xcuda_Previous_doc.Summary_declaration);
            }
            else
            {
                if (item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.BLNumber != null)
                {
                    if (item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.BLNumber != null)
                        ai.Previous_doc.Summary_declaration.Text.Add(item.xcuda_ASYCUDA.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.BLNumber);
                }
            }

        }

        private  void SaveAttachedDocuments(xcuda_Item item, ASYCUDAItem ai)
        {
            var i = 0;
            foreach (var doc in item.xcuda_Attached_documents)
            {

                var adoc = ai.Attached_documents[i]; // new ASYCUDAItemAttached_documents();
                if (doc.Attached_document_code != null)
                    adoc.Attached_document_code.Text.Add(doc.Attached_document_code);
                if (doc.Attached_document_date != null)
                    adoc.Attached_document_date = doc.Attached_document_date;
                if (doc.Attached_document_reference != null)
                    adoc.Attached_document_reference.Text.Add(doc.Attached_document_reference);
                if (doc.Attached_document_from_rule != null)
                    adoc.Attached_document_from_rule.Text.Add(doc.Attached_document_from_rule.ToString());
                if (doc.Attached_document_name != null)
                    adoc.Attached_document_name.Text.Add(doc.Attached_document_name);
                // ai.Attached_documents.Add(adoc);
                i += 1;
            }
        }

        private  ASYCUDAItem SetupItemProperties(xcuda_ASYCUDA da)
        {


            var ai = new ASYCUDAItem();

            ai.Suppliers_link.Suppliers_link_code = "1";
            ai.Tarification.HScode.Precision_1 = "00";

            ai.Packages.Number_of_packages = "0";
            ai.Packages.Kind_of_packages_code = "PK";
            ai.Packages.Kind_of_packages_name = "Package";
            ai.Packages.Marks1_of_packages.Text.Add("Marks");
            ai.Packages.Marks2_of_packages.Text.Add("SAME");

            ai.Valuation_item.Weight_itm.Gross_weight_itm = "1"; //(Decimal)ops.Quantity;
            ai.Valuation_item.Weight_itm.Net_weight_itm = "1"; //(Decimal)ops.Quantity;
            if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure != null)
            {
                if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure != null)
                    ai.Tarification.Extended_customs_procedure = da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure;
                if (da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure != null)
                    ai.Tarification.National_customs_procedure = da.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.National_customs_procedure;
            }
            ai.Tarification.Supplementary_unit.Add(new ASYCUDAItemTarificationSupplementary_unit());
            ai.Tarification.Supplementary_unit.Add(new ASYCUDAItemTarificationSupplementary_unit());


            ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());
            ai.Attached_documents.Add(new ASYCUDAItemAttached_documents());


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

        private  void SaveValuationItem(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Valuation_item != null && item.Statistical_value != 0)
            {
                ai.Valuation_item.Statistical_value = item.Statistical_value.ToString();
                if (item.xcuda_Valuation_item.Total_CIF_itm != 0)
                    ai.Valuation_item.Total_CIF_itm = item.xcuda_Valuation_item.Total_CIF_itm.ToString();
                if (item.xcuda_Valuation_item.Total_cost_itm != 0)
                    ai.Valuation_item.Total_cost_itm = item.xcuda_Valuation_item.Total_cost_itm.ToString();
                if (item.xcuda_Valuation_item.xcuda_Item_Invoice != null && item.xcuda_Valuation_item.xcuda_Item_Invoice.Amount_foreign_currency != 0)
                {
                    var ivc = item.xcuda_Valuation_item.xcuda_Item_Invoice;

                    var av = new ASYCUDAItemValuation_itemItem_Invoice();
                    av.Amount_foreign_currency = Math.Round(ivc.Amount_foreign_currency, 2).ToString();//Convert.ToDecimal(ivc.Amount_foreign_currency);
                    if (ivc.Amount_national_currency != 0)
                        av.Amount_national_currency = ivc.Amount_national_currency.ToString();
                    if (ivc.Currency_code != null)
                        av.Currency_code = ivc.Currency_code;
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
                        af.Currency_code.Text.Add(ief.Currency_code);
                    if (ief.Currency_rate != 0)
                        af.Currency_rate = ief.Currency_rate.ToString();

                    ai.Valuation_item.item_external_freight = af;
                }
                if (item.xcuda_Valuation_item.xcuda_Weight_itm != null && item.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm != 0)
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


        private  void SaveGoodsDescription(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Goods_description != null && item.xcuda_Goods_description.Country_of_origin_code != null)
            {
                if (item.xcuda_Goods_description.Commercial_Description != null)
                    ai.Goods_description.Commercial_Description = item.xcuda_Goods_description.Commercial_Description;
                if (item.xcuda_Goods_description.Country_of_origin_code != null)
                    ai.Goods_description.Country_of_origin_code = item.xcuda_Goods_description.Country_of_origin_code;
                if (item.xcuda_Goods_description.Description_of_goods != null)
                    ai.Goods_description.Description_of_goods.Text.Add(item.xcuda_Goods_description.Description_of_goods);
            }
        }

        private  void SaveTarification(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Tarification != null)
            {
                if (item.xcuda_Tarification.Extended_customs_procedure != null)
                    ai.Tarification.Extended_customs_procedure = item.xcuda_Tarification.Extended_customs_procedure;
                if (item.xcuda_Tarification.National_customs_procedure != null)
                    ai.Tarification.National_customs_procedure = item.xcuda_Tarification.National_customs_procedure;
                if (item.xcuda_Tarification.Item_price != 0)
                    ai.Tarification.Item_price = Math.Round(item.xcuda_Tarification.Item_price, 2).ToString();
                SaveHSCode(item, ai);
                SaveSupplementaryUnit(item, ai);
            }
        }

        private  void SaveHSCode(xcuda_Item item, ASYCUDAItem ai)
        {
            if (item.xcuda_Tarification.xcuda_HScode != null)
            {
                if (item.xcuda_Tarification.xcuda_HScode.Commodity_code != null)
                    ai.Tarification.HScode.Commodity_code = item.xcuda_Tarification.xcuda_HScode.Commodity_code; // item.xcuda_Tarification.xcuda_HScode.Commodity_code;
                // ai.Tarification.HScode.Precision_1 = item.xcuda_Tarification.xcuda_HScode.Precision_1;
                if (item.xcuda_Tarification.xcuda_HScode.Precision_4 != null)
                    ai.Tarification.HScode.Precision_4.Text.Add(item.xcuda_Tarification.xcuda_HScode.Precision_4);
            }
        }

        private  void SaveSupplementaryUnit(xcuda_Item item, ASYCUDAItem ai)
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
                        asupp.Suppplementary_unit_quantity = supp.Suppplementary_unit_quantity.GetValueOrDefault().ToString();
                    ai.Tarification.Supplementary_unit.Insert(i, asupp);
                }
            }
        }

        private  void SaveRegistration(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Registration != null)
            {
                if (da.xcuda_Identification.xcuda_Registration.Date != null)
                    a.Identification.Registration.Date = da.xcuda_Identification.xcuda_Registration.Date;
                if (da.xcuda_Identification.xcuda_Registration.Number != null)
                    a.Identification.Registration.Number = da.xcuda_Identification.xcuda_Registration.Number;
            }
        }

        private  void SaveManifestReferenceNumber(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.Manifest_reference_number != null)
            {
                a.Identification.Manifest_reference_number.Text.Add(da.xcuda_Identification.Manifest_reference_number);
            }
        }

        private  void SaveOfficeSegment(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Identification.xcuda_Office_segment != null)
            {
                if (da.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code != null)
                    a.Identification.Office_segment.Customs_clearance_office_code.Text.Add(da.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code);
                if (da.xcuda_Identification.xcuda_Office_segment.Customs_Clearance_office_name != null)
                    a.Identification.Office_segment.Customs_Clearance_office_name.Text.Add(da.xcuda_Identification.xcuda_Office_segment.Customs_Clearance_office_name);

            }
        }

        private  void SaveDeclarant(xcuda_ASYCUDA da, ASYCUDA a)
        {
            if (da.xcuda_Declarant != null)
            {
                if (da.xcuda_Declarant.Declarant_code != null)
                    a.Declarant.Declarant_code = da.xcuda_Declarant.Declarant_code;
                if (da.xcuda_Declarant.Declarant_name != null)
                    a.Declarant.Declarant_name = da.xcuda_Declarant.Declarant_name;
                if (da.xcuda_Declarant.Declarant_representative != null)
                    a.Declarant.Declarant_representative = da.xcuda_Declarant.Declarant_representative;
                if (da.xcuda_Declarant.Number != null)
                    a.Declarant.Reference.Number = da.xcuda_Declarant.Number;

            }
        }
    }
}

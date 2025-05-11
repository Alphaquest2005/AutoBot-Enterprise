using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Asycuda421;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public void IM72Ex9Document(string filename)
    {
        var zeroitems = "";
        // create blank asycuda document
        ASYCUDA olddoc;
        if (ASYCUDA.CanLoadFromFile(filename))
            olddoc = ASYCUDA.LoadFromFile(filename);
        else if (ASYCUDA.CanLoadFromFile(filename))
            olddoc = ASYCUDA.LoadFromFile(filename);
        else
            throw new ApplicationException($"Can not Load file '{filename}'");


        var newdoc = ASYCUDA.LoadFromFile(filename);

        newdoc.Container = null;

        if (olddoc.Identification.Registration.Date == null)
            throw new ApplicationException("Document is not Assesed! Convert Assessed Documents only");


        newdoc.Item.Clear();

        var cp = GetCustomsProcedure("Duty Free", "IM9");

        var exp = Instance.ExportTemplates
            .Single(x =>
                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                x.Customs_Procedure == cp.CustomsProcedure);

        var linenumber = 0;
        foreach (var olditem in olddoc.Item)
        {
            linenumber += 1;


            // create new entry
            var i = olditem.Clone();

            i.Tarification.Extended_customs_procedure.Text.Clear();
            i.Tarification.Extended_customs_procedure.Text.Add(cp.Extended_customs_procedure);
            i.Tarification.National_customs_procedure.Text.Clear();
            i.Tarification.National_customs_procedure.Text.Add(cp.National_customs_procedure);


            i.Previous_doc.Summary_declaration.Text.Clear();
            i.Previous_doc.Summary_declaration.Text.Add(
                $"{olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0]} {DateTime.Parse(olddoc.Identification.Registration.Date).Year} C {olddoc.Identification.Registration.Number} art. {linenumber}");


            // create previous item


            var pitm = new ASYCUDAPrev_decl
            {
                Prev_decl_HS_code = new ASYCUDAPrev_declPrev_decl_HS_code()
                {
                    Text = new ObservableCollection<string>()
                        { i.Tarification.HScode.Commodity_code.Text.FirstOrDefault() }
                },
                Prev_decl_HS_prec = new ASYCUDAPrev_declPrev_decl_HS_prec()
                {
                    Text = new ObservableCollection<string>()
                        { i.Tarification.HScode.Precision_1.Text.FirstOrDefault() }
                },
                Prev_decl_current_item = linenumber.ToString(), // piggy back the previous item count
                Prev_decl_item_number = linenumber.ToString(),
                Prev_decl_weight = olditem.Valuation_item.Weight_itm.Net_weight_itm
                    .ToString(),
                Prev_decl_weight_written_off = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString()
            };

            if (!string.IsNullOrEmpty(olditem.Packages.Number_of_packages))
            {
                pitm.Prev_decl_number_packages_written_off =
                    Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();


                pitm.Prev_decl_number_packages =
                    Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();
            }

            pitm.Prev_decl_supp_quantity = olditem.Tarification.Supplementary_unit[0]
                .Suppplementary_unit_quantity.ToString();
            pitm.Prev_decl_supp_quantity_written_off = olditem.Tarification.Supplementary_unit[0]
                .Suppplementary_unit_quantity.ToString();


            pitm.Prev_decl_country_origin = new ASYCUDAPrev_declPrev_decl_country_origin()
            {
                Text = new ObservableCollection<string>()
                    { olditem.Goods_description.Country_of_origin_code.Text.FirstOrDefault() }
            };

            var oq = "";

            if (string.IsNullOrEmpty(olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity) ||
                olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity == "0")
            {
                oq = "1";
                zeroitems = "ZeroItems";
            }
            else
            {
                oq = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();
            }


            if (!string.IsNullOrEmpty(olditem.Valuation_item.Total_CIF_itm))
            {
                pitm.Prev_decl_ref_value_written_off =
                    (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();
                pitm.Prev_decl_ref_value =
                    (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq))
                    .ToString(); // * System.Convert.ToDecimal(fa.QUANTITY);
            }

            pitm.Prev_decl_reg_serial = new ASYCUDAPrev_declPrev_decl_reg_serial()
                { Text = new ObservableCollection<string>() { "C" } };
            pitm.Prev_decl_reg_number = olddoc.Identification.Registration.Number;
            pitm.Prev_decl_reg_year = DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString();
            pitm.Prev_decl_office_code =
                olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0];

            newdoc.Prev_decl.Add(pitm);

            i.Valuation_item.Item_Invoice.Currency_code.Text.Clear();
            i.Valuation_item.Item_Invoice.Currency_code.Text.Add(exp.Gs_Invoice_Currency_code);
            i.Valuation_item.Item_Invoice.Amount_foreign_currency = olditem.Valuation_item.Total_CIF_itm;
            i.Valuation_item.Item_Invoice.Amount_national_currency = olditem.Valuation_item.Total_CIF_itm;
            i.Valuation_item.Statistical_value = olditem.Valuation_item.Total_CIF_itm;

            newdoc.Item.Add(i);
        }

        newdoc.Identification.Manifest_reference_number = null;
        newdoc.Identification.Type.Type_of_declaration = cp.Document_Type.Type_of_declaration;
        newdoc.Identification.Type.Declaration_gen_procedure_code =
            cp.Document_Type.Declaration_gen_procedure_code;
        newdoc.Declarant.Reference.Number.Text.Add("Ex9For" + newdoc.Identification.Registration.Number);

        newdoc.Valuation.Gs_Invoice.Currency_code.Text.Add(exp.Gs_Invoice_Currency_code);
        newdoc.Valuation.Gs_Invoice.Amount_foreign_currency = Math
            .Round(
                newdoc.Item.Where(i => !string.IsNullOrEmpty(i.Valuation_item.Total_CIF_itm))
                    .Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();
        newdoc.Valuation.Gs_Invoice.Amount_national_currency = Math
            .Round(
                newdoc.Item.Where(i => !string.IsNullOrEmpty(i.Valuation_item.Total_CIF_itm))
                    .Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();

        var oldfile = new FileInfo(filename);
        newdoc.SaveToFile(Path.Combine(oldfile.DirectoryName,
            olddoc.Identification.Registration.Number + "-Ex9" + zeroitems + oldfile.Extension));
    }
}
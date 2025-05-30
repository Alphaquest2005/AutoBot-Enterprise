using Asycuda421; // Assuming LicenceGeneral_segment is here
using System;
using LicenseDS.Business.Entities; // Assuming xLIC_General_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private void ExportGeneralInfo(LicenceGeneral_segment aItem, xLIC_General_segment dItem)
        {
            try
            {
                // Potential NullReferenceExceptions throughout if dItem or aItem properties are null
                aItem.Arrival_date = dItem.Arrival_date;
                aItem.Application_date = dItem.Application_date;

                //if (dItem.Expiry_date == null) aItem.Expiry_date = new LicenceGeneral_segmentExpiry_date() { @null = new object() }; else aItem.Expiry_date.Text.Add(dItem.Expiry_date); // Original code commented out
                aItem.Importation_date = dItem.Importation_date;

                // Ensure Text collections are initialized if null
                if (aItem.Importer_cellphone.Text == null) aItem.Importer_cellphone.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_address.Text == null) aItem.Exporter_address.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_country_code.Text == null) aItem.Exporter_country_code.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Importer_code.Text == null) aItem.Importer_code.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Owner_code.Text == null) aItem.Owner_code.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_email.Text == null) aItem.Exporter_email.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Importer_email.Text == null) aItem.Importer_email.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Importer_name.Text == null) aItem.Importer_name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Importer_contact.Text == null) aItem.Importer_contact.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_name.Text == null) aItem.Exporter_name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_telephone.Text == null) aItem.Exporter_telephone.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Importer_telephone.Text == null) aItem.Importer_telephone.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_country_name.Text == null) aItem.Exporter_country_name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aItem.Exporter_cellphone.Text == null) aItem.Exporter_cellphone.Text = new System.Collections.ObjectModel.ObservableCollection<string>();

                aItem.Importer_cellphone.Text.Add(dItem.Importer_cellphone);
                aItem.Exporter_address.Text.Add(dItem.Exporter_address);
                aItem.Exporter_country_code.Text.Add(dItem.Exporter_country_code);
                aItem.Importer_code.Text.Add(dItem.Importer_code);
                aItem.Owner_code.Text.Add(dItem.Owner_code);
                aItem.Exporter_email.Text.Add(dItem.Exporter_email);
                aItem.Importer_email.Text.Add(dItem.Importer_email);
                aItem.Importer_name.Text.Add(dItem.Importer_name);
                aItem.Importer_contact.Text.Add(dItem.Importer_contact);
                aItem.Exporter_name.Text.Add(dItem.Exporter_name);
                aItem.Exporter_telephone.Text.Add(dItem.Exporter_telephone);
                aItem.Importer_telephone.Text.Add(dItem.Importer_telephone);
                aItem.Exporter_country_name.Text.Add(dItem.Exporter_country_name);
                aItem.Exporter_cellphone.Text.Add(dItem.Exporter_cellphone);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
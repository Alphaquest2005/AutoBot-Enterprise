using Asycuda421; // Assuming LicenceGeneral_segment is here
using System;
using System.Linq;
using LicenseDS.Business.Entities; // Assuming xLIC_General_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private void SaveGeneralInfo(LicenceGeneral_segment agen, xLIC_General_segment dgen)
        {
            try
            {
                // Potential NullReferenceExceptions throughout if agen or its properties are null
                dgen.Arrival_date = agen.Arrival_date;
                dgen.Application_date = agen.Application_date;
                dgen.Expiry_date = agen.Expiry_date.Text.FirstOrDefault();
                dgen.Importation_date = agen.Importation_date;
                dgen.Importer_cellphone = agen.Importer_cellphone.Text.FirstOrDefault();
                dgen.Exporter_address = agen.Exporter_address.Text.FirstOrDefault();
                dgen.Exporter_country_code = agen.Exporter_country_code.Text.FirstOrDefault();
                dgen.Importer_code = agen.Importer_code.Text.FirstOrDefault();
                dgen.Owner_code = agen.Owner_code.Text.FirstOrDefault();
                dgen.Exporter_email = agen.Exporter_email.Text.FirstOrDefault();
                dgen.Importer_email = agen.Importer_email.Text.FirstOrDefault();
                dgen.Importer_name = agen.Importer_name.Text.FirstOrDefault();
                dgen.Importer_contact = agen.Importer_contact.Text.FirstOrDefault();
                dgen.Exporter_name = agen.Exporter_name.Text.FirstOrDefault();
                dgen.Exporter_telephone = agen.Exporter_telephone.Text.FirstOrDefault();
                dgen.Importer_telephone = agen.Importer_telephone.Text.FirstOrDefault();
                dgen.Exporter_country_name = agen.Exporter_country_name.Text.FirstOrDefault();
                dgen.Exporter_cellphone = agen.Exporter_cellphone.Text.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
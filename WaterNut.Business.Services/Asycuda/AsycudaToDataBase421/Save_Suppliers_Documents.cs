using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Suppliers_documents is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Suppliers_documents is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Suppliers_Documents()
        {
            if (a.Supplier_documents.Count > 0 && a.Supplier_documents[0] == null) return; // Assuming 'a' is accessible field
            for (int i = 0; i < a.Supplier_documents.Count; i++)
            {
                var asd = a.Supplier_documents.ElementAt(i);

                var s = da.Document.xcuda_Suppliers_documents.ElementAtOrDefault(i); // Assuming 'da' is accessible field, Potential NullReferenceException
                if (s == null)
                {
                    s = new xcuda_Suppliers_documents(true)
                    {
                        ASYCUDA_Id = da.Document.ASYCUDA_Id, // Potential NullReferenceException
                        TrackingState = TrackingState.Added
                    };
                    da.Document.xcuda_Suppliers_documents.Add(s); // Potential NullReferenceException
                }

                // var asd = a.Suppliers_documents[0];
                if (asd.Invoice_supplier_city.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_city = asd.Invoice_supplier_city.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_country.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_country = asd.Invoice_supplier_country.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_fax.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_fax = asd.Invoice_supplier_fax.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_name.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_name = asd.Invoice_supplier_name.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_street.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_street = asd.Invoice_supplier_street.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_telephone.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_telephone = asd.Invoice_supplier_telephone.Text[0]; // Potential NullReferenceException

                if (asd.Invoice_supplier_zip_code.Text.Count > 0) // Potential NullReferenceException
                    s.Suppliers_document_zip_code = asd.Invoice_supplier_zip_code.Text[0]; // Potential NullReferenceException

                //await DBaseDataModel.Instance.Savexcuda_Suppliers_documents(s).ConfigureAwait(false); // Assuming Savexcuda_Suppliers_documents exists
            }
        }
    }
}
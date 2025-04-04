using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Attached_documents are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Attached_documents is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Attached_documents(xcuda_Item di, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Attached_documents.Count; i++) // Potential NullReferenceException
            {
                if (ai.Attached_documents[i].Attached_document_code.Text.Count == 0) break; // Potential NullReferenceException

                var ad = di.xcuda_Attached_documents.ElementAtOrDefault(i); // Potential NullReferenceException
                if (ad == null)
                {
                    ad = new xcuda_Attached_documents(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                    di.xcuda_Attached_documents.Add(ad); // Potential NullReferenceException
                }

                ad.Attached_document_date = ai.Attached_documents[i].Attached_document_date; // Potential NullReferenceException

                if (ai.Attached_documents[i].Attached_document_code.Text.Count != 0) // Potential NullReferenceException
                    ad.Attached_document_code = ai.Attached_documents[i].Attached_document_code.Text[0]; // Potential NullReferenceException

                if (ai.Attached_documents[i].Attached_document_from_rule.Text.Count != 0) // Potential NullReferenceException
                    ad.Attached_document_from_rule = Convert.ToInt32(ai.Attached_documents[i].Attached_document_from_rule.Text[0]); // Potential NullReferenceException

                if (ai.Attached_documents[i].Attached_document_name.Text.Count != 0) // Potential NullReferenceException
                    ad.Attached_document_name = ai.Attached_documents[i].Attached_document_name.Text[0]; // Potential NullReferenceException

                if (ai.Attached_documents[i].Attached_document_reference.Text.Count != 0) // Potential NullReferenceException
                    ad.Attached_document_reference = ai.Attached_documents[i].Attached_document_reference.Text[0]; // Potential NullReferenceException

                //await DIBaseDataModel.Instance.Savexcuda_Attached_documents(ad).ConfigureAwait(false); // Assuming Savexcuda_Attached_documents exists
            }
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}
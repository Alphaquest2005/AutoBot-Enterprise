using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_PreviousInvoiceInfo(xcuda_Item di, ASYCUDAItem ai)
        {
            di.Free_text_1 = ai.Free_text_1.Text.FirstOrDefault(); // Potential NullReferenceException
            di.Free_text_2 = ai.Free_text_2.Text.FirstOrDefault(); // Potential NullReferenceException
            if (!string.IsNullOrEmpty(di.Free_text_1))
            {
                var lst = di.Free_text_1.Split('|');
                if (lst.Length == 2)
                {
                    di.PreviousInvoiceNumber = lst[0].Trim();
                    di.PreviousInvoiceLineNumber = lst[1].Trim();
                }

                if (lst.Length == 3)
                {
                    di.PreviousInvoiceNumber = lst[0].Trim();
                    di.PreviousInvoiceLineNumber = lst[1].Trim();
                    di.PreviousInvoiceItemNumber = lst[2].Trim();
                }
            }

            if (!string.IsNullOrEmpty(di.Free_text_2))
            {
                var lst = di.Free_text_2.Split('|');
                if (lst.Length == 2) di.PreviousInvoiceItemNumber = lst[0].Trim();
                //// left the old code incase i have some code with that is been used
                /// also Comment is left out for now.

                // think about saving comment
                // if (lst.Length == 3) di.c = di.Free_text_2.Trim();
            }
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext is from here

namespace AutoBot
{
    public partial class PDFUtils
    {
        public static void ImportPDF()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes
                    .FirstOrDefault(x => x.Id == 17); // Assuming Id 17 is relevant
                var files = new FileInfo[]
                    {new FileInfo(@"D:\OneDrive\Clients\Budget Marine\Emails\30-16170\7006359.pdf")}; // Keeping original path for now
                // Note: The call to the other ImportPDF overload needs to be updated
                // after that method is moved to its own partial class.
                // For now, commenting it out or leaving as is, assuming it will be handled.
                // ImportPDF(files, fileType); // This call will need adjustment
            }
        }
    }
}
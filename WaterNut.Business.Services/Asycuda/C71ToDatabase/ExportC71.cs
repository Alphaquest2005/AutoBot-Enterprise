using Asycuda421; // Assuming Value_declaration_form is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using System;
using System.IO;
using System.Threading.Tasks;
using ValuationDS.Business.Entities; // Assuming xC71_Value_declaration_form is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        public async Task<bool> ExportC71(int docSetId,xC71_Value_declaration_form c71, string fileName)
        {
            try
            {
                var docSet = await BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false); // Assuming GetAsycudaDocumentSet exists
                // This calls DatabaseToC71, which needs to be in its own partial class
                var adoc = DatabaseToC71(c71);
                adoc.SaveToFile(fileName); // Assuming SaveToFile exists on Value_declaration_form
                var fileInfo = new FileInfo(fileName);
                // This calls AttachC71ToDocset, which needs to be in its own partial class
                AttachC71ToDocset(docSet, fileInfo);
                var instructionsFile = Path.Combine(fileInfo.DirectoryName, "C71-Instructions.txt");
                var results = new FileInfo(Path.Combine(fileInfo.DirectoryName, "C71-Results.txt"));
                if (File.Exists(results.FullName)) File.Delete(results.FullName);
                if (File.Exists(instructionsFile)) File.Delete(instructionsFile);

                File.WriteAllText(instructionsFile,
                    $"File\t{fileInfo.FullName}\t{c71.xC71_Identification_segment.xC71_Seller_segment.Address.Replace("\r","").Replace("\n", "")}\t{c71.xC71_Identification_segment.xC71_Seller_segment.CountryCode}\r\n"); // Potential NullReferenceException
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}
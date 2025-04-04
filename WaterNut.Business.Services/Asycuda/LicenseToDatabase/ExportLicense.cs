using Asycuda421; // Assuming Licence is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Common.Utils; // Assuming StringExtensions.Right is here
using LicenseDS.Business.Entities; // Assuming xLIC_License is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        public async Task<bool> ExportLicense(int docSetId, xLIC_License lic, string fileName, List<Tuple<string, string>> invoices)
        {
            try
            {
                var docSet = await BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false); // Assuming GetAsycudaDocumentSet exists
                // This calls DatabaseToLicence, which needs to be in its own partial class
                var adoc = DatabaseToLicence(lic);

                var fileInfo = new FileInfo(fileName);
                if (!Directory.Exists(fileInfo.DirectoryName)) return false;
                adoc.SaveToFile(fileName); // Assuming SaveToFile exists on Licence

                // This calls AttachLicenseToDocSet, which needs to be in its own partial class
                AttachLicenseToDocSet(docSet, fileInfo, "LIC");
                //var emailres = new FileInfo(Path.Combine(fileInfo.DirectoryName, "LICResults.txt"));
                var results = new FileInfo(Path.Combine(fileInfo.DirectoryName, "LIC-Results.txt"));
               // if(File.Exists(results.FullName)) File.Delete(results.FullName); // Original code commented out
                var instructions = Path.Combine(fileInfo.DirectoryName, "LIC-Instructions.txt");
                File.AppendAllText(instructions, $"File\t{fileInfo.FullName}\r\n");
                foreach (var itm in invoices)
                {
                    File.AppendAllText(instructions, $"Attach\t{itm.Item2}\t{itm.Item1.Right(10)}\t{"IV05"}\r\n"); // Assuming StringExtensions.Right exists
                }
                File.AppendAllText(instructions, $"File\t{fileInfo.FullName}\r\n");
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
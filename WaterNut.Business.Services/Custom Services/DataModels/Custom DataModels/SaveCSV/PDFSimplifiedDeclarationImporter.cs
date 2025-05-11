using System.Drawing.Imaging;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Importers.EntryData;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class PDFSimplifiedDeclarationImporter
    {
        private SimplifiedDeclarationImporter _simplifiedDeclarationImporter = new SimplifiedDeclarationImporter();


        public Task<bool> Process(DataFile dataFile)
        {
            if (dataFile.FileType.FileImporterInfos.EntryType != FileTypeManager.EntryTypes.SimplifiedDeclaration
                || dataFile.FileType.FileImporterInfos.Format != FileTypeManager.FileFormats.PDF) return Task.FromResult(false);

            
           return Task.FromResult(this._simplifiedDeclarationImporter.ProcessSimplifiedDeclaration(dataFile));

         

        }
    }
}
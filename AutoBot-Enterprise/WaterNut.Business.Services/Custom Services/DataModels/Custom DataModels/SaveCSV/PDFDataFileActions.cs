using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class PDFDataFileActions
    {
        public static Dictionary<string, Func<DataFile, Task<bool>>> Actions = new Dictionary<string, Func<DataFile, Task<bool>>>()
        {
            {FileTypeManager.EntryTypes.Rider, (dataFile) => new RiderPdfImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Manifest, (dataFile) => new ManifestImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.BL, (dataFile) => new BillOfLadenImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Freight, (dataFile) => new FreightImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ShipmentInvoice, (dataFile) =>  new PDFShipmentInvoiceImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.SimplifiedDeclaration,  (dataFile) =>  new PDFSimplifiedDeclarationImporter().Process(dataFile)},

        };
    }
}
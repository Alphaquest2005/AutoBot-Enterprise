using System;
using System.Collections.Generic;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class PDFDataFileActions
    {
        public static Dictionary<string, Action<DataFile>> Actions = new Dictionary<string, Action<DataFile>>()
        {
            
            {FileTypeManager.EntryTypes.Manifest, (dataFile) => new ManifestImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.BL, (dataFile) => new BillOfLadenImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Freight, (dataFile) => new FreightImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ShipmentInvoice, async (dataFile) => await new PDFShipmentInvoiceImporter().Process(dataFile).ConfigureAwait(false)},
            
            
        };
    }

    public class XLSXDataFileActions
    {
        public static Dictionary<string, Action<DataFile>> Actions = new Dictionary<string, Action<DataFile>>()
        {

            {FileTypeManager.EntryTypes.Dis, (dataFile) => new ManifestImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Po, (dataFile) => new BillOfLadenImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Sales, (dataFile) => new FreightImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.Rider, async (dataFile) => await new PDFShipmentInvoiceImporter().Process(dataFile).ConfigureAwait(false)},


        };
    }
}
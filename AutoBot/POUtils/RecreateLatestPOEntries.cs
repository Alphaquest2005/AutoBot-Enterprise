using System;
using System.Collections.Generic;
using WaterNut.Business.Entities; // Assuming DocumentCT is here
using WaterNut.DataSpace; // Assuming EntryDocSetUtils is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static List<DocumentCT> RecreateLatestPOEntries()
        {
            Console.WriteLine("Create Latest PO Entries");

            var docSet = WaterNut.DataSpace.EntryDocSetUtils.GetLatestDocSet(); // Assuming GetLatestDocSet exists
            var res = EntryDocSetUtils.GetDocSetEntryData(docSet.AsycudaDocumentSetId); // Assuming GetDocSetEntryData exists

            // This calls CreatePOEntries, which needs to be in its own partial class
            return CreatePOEntries(docSet.AsycudaDocumentSetId, res);
        }
    }
}
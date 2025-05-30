using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions; // Assuming StringExtensions is here
using DocumentDS.Business.Entities; // Assuming DocumentDSContext, AsycudaDocumentSet are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static List<Tuple<AsycudaDocumentSet, string>> CurrentPOInfo(int docSetId)
        {
            try
            {
                using (var ctx = new DocumentDSContext())
                {
                    var docSet = ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docSetId);

                    var dirPath = StringExtensions.UpdateToCurrentUser(BaseDataModel.GetDocSetDirectoryName(docSet.Declarant_Reference_Number));
                    return new List<Tuple<AsycudaDocumentSet, string>>()
                        { new Tuple<AsycudaDocumentSet, string>(docSet, dirPath) };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
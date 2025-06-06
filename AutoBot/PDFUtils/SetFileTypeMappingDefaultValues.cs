using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using MoreLinq; // For ForEach

namespace AutoBot
{
    public partial class PDFUtils
    {
        private static void SetFileTypeMappingDefaultValues(FileTypes docFileType, IGrouping<object, IDictionary<string, object>> doc)
        {
            foreach (var mapping in docFileType.FileTypeMappings.Where(x => x.FileTypeMappingValues.Any()).ToList())
            {
                doc.ToList().Cast<IDictionary<string, object>>()
                    .Select(x => ((IDictionary<string, object>)x))
                    .Where(x => !x.ContainsKey(mapping.DestinationName))
                    .ForEach(x => x[mapping.DestinationName] = mapping.FileTypeMappingValues.First().Value);
            }
        }
    }
}
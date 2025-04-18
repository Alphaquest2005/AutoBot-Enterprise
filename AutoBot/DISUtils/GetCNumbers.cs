using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms is here
using MoreLinq; // For DistinctBy

namespace AutoBot
{
    public partial class DISUtils
    {
        private static List<TODO_SubmitDiscrepanciesToCustoms> GetCNumbers(IGrouping<string, TODO_SubmitDiscrepanciesToCustoms> data)
        {
            var emailIds = MoreEnumerable.DistinctBy(data, x => x.CNumber).ToList();
            if (!emailIds.Any())
            {
                throw new NotImplementedException();
            }

            return emailIds;
        }
    }
}
﻿// <autogenerated>
//   This file was generated by T4 code generator AllDataSpaceViewModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Collections.Generic;
using System.Threading.Tasks;
//using SimpleMvvmToolkit;
using PreviousDocumentDS.Business.Entities;
using PreviousDocumentDS.Business.Services;




namespace WaterNut.DataSpace.PreviousDocumentDS.DataModels
{
	public partial class EntryDataDetailsDataModel_AutoGen 
	{
        private static readonly EntryDataDetailsDataModel_AutoGen instance;
        static EntryDataDetailsDataModel_AutoGen()
        {
            instance = new EntryDataDetailsDataModel_AutoGen();
        }

        public static  EntryDataDetailsDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<EntryDataDetails>> SearchEntryDataDetails(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new EntryDataDetailsService())
            {
                return await ctx.GetEntryDataDetailsByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

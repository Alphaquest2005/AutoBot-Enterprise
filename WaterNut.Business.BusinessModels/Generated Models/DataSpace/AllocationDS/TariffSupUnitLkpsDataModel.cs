﻿// <autogenerated>
//   This file was generated by T4 code generator AllDataSpaceViewModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using SimpleMvvmToolkit;
using TrackableEntities;
using System;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;




namespace WaterNut.DataSpace.AllocationDS.DataModels
{
	public partial class TariffSupUnitLkpsDataModel_AutoGen 
	{
        private static readonly TariffSupUnitLkpsDataModel_AutoGen instance;
        static TariffSupUnitLkpsDataModel_AutoGen()
        {
            instance = new TariffSupUnitLkpsDataModel_AutoGen();
        }

        public static  TariffSupUnitLkpsDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<TariffSupUnitLkps>> SearchTariffSupUnitLkps(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new TariffSupUnitLkpsService())
            {
                return await ctx.GetTariffSupUnitLkpsByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

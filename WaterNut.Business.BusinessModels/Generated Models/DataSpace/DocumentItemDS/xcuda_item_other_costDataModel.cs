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
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;




namespace WaterNut.DataSpace.DocumentItemDS.DataModels
{
	public partial class xcuda_item_other_costDataModel_AutoGen 
	{
        private static readonly xcuda_item_other_costDataModel_AutoGen instance;
        static xcuda_item_other_costDataModel_AutoGen()
        {
            instance = new xcuda_item_other_costDataModel_AutoGen();
        }

        public static  xcuda_item_other_costDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_item_other_cost>> Searchxcuda_item_other_cost(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_item_other_costService())
            {
                return await ctx.Getxcuda_item_other_costByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

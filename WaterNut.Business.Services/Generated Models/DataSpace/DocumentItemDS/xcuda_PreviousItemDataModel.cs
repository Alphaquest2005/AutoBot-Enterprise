﻿// <autogenerated>
//   This file was generated by T4 code generator AllDataSpaceViewModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Collections.Generic;
using System.Threading.Tasks;
//using SimpleMvvmToolkit;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;




namespace WaterNut.DataSpace.DocumentItemDS.DataModels
{
	public partial class xcuda_PreviousItemDataModel_AutoGen 
	{
        private static readonly xcuda_PreviousItemDataModel_AutoGen instance;
        static xcuda_PreviousItemDataModel_AutoGen()
        {
            instance = new xcuda_PreviousItemDataModel_AutoGen();
        }

        public static  xcuda_PreviousItemDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_PreviousItem>> Searchxcuda_PreviousItem(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_PreviousItemService())
            {
                return await ctx.Getxcuda_PreviousItemByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

﻿// <autogenerated>
//   This file was generated by T4 code generator AllDataSpaceViewModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Collections.Generic;
using System.Threading.Tasks;
//using SimpleMvvmToolkit;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;




namespace WaterNut.DataSpace.DocumentDS.DataModels
{
	public partial class xcuda_FormsDataModel_AutoGen 
	{
        private static readonly xcuda_FormsDataModel_AutoGen instance;
        static xcuda_FormsDataModel_AutoGen()
        {
            instance = new xcuda_FormsDataModel_AutoGen();
        }

        public static  xcuda_FormsDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_Forms>> Searchxcuda_Forms(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_FormsService())
            {
                return await ctx.Getxcuda_FormsByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

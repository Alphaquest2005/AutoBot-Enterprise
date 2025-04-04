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
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;




namespace WaterNut.DataSpace.DocumentDS.DataModels
{
	public partial class xcuda_DestinationDataModel_AutoGen 
	{
        private static readonly xcuda_DestinationDataModel_AutoGen instance;
        static xcuda_DestinationDataModel_AutoGen()
        {
            instance = new xcuda_DestinationDataModel_AutoGen();
        }

        public static  xcuda_DestinationDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_Destination>> Searchxcuda_Destination(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_DestinationService())
            {
                return await ctx.Getxcuda_DestinationByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

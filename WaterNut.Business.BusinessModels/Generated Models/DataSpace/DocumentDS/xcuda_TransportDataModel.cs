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
	public partial class xcuda_TransportDataModel_AutoGen 
	{
        private static readonly xcuda_TransportDataModel_AutoGen instance;
        static xcuda_TransportDataModel_AutoGen()
        {
            instance = new xcuda_TransportDataModel_AutoGen();
        }

        public static  xcuda_TransportDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_Transport>> Searchxcuda_Transport(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_TransportService())
            {
                return await ctx.Getxcuda_TransportByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

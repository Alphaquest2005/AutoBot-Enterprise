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
	public partial class xcuda_Departure_arrival_informationDataModel_AutoGen 
	{
        private static readonly xcuda_Departure_arrival_informationDataModel_AutoGen instance;
        static xcuda_Departure_arrival_informationDataModel_AutoGen()
        {
            instance = new xcuda_Departure_arrival_informationDataModel_AutoGen();
        }

        public static  xcuda_Departure_arrival_informationDataModel_AutoGen Instance
        {
            get { return instance; }
        }

       //Search Entities 
        public async Task<IEnumerable<xcuda_Departure_arrival_information>> Searchxcuda_Departure_arrival_information(List<string> lst, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_Departure_arrival_informationService())
            {
                return await ctx.Getxcuda_Departure_arrival_informationByExpressionLst(lst, includeLst).ConfigureAwait(false);
            }
        }

    }
}
		

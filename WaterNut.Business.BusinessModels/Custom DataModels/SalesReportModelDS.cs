using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Core.Common.Converters;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using WaterNut.Business.BusinessModels.Custom_DataModels;


namespace WaterNut.DataSpace
{
	public class SalesReportModel
    {
		
        private static readonly SalesReportModel instance;
        static SalesReportModel()
        {
            instance = new SalesReportModel();
        }

        public static SalesReportModel Instance
        {
            get { return instance; }
        }



        //public static void Send2Excel(IEnumerable<SaleReportLine> slst)
        //{
        //    if (slst == null) return;
        //    var s = new ExportToExcel<SaleReportLine, List<SaleReportLine>>();
        //    s.dataToPrint = slst.ToList();
        //    s.GenerateReport();
        //}



	    public async Task<IEnumerable<xcuda_ASYCUDA>> GetSalesDocuments(int docSetId)
	    {
	        using (var ctx = new xcuda_ASYCUDAService())
	        {
	            var lst =

	                (await
	                    ctx.Getxcuda_ASYCUDAByExpressionNav(
	                        string.Format("xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == {0}", docSetId)
	                        ,
	                        new Dictionary<string, string>()
	                        {
	                            {"xcuda_ASYCUDA_ExtendedProperties.DocumentType", "(Type_of_declaration == \"EX\") || (Type_of_declaration == \"IM\" && Declaration_gen_procedure_code == \"4\") " }
	                        },
	                        new List<string>()
	                        {
	                            "xcuda_ASYCUDA_ExtendedProperties",
	                            "xcuda_Declarant"
	                        }
	                        ).ConfigureAwait(false));

	            return lst;
	        }
	    }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using AllocationDS.Business.Services;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsycudaDocumentService = AllocationDS.Business.Services.AsycudaDocumentService;

using WaterNut.Data.DbSetExtensions;

namespace UnitTestProject1
{
    [TestClass]
    public class FilterByExpressionTests
    {
        [TestMethod]
        public void GetAsycudaDocumentSetExsByExpression_All_DocumentTypeEqualsIM7()
        {
            var s = new AsycudaDocumentSetExService();
            var exp = "All";
            var navExp = new Dictionary<string, string> {{"AsycudaDocuments", "DocumentType == \"IM7\""}};
            var lst = s.GetAsycudaDocumentSetExsByExpressionNav(exp, navExp);
           Assert.AreEqual(13,lst.Result.Count());
           
        }

        [TestMethod]
        public void multiple_where()
        {
            var s = new xcuda_ItemService();
            var exp = new List<string>()
            {
                $"EX.Precision_4.ToUpper() == \"{"test"}\""
            };

            var lst = s.Getxcuda_ItemByExpressionLst(exp, new List<string>() {"EX"});
            Assert.AreEqual(1, lst.Result.Count());

        }

        [TestMethod]
        public void LoadRange_All_DocumentTypeEqualsIM7()
        {
            var s = new AsycudaDocumentSetExService();
            var exp = "All";
            var navExp = new Dictionary<string, string> { { "AsycudaDocuments", "DocumentType == \"IM7\"" } };
            var lst = s.LoadRangeNav(0,50,exp, navExp);
            Assert.AreEqual(13, lst.Result.Count());

        }

        [TestMethod]
        public void LoadRange_All_DocumentTypeEqualsIM4()
        {
            var s = new AsycudaDocumentSetExService();
            var exp = "All";
            var navExp = new Dictionary<string, string> { { "AsycudaDocuments", "DocumentType == \"IM4\"" } };
            var lst = s.LoadRangeNav(0, 50, exp, navExp);
            Assert.AreEqual(0, lst.Result.Count());

        }

        //[TestMethod]
        //public void selectnavproperty()
        //{
        //    var s = new AsycudaDocumentSetExService();
        //    var r = s.selectNav();
        //    Assert.AreEqual(2276, r.ASYCUDA_Id);
        //}

        //[TestMethod]
        //public void wherenavproperty()
        //{
           
        //    var s = new AsycudaDocumentSetExService();
        //    var r = s.whereNav();
        //    Assert.AreEqual(1087, r.AsycudaDocumentSetId);
        //}

        //[TestMethod]
        //public void anynavproperty()
        //{
        //    var s = new AsycudaDocumentSetExService();
        //    //var t = new AsycudaDocumentService();
        //    //var r1 = 
        //    //        t.GetAsycudaDocument().Result.Any(y => y.ASYCUDA_Id == 2270);
        //    var r = s.AnyNav();
        //    Assert.AreEqual(true, r);
        //}

        //[TestMethod]
        //public void extenstionprop()
        //{
        //    using (CoreEntitiesContext ctx = new CoreEntitiesContext())
        //    {

        //        var s = ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.AsycudaDocumentSetId == 1087);
                
        //        //var s = ctx.AsycudaDocumentSetExs.Create();
        //            //ctx.AsycudaDocumentSetExs.FirstOrDefault(x => x.AsycudaDocumentSetId == 1087);

        //        //var t = new AsycudaDocumentService();
        //        //var r1 = 
        //        //        t.GetAsycudaDocument().Result.Any(y => y.ASYCUDA_Id == 2270);

        //       // var r = s.IQAsycudaDocuments.Where(z => z.ASYCUDA_Id == 2270).ToList();
        //        //Assert.AreEqual(2270, r.FirstOrDefault() == null ? 0 : r.FirstOrDefault().ASYCUDA_Id);
        //    }
        //}
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WaterNut.DataLayer
//{
//    public partial class CounterPointSales
//    {
//        WaterNutDBEntities db = BaseViewModel.db;//new WaterNutDBEntities();

//        public bool Downloaded
//        {
//            get
//            {
//                return db.EntryData.OfType<Sales>().Where(p => p.EntryDataId == this.INVNO).ToList().Count > 0; ;//&& p.AsycudaDocumentSetId == BaseViewModel.Instance.CurrentAsycudaDocumentSet.AsycudaDocumentSetId).ToList().Count > 0;
//            }

//        }
//    }
//}

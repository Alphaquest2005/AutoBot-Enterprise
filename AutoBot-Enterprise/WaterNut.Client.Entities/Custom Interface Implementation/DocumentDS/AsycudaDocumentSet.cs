//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DomainInterfaces;
//using WaterNut.Interfaces;

//namespace DocumentDS.Client.Entities
//{
//    public partial class AsycudaDocumentSet : IDocument
//    {
//        #region IDocument Members

//       string IDocument.Description
//       {
//           get { return Description; }
//           set { Description = value; }
//       }

//        public string Reference_Number
//        {
//            get
//            {
//                return Declarant_Reference_Number;
//            }
//            set
//            {
//                Declarant_Reference_Number = value;
//            }
//        }

//        #endregion

//        #region IDocumentType Members

//        int IDocumentType.Customs_ProcedureId
//        {
//            get { return (int) Customs_ProcedureId; }
//            set
//            {
//                Customs_ProcedureId = value;
//            }
//        }

//        int IDocumentType.Document_TypeId
//        {
//            get
//            {
//                return Convert.ToInt32(Document_TypeId);
//            }
//            set
//            {
//                Document_TypeId = value;
//            }
//        }

//        #endregion

//        #region ICurrency Members

//        public double? ExchangeRate
//        {
//            get { return Exchange_Rate; }
//            set { Exchange_Rate = value; }
//        }

//        public string CurrencyCode
//        {
//            get { return Currency_Code; }
//            set { Currency_Code = value; }
//        }

//        #endregion
//    }
//}

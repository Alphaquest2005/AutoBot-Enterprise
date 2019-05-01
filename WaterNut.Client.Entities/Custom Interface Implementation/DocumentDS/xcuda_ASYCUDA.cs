
//using System;
//using System.Linq;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Runtime.Serialization;
//using DomainInterfaces;
//using TrackableEntities;
//using WaterNut.Interfaces;
//
//using DocumentDS.Client.DTO;
//using TrackableEntities.Client;
//using Core.Common.Validation;

//namespace DocumentDS.Client.Entities
//{
//       public partial class xcuda_ASYCUDA: IDocument
//    {

//        #region IDocument Members

//        public string Description
//        {
//            get { return _xcuda_ASYCUDA_ExtendedProperties.Description; }
//            set
//            {
//                xcuda_ASYCUDA_ExtendedProperties.Description = value;
//            }
//        }

//        public string Reference_Number
//        {
//            get
//            {
//                return this.xcuda_Declarant.Number;
//            }
//            set
//            {
//                this.xcuda_Declarant.Number = value;
//            }
//        }

//        #endregion

//        #region IDocumentType Members

//        public int Customs_ProcedureId
//        {
//            get { return (int) this._xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId; }
//            set
//            {
//                this.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = value;
//            }
//        }

//        public int Document_TypeId
//        {
//            get
//            {
//                return (int) this.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId;
//            }
//            set
//            {
//                this.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = value;
//            }
//        }

//        #endregion

//        #region ICurrency Members

//        public double? ExchangeRate
//        {
//            get
//            {
//               return this.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate;
//            }
//            set
//            {
//                this.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = (float) value;
//            }
//        }

//        public string CurrencyCode
//        {
//            get
//            {
//               return this.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
//            }
//            set
//            {
//                this.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = value;
//            }
//        }

//        #endregion
//    }
//}



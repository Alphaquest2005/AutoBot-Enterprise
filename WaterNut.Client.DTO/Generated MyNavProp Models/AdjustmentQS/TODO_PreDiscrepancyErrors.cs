﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using CoreEntities;
using CoreEntities.Client.DTO;
//using WaterNut.Client.DTO;
using System.Linq;

namespace AdjustmentQS.Client.DTO
{
    public partial class TODO_PreDiscrepancyErrors
    {
       #region MyNavProp Entities

        AsycudaDocumentSet _asycudaDocumentSet = null;

        public AsycudaDocumentSet AsycudaDocumentSet
        {
            get
            {
                return _asycudaDocumentSet;
            }
            set
            {
                if (value != null)
                {
                    _asycudaDocumentSet = value;

                    AsycudaDocumentSetId = _asycudaDocumentSet.AsycudaDocumentSetId;

                    NotifyPropertyChanged("AsycudaDocumentSet");
                }
            }

        }
        

        
 
 #endregion
    }
   
}
		
﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Linq;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Services;
//using WaterNut.Client.Services;
using EntryDataQS.Client.Services;

using System;

namespace EntryDataQS.Client.Entities
{
    public partial class AsycudaDocumentItemEntryDataDetail
    {
       
       #region IIdentifiable Entities
        public override string EntityId
        {
            get
            {
                return this.EntryDataDetailsId.ToString();//this.EntryDataDetailsId == null?"0":			
            }
            set
            {
                this.EntryDataDetailsId = Convert.ToInt32(value);
            }
        }



         #endregion
    }
   
}
		
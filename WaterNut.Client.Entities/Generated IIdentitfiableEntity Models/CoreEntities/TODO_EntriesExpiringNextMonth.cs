﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Linq;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Services;
//using WaterNut.Client.Services;
using CoreEntities.Client.Services;

using System;

namespace CoreEntities.Client.Entities
{
    public partial class TODO_EntriesExpiringNextMonth
    {
       
       #region IIdentifiable Entities
        public override string EntityId
        {
            get
            {
                return this.Type.ToString();//this.Type == null?"0":			
            }
            set
            {
                this.Type = Convert.ToString(value);
            }
        }



         #endregion
    }
   
}
		
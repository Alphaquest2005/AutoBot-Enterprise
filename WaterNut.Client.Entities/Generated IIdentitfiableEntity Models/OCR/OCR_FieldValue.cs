﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Linq;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Services;
//using WaterNut.Client.Services;
using OCR.Client.Services;

using System;

namespace OCR.Client.Entities
{
    public partial class OCR_FieldValue
    {
       
       #region IIdentifiable Entities
        public override string EntityId
        {
            get
            {
                return this.Id.ToString();//this.Id == null?"0":			
            }
            set
            {
                this.Id = Convert.ToInt32(value);
            }
        }
        public string FieldEntityName
        {
            get
            {
                return this.Field == null ? "" : this.Field.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (FieldsClient ctx = new FieldsClient())
                    {
                        var dto = ctx.GetFields().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Field = (Fields)new Fields().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.Field.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddField");
                        }
                        else
                        {
                            var obj = new Fields(dto);
                           if (this.Field == null || this.Field.EntityId != obj.EntityId) this.Field = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
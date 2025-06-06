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
    public partial class FileTypeReplaceRegex
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
        public string FileTypesEntityName
        {
            get
            {
                return this.FileTypes == null ? "" : this.FileTypes.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (FileTypesClient ctx = new FileTypesClient())
                    {
                        var dto = ctx.GetFileTypes().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.FileTypes = (FileTypes)new FileTypes().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.FileTypes.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddFileTypes");
                        }
                        else
                        {
                            var obj = new FileTypes(dto);
                           if (this.FileTypes == null || this.FileTypes.EntityId != obj.EntityId) this.FileTypes = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
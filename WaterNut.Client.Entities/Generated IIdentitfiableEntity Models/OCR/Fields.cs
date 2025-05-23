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
    public partial class Fields
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
        public string ParentFieldEntityName
        {
            get
            {
                return this.ParentField == null ? "" : this.ParentField.EntityName;
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
                            this.ParentField = (Fields)new Fields().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.ParentField.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddParentField");
                        }
                        else
                        {
                            var obj = new Fields(dto);
                           if (this.ParentField == null || this.ParentField.EntityId != obj.EntityId) this.ParentField = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string LinesEntityName
        {
            get
            {
                return this.Lines == null ? "" : this.Lines.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (LinesClient ctx = new LinesClient())
                    {
                        var dto = ctx.GetLines().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Lines = (Lines)new Lines().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.Lines.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddLines");
                        }
                        else
                        {
                            var obj = new Lines(dto);
                           if (this.Lines == null || this.Lines.EntityId != obj.EntityId) this.Lines = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string FieldValueEntityName
        {
            get
            {
                return this.FieldValue == null ? "" : this.FieldValue.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (OCR_FieldValueClient ctx = new OCR_FieldValueClient())
                    {
                        var dto = ctx.GetOCR_FieldValue().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.FieldValue = (OCR_FieldValue)new OCR_FieldValue().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.FieldValue.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddFieldValue");
                        }
                        else
                        {
                            var obj = new OCR_FieldValue(dto);
                           if (this.FieldValue == null || this.FieldValue.EntityId != obj.EntityId) this.FieldValue = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
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
    public partial class Parts
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
        public string InvoicesEntityName
        {
            get
            {
                return this.Invoices == null ? "" : this.Invoices.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (InvoicesClient ctx = new InvoicesClient())
                    {
                        var dto = ctx.GetInvoices().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Invoices = (Invoices)new Invoices().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.Invoices.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddInvoices");
                        }
                        else
                        {
                            var obj = new Invoices(dto);
                           if (this.Invoices == null || this.Invoices.EntityId != obj.EntityId) this.Invoices = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string PartTypesEntityName
        {
            get
            {
                return this.PartTypes == null ? "" : this.PartTypes.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (PartTypesClient ctx = new PartTypesClient())
                    {
                        var dto = ctx.GetPartTypes().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.PartTypes = (PartTypes)new PartTypes().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.PartTypes.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddPartTypes");
                        }
                        else
                        {
                            var obj = new PartTypes(dto);
                           if (this.PartTypes == null || this.PartTypes.EntityId != obj.EntityId) this.PartTypes = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string RecuringPartEntityName
        {
            get
            {
                return this.RecuringPart == null ? "" : this.RecuringPart.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (RecuringPartClient ctx = new RecuringPartClient())
                    {
                        var dto = ctx.GetRecuringPart().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.RecuringPart = (RecuringPart)new RecuringPart().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.RecuringPart.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddRecuringPart");
                        }
                        else
                        {
                            var obj = new RecuringPart(dto);
                           if (this.RecuringPart == null || this.RecuringPart.EntityId != obj.EntityId) this.RecuringPart = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
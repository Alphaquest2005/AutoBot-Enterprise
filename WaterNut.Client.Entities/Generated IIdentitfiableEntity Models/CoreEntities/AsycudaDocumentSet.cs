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
    public partial class AsycudaDocumentSet
    {
       
       #region IIdentifiable Entities
        public override string EntityId
        {
            get
            {
                return this.AsycudaDocumentSetId.ToString();//this.AsycudaDocumentSetId == null?"0":			
            }
            set
            {
                this.AsycudaDocumentSetId = Convert.ToInt32(value);
            }
        }
        public string ApplicationSettingsEntityName
        {
            get
            {
                return this.ApplicationSettings == null ? "" : this.ApplicationSettings.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (ApplicationSettingsClient ctx = new ApplicationSettingsClient())
                    {
                        var dto = ctx.GetApplicationSettings().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.ApplicationSettings = (ApplicationSettings)new ApplicationSettings().CreateEntityFromString(value);
							
							this.AsycudaDocumentSetId = Convert.ToInt32(this.ApplicationSettings.ApplicationSettingsId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddApplicationSettings");
                        }
                        else
                        {
                            var obj = new ApplicationSettings(dto);
                           if (this.ApplicationSettings == null || this.ApplicationSettings.EntityId != obj.EntityId) this.ApplicationSettings = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string Customs_ProcedureEntityName
        {
            get
            {
                return this.Customs_Procedure == null ? "" : this.Customs_Procedure.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (Customs_ProcedureClient ctx = new Customs_ProcedureClient())
                    {
                        var dto = ctx.GetCustoms_Procedure().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Customs_Procedure = (Customs_Procedure)new Customs_Procedure().CreateEntityFromString(value);
							
							this.AsycudaDocumentSetId = Convert.ToInt32(this.Customs_Procedure.Customs_ProcedureId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddCustoms_Procedure");
                        }
                        else
                        {
                            var obj = new Customs_Procedure(dto);
                           if (this.Customs_Procedure == null || this.Customs_Procedure.EntityId != obj.EntityId) this.Customs_Procedure = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string ConsigneesEntityName
        {
            get
            {
                return this.Consignees == null ? "" : this.Consignees.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (ConsigneesClient ctx = new ConsigneesClient())
                    {
                        var dto = ctx.GetConsignees().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Consignees = (Consignees)new Consignees().CreateEntityFromString(value);
							
							this.AsycudaDocumentSetId = Convert.ToInt32(this.Consignees.ConsigneeName);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddConsignees");
                        }
                        else
                        {
                            var obj = new Consignees(dto);
                           if (this.Consignees == null || this.Consignees.EntityId != obj.EntityId) this.Consignees = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
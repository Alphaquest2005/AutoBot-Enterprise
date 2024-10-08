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
    public partial class AsycudaDocument
    {
       
       #region IIdentifiable Entities
        public override string EntityId
        {
            get
            {
                return this.ASYCUDA_Id.ToString();//this.ASYCUDA_Id == null?"0":			
            }
            set
            {
                this.ASYCUDA_Id = Convert.ToInt32(value);
            }
        }
        public string AsycudaDocumentSetExEntityName
        {
            get
            {
                return this.AsycudaDocumentSetEx == null ? "" : this.AsycudaDocumentSetEx.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (AsycudaDocumentSetExClient ctx = new AsycudaDocumentSetExClient())
                    {
                        var dto = ctx.GetAsycudaDocumentSetExs().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.AsycudaDocumentSetEx = (AsycudaDocumentSetEx)new AsycudaDocumentSetEx().CreateEntityFromString(value);
							
							this.ASYCUDA_Id = Convert.ToInt32(this.AsycudaDocumentSetEx.AsycudaDocumentSetId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddAsycudaDocumentSetEx");
                        }
                        else
                        {
                            var obj = new AsycudaDocumentSetEx(dto);
                           if (this.AsycudaDocumentSetEx == null || this.AsycudaDocumentSetEx.EntityId != obj.EntityId) this.AsycudaDocumentSetEx = obj;
                           
                        }
                         


                    }
            
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
							
							this.ASYCUDA_Id = Convert.ToInt32(this.ApplicationSettings.ApplicationSettingsId);
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
							
							this.ASYCUDA_Id = Convert.ToInt32(this.Customs_Procedure.Customs_ProcedureId);
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
        public string Document_TypeEntityName
        {
            get
            {
                return this.Document_Type == null ? "" : this.Document_Type.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (Document_TypeClient ctx = new Document_TypeClient())
                    {
                        var dto = ctx.GetDocument_Type().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Document_Type = (Document_Type)new Document_Type().CreateEntityFromString(value);
							
							this.Document_TypeId = Convert.ToInt32(this.Document_Type.Document_TypeId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddDocument_Type");
                        }
                        else
                        {
                            var obj = new Document_Type(dto);
                           if (this.Document_Type == null || this.Document_Type.EntityId != obj.EntityId) this.Document_Type = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
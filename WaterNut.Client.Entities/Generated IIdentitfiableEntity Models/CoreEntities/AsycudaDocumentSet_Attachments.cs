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
    public partial class AsycudaDocumentSet_Attachments
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
        public string AttachmentsEntityName
        {
            get
            {
                return this.Attachments == null ? "" : this.Attachments.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (AttachmentsClient ctx = new AttachmentsClient())
                    {
                        var dto = ctx.GetAttachments().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Attachments = (Attachments)new Attachments().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.Attachments.Id);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddAttachments");
                        }
                        else
                        {
                            var obj = new Attachments(dto);
                           if (this.Attachments == null || this.Attachments.EntityId != obj.EntityId) this.Attachments = obj;
                           
                        }
                         


                    }
            
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
							
							this.Id = Convert.ToInt32(this.AsycudaDocumentSetEx.AsycudaDocumentSetId);
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
        public string EmailsEntityName
        {
            get
            {
                return this.Emails == null ? "" : this.Emails.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (EmailsClient ctx = new EmailsClient())
                    {
                        var dto = ctx.GetEmails().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.Emails = (Emails)new Emails().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.Emails.EmailId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddEmails");
                        }
                        else
                        {
                            var obj = new Emails(dto);
                           if (this.Emails == null || this.Emails.EntityId != obj.EntityId) this.Emails = obj;
                           
                        }
                         


                    }
            
            }

      }
        public string AsycudaDocumentSetEntityName
        {
            get
            {
                return this.AsycudaDocumentSet == null ? "" : this.AsycudaDocumentSet.EntityName;
            }
            set
            {
                                if (string.IsNullOrEmpty(value)) return;
                string[] vals = value.Split(',');
               
                    using (AsycudaDocumentSetClient ctx = new AsycudaDocumentSetClient())
                    {
                        var dto = ctx.GetAsycudaDocumentSet().Result.AsEnumerable().FirstOrDefault(x => x.EntityName == value);
                        

                        if ( dto == null)
                        {
                            this.AsycudaDocumentSet = (AsycudaDocumentSet)new AsycudaDocumentSet().CreateEntityFromString(value);
							
							this.Id = Convert.ToInt32(this.AsycudaDocumentSet.AsycudaDocumentSetId);
                            this.TrackingState=TrackableEntities.TrackingState.Modified;
                           NotifyPropertyChanged("AddAsycudaDocumentSet");
                        }
                        else
                        {
                            var obj = new AsycudaDocumentSet(dto);
                           if (this.AsycudaDocumentSet == null || this.AsycudaDocumentSet.EntityId != obj.EntityId) this.AsycudaDocumentSet = obj;
                           
                        }
                         


                    }
            
            }

      }



         #endregion
    }
   
}
		
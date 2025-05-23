﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TrackableEntities.Client;
using Core.Common.Client.Entities;
using AdjustmentQS.Client.DTO;


using Core.Common.Validation;

namespace AdjustmentQS.Client.Entities
{
       public partial class AsycudaDocumentItemEntryDataDetail: BaseEntity<AsycudaDocumentItemEntryDataDetail>
    {
        DTO.AsycudaDocumentItemEntryDataDetail asycudadocumentitementrydatadetail;
        public AsycudaDocumentItemEntryDataDetail(DTO.AsycudaDocumentItemEntryDataDetail dto )
        {
              asycudadocumentitementrydatadetail = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.AsycudaDocumentItemEntryDataDetail>(asycudadocumentitementrydatadetail);

        }

        public DTO.AsycudaDocumentItemEntryDataDetail DTO
        {
            get
            {
             return asycudadocumentitementrydatadetail;
            }
            set
            {
                asycudadocumentitementrydatadetail = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.asycudadocumentitementrydatadetail.EntryDataDetailsId; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.EntryDataDetailsId) return;
				this.asycudadocumentitementrydatadetail.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Item_ is required")]
       
public int Item_Id
		{ 
		    get { return this.asycudadocumentitementrydatadetail.Item_Id; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.Item_Id) return;
				this.asycudadocumentitementrydatadetail.Item_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Item_Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.asycudadocumentitementrydatadetail.ItemNumber; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.ItemNumber) return;
				this.asycudadocumentitementrydatadetail.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       
       
                
                [MaxLength(101, ErrorMessage = "key has a max length of 101 letters ")]
public string key
		{ 
		    get { return this.asycudadocumentitementrydatadetail.key; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.key) return;
				this.asycudadocumentitementrydatadetail.key = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("key");
			}
		}
     

       
       
                
                [MaxLength(40, ErrorMessage = "DocumentType has a max length of 40 letters ")]
public string DocumentType
		{ 
		    get { return this.asycudadocumentitementrydatadetail.DocumentType; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.DocumentType) return;
				this.asycudadocumentitementrydatadetail.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> Quantity
		{ 
		    get { return this.asycudadocumentitementrydatadetail.Quantity; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.Quantity) return;
				this.asycudadocumentitementrydatadetail.Quantity = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Quantity");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ImportComplete is required")]
       
public bool ImportComplete
		{ 
		    get { return this.asycudadocumentitementrydatadetail.ImportComplete; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.ImportComplete) return;
				this.asycudadocumentitementrydatadetail.ImportComplete = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ImportComplete");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData_ is required")]
       
public int EntryData_Id
		{ 
		    get { return this.asycudadocumentitementrydatadetail.EntryData_Id; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.EntryData_Id) return;
				this.asycudadocumentitementrydatadetail.EntryData_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryData_Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "CustomsProcedure is required")]
       
                
                [MaxLength(11, ErrorMessage = "CustomsProcedure has a max length of 11 letters ")]
public string CustomsProcedure
		{ 
		    get { return this.asycudadocumentitementrydatadetail.CustomsProcedure; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.CustomsProcedure) return;
				this.asycudadocumentitementrydatadetail.CustomsProcedure = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CustomsProcedure");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Asycuda_id is required")]
       [NumberValidationAttribute]
public int Asycuda_id
		{ 
		    get { return this.asycudadocumentitementrydatadetail.Asycuda_id; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.Asycuda_id) return;
				this.asycudadocumentitementrydatadetail.Asycuda_id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Asycuda_id");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "EntryDataType has a max length of 50 letters ")]
public string EntryDataType
		{ 
		    get { return this.asycudadocumentitementrydatadetail.EntryDataType; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.EntryDataType) return;
				this.asycudadocumentitementrydatadetail.EntryDataType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataType");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.asycudadocumentitementrydatadetail.ApplicationSettingsId; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.ApplicationSettingsId) return;
				this.asycudadocumentitementrydatadetail.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.asycudadocumentitementrydatadetail.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.AsycudaDocumentSetId) return;
				this.asycudadocumentitementrydatadetail.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "CNumber has a max length of 20 letters ")]
public string CNumber
		{ 
		    get { return this.asycudadocumentitementrydatadetail.CNumber; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.CNumber) return;
				this.asycudadocumentitementrydatadetail.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "LineNumber is required")]
       [NumberValidationAttribute]
public int LineNumber
		{ 
		    get { return this.asycudadocumentitementrydatadetail.LineNumber; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.LineNumber) return;
				this.asycudadocumentitementrydatadetail.LineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LineNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "CustomsOperation has a max length of 50 letters ")]
public string CustomsOperation
		{ 
		    get { return this.asycudadocumentitementrydatadetail.CustomsOperation; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.CustomsOperation) return;
				this.asycudadocumentitementrydatadetail.CustomsOperation = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CustomsOperation");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public long Id
		{ 
		    get { return this.asycudadocumentitementrydatadetail.Id; }
			set
			{
			    if (value == this.asycudadocumentitementrydatadetail.Id) return;
				this.asycudadocumentitementrydatadetail.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       private AdjustmentOver _AdjustmentOver;
        public  AdjustmentOver AdjustmentOver
		{
		    get
               { 
                  if (this.asycudadocumentitementrydatadetail != null)
                   {
                       if (_AdjustmentOver != null)
                       {
                           if (this.asycudadocumentitementrydatadetail.AdjustmentOver !=
                               _AdjustmentOver.DTO)
                           {
                                if (this.asycudadocumentitementrydatadetail.AdjustmentOver  != null)
                               _AdjustmentOver = new AdjustmentOver(this.asycudadocumentitementrydatadetail.AdjustmentOver);
                           }
                       }
                       else
                       {
                             if (this.asycudadocumentitementrydatadetail.AdjustmentOver  != null)
                           _AdjustmentOver = new AdjustmentOver(this.asycudadocumentitementrydatadetail.AdjustmentOver);
                       }
                   }


             //       if (_AdjustmentOver != null) return _AdjustmentOver;
                       
             //       var i = new AdjustmentOver(){TrackingState = TrackingState.Added};
			//		//if (this.asycudadocumentitementrydatadetail.AdjustmentOver == null) Debugger.Break();
			//		if (this.asycudadocumentitementrydatadetail.AdjustmentOver != null)
            //        {
            //           i. = this.asycudadocumentitementrydatadetail.AdjustmentOver;
            //        }
            //        else
            //        {
            //            this.asycudadocumentitementrydatadetail.AdjustmentOver = i.;
             //       }
                           
            //        _AdjustmentOver = i;
                     
                    return _AdjustmentOver;
               }
			set
			{
			    if (value == _AdjustmentOver) return;
                _AdjustmentOver = value;
                if(value != null)
                     this.asycudadocumentitementrydatadetail.AdjustmentOver = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("AdjustmentOver");
			}
		}
        

       private AdjustmentShort _AdjustmentShort;
        public  AdjustmentShort AdjustmentShort
		{
		    get
               { 
                  if (this.asycudadocumentitementrydatadetail != null)
                   {
                       if (_AdjustmentShort != null)
                       {
                           if (this.asycudadocumentitementrydatadetail.AdjustmentShort !=
                               _AdjustmentShort.DTO)
                           {
                                if (this.asycudadocumentitementrydatadetail.AdjustmentShort  != null)
                               _AdjustmentShort = new AdjustmentShort(this.asycudadocumentitementrydatadetail.AdjustmentShort);
                           }
                       }
                       else
                       {
                             if (this.asycudadocumentitementrydatadetail.AdjustmentShort  != null)
                           _AdjustmentShort = new AdjustmentShort(this.asycudadocumentitementrydatadetail.AdjustmentShort);
                       }
                   }


             //       if (_AdjustmentShort != null) return _AdjustmentShort;
                       
             //       var i = new AdjustmentShort(){TrackingState = TrackingState.Added};
			//		//if (this.asycudadocumentitementrydatadetail.AdjustmentShort == null) Debugger.Break();
			//		if (this.asycudadocumentitementrydatadetail.AdjustmentShort != null)
            //        {
            //           i. = this.asycudadocumentitementrydatadetail.AdjustmentShort;
            //        }
            //        else
            //        {
            //            this.asycudadocumentitementrydatadetail.AdjustmentShort = i.;
             //       }
                           
            //        _AdjustmentShort = i;
                     
                    return _AdjustmentShort;
               }
			set
			{
			    if (value == _AdjustmentShort) return;
                _AdjustmentShort = value;
                if(value != null)
                     this.asycudadocumentitementrydatadetail.AdjustmentShort = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("AdjustmentShort");
			}
		}
        

       private AdjustmentDetail _AdjustmentDetail;
        public  AdjustmentDetail AdjustmentDetail
		{
		    get
               { 
                  if (this.asycudadocumentitementrydatadetail != null)
                   {
                       if (_AdjustmentDetail != null)
                       {
                           if (this.asycudadocumentitementrydatadetail.AdjustmentDetail !=
                               _AdjustmentDetail.DTO)
                           {
                                if (this.asycudadocumentitementrydatadetail.AdjustmentDetail  != null)
                               _AdjustmentDetail = new AdjustmentDetail(this.asycudadocumentitementrydatadetail.AdjustmentDetail);
                           }
                       }
                       else
                       {
                             if (this.asycudadocumentitementrydatadetail.AdjustmentDetail  != null)
                           _AdjustmentDetail = new AdjustmentDetail(this.asycudadocumentitementrydatadetail.AdjustmentDetail);
                       }
                   }


             //       if (_AdjustmentDetail != null) return _AdjustmentDetail;
                       
             //       var i = new AdjustmentDetail(){TrackingState = TrackingState.Added};
			//		//if (this.asycudadocumentitementrydatadetail.AdjustmentDetail == null) Debugger.Break();
			//		if (this.asycudadocumentitementrydatadetail.AdjustmentDetail != null)
            //        {
            //           i. = this.asycudadocumentitementrydatadetail.AdjustmentDetail;
            //        }
            //        else
            //        {
            //            this.asycudadocumentitementrydatadetail.AdjustmentDetail = i.;
             //       }
                           
            //        _AdjustmentDetail = i;
                     
                    return _AdjustmentDetail;
               }
			set
			{
			    if (value == _AdjustmentDetail) return;
                _AdjustmentDetail = value;
                if(value != null)
                     this.asycudadocumentitementrydatadetail.AdjustmentDetail = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("AdjustmentDetail");
			}
		}
        


        ChangeTrackingCollection<DTO.AsycudaDocumentItemEntryDataDetail> _changeTracker;    
        public ChangeTrackingCollection<DTO.AsycudaDocumentItemEntryDataDetail> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

        public new TrackableEntities.TrackingState TrackingState
        {
            get
            {
                return this.DTO.TrackingState;
            }
            set
            {
                this.DTO.TrackingState = value;
                NotifyPropertyChanged("TrackingState");
            }
        }

    }
}



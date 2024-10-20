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
using EntryDataQS.Client.DTO;


using Core.Common.Validation;

namespace EntryDataQS.Client.Entities
{
       public partial class EntryData: BaseEntity<EntryData>
    {
        DTO.EntryData entrydata;
        public EntryData(DTO.EntryData dto )
        {
              entrydata = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.EntryData>(entrydata);

        }

        public DTO.EntryData DTO
        {
            get
            {
             return entrydata;
            }
            set
            {
                entrydata = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= "EntryData is required")]
       
                
                [MaxLength(50, ErrorMessage = "EntryDataId has a max length of 50 letters ")]
public string EntryDataId
		{ 
		    get { return this.entrydata.EntryDataId; }
			set
			{
			    if (value == this.entrydata.EntryDataId) return;
				this.entrydata.EntryDataId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryDataDate is required")]
       
public System.DateTime EntryDataDate
		{ 
		    get { return this.entrydata.EntryDataDate; }
			set
			{
			    if (value == this.entrydata.EntryDataDate) return;
				this.entrydata.EntryDataDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDate");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> ImportedLines
		{ 
		    get { return this.entrydata.ImportedLines; }
			set
			{
			    if (value == this.entrydata.ImportedLines) return;
				this.entrydata.ImportedLines = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ImportedLines");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalFreight
		{ 
		    get { return this.entrydata.TotalFreight; }
			set
			{
			    if (value == this.entrydata.TotalFreight) return;
				this.entrydata.TotalFreight = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalFreight");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalInternalFreight
		{ 
		    get { return this.entrydata.TotalInternalFreight; }
			set
			{
			    if (value == this.entrydata.TotalInternalFreight) return;
				this.entrydata.TotalInternalFreight = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalInternalFreight");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalWeight
		{ 
		    get { return this.entrydata.TotalWeight; }
			set
			{
			    if (value == this.entrydata.TotalWeight) return;
				this.entrydata.TotalWeight = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalWeight");
			}
		}
     

       
       
                
                [MaxLength(4, ErrorMessage = "Currency has a max length of 4 letters ")]
public string Currency
		{ 
		    get { return this.entrydata.Currency; }
			set
			{
			    if (value == this.entrydata.Currency) return;
				this.entrydata.Currency = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Currency");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.entrydata.ApplicationSettingsId; }
			set
			{
			    if (value == this.entrydata.ApplicationSettingsId) return;
				this.entrydata.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "EmailId has a max length of 255 letters ")]
public string EmailId
		{ 
		    get { return this.entrydata.EmailId; }
			set
			{
			    if (value == this.entrydata.EmailId) return;
				this.entrydata.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     

       
       
public Nullable<int> FileTypeId
		{ 
		    get { return this.entrydata.FileTypeId; }
			set
			{
			    if (value == this.entrydata.FileTypeId) return;
				this.entrydata.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       
       
                
                [MaxLength(100, ErrorMessage = "SupplierCode has a max length of 100 letters ")]
public string SupplierCode
		{ 
		    get { return this.entrydata.SupplierCode; }
			set
			{
			    if (value == this.entrydata.SupplierCode) return;
				this.entrydata.SupplierCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SupplierCode");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceTotal
		{ 
		    get { return this.entrydata.InvoiceTotal; }
			set
			{
			    if (value == this.entrydata.InvoiceTotal) return;
				this.entrydata.InvoiceTotal = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceTotal");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalOtherCost
		{ 
		    get { return this.entrydata.TotalOtherCost; }
			set
			{
			    if (value == this.entrydata.TotalOtherCost) return;
				this.entrydata.TotalOtherCost = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalOtherCost");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalInsurance
		{ 
		    get { return this.entrydata.TotalInsurance; }
			set
			{
			    if (value == this.entrydata.TotalInsurance) return;
				this.entrydata.TotalInsurance = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalInsurance");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalDeduction
		{ 
		    get { return this.entrydata.TotalDeduction; }
			set
			{
			    if (value == this.entrydata.TotalDeduction) return;
				this.entrydata.TotalDeduction = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalDeduction");
			}
		}
     

       
       
                
                
public string SourceFile
		{ 
		    get { return this.entrydata.SourceFile; }
			set
			{
			    if (value == this.entrydata.SourceFile) return;
				this.entrydata.SourceFile = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SourceFile");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData_ is required")]
       
public int EntryData_Id
		{ 
		    get { return this.entrydata.EntryData_Id; }
			set
			{
			    if (value == this.entrydata.EntryData_Id) return;
				this.entrydata.EntryData_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryData_Id");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> Packages
		{ 
		    get { return this.entrydata.Packages; }
			set
			{
			    if (value == this.entrydata.Packages) return;
				this.entrydata.Packages = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Packages");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "UpgradeKey has a max length of 50 letters ")]
public string UpgradeKey
		{ 
		    get { return this.entrydata.UpgradeKey; }
			set
			{
			    if (value == this.entrydata.UpgradeKey) return;
				this.entrydata.UpgradeKey = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("UpgradeKey");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "EntryType has a max length of 50 letters ")]
public string EntryType
		{ 
		    get { return this.entrydata.EntryType; }
			set
			{
			    if (value == this.entrydata.EntryType) return;
				this.entrydata.EntryType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryType");
			}
		}
     

        ObservableCollection<AsycudaDocumentEntryData> _AsycudaDocumentEntryDatas = null;
        public  ObservableCollection<AsycudaDocumentEntryData> AsycudaDocumentEntryDatas
		{
            
		    get 
				{ 
					if(_AsycudaDocumentEntryDatas != null) return _AsycudaDocumentEntryDatas;
					//if (this.entrydata.AsycudaDocumentEntryDatas == null) Debugger.Break();
					if(this.entrydata.AsycudaDocumentEntryDatas != null)
					{
						_AsycudaDocumentEntryDatas = new ObservableCollection<AsycudaDocumentEntryData>(this.entrydata.AsycudaDocumentEntryDatas.Select(x => new AsycudaDocumentEntryData(x)));
					}
					
						_AsycudaDocumentEntryDatas.CollectionChanged += AsycudaDocumentEntryDatas_CollectionChanged; 
					
					return _AsycudaDocumentEntryDatas; 
				}
			set
			{
			    if (Equals(value, _AsycudaDocumentEntryDatas)) return;
				if (value != null)
					this.entrydata.AsycudaDocumentEntryDatas = new ChangeTrackingCollection<DTO.AsycudaDocumentEntryData>(value.Select(x => x.DTO).ToList());
                _AsycudaDocumentEntryDatas = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_AsycudaDocumentEntryDatas != null)
				_AsycudaDocumentEntryDatas.CollectionChanged += AsycudaDocumentEntryDatas_CollectionChanged;               
				NotifyPropertyChanged("AsycudaDocumentEntryDatas");
			}
		}
        
        void AsycudaDocumentEntryDatas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (AsycudaDocumentEntryData itm in e.NewItems)
                    {
                        if (itm != null)
                        entrydata.AsycudaDocumentEntryDatas.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AsycudaDocumentEntryData itm in e.OldItems)
                    {
                        if (itm != null)
                        entrydata.AsycudaDocumentEntryDatas.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }

        ObservableCollection<AsycudaDocumentSetEntryData> _AsycudaDocumentSetEntryDatas = null;
        public  ObservableCollection<AsycudaDocumentSetEntryData> AsycudaDocumentSetEntryDatas
		{
            
		    get 
				{ 
					if(_AsycudaDocumentSetEntryDatas != null) return _AsycudaDocumentSetEntryDatas;
					//if (this.entrydata.AsycudaDocumentSetEntryDatas == null) Debugger.Break();
					if(this.entrydata.AsycudaDocumentSetEntryDatas != null)
					{
						_AsycudaDocumentSetEntryDatas = new ObservableCollection<AsycudaDocumentSetEntryData>(this.entrydata.AsycudaDocumentSetEntryDatas.Select(x => new AsycudaDocumentSetEntryData(x)));
					}
					
						_AsycudaDocumentSetEntryDatas.CollectionChanged += AsycudaDocumentSetEntryDatas_CollectionChanged; 
					
					return _AsycudaDocumentSetEntryDatas; 
				}
			set
			{
			    if (Equals(value, _AsycudaDocumentSetEntryDatas)) return;
				if (value != null)
					this.entrydata.AsycudaDocumentSetEntryDatas = new ChangeTrackingCollection<DTO.AsycudaDocumentSetEntryData>(value.Select(x => x.DTO).ToList());
                _AsycudaDocumentSetEntryDatas = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_AsycudaDocumentSetEntryDatas != null)
				_AsycudaDocumentSetEntryDatas.CollectionChanged += AsycudaDocumentSetEntryDatas_CollectionChanged;               
				NotifyPropertyChanged("AsycudaDocumentSetEntryDatas");
			}
		}
        
        void AsycudaDocumentSetEntryDatas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (AsycudaDocumentSetEntryData itm in e.NewItems)
                    {
                        if (itm != null)
                        entrydata.AsycudaDocumentSetEntryDatas.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AsycudaDocumentSetEntryData itm in e.OldItems)
                    {
                        if (itm != null)
                        entrydata.AsycudaDocumentSetEntryDatas.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.EntryData> _changeTracker;    
        public ChangeTrackingCollection<DTO.EntryData> ChangeTracker
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



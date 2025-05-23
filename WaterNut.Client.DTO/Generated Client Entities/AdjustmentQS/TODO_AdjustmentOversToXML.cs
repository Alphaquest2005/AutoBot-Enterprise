﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;


using Core.Common.Client.DTO;
using TrackableEntities;
using TrackableEntities.Client;

namespace AdjustmentQS.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class TODO_AdjustmentOversToXML : BaseEntity<TODO_AdjustmentOversToXML>, ITrackable, IEquatable<TODO_AdjustmentOversToXML>
    {
        [DataMember]
        public int EntryDataDetailsId
		{ 
		    get { return _EntryDataDetailsId; }
			set
			{
			    if (value == _EntryDataDetailsId) return;
				_EntryDataDetailsId = value;
				NotifyPropertyChanged();//m => this.EntryDataDetailsId
			}
		}
        private int _EntryDataDetailsId;

        [DataMember]
        public int EntryData_Id
		{ 
		    get { return _EntryData_Id; }
			set
			{
			    if (value == _EntryData_Id) return;
				_EntryData_Id = value;
				NotifyPropertyChanged();//m => this.EntryData_Id
			}
		}
        private int _EntryData_Id;

        [DataMember]
        public string EntryDataId
		{ 
		    get { return _EntryDataId; }
			set
			{
			    if (value == _EntryDataId) return;
				_EntryDataId = value;
				NotifyPropertyChanged();//m => this.EntryDataId
			}
		}
        private string _EntryDataId;

        [DataMember]
        public Nullable<int> LineNumber
		{ 
		    get { return _LineNumber; }
			set
			{
			    if (value == _LineNumber) return;
				_LineNumber = value;
				NotifyPropertyChanged();//m => this.LineNumber
			}
		}
        private Nullable<int> _LineNumber;

        [DataMember]
        public string ItemNumber
		{ 
		    get { return _ItemNumber; }
			set
			{
			    if (value == _ItemNumber) return;
				_ItemNumber = value;
				NotifyPropertyChanged();//m => this.ItemNumber
			}
		}
        private string _ItemNumber;

        [DataMember]
        public Nullable<double> Quantity
		{ 
		    get { return _Quantity; }
			set
			{
			    if (value == _Quantity) return;
				_Quantity = value;
				NotifyPropertyChanged();//m => this.Quantity
			}
		}
        private Nullable<double> _Quantity;

        [DataMember]
        public string Units
		{ 
		    get { return _Units; }
			set
			{
			    if (value == _Units) return;
				_Units = value;
				NotifyPropertyChanged();//m => this.Units
			}
		}
        private string _Units;

        [DataMember]
        public string ItemDescription
		{ 
		    get { return _ItemDescription; }
			set
			{
			    if (value == _ItemDescription) return;
				_ItemDescription = value;
				NotifyPropertyChanged();//m => this.ItemDescription
			}
		}
        private string _ItemDescription;

        [DataMember]
        public Nullable<double> Cost
		{ 
		    get { return _Cost; }
			set
			{
			    if (value == _Cost) return;
				_Cost = value;
				NotifyPropertyChanged();//m => this.Cost
			}
		}
        private Nullable<double> _Cost;

        [DataMember]
        public Nullable<double> QtyAllocated
		{ 
		    get { return _QtyAllocated; }
			set
			{
			    if (value == _QtyAllocated) return;
				_QtyAllocated = value;
				NotifyPropertyChanged();//m => this.QtyAllocated
			}
		}
        private Nullable<double> _QtyAllocated;

        [DataMember]
        public Nullable<double> UnitWeight
		{ 
		    get { return _UnitWeight; }
			set
			{
			    if (value == _UnitWeight) return;
				_UnitWeight = value;
				NotifyPropertyChanged();//m => this.UnitWeight
			}
		}
        private Nullable<double> _UnitWeight;

        [DataMember]
        public Nullable<bool> DoNotAllocate
		{ 
		    get { return _DoNotAllocate; }
			set
			{
			    if (value == _DoNotAllocate) return;
				_DoNotAllocate = value;
				NotifyPropertyChanged();//m => this.DoNotAllocate
			}
		}
        private Nullable<bool> _DoNotAllocate;

        [DataMember]
        public string TariffCode
		{ 
		    get { return _TariffCode; }
			set
			{
			    if (value == _TariffCode) return;
				_TariffCode = value;
				NotifyPropertyChanged();//m => this.TariffCode
			}
		}
        private string _TariffCode;

        [DataMember]
        public string CNumber
		{ 
		    get { return _CNumber; }
			set
			{
			    if (value == _CNumber) return;
				_CNumber = value;
				NotifyPropertyChanged();//m => this.CNumber
			}
		}
        private string _CNumber;

        [DataMember]
        public Nullable<int> CLineNumber
		{ 
		    get { return _CLineNumber; }
			set
			{
			    if (value == _CLineNumber) return;
				_CLineNumber = value;
				NotifyPropertyChanged();//m => this.CLineNumber
			}
		}
        private Nullable<int> _CLineNumber;

        [DataMember]
        public Nullable<int> AsycudaDocumentSetId
		{ 
		    get { return _AsycudaDocumentSetId; }
			set
			{
			    if (value == _AsycudaDocumentSetId) return;
				_AsycudaDocumentSetId = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentSetId
			}
		}
        private Nullable<int> _AsycudaDocumentSetId;

        [DataMember]
        public Nullable<double> InvoiceQty
		{ 
		    get { return _InvoiceQty; }
			set
			{
			    if (value == _InvoiceQty) return;
				_InvoiceQty = value;
				NotifyPropertyChanged();//m => this.InvoiceQty
			}
		}
        private Nullable<double> _InvoiceQty;

        [DataMember]
        public Nullable<double> ReceivedQty
		{ 
		    get { return _ReceivedQty; }
			set
			{
			    if (value == _ReceivedQty) return;
				_ReceivedQty = value;
				NotifyPropertyChanged();//m => this.ReceivedQty
			}
		}
        private Nullable<double> _ReceivedQty;

        [DataMember]
        public string PreviousInvoiceNumber
		{ 
		    get { return _PreviousInvoiceNumber; }
			set
			{
			    if (value == _PreviousInvoiceNumber) return;
				_PreviousInvoiceNumber = value;
				NotifyPropertyChanged();//m => this.PreviousInvoiceNumber
			}
		}
        private string _PreviousInvoiceNumber;

        [DataMember]
        public string PreviousCNumber
		{ 
		    get { return _PreviousCNumber; }
			set
			{
			    if (value == _PreviousCNumber) return;
				_PreviousCNumber = value;
				NotifyPropertyChanged();//m => this.PreviousCNumber
			}
		}
        private string _PreviousCNumber;

        [DataMember]
        public string Comment
		{ 
		    get { return _Comment; }
			set
			{
			    if (value == _Comment) return;
				_Comment = value;
				NotifyPropertyChanged();//m => this.Comment
			}
		}
        private string _Comment;

        [DataMember]
        public string Status
		{ 
		    get { return _Status; }
			set
			{
			    if (value == _Status) return;
				_Status = value;
				NotifyPropertyChanged();//m => this.Status
			}
		}
        private string _Status;

        [DataMember]
        public Nullable<System.DateTime> EffectiveDate
		{ 
		    get { return _EffectiveDate; }
			set
			{
			    if (value == _EffectiveDate) return;
				_EffectiveDate = value;
				NotifyPropertyChanged();//m => this.EffectiveDate
			}
		}
        private Nullable<System.DateTime> _EffectiveDate;

        [DataMember]
        public string Currency
		{ 
		    get { return _Currency; }
			set
			{
			    if (value == _Currency) return;
				_Currency = value;
				NotifyPropertyChanged();//m => this.Currency
			}
		}
        private string _Currency;

        [DataMember]
        public Nullable<int> ApplicationSettingsId
		{ 
		    get { return _ApplicationSettingsId; }
			set
			{
			    if (value == _ApplicationSettingsId) return;
				_ApplicationSettingsId = value;
				NotifyPropertyChanged();//m => this.ApplicationSettingsId
			}
		}
        private Nullable<int> _ApplicationSettingsId;

        [DataMember]
        public string Type
		{ 
		    get { return _Type; }
			set
			{
			    if (value == _Type) return;
				_Type = value;
				NotifyPropertyChanged();//m => this.Type
			}
		}
        private string _Type;

        [DataMember]
        public string DutyFreePaid
		{ 
		    get { return _DutyFreePaid; }
			set
			{
			    if (value == _DutyFreePaid) return;
				_DutyFreePaid = value;
				NotifyPropertyChanged();//m => this.DutyFreePaid
			}
		}
        private string _DutyFreePaid;

        [DataMember]
        public string EmailId
		{ 
		    get { return _EmailId; }
			set
			{
			    if (value == _EmailId) return;
				_EmailId = value;
				NotifyPropertyChanged();//m => this.EmailId
			}
		}
        private string _EmailId;

        [DataMember]
        public Nullable<int> FileTypeId
		{ 
		    get { return _FileTypeId; }
			set
			{
			    if (value == _FileTypeId) return;
				_FileTypeId = value;
				NotifyPropertyChanged();//m => this.FileTypeId
			}
		}
        private Nullable<int> _FileTypeId;

        [DataMember]
        public Nullable<System.DateTime> InvoiceDate
		{ 
		    get { return _InvoiceDate; }
			set
			{
			    if (value == _InvoiceDate) return;
				_InvoiceDate = value;
				NotifyPropertyChanged();//m => this.InvoiceDate
			}
		}
        private Nullable<System.DateTime> _InvoiceDate;

        [DataMember]
        public string Subject
		{ 
		    get { return _Subject; }
			set
			{
			    if (value == _Subject) return;
				_Subject = value;
				NotifyPropertyChanged();//m => this.Subject
			}
		}
        private string _Subject;

        [DataMember]
        public Nullable<System.DateTime> EmailDate
		{ 
		    get { return _EmailDate; }
			set
			{
			    if (value == _EmailDate) return;
				_EmailDate = value;
				NotifyPropertyChanged();//m => this.EmailDate
			}
		}
        private Nullable<System.DateTime> _EmailDate;

        [DataMember]
        public int AlreadyExecuted
		{ 
		    get { return _AlreadyExecuted; }
			set
			{
			    if (value == _AlreadyExecuted) return;
				_AlreadyExecuted = value;
				NotifyPropertyChanged();//m => this.AlreadyExecuted
			}
		}
        private int _AlreadyExecuted;

        [DataMember]
        public string Vendor
		{ 
		    get { return _Vendor; }
			set
			{
			    if (value == _Vendor) return;
				_Vendor = value;
				NotifyPropertyChanged();//m => this.Vendor
			}
		}
        private string _Vendor;

        [DataMember]
        public string SourceFile
		{ 
		    get { return _SourceFile; }
			set
			{
			    if (value == _SourceFile) return;
				_SourceFile = value;
				NotifyPropertyChanged();//m => this.SourceFile
			}
		}
        private string _SourceFile;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<TODO_AdjustmentOversToXML>.Equals(TODO_AdjustmentOversToXML other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}




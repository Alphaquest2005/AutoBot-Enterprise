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
    public partial class EntryDataDetail : BaseEntity<EntryDataDetail>, ITrackable, IEquatable<EntryDataDetail>
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
        public double Quantity
		{ 
		    get { return _Quantity; }
			set
			{
			    if (value == _Quantity) return;
				_Quantity = value;
				NotifyPropertyChanged();//m => this.Quantity
			}
		}
        private double _Quantity;

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
        public double Cost
		{ 
		    get { return _Cost; }
			set
			{
			    if (value == _Cost) return;
				_Cost = value;
				NotifyPropertyChanged();//m => this.Cost
			}
		}
        private double _Cost;

        [DataMember]
        public double QtyAllocated
		{ 
		    get { return _QtyAllocated; }
			set
			{
			    if (value == _QtyAllocated) return;
				_QtyAllocated = value;
				NotifyPropertyChanged();//m => this.QtyAllocated
			}
		}
        private double _QtyAllocated;

        [DataMember]
        public double UnitWeight
		{ 
		    get { return _UnitWeight; }
			set
			{
			    if (value == _UnitWeight) return;
				_UnitWeight = value;
				NotifyPropertyChanged();//m => this.UnitWeight
			}
		}
        private double _UnitWeight;

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
        public Nullable<double> Freight
		{ 
		    get { return _Freight; }
			set
			{
			    if (value == _Freight) return;
				_Freight = value;
				NotifyPropertyChanged();//m => this.Freight
			}
		}
        private Nullable<double> _Freight;

        [DataMember]
        public Nullable<double> Weight
		{ 
		    get { return _Weight; }
			set
			{
			    if (value == _Weight) return;
				_Weight = value;
				NotifyPropertyChanged();//m => this.Weight
			}
		}
        private Nullable<double> _Weight;

        [DataMember]
        public Nullable<double> InternalFreight
		{ 
		    get { return _InternalFreight; }
			set
			{
			    if (value == _InternalFreight) return;
				_InternalFreight = value;
				NotifyPropertyChanged();//m => this.InternalFreight
			}
		}
        private Nullable<double> _InternalFreight;

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
        public Nullable<bool> IsReconciled
		{ 
		    get { return _IsReconciled; }
			set
			{
			    if (value == _IsReconciled) return;
				_IsReconciled = value;
				NotifyPropertyChanged();//m => this.IsReconciled
			}
		}
        private Nullable<bool> _IsReconciled;

        [DataMember]
        public Nullable<double> TaxAmount
		{ 
		    get { return _TaxAmount; }
			set
			{
			    if (value == _TaxAmount) return;
				_TaxAmount = value;
				NotifyPropertyChanged();//m => this.TaxAmount
			}
		}
        private Nullable<double> _TaxAmount;

        [DataMember]
        public Nullable<double> LastCost
		{ 
		    get { return _LastCost; }
			set
			{
			    if (value == _LastCost) return;
				_LastCost = value;
				NotifyPropertyChanged();//m => this.LastCost
			}
		}
        private Nullable<double> _LastCost;

        [DataMember]
        public Nullable<double> TotalCost
		{ 
		    get { return _TotalCost; }
			set
			{
			    if (value == _TotalCost) return;
				_TotalCost = value;
				NotifyPropertyChanged();//m => this.TotalCost
			}
		}
        private Nullable<double> _TotalCost;

        [DataMember]
        public int InventoryItemId
		{ 
		    get { return _InventoryItemId; }
			set
			{
			    if (value == _InventoryItemId) return;
				_InventoryItemId = value;
				NotifyPropertyChanged();//m => this.InventoryItemId
			}
		}
        private int _InventoryItemId;

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
        public Nullable<int> FileLineNumber
		{ 
		    get { return _FileLineNumber; }
			set
			{
			    if (value == _FileLineNumber) return;
				_FileLineNumber = value;
				NotifyPropertyChanged();//m => this.FileLineNumber
			}
		}
        private Nullable<int> _FileLineNumber;

        [DataMember]
        public Nullable<int> UpgradeKey
		{ 
		    get { return _UpgradeKey; }
			set
			{
			    if (value == _UpgradeKey) return;
				_UpgradeKey = value;
				NotifyPropertyChanged();//m => this.UpgradeKey
			}
		}
        private Nullable<int> _UpgradeKey;

        [DataMember]
        public Nullable<double> VolumeLiters
		{ 
		    get { return _VolumeLiters; }
			set
			{
			    if (value == _VolumeLiters) return;
				_VolumeLiters = value;
				NotifyPropertyChanged();//m => this.VolumeLiters
			}
		}
        private Nullable<double> _VolumeLiters;

        [DataMember]
        public string EntryDataDetailsKey
		{ 
		    get { return _EntryDataDetailsKey; }
			set
			{
			    if (value == _EntryDataDetailsKey) return;
				_EntryDataDetailsKey = value;
				NotifyPropertyChanged();//m => this.EntryDataDetailsKey
			}
		}
        private string _EntryDataDetailsKey;

        [DataMember]
        public double TotalValue
		{ 
		    get { return _TotalValue; }
			set
			{
			    if (value == _TotalValue) return;
				_TotalValue = value;
				NotifyPropertyChanged();//m => this.TotalValue
			}
		}
        private double _TotalValue;

       
        [DataMember]
        public ChangeTrackingCollection<AsycudaSalesAllocation> AsycudaSalesAllocations
		{
		    get { return _AsycudaSalesAllocations; }
			set
			{
			    if (Equals(value, _AsycudaSalesAllocations)) return;
				_AsycudaSalesAllocations = value;
				NotifyPropertyChanged();//m => this.AsycudaSalesAllocations
			}
		}
        private ChangeTrackingCollection<AsycudaSalesAllocation> _AsycudaSalesAllocations = new ChangeTrackingCollection<AsycudaSalesAllocation>();

        [DataMember]
        public InventoryItemsEx InventoryItemsEx
		{
		    get { return _InventoryItemsEx; }
			set
			{
			    if (value == _InventoryItemsEx) return;
				_InventoryItemsEx = value;
                InventoryItemsExChangeTracker = _InventoryItemsEx == null ? null
                    : new ChangeTrackingCollection<InventoryItemsEx> { _InventoryItemsEx };
				NotifyPropertyChanged();//m => this.InventoryItemsEx
			}
		}
        private InventoryItemsEx _InventoryItemsEx;
        private ChangeTrackingCollection<InventoryItemsEx> InventoryItemsExChangeTracker { get; set; }

        [DataMember]
        public ChangeTrackingCollection<AdjustmentOversAllocation> AdjustmentOversAllocations
		{
		    get { return _AdjustmentOversAllocations; }
			set
			{
			    if (Equals(value, _AdjustmentOversAllocations)) return;
				_AdjustmentOversAllocations = value;
				NotifyPropertyChanged();//m => this.AdjustmentOversAllocations
			}
		}
        private ChangeTrackingCollection<AdjustmentOversAllocation> _AdjustmentOversAllocations = new ChangeTrackingCollection<AdjustmentOversAllocation>();

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<EntryDataDetail>.Equals(EntryDataDetail other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



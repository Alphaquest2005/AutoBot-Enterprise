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

namespace CoreEntities.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class TODO_SubmitPOInfo : BaseEntity<TODO_SubmitPOInfo>, ITrackable, IEquatable<TODO_SubmitPOInfo>
    {
        [DataMember]
        public int Id
		{ 
		    get { return _Id; }
			set
			{
			    if (value == _Id) return;
				_Id = value;
				NotifyPropertyChanged();//m => this.Id
			}
		}
        private int _Id;

        [DataMember]
        public string Number
		{ 
		    get { return _Number; }
			set
			{
			    if (value == _Number) return;
				_Number = value;
				NotifyPropertyChanged();//m => this.Number
			}
		}
        private string _Number;

        [DataMember]
        public string Date
		{ 
		    get { return _Date; }
			set
			{
			    if (value == _Date) return;
				_Date = value;
				NotifyPropertyChanged();//m => this.Date
			}
		}
        private string _Date;

        [DataMember]
        public string SupplierInvoiceNo
		{ 
		    get { return _SupplierInvoiceNo; }
			set
			{
			    if (value == _SupplierInvoiceNo) return;
				_SupplierInvoiceNo = value;
				NotifyPropertyChanged();//m => this.SupplierInvoiceNo
			}
		}
        private string _SupplierInvoiceNo;

        [DataMember]
        public int ApplicationSettingsId
		{ 
		    get { return _ApplicationSettingsId; }
			set
			{
			    if (value == _ApplicationSettingsId) return;
				_ApplicationSettingsId = value;
				NotifyPropertyChanged();//m => this.ApplicationSettingsId
			}
		}
        private int _ApplicationSettingsId;

        [DataMember]
        public Nullable<int> EmailId
		{ 
		    get { return _EmailId; }
			set
			{
			    if (value == _EmailId) return;
				_EmailId = value;
				NotifyPropertyChanged();//m => this.EmailId
			}
		}
        private Nullable<int> _EmailId;

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
        public string CustomsProcedure
		{ 
		    get { return _CustomsProcedure; }
			set
			{
			    if (value == _CustomsProcedure) return;
				_CustomsProcedure = value;
				NotifyPropertyChanged();//m => this.CustomsProcedure
			}
		}
        private string _CustomsProcedure;

        [DataMember]
        public string DocumentType
		{ 
		    get { return _DocumentType; }
			set
			{
			    if (value == _DocumentType) return;
				_DocumentType = value;
				NotifyPropertyChanged();//m => this.DocumentType
			}
		}
        private string _DocumentType;

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
        public string Reference
		{ 
		    get { return _Reference; }
			set
			{
			    if (value == _Reference) return;
				_Reference = value;
				NotifyPropertyChanged();//m => this.Reference
			}
		}
        private string _Reference;

        [DataMember]
        public Nullable<decimal> Totals_taxes
		{ 
		    get { return _Totals_taxes; }
			set
			{
			    if (value == _Totals_taxes) return;
				_Totals_taxes = value;
				NotifyPropertyChanged();//m => this.Totals_taxes
			}
		}
        private Nullable<decimal> _Totals_taxes;

        [DataMember]
        public double Total_CIF
		{ 
		    get { return _Total_CIF; }
			set
			{
			    if (value == _Total_CIF) return;
				_Total_CIF = value;
				NotifyPropertyChanged();//m => this.Total_CIF
			}
		}
        private double _Total_CIF;

        [DataMember]
        public string WarehouseNo
		{ 
		    get { return _WarehouseNo; }
			set
			{
			    if (value == _WarehouseNo) return;
				_WarehouseNo = value;
				NotifyPropertyChanged();//m => this.WarehouseNo
			}
		}
        private string _WarehouseNo;

        [DataMember]
        public string BillingLine
		{ 
		    get { return _BillingLine; }
			set
			{
			    if (value == _BillingLine) return;
				_BillingLine = value;
				NotifyPropertyChanged();//m => this.BillingLine
			}
		}
        private string _BillingLine;

        [DataMember]
        public Nullable<bool> IsSubmitted
		{ 
		    get { return _IsSubmitted; }
			set
			{
			    if (value == _IsSubmitted) return;
				_IsSubmitted = value;
				NotifyPropertyChanged();//m => this.IsSubmitted
			}
		}
        private Nullable<bool> _IsSubmitted;

        [DataMember]
        public string PONumber
		{ 
		    get { return _PONumber; }
			set
			{
			    if (value == _PONumber) return;
				_PONumber = value;
				NotifyPropertyChanged();//m => this.PONumber
			}
		}
        private string _PONumber;

        [DataMember]
        public int ASYCUDA_Id
		{ 
		    get { return _ASYCUDA_Id; }
			set
			{
			    if (value == _ASYCUDA_Id) return;
				_ASYCUDA_Id = value;
				NotifyPropertyChanged();//m => this.ASYCUDA_Id
			}
		}
        private int _ASYCUDA_Id;

        [DataMember]
        public string Marks2_of_packages
		{ 
		    get { return _Marks2_of_packages; }
			set
			{
			    if (value == _Marks2_of_packages) return;
				_Marks2_of_packages = value;
				NotifyPropertyChanged();//m => this.Marks2_of_packages
			}
		}
        private string _Marks2_of_packages;

        [DataMember]
        public int AsycudaDocumentSetId
		{ 
		    get { return _AsycudaDocumentSetId; }
			set
			{
			    if (value == _AsycudaDocumentSetId) return;
				_AsycudaDocumentSetId = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentSetId
			}
		}
        private int _AsycudaDocumentSetId;

        [DataMember]
        public Nullable<int> EntryData_Id
		{ 
		    get { return _EntryData_Id; }
			set
			{
			    if (value == _EntryData_Id) return;
				_EntryData_Id = value;
				NotifyPropertyChanged();//m => this.EntryData_Id
			}
		}
        private Nullable<int> _EntryData_Id;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<TODO_SubmitPOInfo>.Equals(TODO_SubmitPOInfo other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



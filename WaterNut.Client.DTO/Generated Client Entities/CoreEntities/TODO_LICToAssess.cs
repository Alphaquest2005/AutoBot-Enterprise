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
    public partial class TODO_LICToAssess : BaseEntity<TODO_LICToAssess>, ITrackable, IEquatable<TODO_LICToAssess>
    {
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
        public string Country_of_origin_code
		{ 
		    get { return _Country_of_origin_code; }
			set
			{
			    if (value == _Country_of_origin_code) return;
				_Country_of_origin_code = value;
				NotifyPropertyChanged();//m => this.Country_of_origin_code
			}
		}
        private string _Country_of_origin_code;

        [DataMember]
        public string Currency_Code
		{ 
		    get { return _Currency_Code; }
			set
			{
			    if (value == _Currency_Code) return;
				_Currency_Code = value;
				NotifyPropertyChanged();//m => this.Currency_Code
			}
		}
        private string _Currency_Code;

        [DataMember]
        public string Manifest_Number
		{ 
		    get { return _Manifest_Number; }
			set
			{
			    if (value == _Manifest_Number) return;
				_Manifest_Number = value;
				NotifyPropertyChanged();//m => this.Manifest_Number
			}
		}
        private string _Manifest_Number;

        [DataMember]
        public string BLNumber
		{ 
		    get { return _BLNumber; }
			set
			{
			    if (value == _BLNumber) return;
				_BLNumber = value;
				NotifyPropertyChanged();//m => this.BLNumber
			}
		}
        private string _BLNumber;

        [DataMember]
        public string Type_of_declaration
		{ 
		    get { return _Type_of_declaration; }
			set
			{
			    if (value == _Type_of_declaration) return;
				_Type_of_declaration = value;
				NotifyPropertyChanged();//m => this.Type_of_declaration
			}
		}
        private string _Type_of_declaration;

        [DataMember]
        public string Declaration_gen_procedure_code
		{ 
		    get { return _Declaration_gen_procedure_code; }
			set
			{
			    if (value == _Declaration_gen_procedure_code) return;
				_Declaration_gen_procedure_code = value;
				NotifyPropertyChanged();//m => this.Declaration_gen_procedure_code
			}
		}
        private string _Declaration_gen_procedure_code;

        [DataMember]
        public string Declarant_Reference_Number
		{ 
		    get { return _Declarant_Reference_Number; }
			set
			{
			    if (value == _Declarant_Reference_Number) return;
				_Declarant_Reference_Number = value;
				NotifyPropertyChanged();//m => this.Declarant_Reference_Number
			}
		}
        private string _Declarant_Reference_Number;

        [DataMember]
        public Nullable<int> TotalInvoices
		{ 
		    get { return _TotalInvoices; }
			set
			{
			    if (value == _TotalInvoices) return;
				_TotalInvoices = value;
				NotifyPropertyChanged();//m => this.TotalInvoices
			}
		}
        private Nullable<int> _TotalInvoices;

        [DataMember]
        public Nullable<int> DocumentsCount
		{ 
		    get { return _DocumentsCount; }
			set
			{
			    if (value == _DocumentsCount) return;
				_DocumentsCount = value;
				NotifyPropertyChanged();//m => this.DocumentsCount
			}
		}
        private Nullable<int> _DocumentsCount;

        [DataMember]
        public Nullable<double> InvoiceTotal
		{ 
		    get { return _InvoiceTotal; }
			set
			{
			    if (value == _InvoiceTotal) return;
				_InvoiceTotal = value;
				NotifyPropertyChanged();//m => this.InvoiceTotal
			}
		}
        private Nullable<double> _InvoiceTotal;

        [DataMember]
        public Nullable<int> LicenseLines
		{ 
		    get { return _LicenseLines; }
			set
			{
			    if (value == _LicenseLines) return;
				_LicenseLines = value;
				NotifyPropertyChanged();//m => this.LicenseLines
			}
		}
        private Nullable<int> _LicenseLines;

        [DataMember]
        public Nullable<double> TotalCIF
		{ 
		    get { return _TotalCIF; }
			set
			{
			    if (value == _TotalCIF) return;
				_TotalCIF = value;
				NotifyPropertyChanged();//m => this.TotalCIF
			}
		}
        private Nullable<double> _TotalCIF;

        [DataMember]
        public Nullable<int> QtyLicensesRequired
		{ 
		    get { return _QtyLicensesRequired; }
			set
			{
			    if (value == _QtyLicensesRequired) return;
				_QtyLicensesRequired = value;
				NotifyPropertyChanged();//m => this.QtyLicensesRequired
			}
		}
        private Nullable<int> _QtyLicensesRequired;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<TODO_LICToAssess>.Equals(TODO_LICToAssess other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}




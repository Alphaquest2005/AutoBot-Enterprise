//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AsycudaDocumentSetEx
    {
        public AsycudaDocumentSetEx()
        {
            this.AsycudaDocuments = new HashSet<AsycudaDocument>();
            this.LicenceSummary = new HashSet<LicenceSummary>();
            this.AsycudaDocumentSet_Attachments = new HashSet<AsycudaDocumentSet_Attachments>();
            this.AsycudaDocumentSetEntryDataEx = new HashSet<AsycudaDocumentSetEntryDataEx>();
            this.AsycudaDocumentSetAttachments = new HashSet<AsycudaDocumentSetAttachments>();
        }
    
        public int AsycudaDocumentSetId { get; set; }
        public string Declarant_Reference_Number { get; set; }
        public Nullable<double> Exchange_Rate { get; set; }
        public Nullable<int> Customs_ProcedureId { get; set; }
        public string Country_of_origin_code { get; set; }
        public string Currency_Code { get; set; }
        public int Document_TypeId { get; set; }
        public string Description { get; set; }
        public string Manifest_Number { get; set; }
        public string BLNumber { get; set; }
        public Nullable<System.DateTime> EntryTimeStamp { get; set; }
        public Nullable<int> StartingFileCount { get; set; }
        public Nullable<int> DocumentsCount { get; set; }
        public string ApportionMethod { get; set; }
        public Nullable<double> TotalCIF { get; set; }
        public Nullable<double> TotalFreight { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public int ApplicationSettingsId { get; set; }
        public Nullable<int> TotalPackages { get; set; }
        public Nullable<int> LastFileNumber { get; set; }
        public Nullable<int> TotalInvoices { get; set; }
        public Nullable<int> ImportedInvoices { get; set; }
        public Nullable<int> ClassifiedLines { get; set; }
        public Nullable<int> TotalLines { get; set; }
        public Nullable<int> MaxLines { get; set; }
        public string LocationOfGoods { get; set; }
        public Nullable<int> LicenseLines { get; set; }
        public Nullable<double> InvoiceTotal { get; set; }
        public string FreightCurrencyCode { get; set; }
        public Nullable<int> QtyLicensesRequired { get; set; }
        public Nullable<int> EntryPackages { get; set; }
        public double CurrencyRate { get; set; }
        public double FreightCurrencyRate { get; set; }
        public Nullable<int> ExpectedEntries { get; set; }
    
        public virtual ICollection<AsycudaDocument> AsycudaDocuments { get; set; }
        public virtual ICollection<LicenceSummary> LicenceSummary { get; set; }
        public virtual ApplicationSettings ApplicationSettings { get; set; }
        public virtual ICollection<AsycudaDocumentSet_Attachments> AsycudaDocumentSet_Attachments { get; set; }
        public virtual ICollection<AsycudaDocumentSetEntryDataEx> AsycudaDocumentSetEntryDataEx { get; set; }
        public virtual ICollection<AsycudaDocumentSetAttachments> AsycudaDocumentSetAttachments { get; set; }
    }
}

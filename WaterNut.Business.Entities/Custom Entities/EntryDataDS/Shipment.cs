using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace EntryDataDS.Business.Entities
{
    
    public partial class Shipment
    {
        [IgnoreDataMember]
        [NotMapped]
        public string EmailId { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public string Body { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public List<ShipmentInvoice> Invoices { get; set; }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Body)) return Body;

            return $@"  Expected Entries: {ExpectedEntries}
Manifest: {(string.IsNullOrEmpty(ManifestNumber) ? "Manifest Not Found! BL Number must equal Manifest WayBill" : ManifestNumber)}

Consignee Code: {(string.IsNullOrEmpty(ConsigneeCode) ? "Consignee Code Not Found" : ConsigneeCode)}

Consignee Name: {(string.IsNullOrEmpty(ConsigneeName) ? "Consignee Not Found" : ConsigneeName)}

Consignee Address: {(string.IsNullOrEmpty(ConsigneeAddress) ? "Consignee Address Not Found" : ConsigneeAddress)}

BL: {(string.IsNullOrEmpty(BLNumber) ? "BL Not Found" : BLNumber)}

Freight: {(Freight.GetValueOrDefault() == 0.00 ? "Freight not Found" : Freight.Value.ToString())}

Weight(kg): {(WeightKG.GetValueOrDefault() == 0.00 ? "Manifest Or Bill of Laden not Found": WeightKG.Value.ToString())}

Currency: {Currency}

Country of Origin: {Origin}

Total Invoices: {TotalInvoices}

Packages: {Packages}

Freight Currency: {FreightCurrency}

Location of Goods: {Location}

Office: {Office}";
        }

    }
}

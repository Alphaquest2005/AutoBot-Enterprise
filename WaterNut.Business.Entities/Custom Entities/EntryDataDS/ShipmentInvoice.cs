using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace EntryDataDS.Business.Entities
{

    public partial class ShipmentInvoice
    {

        [IgnoreDataMember]
        [NotMapped]
        public double ImportedTotalDifference => Math.Abs(ImportedTotal - ImportedSubTotal);

        [IgnoreDataMember]
        [NotMapped]
        public double ImportedTotal
        {
            get
            {
                var importedTotalCost = InvoiceDetails.Sum(x => x.TotalCost.GetValueOrDefault());
                if (Math.Abs(importedTotalCost) < 0.01)
                    importedTotalCost = this.InvoiceDetails.Sum(x => x.Cost * x.Quantity);
                return importedTotalCost;

            }

        }

        [IgnoreDataMember]
        [NotMapped]
        public double ImportedSubTotal
        {
            get
            {
                var subtotal = SubTotal.GetValueOrDefault();
                if (Math.Abs(subtotal) < 0.01)
                    subtotal = InvoiceTotal.GetValueOrDefault()
                               - TotalInternalFreight.GetValueOrDefault()
                               - TotalOtherCost.GetValueOrDefault()
                               - TotalInsurance.GetValueOrDefault()
                               + TotalDeduction.GetValueOrDefault();
                return subtotal;

            }

        }
    }

    public partial class InvoiceDetails
    {

       

        [IgnoreDataMember]
        [NotMapped]
        public double ImportedTotalCost
        {
            get
            {
                return this.Cost * this.Quantity;
            }

        }

        [IgnoreDataMember]
        [NotMapped]
        public double ImportedCost
        {
            get
            {
                var cost = TotalCost ?? ImportedTotalCost;
                return cost / Quantity;

            }

        }
        [IgnoreDataMember]
        [NotMapped]
        public double ImportedQuantity
        {
            get
            {
                 return  ImportedTotalCost/ ImportedCost;
               

            }

        }
    }

}

namespace EntryDataDS.Business.Entities
{
    public partial class InvoiceDetails
    {
        [IgnoreDataMember]
        [NotMapped]
        public string Section { get; set; }
        
    }
}
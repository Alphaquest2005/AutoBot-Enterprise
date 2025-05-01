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

        [IgnoreDataMember]
        [NotMapped]
        public string PONumber { get; set; }
        
        [IgnoreDataMember]
        [NotMapped]
        public double TotalsZero
        {
            get
            {
                // Check 1: Sum of differences at the detail level
                double detailLevelDifference = this.InvoiceDetails
                    .Sum(detail => (detail.TotalCost ?? 0.0) - ((detail.Cost) * (detail.Quantity)));

                // Calculate the subtotal using the best available value per line for Check 2
                double calculatedSubTotal = this.InvoiceDetails
                    .Sum(detail => detail.TotalCost ?? ((detail.Cost) * (detail.Quantity)));

                // Check 2: Difference at the header level (Calculated Components - InvoiceTotal)
                double headerLevelDifference = (calculatedSubTotal
                                                + (this.TotalInternalFreight ?? 0)
                                                + (this.TotalOtherCost ?? 0)
                                                + (this.TotalInsurance ?? 0)
                                                - (this.TotalDeduction ?? 0))
                                               - (this.InvoiceTotal ?? 0);

                // TotalsZero is the sum of both checks
                return detailLevelDifference + headerLevelDifference;
            }
            // No setter - calculated property
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
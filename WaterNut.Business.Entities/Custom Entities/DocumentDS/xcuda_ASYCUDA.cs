
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DocumentItemDS.Business.Entities;

using EntryDataDS.Business.Entities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities;
using TrackableEntities.Client;


namespace DocumentDS.Business.Entities
{
    public partial class xcuda_ASYCUDA //: IHasEntryTimeStamp
    {
        [IgnoreDataMember]
        [NotMapped]
        public bool DoNotAllocate
        {
            get { return this.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate; }
            set { this.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = value; }
        }
        //public void RefreshItemCount()
        //{
        //    for (var i = 0; i < xcuda_Item.Count; i++)
        //    {
        //        var itm = xcuda_Item.ElementAt(i);
        //        itm.LineNumber = i + 1;
        //    }
        //}


        public void SetupProperties()
        {
            if (xcuda_Identification == null && this.ASYCUDA_Id == 0)
                xcuda_Identification = new xcuda_Identification(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Type == null && this.ASYCUDA_Id == 0)
                xcuda_Identification.xcuda_Type = new xcuda_Type(true) { TrackingState = TrackingState.Added };

            if (xcuda_ASYCUDA_ExtendedProperties == null && this.ASYCUDA_Id == 0)
                xcuda_ASYCUDA_ExtendedProperties = new xcuda_ASYCUDA_ExtendedProperties(true) { TrackingState = TrackingState.Added };

            if (xcuda_Declarant == null && this.ASYCUDA_Id == 0)
                xcuda_Declarant = new xcuda_Declarant(true) { TrackingState = TrackingState.Added };

            if (xcuda_General_information == null && this.ASYCUDA_Id == 0)
                xcuda_General_information = new xcuda_General_information(true) { TrackingState = TrackingState.Added };

            if (xcuda_General_information.xcuda_Country == null && this.ASYCUDA_Id == 0)
                xcuda_General_information.xcuda_Country = new xcuda_Country(true) { TrackingState = TrackingState.Added,
                                                                                    xcuda_Destination = new xcuda_Destination(true) {TrackingState = TrackingState.Added},
                                                                                    xcuda_Export = new xcuda_Export(true) {TrackingState = TrackingState.Added} };

            if (xcuda_Identification.xcuda_Office_segment == null && this.ASYCUDA_Id == 0)
                xcuda_Identification.xcuda_Office_segment = new xcuda_Office_segment(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Registration == null && this.ASYCUDA_Id == 0)
                xcuda_Identification.xcuda_Registration = new xcuda_Registration(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Type == null && this.ASYCUDA_Id == 0)
                xcuda_Identification.xcuda_Type = new xcuda_Type(true) { TrackingState = TrackingState.Added };

            if (xcuda_Valuation == null && this.ASYCUDA_Id == 0)
                xcuda_Valuation = new xcuda_Valuation(true) { TrackingState = TrackingState.Added };


            if (xcuda_Valuation.xcuda_Gs_Invoice == null && this.ASYCUDA_Id == 0)
            {
                xcuda_Valuation.Calculation_working_mode = "0";
                xcuda_Valuation.xcuda_Gs_Invoice = new xcuda_Gs_Invoice(true) { TrackingState = TrackingState.Added };
            }
            if (xcuda_Valuation.xcuda_Gs_other_cost == null && this.ASYCUDA_Id == 0)
            {
                
                xcuda_Valuation.xcuda_Gs_other_cost = new xcuda_Gs_other_cost(true) { TrackingState = TrackingState.Added };
            }
            if (xcuda_Valuation.xcuda_Gs_deduction == null && this.ASYCUDA_Id == 0)
            {
                
                xcuda_Valuation.xcuda_Gs_deduction = new xcuda_Gs_deduction(true) { TrackingState = TrackingState.Added };
            }
            if (xcuda_Valuation.xcuda_Gs_insurance == null && this.ASYCUDA_Id == 0)
            {
                
                xcuda_Valuation.xcuda_Gs_insurance = new xcuda_Gs_insurance(true) { TrackingState = TrackingState.Added };
            }
            if (xcuda_Valuation.xcuda_Gs_internal_freight == null && this.ASYCUDA_Id == 0)
            {

                xcuda_Valuation.xcuda_Gs_internal_freight = new xcuda_Gs_internal_freight(true) { TrackingState = TrackingState.Added };
            }

            if (xcuda_Property == null && this.ASYCUDA_Id == 0)
                xcuda_Property = new xcuda_Property(true) { TrackingState = TrackingState.Added };

            if (xcuda_Traders == null && this.ASYCUDA_Id == 0)
                xcuda_Traders = new xcuda_Traders(true)
                {
                    xcuda_Exporter = new xcuda_Exporter(true) { TrackingState = TrackingState.Added},
                    xcuda_Consignee = new xcuda_Consignee(true) { TrackingState = TrackingState.Added },
                    TrackingState = TrackingState.Added
                };

            if (AsycudaDocumentEntryDatas == null && this.ASYCUDA_Id == 0)
                AsycudaDocumentEntryDatas = new List<AsycudaDocumentEntryData>();

            if (AsycudaDocument_Attachments == null && this.ASYCUDA_Id == 0)
                AsycudaDocument_Attachments = new List<AsycudaDocument_Attachments>();


            if (xcuda_Assessment_notice == null && this.ASYCUDA_Id == 0)
                this.xcuda_Assessment_notice = new List<xcuda_Assessment_notice>();

            if (xcuda_Container == null && this.ASYCUDA_Id == 0)
                this.xcuda_Container = new List<xcuda_Container>();

            if (xcuda_Export_release == null && this.ASYCUDA_Id == 0)
                this.xcuda_Export_release = new List<xcuda_Export_release>();

            if (xcuda_Financial == null && this.ASYCUDA_Id == 0)
                this.xcuda_Financial = new List<xcuda_Financial>();

            if (xcuda_Global_taxes == null && this.ASYCUDA_Id == 0)
                this.xcuda_Global_taxes = new List<xcuda_Global_taxes>();

            if (xcuda_Suppliers_documents == null && this.ASYCUDA_Id == 0)
                this.xcuda_Suppliers_documents = new List<xcuda_Suppliers_documents>();

            if (xcuda_Transit == null && this.ASYCUDA_Id == 0)
                this.xcuda_Transit = new List<xcuda_Transit>();

            if (xcuda_Transport == null && this.ASYCUDA_Id == 0)
                this.xcuda_Transport = new List<xcuda_Transport>();

            if (xcuda_Warehouse == null && this.ASYCUDA_Id == 0)
                this.xcuda_Warehouse = new List<xcuda_Warehouse>();

            if (AsycudaDocumentEntryDatas == null && this.ASYCUDA_Id == 0)
                this.AsycudaDocumentEntryDatas = new List<AsycudaDocumentEntryData>();

            if (AsycudaDocument_Attachments == null && this.ASYCUDA_Id == 0)
                this.AsycudaDocument_Attachments = new List<AsycudaDocument_Attachments>();

        }

        //public ObservableCollection<xcuda_Item> xcuda_Item
        //{
        //    get
        //    {
        //        using (var ctx = new xcuda_ItemBusiness())
        //        {
        //            var res = ctx.Getxcuda_ItemByASYCUDA_Id(ASYCUDA_Id.ToString()).Result.Select(x => new xcuda_Item(x));
        //            if (res != null)
        //            {
        //                return new ObservableCollection<xcuda_Item>(res);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //}

        //public int EntryLineCount
        //{
        //    get
        //    {
        //        return xcuda_Item.Count;
        //    }
        //}


        [NotMapped]
        [IgnoreDataMember]
        public string ReferenceNumber
        {
            get
            {
                
                        if (xcuda_Declarant != null
                                 && xcuda_Declarant.Number != null)
                        {
                            return xcuda_Declarant.Number;
                        }
                        return "";
              
            }
            set
            {
                if (xcuda_ASYCUDA_ExtendedProperties != null)
                {
                    if (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != null && xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed == true)
                    {
                        xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber = value;
                    }
                    else
                    {
                        if (xcuda_Declarant != null
                                 && xcuda_Declarant.Number != null)
                        {
                            xcuda_Declarant.Number = value;
                        }

                    }
                }
            }
        }

         [NotMapped]
         [IgnoreDataMember]
        public string CNumber
        {
            get
            {
                if (xcuda_ASYCUDA_ExtendedProperties != null)
                {
                    if (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != null && xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed == true)
                    {
                        return xcuda_ASYCUDA_ExtendedProperties.CNumber;
                    }
                    else
                    {
                        if (xcuda_Identification != null
                                 && xcuda_Identification.xcuda_Registration != null
                                 && xcuda_Identification.xcuda_Registration.Number != null)
                        {
                            return xcuda_Identification.xcuda_Registration.Number;
                        }
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        DateTime _registrationDate = DateTime.MinValue;
         [NotMapped]
        [IgnoreDataMember]
        
        public DateTime RegistrationDate
        {
            get
            {


                if (xcuda_ASYCUDA_ExtendedProperties != null)
                {

                    if (xcuda_Identification.xcuda_Registration.Date != null)
                    {
                        _registrationDate = Convert.ToDateTime(xcuda_Identification.xcuda_Registration.Date);
                    }
                    if (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != null && xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed == true)
                    {
                        _registrationDate = Convert.ToDateTime(xcuda_ASYCUDA_ExtendedProperties.RegistrationDate);
                    }

                }
                return _registrationDate;


            }
        }
         [NotMapped]
         [IgnoreDataMember]
        public DateTime AssessmentDate
        {
            get
            {
                if (xcuda_ASYCUDA_ExtendedProperties != null
                    && (xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate != null
                             && xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate != DateTime.MinValue))
                {
                    return Convert.ToDateTime(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate);
                }
                return RegistrationDate;


            }
        }
        [NotMapped]
        [IgnoreDataMember]
        public DateTime? EffectiveRegistrationDate
        {
            get { return xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate; }
            set { xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = value; }
        }
    }
}


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
        public bool? DoNotAllocate
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


        partial void SetupProperties()
        {
            if (xcuda_Identification == null)
                xcuda_Identification = new xcuda_Identification(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Type == null)
                xcuda_Identification.xcuda_Type = new xcuda_Type(true) { TrackingState = TrackingState.Added };

            if (xcuda_ASYCUDA_ExtendedProperties == null)
                xcuda_ASYCUDA_ExtendedProperties = new xcuda_ASYCUDA_ExtendedProperties(true) { TrackingState = TrackingState.Added };

            if (xcuda_Declarant == null)
                xcuda_Declarant = new xcuda_Declarant(true) { TrackingState = TrackingState.Added };

            if (xcuda_General_information == null)
                xcuda_General_information = new xcuda_General_information(true) { TrackingState = TrackingState.Added };

            if (xcuda_General_information.xcuda_Country == null)
                xcuda_General_information.xcuda_Country = new xcuda_Country(true) { TrackingState = TrackingState.Added,
                                                                                    xcuda_Destination = new xcuda_Destination(true) {TrackingState = TrackingState.Added},
                                                                                    xcuda_Export = new xcuda_Export(true) {TrackingState = TrackingState.Added} };

            if (xcuda_Identification.xcuda_Office_segment == null)
                xcuda_Identification.xcuda_Office_segment = new xcuda_Office_segment(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Registration == null)
                xcuda_Identification.xcuda_Registration = new xcuda_Registration(true) { TrackingState = TrackingState.Added };

            if (xcuda_Identification.xcuda_Type == null)
                xcuda_Identification.xcuda_Type = new xcuda_Type(true) { TrackingState = TrackingState.Added };

            if (xcuda_Valuation == null)
                xcuda_Valuation = new xcuda_Valuation(true) { TrackingState = TrackingState.Added };


            if (xcuda_Valuation.xcuda_Gs_Invoice == null)
            {
                xcuda_Valuation.Calculation_working_mode = "0";
                xcuda_Valuation.xcuda_Gs_Invoice = new xcuda_Gs_Invoice(true) { TrackingState = TrackingState.Added };
            }

            if (xcuda_Property == null)
                xcuda_Property = new xcuda_Property(true) { TrackingState = TrackingState.Added };


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

                    if (xcuda_Identification.xcuda_Registration.Date != "")
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

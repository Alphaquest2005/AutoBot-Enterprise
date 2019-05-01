
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;


namespace WaterNut.DataLayer
{
    public partial class xcuda_ASYCUDA : IHasEntryTimeStamp
    {
        public xcuda_ASYCUDA()
        {
          //  PropertyChanged += BaseViewModel.xcuda_ASYCUDA_PropertyChanged;
           
            
        }      

        public void RefreshItemCount()
        {
            for (var i = 0; i < xcuda_Item.Count; i++)
            {
                var itm = xcuda_Item.ElementAt(i);
                itm.LineNumber = i + 1;
            }
        }


       public void SetupProperties()
        {
            if (xcuda_Identification == null)
                xcuda_Identification = new xcuda_Identification();

            if (xcuda_Identification.xcuda_Type == null)
                xcuda_Identification.xcuda_Type = new xcuda_Type();

           if(xcuda_ASYCUDA_ExtendedProperties == null)
               xcuda_ASYCUDA_ExtendedProperties = new xcuda_ASYCUDA_ExtendedProperties();

           if (xcuda_Declarant == null)
               xcuda_Declarant = new xcuda_Declarant();

           if (xcuda_General_information == null)
               xcuda_General_information = new xcuda_General_information();

           if (xcuda_General_information.xcuda_Country == null)
               xcuda_General_information.xcuda_Country = new xcuda_Country();

           if (xcuda_Identification.xcuda_Office_segment == null)
               xcuda_Identification.xcuda_Office_segment = new xcuda_Office_segment();

           if (xcuda_Identification.xcuda_Registration == null)
               xcuda_Identification.xcuda_Registration = new xcuda_Registration();

           if (xcuda_Identification.xcuda_Type == null)
               xcuda_Identification.xcuda_Type = new xcuda_Type();
           
           if (xcuda_Valuation == null)
               xcuda_Valuation = new xcuda_Valuation();

      

           if (xcuda_Valuation.xcuda_Gs_Invoice == null)
           {
               xcuda_Valuation.Calculation_working_mode = "1";
               xcuda_Valuation.xcuda_Gs_Invoice = new xcuda_Gs_Invoice();
           }

           if (xcuda_Property == null)
               xcuda_Property = new xcuda_Property();

           
        }

       public int EntryLineCount
       {
           get
           {
               return xcuda_Item.Count;
           }
       }

       

       public string ReferenceNumber
       {
           get
           {
               if (xcuda_ASYCUDA_ExtendedProperties != null)
               {
                   if (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != null && xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed == true)
                   {
                       return xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber;
                   }
                   else
                   {
                       if (xcuda_Declarant != null
                                && xcuda_Declarant.Number != null)                               
                       {
                           return xcuda_Declarant.Number;
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
       public DateTime RegistrationDate
       {
           get

           {
              
                   
                   if (xcuda_ASYCUDA_ExtendedProperties != null)
                   {
                       
                       if (xcuda_Identification.xcuda_Registration.Date != "")
                       {
                           _registrationDate =  Convert.ToDateTime(xcuda_Identification.xcuda_Registration.Date);
                       }
                       if (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != null && xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed == true)
                       {
                           _registrationDate = Convert.ToDateTime(xcuda_ASYCUDA_ExtendedProperties.RegistrationDate);
                       }
                       
                   }
                   return _registrationDate;
               
             
           }
       }

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

    }
}

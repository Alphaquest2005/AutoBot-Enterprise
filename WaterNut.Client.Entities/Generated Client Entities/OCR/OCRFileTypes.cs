﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TrackableEntities.Client;
using Core.Common.Client.Entities;
using OCR.Client.DTO;


using Core.Common.Validation;

namespace OCR.Client.Entities
{
       public partial class OCRFileTypes: BaseEntity<OCRFileTypes>
    {
        DTO.OCRFileTypes ocrfiletypes;
        public OCRFileTypes(DTO.OCRFileTypes dto )
        {
              ocrfiletypes = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.OCRFileTypes>(ocrfiletypes);

        }

        public DTO.OCRFileTypes DTO
        {
            get
            {
             return ocrfiletypes;
            }
            set
            {
                ocrfiletypes = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "OCRInvoice is required")]
       
public int OCRInvoiceId
		{ 
		    get { return this.ocrfiletypes.OCRInvoiceId; }
			set
			{
			    if (value == this.ocrfiletypes.OCRInvoiceId) return;
				this.ocrfiletypes.OCRInvoiceId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("OCRInvoiceId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "FileType is required")]
       
public int FileTypeId
		{ 
		    get { return this.ocrfiletypes.FileTypeId; }
			set
			{
			    if (value == this.ocrfiletypes.FileTypeId) return;
				this.ocrfiletypes.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       private Invoices _Invoices;
        public  Invoices Invoices
		{
		    get
               { 
                  if (this.ocrfiletypes != null)
                   {
                       if (_Invoices != null)
                       {
                           if (this.ocrfiletypes.Invoices !=
                               _Invoices.DTO)
                           {
                                if (this.ocrfiletypes.Invoices  != null)
                               _Invoices = new Invoices(this.ocrfiletypes.Invoices);
                           }
                       }
                       else
                       {
                             if (this.ocrfiletypes.Invoices  != null)
                           _Invoices = new Invoices(this.ocrfiletypes.Invoices);
                       }
                   }


             //       if (_Invoices != null) return _Invoices;
                       
             //       var i = new Invoices(){TrackingState = TrackingState.Added};
			//		//if (this.ocrfiletypes.Invoices == null) Debugger.Break();
			//		if (this.ocrfiletypes.Invoices != null)
            //        {
            //           i. = this.ocrfiletypes.Invoices;
            //        }
            //        else
            //        {
            //            this.ocrfiletypes.Invoices = i.;
             //       }
                           
            //        _Invoices = i;
                     
                    return _Invoices;
               }
			set
			{
			    if (value == _Invoices) return;
                _Invoices = value;
                if(value != null)
                     this.ocrfiletypes.Invoices = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("Invoices");
			}
		}
        


        ChangeTrackingCollection<DTO.OCRFileTypes> _changeTracker;    
        public ChangeTrackingCollection<DTO.OCRFileTypes> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

        public new TrackableEntities.TrackingState TrackingState
        {
            get
            {
                return this.DTO.TrackingState;
            }
            set
            {
                this.DTO.TrackingState = value;
                NotifyPropertyChanged("TrackingState");
            }
        }

    }
}


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
       public partial class Part: BaseEntity<Part>
    {
        DTO.Part part;
        public Part(DTO.Part dto )
        {
              part = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.Part>(part);

        }

        public DTO.Part DTO
        {
            get
            {
             return part;
            }
            set
            {
                part = value;
            }
        }
       
       
                
                [MaxLength(50, ErrorMessage = "Name has a max length of 50 letters ")]
public string Name
		{ 
		    get { return this.part.Name; }
			set
			{
			    if (value == this.part.Name) return;
				this.part.Name = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Name");
			}
		}
     

       
       
public Nullable<bool> StartMultiLine
		{ 
		    get { return this.part.StartMultiLine; }
			set
			{
			    if (value == this.part.StartMultiLine) return;
				this.part.StartMultiLine = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("StartMultiLine");
			}
		}
     

       
       
                
                
public string Start
		{ 
		    get { return this.part.Start; }
			set
			{
			    if (value == this.part.Start) return;
				this.part.Start = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Start");
			}
		}
     

       
       
public Nullable<bool> EndMultiLine
		{ 
		    get { return this.part.EndMultiLine; }
			set
			{
			    if (value == this.part.EndMultiLine) return;
				this.part.EndMultiLine = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EndMultiLine");
			}
		}
     

       
       
                
                
public string End
		{ 
		    get { return this.part.End; }
			set
			{
			    if (value == this.part.End) return;
				this.part.End = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("End");
			}
		}
     

       
       
public Nullable<bool> IsRecuring
		{ 
		    get { return this.part.IsRecuring; }
			set
			{
			    if (value == this.part.IsRecuring) return;
				this.part.IsRecuring = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("IsRecuring");
			}
		}
     

       
       
public Nullable<bool> IsComposite
		{ 
		    get { return this.part.IsComposite; }
			set
			{
			    if (value == this.part.IsComposite) return;
				this.part.IsComposite = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("IsComposite");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.part.Id; }
			set
			{
			    if (value == this.part.Id) return;
				this.part.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       
       
public Nullable<int> StartRegExId
		{ 
		    get { return this.part.StartRegExId; }
			set
			{
			    if (value == this.part.StartRegExId) return;
				this.part.StartRegExId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("StartRegExId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Invoice is required")]
       
public int InvoiceId
		{ 
		    get { return this.part.InvoiceId; }
			set
			{
			    if (value == this.part.InvoiceId) return;
				this.part.InvoiceId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceName is required")]
       
                
                
public string InvoiceName
		{ 
		    get { return this.part.InvoiceName; }
			set
			{
			    if (value == this.part.InvoiceName) return;
				this.part.InvoiceName = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceName");
			}
		}
     

       private Invoice _Invoice;
        public  Invoice Invoice
		{
		    get
               { 
                  if (this.part != null)
                   {
                       if (_Invoice != null)
                       {
                           if (this.part.Invoice !=
                               _Invoice.DTO)
                           {
                                if (this.part.Invoice  != null)
                               _Invoice = new Invoice(this.part.Invoice);
                           }
                       }
                       else
                       {
                             if (this.part.Invoice  != null)
                           _Invoice = new Invoice(this.part.Invoice);
                       }
                   }


             //       if (_Invoice != null) return _Invoice;
                       
             //       var i = new Invoice(){TrackingState = TrackingState.Added};
			//		//if (this.part.Invoice == null) Debugger.Break();
			//		if (this.part.Invoice != null)
            //        {
            //           i. = this.part.Invoice;
            //        }
            //        else
            //        {
            //            this.part.Invoice = i.;
             //       }
                           
            //        _Invoice = i;
                     
                    return _Invoice;
               }
			set
			{
			    if (value == _Invoice) return;
                _Invoice = value;
                if(value != null)
                     this.part.Invoice = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("Invoice");
			}
		}
        


        ChangeTrackingCollection<DTO.Part> _changeTracker;    
        public ChangeTrackingCollection<DTO.Part> ChangeTracker
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


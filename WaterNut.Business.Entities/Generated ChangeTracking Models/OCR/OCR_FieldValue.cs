﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using TrackableEntities.Client;


		namespace OCR.Business.Entities
{
    public partial class OCR_FieldValue
    {
       
         partial void TrackableStartUp()
         {
           // _changeTracker = new ChangeTrackingCollection<OCR_FieldValue>(this);
         }

        ChangeTrackingCollection<OCR_FieldValue> _changeTracker;

        [NotMapped]
        [IgnoreDataMember]
        public new ChangeTrackingCollection<OCR_FieldValue> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

         public new void StartTracking()
        {
            _changeTracker = new ChangeTrackingCollection<OCR_FieldValue>(this);
        }
   
    }
}
		
﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using TrackableEntities.Client;


		namespace CoreEntities.Business.Entities
{
    public partial class AsycudaDocumentBasicInfo
    {
       
         partial void TrackableStartUp()
         {
           // _changeTracker = new ChangeTrackingCollection<AsycudaDocumentBasicInfo>(this);
         }

        ChangeTrackingCollection<AsycudaDocumentBasicInfo> _changeTracker;

        [NotMapped]
        [IgnoreDataMember]
        public new ChangeTrackingCollection<AsycudaDocumentBasicInfo> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

         public new void StartTracking()
        {
            _changeTracker = new ChangeTrackingCollection<AsycudaDocumentBasicInfo>(this);
        }
   
    }
}
		
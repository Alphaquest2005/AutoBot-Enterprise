﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using TrackableEntities.Client;


		namespace AllocationDS.Business.Entities
{
    public partial class AsycudaSalesAllocations_XcudaItemsToAllocate
    {
       
         partial void TrackableStartUp()
         {
           // _changeTracker = new ChangeTrackingCollection<AsycudaSalesAllocations_XcudaItemsToAllocate>(this);
         }

        ChangeTrackingCollection<AsycudaSalesAllocations_XcudaItemsToAllocate> _changeTracker;

        [NotMapped]
        [IgnoreDataMember]
        public new ChangeTrackingCollection<AsycudaSalesAllocations_XcudaItemsToAllocate> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

         public new void StartTracking()
        {
            _changeTracker = new ChangeTrackingCollection<AsycudaSalesAllocations_XcudaItemsToAllocate>(this);
        }
   
    }
}
		
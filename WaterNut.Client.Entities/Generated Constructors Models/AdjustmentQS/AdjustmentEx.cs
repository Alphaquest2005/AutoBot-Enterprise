﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using AdjustmentQS.Client.DTO;
using TrackableEntities.Client;



namespace AdjustmentQS.Client.Entities
{

    public partial class AdjustmentEx 
    {
       public AdjustmentEx()
        {
            this.DTO = new DTO.AdjustmentEx();//{TrackingState = TrackableEntities.TrackingState.Added}
            _changeTracker = new ChangeTrackingCollection<DTO.AdjustmentEx>(this.DTO);

            CustomClassStartUp();
            MyNavPropStartUp();
            IIdentifiableEntityStartUp();
            AutoGenStartUp();
        }
    partial void CustomClassStartUp();
    partial void AutoGenStartUp();
    partial void MyNavPropStartUp();
    partial void IIdentifiableEntityStartUp();
   
    }
}
		
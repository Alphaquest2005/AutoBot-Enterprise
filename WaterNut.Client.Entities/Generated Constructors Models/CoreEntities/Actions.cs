﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using CoreEntities.Client.DTO;
using TrackableEntities.Client;



namespace CoreEntities.Client.Entities
{

    public partial class Actions 
    {
       public Actions()
        {
            this.DTO = new DTO.Actions();//{TrackingState = TrackableEntities.TrackingState.Added}
            _changeTracker = new ChangeTrackingCollection<DTO.Actions>(this.DTO);

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
		
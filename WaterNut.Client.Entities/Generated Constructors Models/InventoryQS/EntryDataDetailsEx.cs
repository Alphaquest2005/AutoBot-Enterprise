﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using InventoryQS.Client.DTO;
using TrackableEntities.Client;



namespace InventoryQS.Client.Entities
{

    public partial class EntryDataDetailsEx 
    {
       public EntryDataDetailsEx()
        {
            this.DTO = new DTO.EntryDataDetailsEx();//{TrackingState = TrackableEntities.TrackingState.Added}
            _changeTracker = new ChangeTrackingCollection<DTO.EntryDataDetailsEx>(this.DTO);

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
		
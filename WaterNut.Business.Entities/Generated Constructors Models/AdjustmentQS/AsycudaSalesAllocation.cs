﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

		namespace AdjustmentQS.Business.Entities
{
    public partial class AsycudaSalesAllocation
    {
       public AsycudaSalesAllocation()
        {
            CustomClassStartUp();
            MyNavPropStartUp();
            //SetupProperties();
            IIdentifiableEntityStartUp();
            AutoGenStartUp();  
            TrackableStartUp();
        }

      public AsycudaSalesAllocation(bool start) : this()
       {
          if(start) StartTracking();
       }
    // partial void SetupProperties();
    partial void AutoGenStartUp();
    partial void CustomClassStartUp();
    partial void MyNavPropStartUp();
    partial void IIdentifiableEntityStartUp();
    partial void TrackableStartUp();
   
    }
}
		
﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

		namespace CounterPointQS.Business.Entities
{
    public partial class CounterPointPODetails
    {
       public CounterPointPODetails()
        {
            CustomClassStartUp();
            MyNavPropStartUp();
            //SetupProperties();
            IIdentifiableEntityStartUp();
            AutoGenStartUp();  
            TrackableStartUp();
        }

      public CounterPointPODetails(bool start) : this()
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
		
﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

		namespace DocumentItemDS.Business.Entities
{
    public partial class xcuda_Tarification
    {
       public xcuda_Tarification()
        {
            CustomClassStartUp();
            MyNavPropStartUp();
            //SetupProperties();
            IIdentifiableEntityStartUp();
            AutoGenStartUp();  
            TrackableStartUp();
        }

      public xcuda_Tarification(bool start) : this()
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
		
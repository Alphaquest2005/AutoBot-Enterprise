﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using CoreEntities;
using CoreEntities.Client.Entities;
//using WaterNut.Client.Services;
using CoreEntities.Client.Services;
using System.Linq;

namespace CoreEntities.Client.Entities
{
    public partial class TODO_C71ToCreate
    {
        
            partial void MyNavPropStartUp()
            {

              PropertyChanged += UpdateMyNavProp;

            }


      
       #region MyNavProp Entities
      
      

        void UpdateMyNavProp(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if (e.PropertyName == "ApplicationSettingsId")
            {
                UpdateApplicationSettings();
            }
        }

        private void UpdateApplicationSettings()
        {
            using (var ctx = new ApplicationSettingsClient())
            {
                var dto = ctx.GetApplicationSettings().Result.FirstOrDefault(x => x.ApplicationSettingsId == this.ApplicationSettingsId);
                if(dto != null)ApplicationSettings = new ApplicationSettings(dto);
            }
        }        

        ApplicationSettings _applicationSettings = null;

        public ApplicationSettings ApplicationSettings
        {
            get
            {
                if(_applicationSettings != null) return _applicationSettings;
                UpdateApplicationSettings();
                return _applicationSettings;
            }
            set
            {
                if (value != null)
                {
                    _applicationSettings = value;

                    ApplicationSettingsId = _applicationSettings.ApplicationSettingsId;

                    NotifyPropertyChanged("ApplicationSettings");
                }
            }

        }
        

         #endregion
 
    }
   
}
		
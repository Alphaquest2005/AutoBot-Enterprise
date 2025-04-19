using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Core.Common.UI;
using SimpleMvvmToolkit;


namespace WaterNut
{
	public class MainWindowModel : ViewModelBase<MainWindowModel>
	{
		public MainWindowModel()
		{
           
		}

       
       public void CloseWindow ()
        {
            Application.Current.Shutdown();
        }
        

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}
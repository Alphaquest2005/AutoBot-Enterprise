using System;
using System.ComponentModel;
using System.Windows;
using SimpleMvvmToolkit;

namespace RegexImporter
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
		public new event PropertyChangedEventHandler PropertyChanged;

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
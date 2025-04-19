using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Windows.Threading;

namespace InsightPresentationLayer
{
	public class StatusModel : INotifyPropertyChanged
	{
	   
		public StatusModel()
		{
			timer.Elapsed += timer_Elapsed;
			StaticPropertyChanged += StatusModel_StaticPropertyChanged;
		   
		}

		void StatusModel_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}
		
	   
		private static double _maxVal = 0;
		public double MaxVal 
		{ 
			get { return _maxVal; }
			set 
			{
				_maxVal = value;
				
			}
		}

		private static double _val;
		public double Val
		{
			get { return _val; }
			set
			{
				_val = value;
			   
			}
		}

		private static string _operation;

		private static string _statusTxt = "test";
		public string StatusTxt 
		{ 
			get { return _statusTxt; }
			set
			{
				_statusTxt = value;
				
			}
		}

		private static System.Windows.Visibility _statusVisibility = Visibility.Hidden;
		public System.Windows.Visibility StatusVisibility
		{
			get { return _statusVisibility; }
			set
			{
				_statusVisibility = value;
			   
			}
		}

		

		public static void StartStatusUpdate(string operation, double max, double value = 1)
		{
			_val = 0;
			_maxVal = max;
			_operation = operation;
			_statusTxt = string.Format("{0} | {1} of {2}", operation, value, max);
			_statusVisibility = Visibility.Visible;
		  // timer.Enabled = true;
			OnStaticPropertyChanged("MaxVal");
			OnStaticPropertyChanged("StatusTxt");
			OnStaticPropertyChanged("Val");
			OnStaticPropertyChanged("StatusVisibility");
            Refresh();
		}

        public static void Refresh()
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
	   

		public static void StatusUpdate( string operation = "" ,double value = 0)
		{
		   _val = value == 0 ? _val += 1 : value;
			_operation = operation == ""? _operation: operation;

			_statusTxt = string.Format("{0} | {1} of {2}", _operation, _val, _maxVal);
			if(_maxVal == _val) StopStatusUpdate();
			OnStaticPropertyChanged("MaxVal");
			OnStaticPropertyChanged("StatusTxt");
			OnStaticPropertyChanged("Val");
			OnStaticPropertyChanged("StatusVisibility");
            Refresh();
		}

		public static void StopStatusUpdate()
		{
			_val = 0;
			_maxVal = 0;
			_operation = "";
			_statusTxt = "";
			_statusVisibility = Visibility.Hidden;
			if (timer.Enabled == true) timer.Enabled = false;
			OnStaticPropertyChanged("MaxVal");
			OnStaticPropertyChanged("StatusTxt");
			OnStaticPropertyChanged("Val");
			OnStaticPropertyChanged("StatusVisibility");
            Refresh();
		}

		

		private static DateTime _startTime ;
		private static System.Timers.Timer timer = new System.Timers.Timer(1000);
		public static void Timer(string operation)
		{
			_val = 0;
			_maxVal = 10;
			_operation = operation;
			_statusTxt = "";
			_statusVisibility = Visibility.Visible;
			_startTime = DateTime.Now;
			if (timer.Enabled == true) timer.Enabled = false;
			timer.Enabled = true;
			OnStaticPropertyChanged("MaxVal");
			OnStaticPropertyChanged("StatusTxt");
			OnStaticPropertyChanged("Val");
			OnStaticPropertyChanged("StatusVisibility");
            Refresh();
		}

		static void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
		   _val = _val == _maxVal ? 0 : _val += 1;
			_statusTxt = string.Format("{0} | {1}", _operation, (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss"));
			OnStaticPropertyChanged("StatusTxt");
			OnStaticPropertyChanged("Val");
		}

		
		public static event PropertyChangedEventHandler StaticPropertyChanged;

		public static void OnStaticPropertyChanged(String info)
		{
			if (StaticPropertyChanged != null)
			{
				StaticPropertyChanged(null, new PropertyChangedEventArgs(info));
			}
		}

		public event
			PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				Application.Current.Dispatcher.InvokeAsync(
					() => this.PropertyChanged(this, new PropertyChangedEventArgs(info)), DispatcherPriority.Normal);
				//this.PropertyChanged(this, new PropertyChangedEventArgs(info));
				//OnStaticPropertyChanged(info);
			}
		}
	}
}
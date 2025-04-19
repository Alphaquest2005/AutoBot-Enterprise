using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using SimpleMvvmToolkit;
using Timer = System.Timers.Timer;

namespace Core.Common.UI
{
    public class StatusModel : ViewModelBase<StatusModel>
    {
        public const string MaxValChanged = "MaxValChanged";
        public const string ErrorTxtChanged = "ErrorTxtChanged";
        public const string StatusTxtChanged = "StatusTxtChanged";
        public const string StatusValueChanged = "StatusValueChanged";
        public const string StatusVisibilityChanged = "StatusVisibilityChanged";

        private static double _maxVal;

        private static double _statusValue;


        private static string _operation;

        private static string _statusTxt = "";

        private static Visibility _statusVisibility = Visibility.Hidden;

        private static string _errorTxt = "test";


        private static DateTime _startTime;
        private static readonly Timer timer = new Timer(1000);

        static StatusModel()
        {
            Instance = new StatusModel();
        }


        public StatusModel()
        {
            timer.Elapsed += timer_Elapsed;
            RegisterToReceiveMessages(MaxValChanged, OnMaxValChanged);
            RegisterToReceiveMessages(StatusTxtChanged, OnStatusTxtChanged);
            RegisterToReceiveMessages(ErrorTxtChanged, OnErrorTxtChanged);
            RegisterToReceiveMessages(StatusValueChanged, OnStatusValueChanged);
            RegisterToReceiveMessages(StatusVisibilityChanged, OnStatusVisibilityChanged);
        }

        public static StatusModel Instance { get; }

        public double MaxVal
        {
            get => _maxVal;
            set
            {
                _maxVal = value;
                NotifyPropertyChanged(x => MaxVal);
            }
        }

        public double StatusValue
        {
            get => _statusValue;
            set
            {
                _statusValue = value;
                NotifyPropertyChanged(x => StatusValue);
            }
        }

        public string Operation
        {
            get => _operation;
            set
            {
                _operation = value;
                NotifyPropertyChanged(x => Operation);
            }
        }

        public string StatusTxt
        {
            get => _statusTxt;
            set
            {
                _statusTxt = value;
                NotifyPropertyChanged(x => StatusTxt);
            }
        }

        public Visibility StatusVisibility
        {
            get => _statusVisibility;
            set
            {
                _statusVisibility = value;
                NotifyPropertyChanged(x => StatusVisibility);
            }
        }

        public string ErrorTxt
        {
            get => _errorTxt;
            set
            {
                _errorTxt = value;
                NotifyPropertyChanged(x => ErrorTxt);
            }
        }

        private void OnErrorTxtChanged(object sender, NotificationEventArgs e)
        {
            NotifyPropertyChanged(x => ErrorTxt);
        }

        private void OnStatusVisibilityChanged(object sender, NotificationEventArgs e)
        {
            NotifyPropertyChanged(x => StatusVisibility);
        }

        private void OnStatusValueChanged(object sender, NotificationEventArgs e)
        {
            NotifyPropertyChanged(x => StatusValue);
        }

        private void OnStatusTxtChanged(object sender, NotificationEventArgs e)
        {
            NotifyPropertyChanged(x => StatusTxt);
        }

        private void OnMaxValChanged(object sender, NotificationEventArgs e)
        {
            NotifyPropertyChanged(x => MaxVal);
        }


        public static void StartStatusUpdate(string operation, double max, double value = 1)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    new Action(() =>
                    {
                        _statusValue = 0;
                        _maxVal = max;
                        _operation = operation;
                        _statusTxt = $"{operation} | {value} of {max}";
                        _statusVisibility = Visibility.Visible;
                        // timer.Enabled = true;
                        MessageBus.Default.BeginNotify(MaxValChanged, null, new NotificationEventArgs(MaxValChanged));
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusValueChanged, null,
                            new NotificationEventArgs(StatusValueChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));

                        Refresh();
                    }));
        }

        public static void Refresh()
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(delegate { }));
        }

        public static void RefreshNow()
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(delegate { }));
        }


        public static void StatusUpdate(string operation = "", double value = 0)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    new Action(() =>
                    {
                        Instance.StatusValue = value == 0 ? _statusValue += 1 : value;
                        Instance.Operation = operation == "" ? _operation : operation;
                        if (Instance.StatusVisibility == Visibility.Hidden)
                            Instance.StatusVisibility = Visibility.Visible;
                        Instance.StatusTxt = $"{_operation} | {_statusValue} of {_maxVal}";
                        if (Instance.MaxVal == Instance.StatusValue) StopStatusUpdate();
                        Debug.WriteLine(operation);
                        MessageBus.Default.BeginNotify(MaxValChanged, null, new NotificationEventArgs(MaxValChanged));
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusValueChanged, null,
                            new NotificationEventArgs(StatusValueChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));
                        Refresh();
                    }));
        }

        public static void Message(string message)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (_statusVisibility == Visibility.Hidden) _statusVisibility = Visibility.Visible;
                        _statusTxt = $"{message}";
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));
                        Refresh();
                    }));
        }

        public static void Error(string message)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (_statusVisibility == Visibility.Hidden) _statusVisibility = Visibility.Visible;
                        _errorTxt = $"{message}";
                        MessageBus.Default.BeginNotify(ErrorTxtChanged, null,
                            new NotificationEventArgs(ErrorTxtChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));
                        Refresh();
                    }));
        }

        public static void StopStatusUpdate()
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    new Action(() =>
                    {
                        _statusValue = 0;
                        _maxVal = 0;
                        _operation = "";
                        _statusTxt = "";
                        _statusVisibility = Visibility.Hidden;
                        if (timer.Enabled) timer.Enabled = false;
                        MessageBus.Default.BeginNotify(MaxValChanged, null, new NotificationEventArgs(MaxValChanged));
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusValueChanged, null,
                            new NotificationEventArgs(StatusValueChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));
                        Refresh();
                    }));
        }

        public static void Timer(string operation)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    new Action(() =>
                    {
                        _statusValue = 0;
                        _maxVal = 10;
                        _operation = operation;
                        _statusTxt = operation; //"";
                        _statusVisibility = Visibility.Visible;
                        _startTime = DateTime.Now;
                        if (timer.Enabled) timer.Enabled = false;
                        timer.Enabled = true;
                        MessageBus.Default.BeginNotify(MaxValChanged, null, new NotificationEventArgs(MaxValChanged));
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusValueChanged, null,
                            new NotificationEventArgs(StatusValueChanged));
                        MessageBus.Default.BeginNotify(StatusVisibilityChanged, null,
                            new NotificationEventArgs(StatusVisibilityChanged));
                        Refresh();
                    }));
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        _statusValue = _statusValue == _maxVal ? 0 : _statusValue += 1;
                        _statusTxt = $"{_operation} | {(DateTime.Now - _startTime).ToString(@"hh\:mm\:ss")}";
                        MessageBus.Default.BeginNotify(StatusTxtChanged, null,
                            new NotificationEventArgs(StatusTxtChanged));
                        MessageBus.Default.BeginNotify(StatusValueChanged, null,
                            new NotificationEventArgs(StatusValueChanged));
                        Refresh();
                    }));
        }
    }
}
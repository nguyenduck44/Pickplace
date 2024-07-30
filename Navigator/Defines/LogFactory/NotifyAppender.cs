using System;
using log4net.Appender;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using log4net.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace NavigatorMachine.Defines
{
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged
    {
        #region Members and events
        public static int MaxUILogRow { get; set; } = 200;
        private readonly object UILogObject = new object();

        private static ObservableCollection<string> _notification = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public ObservableCollection<string> Notification
        {
            get => _notification;
            set
            {
                if (_notification == value) return;

                _notification = value;
                OnPropertyChanged(nameof(Notification));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NotifyAppender Appender => UILog.Appender;

        public void AddLog(string message)
        {
            Application.Current.Dispatcher.Invoke(() => Notification.Add(message));
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(writer, loggingEvent);
            try
            {
                if (Application.Current == null) return;

                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    lock (UILogObject)
                    {
                        if (Notification.Count >= MaxUILogRow)
                        {
                            Notification.RemoveAt(0);
                        }
                        Notification.Add(writer.ToString());
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

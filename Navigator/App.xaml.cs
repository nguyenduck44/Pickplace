using NavigatorMachine.Defines;
using NavigatorMachine.MVVM.ViewModels;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace NavigatorMachine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        protected override void OnStartup(StartupEventArgs e)
        {
            PreventMultipleApplicationLaunch();

            CDef.MainWindowView = new MainWindow();
            CDef.MainWindowVM = new MainViewModel();

            CDef.MainWindowView.DataContext = CDef.MainWindowVM;


            CDef.MainWindowView.Show();
            base.OnStartup(e);
        }

        private void PreventMultipleApplicationLaunch()
        {
            bool createdNew;
            // Do not change the GUID inside Mutex
            _mutex = new Mutex(true, "B702D6FF-E359-4035-8E38-02C741DACF3E", out createdNew);

            if (!createdNew)
            {
                // Bring other instance to front and exit.
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }

                MessageBox.Show("Application Already Launch!!");

                Application.Current.Shutdown();
            }
        }
    }
}

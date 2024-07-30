using NavigatorMachine.Defines;
using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace NavigatorMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process _externalAppProcess;
        private const string SettingsFilePath = "settings.txt";
        public string ExeFilePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            DataContext = this; // Ensure DataContext is set to this MainWindow
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartExternalApp();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn đóng ứng dụng không?", "Xác nhận", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
            {

                e.Cancel = true; // Ngăn chặn đóng ứng dụng nếu người dùng chọn Cancel.
                SaveSettings();
                CloseExternalApp();
            }
            else
            {
                CDef.MainWindowVM.ExitButtonCommand.Execute(null);

            }
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                //Filter = "Executable Files (*.exe)|*.exe"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ExeFilePath = openFileDialog.FileName;
                //txtOpenApps.Text = ExeFilePath;
                SaveSettings();
                RestartExternalApp();
            }
        }

        private void StartExternalApp()
        {
            //if (File.Exists(ExeFilePath))
            //{
            //    _externalAppProcess = Process.Start(ExeFilePath);
            //    if (_externalAppProcess != null)
            //    {
            //        _externalAppProcess.EnableRaisingEvents = true;
            //        _externalAppProcess.Exited += ExternalAppProcess_Exited;
            //    }
            //}
        }

        private void RestartExternalApp()
        {
            CloseExternalApp();
            StartExternalApp();
        }

        private void ExternalAppProcess_Exited(object sender, EventArgs e)
        {
            _externalAppProcess = null;
        }

        private void CloseExternalApp()
        {
            if (_externalAppProcess != null && !_externalAppProcess.HasExited)
            {
                _externalAppProcess.Kill();
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                ExeFilePath = File.ReadAllText(SettingsFilePath);
                //txtOpenApps.Text = ExeFilePath;
            }
        }

        private void SaveSettings()
        {
            File.WriteAllText(SettingsFilePath, ExeFilePath);
        }
    }
}

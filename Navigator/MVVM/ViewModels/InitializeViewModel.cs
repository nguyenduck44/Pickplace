using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using System;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;

namespace NavigatorMachine.MVVM.ViewModels
{
    public delegate void EventHandler();
    public delegate void ChangeRecipeNameEventHandler(string recipeName);
    public class InitializeViewModel : ViewModelBase
    {
        public ChangeRecipeNameEventHandler CurrentRecipeChanged;

        #region Properties

        public string InitString
        {
            get { return _initString; }
            set
            {
                _initString = value;
                OnPropertyChanged();
            }
        }
        private bool IsInitialize;
        #endregion

        #region Constructor (s)
        public InitializeViewModel()
        {
            IsInitialize = true;
            _initTimer = new System.Timers.Timer();
            _initTimer.Interval = 100;
            _initTimer.Elapsed += InitTimer_Elapsed;
            _initTimer.AutoReset = true;
            _initTimer.Start();
        }


        #endregion

        #region Methods
        public void Terminate()
        {
            step = 0;
            IsInitialize = false;
            _initTimer.Enabled = true;
        }

        public void Initialize()
        {
            step = 1;
            IsInitialize = true;
            _initTimer.Enabled = true;
        }

        private void InitTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _initTimer.Enabled = false;
            try
            {
                switch (step)
                {
                    case 0:
                        if (IsInitialize)
                        {
                            Debug($"Initialize Vision Inspect - {CDef.PGM_version}........");
                            CDef.IO.Output_TowerLampGreen = false;
                            CDef.IO.Output_TowerLampRed = false;
                            CDef.IO.Output_Buzzer = false;
                        }
                        else
                        {
                            CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.InitializeVM;
                            Debug("Terminate........");
                        }

                        step++;
                        break;
                    case 1:
                        if (IsInitialize)
                        {
                            Debug("Initialize Recipe..........");

                            if (InitRecipe())
                            {
                                Debug("Initialize Recipe Success!");
                                CurrentRecipeChanged?.Invoke(CRecipeFolder.CurrentRecipe.Name);
                            }

                            Debug("Initialize Tray..........");
                            _trayVM.InitCurrentTray();
                            _trayVM.InitLastTray();
                        }
                        else
                        {

                        }


                        step++;
                        break;
                    case 2:
                        if (IsInitialize)
                        {
#if !Simulation
                            Debug("Initialize IO Board..........");
                            CDef.IO.Connect();
#endif

                            Debug("Initialize Work Data..........");
                            CDef.WorkData = new CWorkData();

                            Debug("Initialize Takt Time..........");
                            CDef.TaktTime = new CTaktTime();
                        }
                        else
                        {

                        }

                        step++;
                        break;
                    case 3:
                        if (IsInitialize)
                        {
                            Debug($"Open folder Data: {CRecipeFolder.DataFolder}");
                            Debug($"Open folder Recipe: {CRecipeFolder.RecipeFolder}");
                            Debug($"Open folder Log: {CRecipeFolder.LOGFolder}");
                            Debug($"Open folder Picture: {CRecipeFolder.PictureFolder}");

                        }
                        else
                        {
                            
                        }

                        step++;
                        break;
                    case 4:
                        if (IsInitialize)
                        {
                            Debug("Initialize Robot Process............");
                            CDef.RobotProcess.Start();
                            
                            Sleep(10);
                        }
                        else
                        {
                            Debug("Kill Process");

                            CDef.RobotProcess.Stop();
                        }

                        step++;
                        break;
                    case 5:
                        if (IsInitialize)
                        {
                            CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.AutoVM;
                        }
                        else
                        {

                        }

                        step++;
                        break;
                    case 6:
                        if (IsInitialize)
                        {
                            Debug("Initialize Done!");
                        }
                        else
                        {
                            Debug("Terminate Done!");
                        }

                        _initTimer.AutoReset = false;
                        _initTimer.Stop();
                        step++;
                        break;
                    default:
                        _initTimer.Enabled = false;
                        if (!IsInitialize)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Application.Current.Shutdown();
                            });
                        }
                        return;
                }
            }
            catch(Exception ex)
            {
                LOG.Warn($"Initialize Fail, Step = {step}, \n Exception: {ex.ToString()}");
                step++;
            }

            _initTimer.Enabled = true;
        }

        public bool InitRecipe()
        {
            _commonRecipe = new CCommonRecipe().Load<CCommonRecipe>();

            if (_commonRecipe == null)
            {
                _commonRecipe = new CCommonRecipe();

                _commonRecipe.Save();
            }

            _trayRecipe = new CTrayRecipe().Load<CTrayRecipe>();

            if (_trayRecipe == null)
            {
                _trayRecipe = new CTrayRecipe();

                _trayRecipe.Save();
            }

            _trayRecipe.Initialized = true;
            _commonRecipe.Initialized = true;

            return true;
        }

        void Debug(string debug)
        {
            InitString = debug;
            LOG.Debug(debug);
        }

        void Warn(string warning)
        {
            InitString = warning;
            LOG.Warn(warning);
        }

        private void Sleep(int ms) => Thread.Sleep(ms);
#endregion

        #region Privates
        private string _initString;
        private CCommonRecipe _commonRecipe
        {
            get => CDef.CommonRecipe;
            set
            {
                CDef.CommonRecipe = value;
            }
        }
        private CTrayRecipe _trayRecipe
        { 
            get => CDef.TrayRecipe;
            set
            {
                CDef.TrayRecipe = value;
            }
        }

        private System.Timers.Timer _initTimer;

        private int step = 0;

        private TrayViewModel _trayVM => CDef.MainWindowVM.AutoVM.TrayVM;
        #endregion
    }
}

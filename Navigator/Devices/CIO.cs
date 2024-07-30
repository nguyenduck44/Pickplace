using NavigatorMachine.Defines;
using NavigatorMachine.Devices.PlusE;
using NavigatorMachine.Models;
using System;
using System.Net;
using System.Threading;

namespace NavigatorMachine.Devices
{
    public class CIO : ObservableObject
    {
        #region Properties
        IOBoardPlusE IOBoard {  get; set; }
        public bool InputTrigerFromRobot
        {
            get
            {
                return IOBoard.Input[0];
            }
        }

        public bool InputVisionChecking
        {
            get
            {
                return IOBoard.Input[1]; 
            }
        }

        public bool InputVisionInspectOK
        {
            get
            {
                return IOBoard.Input[2];
            }
        }

        public bool Input_SWControl_Robot
        {
            get
            {
                return IOBoard.Input[3];
            }
        }

        public bool Input_SWReset_Robot
        {
            get
            {
                return IOBoard.Input[4];
            }
        }

        public bool Input_No_Production
        {
            get
            {
                return IOBoard.Input[6];
            }
        }

        public bool OutputTriggerVisionInspect
        {
            get { return IOBoard.Output[0]; }
            set
            {
                IOBoard.Output[0] = value;
                OnPropertyChanged();
            }
        }

        public bool Output_SWControlRobot
        {
            get { return IOBoard.Output[1]; }
            set
            {
                IOBoard.Output[1] = value;
                OnPropertyChanged();
            }
        }

        public bool Output_TowerLampRed
        {
            get { return IOBoard.Output[2]; }
            set
            {
                IOBoard.Output[2] = value;
                OnPropertyChanged();
            }
        }

        public bool Output_TowerLampGreen
        {
            get { return IOBoard.Output[3]; }
            set
            {
                IOBoard.Output[3] = value;
                OnPropertyChanged();
            }
        }

        public bool Output_Buzzer
        {
            get { return IOBoard.Output[4]; }
            set
            {
                IOBoard.Output[4] = value;
                OnPropertyChanged();
            }
        }


        public int Index = 0;
        #endregion

        #region Constructor
        public CIO(int index)
        {
            IOBoard = new IOBoardPlusE("IO Plus E", index);

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                OnPropertyChanged(nameof(InputTrigerFromRobot));
                OnPropertyChanged(nameof(InputVisionChecking));
                OnPropertyChanged(nameof(InputVisionInspectOK));
                OnPropertyChanged(nameof(Input_SWControl_Robot));
                OnPropertyChanged(nameof(Input_SWReset_Robot));
                OnPropertyChanged(nameof(Input_No_Production));

                OnPropertyChanged(nameof(OutputTriggerVisionInspect));
                OnPropertyChanged(nameof(Output_SWControlRobot));

                OnPropertyChanged(nameof(Output_TowerLampGreen));
                OnPropertyChanged(nameof(Output_TowerLampRed));

                OnPropertyChanged(nameof(Output_Buzzer));
            };
            timer.Start();
        }
        #endregion

        #region Methods
        public bool Connect()
        {
            string ip = "192.168.0.33";
            
            IPAddress iPAddress = IPAddress.Parse(ip);
         
            if (FASTECH.EziMOTIONPlusELib.FAS_Connect(iPAddress, 0) != true)
            {
                UILog.Error($"IO Board connect error");
                return false;
            }
            else
            {
                UILog.Debug($"IO Board connect success");
                return true;
            }
        }

        public void Press_SWControlRobot()
        {
            Output_SWControlRobot = true;
            Thread.Sleep(500);
            Output_SWControlRobot = false;
        }
        #endregion

        #region Privates
        private bool _InputTrigerFromRobot;
        private bool _outputFromRobot;
        #endregion
    }
}

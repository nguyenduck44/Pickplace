using log4net;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using NavigatorMachine.Processing.Enums;
using System;
using System.Threading;

namespace TopEquipment.Domain.Process
{
    public class CProcessBase : ObservableObject
    {
        #region Properties

        public string Name { get; set; }

        public bool IsAlive { get; private set; }

        public int Interval { get; }

        public CProcessTimer ProcessTimer { get; set; }

        public CProcessStep Step { get; set; }

        public EMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                IsToRunDone = false;

                Step.ToRunStep = 0;
                Step.RunStep = 0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ModeToString));
                OnPropertyChanged(nameof(IsRunningReverse));
            }
        }

        public string ModeToString => Mode.ToString();
        public bool IsRunningReverse => Mode == EMode.Run || Mode == EMode.ToRun ? false : true;

        /// <summary>
        /// Log(er) of the process
        /// </summary>
        public ILog LOG { get; set; }
        private EMode _mode = EMode.Stop;
        public string strWarning;
        public bool IsToRunDone;
        #endregion

        #region Constructors
        public CProcessBase(string processName)
        {
            LogFactory.Configure();
            LOG = LogManager.GetLogger(processName);

            Interval = 10;
            Step = new CProcessStep();

            ProcessTimer = new CProcessTimer();
            Step.StepChangedHandler = delegate { ProcessTimer.StepStartTime = ProcessTimer.Now; };

            Name = processName;
            thread = new Thread(new ThreadStart(OnThreadStart));
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            if (thread.ThreadState != ThreadState.Unstarted)
            {
                thread.Join();
            }
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            IsAlive = true;
            thread.Start();
        }

        public void Stop()
        {
            IsAlive = false;

            Dispose();
        }

        public virtual EPRtnCode PreProcess()
        {
            return EPRtnCode.RtnOk;
        }

        public virtual EPRtnCode ProcessRun()
        {
            return EPRtnCode.RtnOk;
        }
        public virtual EPRtnCode ProcessPause()
        {
            return EPRtnCode.RtnOk;
        }
        public virtual EPRtnCode ProcessStop()
        {
            return EPRtnCode.RtnOk;
        }

        public virtual EPRtnCode ProcessToPause()
        {
            Mode = EMode.Pause;
            return EPRtnCode.RtnOk;
        }
        public virtual EPRtnCode ProcessToRun()
        {
            Step.RunStep = 0;

            IsToRunDone = true;
            return EPRtnCode.RtnOk;
        }
        public virtual EPRtnCode ProcessToStop()
        {
            Mode = EMode.Stop;
            return EPRtnCode.RtnOk;
        }

        public void Sleep(int millisecond)
        {
            Thread.Sleep(millisecond);
        }
        #endregion

        #region Privates
        private readonly Thread thread;
        #endregion

        #region Private methods
        private void OnThreadStart()
        {
            while (IsAlive)
            {
                try
                {
                    PreProcess();

                    switch (Mode)
                    {
                        case EMode.ToRun:
                            ProcessToRun();
                            break;
                        case EMode.Run:
                            ProcessRun();
                            break;
                        case EMode.ToPause:
                            ProcessToPause();
                            break;
                        case EMode.Pause:
                            ProcessPause();
                            break;
                        case EMode.ToStop:
                            ProcessToStop();
                            break;
                        case EMode.Stop:
                            ProcessStop();
                            break;
                        
                        default:
                            break;
                    }

                    Sleep(Interval);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        #endregion
    }
}

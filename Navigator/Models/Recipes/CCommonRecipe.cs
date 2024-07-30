using NavigatorMachine.Defines;
using NavigatorMachine.MVVM.ViewModels;

namespace NavigatorMachine.Models
{
    public enum ELaserMode
    {
        OnePoint,
        OnePointRow,
        AllPoint
        
    }
    public class CCommonRecipe : CRecipeBase
    {
        #region Properties
        public EventHandler ChangePoinsLaserEvent;
        public double DelayTime_VisionInspect
        {
            get { return _delayTime; }
            set
            {
                if (_delayTime != value)
                {
                    _delayTime = value;
                    OnPropertyChanged(nameof(DelayTime_VisionInspect));

                    Save();
                }
            }
        }

        public double TimeOut_ReceiveSignalInput
        {
            get { return _timeOut; }
            set
            {
                if (_timeOut != value)
                {

                    _timeOut = value;
                    OnPropertyChanged(nameof(TimeOut_ReceiveSignalInput));

                    Save();
                }
            }
        }


        public bool StopMachineIfVisionInspectNg
        {
            get { return _stopMachineIfVisionInspectNg; }
            set
            {
                if (!_stopMachineIfVisionInspectNg)
                {
                    _stopMachineIfVisionInspectNg = value;
                    OnPropertyChanged(nameof(StopMachineIfVisionInspectNg));

                    Save();
                }
                
            }
        }


        public bool UseExportFileData
        {
            get { return _useExportFileData; }
            set
            {
                _useExportFileData = value;
                OnPropertyChanged();

                Save();
            }
        }

        public int TriggerInCycleCount
        {
            get { return _triggerInCycleCount; }
            set
            {
                if (value < 0)
                {
                    _triggerInCycleCount = value;
                    OnPropertyChanged(nameof(TriggerInCycleCount));

                    Save();
                }
                
            }
        }

        public string FileDataPath => CRecipeFolder.DataFolder;
        #endregion

        #region Constructor
        #endregion

        #region Privates
        private double _delayTime;
        private double _timeOut;
        private bool _stopMachineIfVisionInspectNg;

        private int _triggerInCycleCount = 1;
        private bool _useExportFileData;
        #endregion
    }
}

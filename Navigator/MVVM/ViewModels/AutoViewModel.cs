using NavigatorMachine.Commands;
using NavigatorMachine.Defines;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class AutoViewModel : ViewModelBase
    {
        private WorkDataViewModel _workDataVM;
        public WorkDataViewModel WorkDataVM => _workDataVM ?? (_workDataVM = new WorkDataViewModel());

        private TaktTimeViewModel _taktTimeVM;
        public TaktTimeViewModel TaktTimeVM => _taktTimeVM ?? (_taktTimeVM = new TaktTimeViewModel());

        private TrayViewModel _trayVM;
        public TrayViewModel TrayVM => _trayVM ?? (_trayVM = new TrayViewModel());

        private VisionViewModel _visionVM;
        public VisionViewModel VisionVM => _visionVM ?? (_visionVM = new VisionViewModel());

        private PictureViewModel _pictureVM;
        public PictureViewModel PictureVM => _pictureVM ?? (_pictureVM = new PictureViewModel());

        public AutoViewModel()
        {
        }

        #region Commands
        public RelayCommand ResetCountDataButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    UILog.Debug("Reset Count Data Button Click!");
                    CDef.WorkData.Clear();
                    CDef.TaktTime.Clear();
                });
            }
        }
        #endregion
    }
}

using NavigatorMachine.Commands;
using NavigatorMachine.Defines;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class TaktTimeViewModel : ViewModelBase
    {
        public TaktTimeViewModel()
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

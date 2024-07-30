using NavigatorMachine.Commands;
using NavigatorMachine.Defines;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class NavigatorViewModel
    {
        public RelayCommand UpdateCurrentViewModelCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch (o)
                    {
                        case "Auto":
                            CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.AutoVM;
                            break;
                        case "Recipe":
                            CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.RecipeVM;
                            break;
                        case "IO":
                            CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.ManualVM;
                            break;
                        //case "Manual":
                        //    CDef.MainWindowVM.CurrentVM = CDef.MainWindowVM.ManualVM;
                        //    break;
                    }
                });
            }
        }
    }
}

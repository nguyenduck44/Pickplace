using NavigatorMachine.Commands;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using System.Diagnostics;
using System.Windows;
namespace NavigatorMachine.MVVM.ViewModels
{
    public class RecipeViewModel : ViewModelBase
    {
        public CTrayRecipe _trayRecipe => CDef.TrayRecipe;
        public CCommonRecipe _commonRecipe => CDef.CommonRecipe;

        public RecipeChangeViewModel _recipeChangeVM;
        public RecipeChangeViewModel RecipeChangeVM => _recipeChangeVM ?? (_recipeChangeVM = new RecipeChangeViewModel());
        public TrayViewModel _trayViewModel => CDef.MainWindowVM.AutoVM.TrayVM;

        public RecipeViewModel()
        {
            
            _trayRecipe.ChangeColumnRowEvent += () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _trayViewModel.InitCurrentTray();
                    _trayViewModel.InitLastTray();
                });
            };
        }

        #region Commands
        public RelayCommand OpenPathButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Process.Start(new ProcessStartInfo { FileName = _commonRecipe.FileDataPath, UseShellExecute = true });
                });
            }
        }

        public RelayCommand SelecteTrayDirectionButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    switch (o)
                    {
                        case "Normal":
                            CDef.TrayRecipe.Direction = ETrayDirection.Normal;
                            break;
                        case "Zigzag":
                            CDef.TrayRecipe.Direction = ETrayDirection.ZigZag;
                            break;
                    }

                    _trayViewModel.InitCurrentTray();
                    _trayViewModel.InitLastTray();
                });
            }
        }
        #endregion

        #region Privates
        #endregion
    }
}

using NavigatorMachine.Commands;
using NavigatorMachine.Defines;
using NavigatorMachine.Processing.Enums;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        private InitializeViewModel _initializeVM;
        public InitializeViewModel InitializeVM => _initializeVM ?? (_initializeVM = new InitializeViewModel());

        private AutoViewModel _autoVM;
        public AutoViewModel AutoVM =>_autoVM ?? (_autoVM = new AutoViewModel());

        private RecipeViewModel _recipeVM;
        public RecipeViewModel RecipeVM =>_recipeVM ?? (_recipeVM = new RecipeViewModel());

        private ManualViewModel _manualVM;
        public ManualViewModel ManualVM =>_manualVM ?? (_manualVM = new ManualViewModel());

        //private ManualViewModel _IOVM;
        //public ManualViewModel IOVM => _IOVM ?? (_IOVM = new IOViewModel());

        private NavigatorViewModel _navigatorVM;
        public NavigatorViewModel NavigatorVM => _navigatorVM ?? (_navigatorVM = new NavigatorViewModel());

        private object _currentVM;
        public object CurrentVM
        {
            get { return _currentVM; }
            set 
            { 
                _currentVM = value;
                OnPropertyChanged(nameof(CurrentVM));
            }
        }


        public string CurrentRecipe
        {
            get { return _currentRecipe; }
            set
            {
                _currentRecipe = value;
                OnPropertyChanged();
            }
        }

        private string _controlSW_Content = "START";

        public string ControlSW_Content
        {
            get { return _controlSW_Content; }
            set 
            {
                _controlSW_Content = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor (s)
        public MainViewModel()
        {
            CurrentVM = InitializeVM;

            InitializeVM.CurrentRecipeChanged = (recipeName) =>
            {
                CurrentRecipe = recipeName;
            };
        }
        #endregion

        #region Commands
        public RelayCommand ExitButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    _initializeVM.Terminate();
                });
            }
        }
        
        public RelayCommand StartButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (CDef.RobotProcess.Mode == EMode.Run
                    || CDef.RobotProcess.Mode == EMode.ToRun)
                    {
                        CDef.RobotProcess.Mode = EMode.ToPause;
                    }
                    else
                    {
                        CDef.RobotProcess.Mode = EMode.ToRun;
                    }

                    CDef.IO.Press_SWControlRobot();
                });
            }
        }
        #endregion

        #region Privates
        private string _currentRecipe;
        #endregion
    }
}

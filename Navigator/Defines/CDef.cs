using NavigatorMachine.Devices;
using NavigatorMachine.Models;
using NavigatorMachine.MVVM.ViewModels;
using NavigatorMachine.Processing;

namespace NavigatorMachine.Defines
{
    public static class CDef
    {
        public static MainWindow MainWindowView;
        public static MainViewModel MainWindowVM;
        public static string PGM_version = "ver 1.0.0.5";
        public static CTrayRecipe TrayRecipe { get; set; }
        public static CCommonRecipe CommonRecipe { get; set; }
        public static CProcessRobot RobotProcess { get; set; } = new CProcessRobot("Robot Process");
        public static CWorkData WorkData { get; set; }
        public static CTaktTime TaktTime { get; set; }
        public static CIO IO { get; set; } = new CIO(0);
    }
}

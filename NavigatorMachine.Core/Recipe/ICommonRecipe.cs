using NavigatorMachine.Core;

namespace NavigatorMachine.Core
{
    public interface ICommonRecipe : IRecipe
    {
        int COMPort { get; set; }
        double DelayTime { get; set; }
        string FileDataPath { get; }
        double LaserMax { get; set; }
        double LaserMin { get; set; }
        int LaserPointCount { get; set; }
        bool UseExportFileData { get; set; }
    }
}
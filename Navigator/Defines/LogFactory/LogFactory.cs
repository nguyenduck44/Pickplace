using log4net.Config;
using System;
using System.IO;

namespace NavigatorMachine.Defines
{
    public static class LogFactory
    {
        public const string ConfigFile = "log4net.config";

        public static void Configure(string configFileName = ConfigFile)
        {
            Type type = typeof(LogFactory);
            FileInfo configFile = new FileInfo(configFileName);
            XmlConfigurator.ConfigureAndWatch(configFile);
        }
    }
}

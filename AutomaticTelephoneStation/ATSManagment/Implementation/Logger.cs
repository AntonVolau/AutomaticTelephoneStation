using System;
using System.Globalization;
using System.Configuration;
using System.IO;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    internal class Logger
    {
        public static string LogFilesPath = ConfigurationManager.AppSettings["LogFilesPath"];

        internal static void WriteLine(string message)
        {
            using var streamWriter = new StreamWriter(LogFilesPath, true);
            streamWriter.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture) + ":",-21} {message}");
        }
    }
}

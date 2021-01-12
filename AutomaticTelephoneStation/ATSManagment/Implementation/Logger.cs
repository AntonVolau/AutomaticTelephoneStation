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
            try
            {
                streamWriter.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture) + ":",-21} {message}"); // Write new line in log with added time
            }
            catch (Exception)
            {
                throw new Exception(message);
            }
        }
    }
}

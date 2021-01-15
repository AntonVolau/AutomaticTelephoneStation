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
            using var streamWriter = new StreamWriter(LogFilesPath, true); // true represents boolean type that selects wether new streamWriter will rewrite (false) or append (true) new line in log
            try
            {
                streamWriter.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture) + ":",-21} {message}"); // Write new line in log and save time when this line was added
            }
            catch (Exception e)
            {
                throw new Exception("Exception occured when trying to add line to log", e);
            }
            streamWriter?.Dispose();
        }
    }
}

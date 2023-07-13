using System;
using System.IO;

namespace WirelessSensorNetworkSimpleSecond
{
    public class Logger
    {
        private string logFilePath;
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            string logEntry = $"{DateTime.Now} - {message}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

    }
}

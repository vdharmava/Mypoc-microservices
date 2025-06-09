using System;

namespace SharedLibrary.Utilities
{
    public static class InsecureLogger
    {
        // Vulnerability: Logging sensitive data to console
        public static void LogSensitiveData(string data)
        {
            Console.WriteLine($"Sensitive Data: {data}"); // Detectable by SAST
        }
    }
}
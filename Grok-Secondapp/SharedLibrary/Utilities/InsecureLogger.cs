using System;

namespace SharedLibrary.Utilities
{
    public static class InsecureLogger
    {
        // Vulnerability: Logging sensitive data
        public static void LogSensitiveData(string data)
        {
            Console.WriteLine($"Sensitive Data: {data}"); // SAST-detectable
        }
    }
}
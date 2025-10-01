using System;

namespace AMTools.Services
{
    public class LoggerService
    {
        public void Info(string message) => Console.WriteLine($"[INFO] {message}");
        public void Warn(string message) => Console.WriteLine($"[WARN] {message}");
        public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
    }
}

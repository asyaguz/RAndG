using System;
using System.Diagnostics;
using System.IO;
namespace RAndG.Services
{
    public static class PerformanceMonitor
    {
        private static readonly object Sync = new();
        private static readonly string LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RAndG");
        private static readonly string LogPath = Path.Combine(LogDirectory, "performance.log");
        public static T Measure<T>(string operation, Func<T> action, int limitMilliseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                return action();
            }
            finally
            {
                stopwatch.Stop();
                WriteResult(operation, stopwatch.ElapsedMilliseconds, limitMilliseconds);
            }
        }
        public static void Measure(string operation, Action action, int limitMilliseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                WriteResult(operation, stopwatch.ElapsedMilliseconds, limitMilliseconds);
            }
        }
        public static void WriteResult(string operation, long elapsedMilliseconds, int limitMilliseconds)
        {
            string result = elapsedMilliseconds <= limitMilliseconds ? "PASS" : "FAIL";
            string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {operation} | {elapsedMilliseconds} ms | Limit: {limitMilliseconds} ms | {result}";
            Debug.WriteLine(message);
            try
            {
                lock (Sync)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(LogPath, message + Environment.NewLine);
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("Не удалось записать журнал производительности.");
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("Нет доступа к журналу производительности.");
            }
        }
    }
}
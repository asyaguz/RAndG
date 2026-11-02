using System;
using System.Diagnostics;

namespace RAndG.Services
{
    // Класс для измерения времени выполнения операций приложения
    public class PerformanceMonitor
    {
        private Stopwatch watch;
        public PerformanceMonitor()
        {
            watch = new Stopwatch();
        }
        public void StartMeasure()
        {
            watch.Reset();
            watch.Start();
        }
        // Завершает измерение и выводит время выполнения операции
        public void EndMeasure(string operationName)
        {
            watch.Stop();
            long elapsedMilliseconds = watch.ElapsedMilliseconds;
            Console.WriteLine("Операция '" + operationName + "' заняла " + elapsedMilliseconds + " мс.");
        }
    }
}

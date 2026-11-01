using System;
using System.Diagnostics;

namespace RAndG.Services
{
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
        // Вывод времени в миллисекундах подтверждает соблюдение нефункциональных требований по задержкам
        public void EndMeasure(string operationName)
        {
            watch.Stop();
            long elapsedMilliseconds = watch.ElapsedMilliseconds;
            Console.WriteLine("Операция '" + operationName + "' заняла " + elapsedMilliseconds + " мс.");
        }
    }
}

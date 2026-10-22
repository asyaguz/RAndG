using RAndG.Data;
using RAndG.Services;
using System;
using System.Diagnostics;
using System.Windows.Forms;
namespace RAndG
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Stopwatch startupTimer = Stopwatch.StartNew();
            ApplicationConfiguration.Initialize();
            try
            {
                Database.Initialize();
                MainForm mainForm = new MainForm();
                bool startupRecorded = false;
                mainForm.Shown += (_, _) =>
                {
                    if (startupRecorded)
                        return;
                    startupRecorded = true;
                    startupTimer.Stop();
                    PerformanceMonitor.WriteResult("ApplicationStartup", startupTimer.ElapsedMilliseconds, 5000);
                };
                Application.Run(mainForm);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка запуска приложения: " + exception.Message, "RAndG", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
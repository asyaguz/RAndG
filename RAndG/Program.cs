using System;
using System.Windows.Forms;

namespace RAndG
{
    // Содержит точку входа в приложение
    static class Program
    {
        [STAThread]
        // Настраивает параметры Windows Forms и запускает главную форму приложения
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
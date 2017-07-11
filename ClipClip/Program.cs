using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipClip
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mainForm = new MainForm();
            Application.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebAppShipping
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "WebAppShipping");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Try to acquire the Mutex. If it fails, it means an instance is already running.
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("Another instance of this application is already running.",
                                "Instance Already Running",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            // Release the Mutex when the application exits.
            mutex.ReleaseMutex();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}

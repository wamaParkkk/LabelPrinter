using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrinter
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (IsExistProcess(Process.GetCurrentProcess().ProcessName))
            {
                MessageBox.Show("Program is already running", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }            
        }

        static bool IsExistProcess(string processName)
        {
            Process[] process = Process.GetProcesses();
            
            int cnt = 0;            
            foreach (var p in process)
            {
                if (p.ProcessName == processName)
                    cnt++;

                if (cnt > 1)
                    return true;
            }
            return false;
        }
    }
}

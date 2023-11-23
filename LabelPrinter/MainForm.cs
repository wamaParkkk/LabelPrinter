using System;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Diagnostics;

namespace LabelPrinter
{
    public partial class MainForm : Form
    {
        RunWeb runWeb;
        private static Timer aTimer = new Timer();

        public MainForm()
        {
            InitializeComponent();                      

            CreateThread();

            aTimer = new Timer(500);
            aTimer.Elapsed += timerFunction;
            //aTimer.Enabled = true;            
        }        

        private void MainForm_Load(object sender, EventArgs e)
        {
            Width = 185;
            Height = 72;
            Left = 1687;
            Top = 958;
        }

        private void CreateThread()
        {
            runWeb = new RunWeb();            
        }
        
        private void timerFunction(object sender, System.Timers.ElapsedEventArgs e)
        {
            //
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            runWeb = new RunWeb();
        }        

        private void btnExit_Click(object sender, EventArgs e)
        {
            runWeb.Dispose();
            Dispose(); 
            
            Application.ExitThread();
            ProcessFindAndKill();
            Environment.Exit(0);            
        }

        private void ProcessFindAndKill()
        {
            foreach(Process process in Process.GetProcesses()) 
            {
                if (process.ProcessName.StartsWith("chromedriver"))
                {
                    process.Kill();
                }
            }
        }
    }
}

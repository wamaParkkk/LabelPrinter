using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LabelPrinter
{
    class RunWeb
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        Thread thread;
        bool bChkUrl;
        bool bChkDataParsing;

        string strReturnDate;
        string strName;
        string strSpm;
        string strSid;
        string strQty;
        string strEqSerialNo;
        string strReturnStatus = "Ready for Scrap";
        string strStorageLocation;

        public RunWeb()
        {                 
            thread = new Thread(new ThreadStart(Execute));                        
            thread.Start();            
        }

        public void Dispose()
        {
            thread.Abort();
        }

        private void Execute()
        {            
            try
            {
                bChkUrl = false;
                bChkDataParsing = false;

                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");

                ChromeDriver _driver = new ChromeDriver(driverService, options);

                INavigation navigation = _driver.Navigate();
                navigation.GoToUrl("https://atknet.amkor.co.kr/#/departments/");

                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(d => (d as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"));
                
                while (true)
                {                    
                    string sUrl = _driver.Url;

                    //Task.Delay(500);
                    Thread.Sleep(500);

                    if (sUrl.Contains("view/SPM"))
                    {
                        bChkUrl = true;                        
                    }                        
                    else
                    {
                        bChkUrl = false;
                        bChkDataParsing = false;
                    }                        

                    if ((bChkUrl) && (!bChkDataParsing))
                    {
                        // Return date, Name
                        var varReturnDate = _driver.FindElementByXPath("//*[@id=\'root\']/div/div/main/div[2]/div/form/div/div[2]/div[1]/span/span[2]/span[2]");
                        string strWebData = varReturnDate.Text;                        
                        string[] words = strWebData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        strName = words[0];
                        strReturnDate = words[1].Substring(0, 10);

                        // SPM#
                        var varSPM = _driver.FindElementByXPath("//*[@id='root']/div/div/main/div[2]/div/form/div/div[1]/span");
                        strSpm = varSPM.Text;

                        // SID
                        var varSID = _driver.FindElementByXPath("//*[@id=\'root\']/div/div/main/div[2]/div/form/div/div[3]/div[1]/div[4]");
                        strSid = varSID.Text;

                        // Qty
                        var varQty = _driver.FindElementByXPath("//*[@id=\'root\']/div/div/main/div[2]/div/form/div/div[3]/table/tbody/tr/td[7]");
                        strQty = varQty.Text;

                        // EQ Serial no
                        var varEqSN = _driver.FindElementByXPath("//*[@id=\'root\']/div/div/main/div[2]/div/form/div/div[3]/div[3]/div[4]");
                        strEqSerialNo = varEqSN.Text;

                        // Storage Location
                        var varStorageLocation = _driver.FindElementByXPath("//*[@id=\'root\']/div/div/main/div[2]/div/form/div/div[3]/table/tbody/tr/td[2]");
                        strStorageLocation = varStorageLocation.Text;


                        // Ini file write
                        _Write_Ini_File(strReturnDate, strName, strSpm, strSid, strQty, strEqSerialNo, strStorageLocation);

                        bChkDataParsing = true;
                    }                                                            
                }

                _driver.Quit();
            }
            catch (Exception e)
            {                                
                //MessageBox.Show(e.Message);
                Dispose();
                return;
            }            
        }      
        
        private void _Write_Ini_File(string sDate, string sName, string sSpm, string sSid, string sQty, string sEqSerialNo, string sStorageLocation)
        {
            string sFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\"));
            WritePrivateProfileString("ReturnDate", "ReturnDate", sDate, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("Name", "Name", sName, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("Spm", "Spm", sSpm, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("Sid", "Sid", sSid, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("Qty", "Qty", sQty, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("EqSerialNo", "EqSerialNo", sEqSerialNo, string.Format("{0}{1}", sFilePath, "Items.ini"));
            WritePrivateProfileString("StorageLocation", "StorageLocation", sStorageLocation, string.Format("{0}{1}", sFilePath, "Items.ini"));
        }
    }
}

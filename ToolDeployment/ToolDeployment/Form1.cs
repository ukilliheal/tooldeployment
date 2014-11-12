using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Management;
using System.Diagnostics;


/*The MIT License (MIT)

Copyright (c) 2014 Ukilliheal

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 * 
 * 
 * 
 * 
 * Its a mess, yes. I am still in the process of learning C sharp . 
 * I am working on adding more comments to better explain the code. Bare with me here.
 * More comments on the way!
 * */
namespace ToolDeployment
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //Unmanaged function from user32.dll -  geekpedia.com/tutorial210_Retrieving-the-Operating-System-Idle-Time-Uptime-and-Last-Input-Time.html
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        // Struct we'll need to pass to the function
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        BackgroundWorker myBGWorker = new BackgroundWorker();

        #region Vars being defined //Edit to add new Tools (Buttons, Progress bars, etc)
        //This is where I list each tool, and define the file name. 
        int idlethreashhold = 15; //In minutes
        string configfile = "config.ini";
        string password = "cake";
        string BTNDefaulttxt = "File missing";

        string ccleanervar = "CCleaner.exe";
        string jrtvar = "JRT.exe";
        string adwvar = "AdwCleaner.exe";
        string mbamvar = "Malwarebytes.exe";
        string hitmanx64var = "HitmanPro_x64.exe";
        string hitmanx32var = "HitmanPro.exe";
        string nintievar = "Ninite.exe";
        string mbaevar = "MBAMAnti-Exploit.exe";
        string uncheckvar = "Unchecky.exe";
        string abpievar = "ABPie.exe";
        string AVG2014var = "AVG2015.exe";
        string callingcardvar = "CallingCard.msi";
        string killemallvar = "KillEmAll.scr";
        string rkillvar = "RKill.exe";
        string autorunsvar = "Autoruns.exe";
        string tdssvar = "TDSSKiller.exe";
        string superaintivar = "SUPERAntiSpyware.exe";
        string tweakingtoolsvar = "TweakRepair.exe";
        string avgremovalvar = "AVGRemoval.exe";
        string sfcvar = "SFCFix.exe";
        string revelationsvar = "Revelation.exe";
        string nortonremovalvar = "Norton_Removal_Tool.exe";
        string mcafferemovalvar = "McAffe_Removal_Tool.exe";
        string roguekiller64var = "RogueKillerX64.exe";
        string roguekiller32var = "RogueKiller.exe";
        string Esetvar = "EsetOnlineScanner.exe";
        string produkeyvar = "ProduKey.exe";
        string hjtvar = "HijackThis.exe";
        string pcdecrapvar = "PDdecrapifier.exe";
        string teamvar = "TeamViewerQS.exe";
        string revovar = "RevoUninstaller.exe";
        string chromevar = "ChromeInstaller.exe";
        string classicsrtvar = "ClassicStartInstaller.exe";
        string readervar = "ReaderInstaller.exe";
        string libraofficevar = "LibreOfficeInstaller.exe";

        //I use the cf6 notes to help me keep track of the computer repair. 
        string CF6NotesDefaultText = "Msconfig:? | Appwiz:? | Ccleaner:? ";

        string applicationpath = Application.StartupPath;
        string localremotetools = "";
        string baseurl = "http://www.example.com/"; //If your file was located at www.example.com/file.exe then you want this: http://www.example.com/
        string secondurl = ""; //not being used right now. TODO: make function to check if server is up, switch to new server if down. 

        string savetoo = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string oldsaveto = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string systemrootdrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

        Boolean is64bit = false;
        Boolean bypasslogin = false;
        Boolean isdownloadingstuff = false;
        Boolean backgroundworkeriscancled = false;

        Color BTNDefaultForeColor = Color.Black;
        Color BTNDefaultBackColor = Color.LightSteelBlue;
        Color CHKBXDefaultForeColor = Color.Black;

        Random random = new Random();
        private const uint WM_COMMAND = 0x0111;
        private const int BN_CLICKED = 245;
        private const int IDOK = 1;

        //I like lists. They are fun. 
        List<string> filesavablibleonline = new List<string> { };
        List<string> filestoobedeleted = new List<string> { };
        List<string> activedownloads = new List<string> { };
        List<string> automationlist = new List<string> { };
        List<string> standardtools = new List<string> { };
        List<string> advancedtools = new List<string> { };
        List<string> reporteddone = new List<string> { };
        List<string> waslaunched = new List<string> { };
        List<string> wascancled = new List<string> { };
        List<string> toolnames = new List<string> { };
        List<string> ischecked = new List<string> { };
        List<string> md5hash = new List<string> { };
        List<string> names = new List<string> {
            "Tool Deployment", 
            "Tool Downloader", 
            "Downloader of Tools", 
            "Tools", 
            "Tool Launcher", 
            "Cleaning Utilities", 
            "Mr Shine",  
            "Optimizing Tools",
            "Dr. Download",
            "Remote Tools",
            "Toolbox",
            "OmniTool",
            "Virus Removal Tool",
            "Malware Removal Tool",
            "Spyware Killer",
            "BFG",
            "Technician Tools",
            "Tool Panel",

        
     };
        #endregion

        public Form1(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == password)
                {
                    bypasslogin = true;
                }
            }
            //This is to see if the base URL was changed from default, if not remind user. 
            if (baseurl.Contains("http://www.example.com"))
            {
                MessageBox.Show("Looks like the base URL is set to:\n" + baseurl + "\nIt is recommended that this be changed.");
            }

            //Continue on. 
            Shown += Form1_Shown; //adds shown event listener
            this.FormClosing += Form1_FormClosing; //adds closing event listener
            InitializeComponent(); //does stuff


        }
        private void Form1_Load(object sender, EventArgs e) //This runs before the form is loaded
        {

            int index = random.Next(names.Count);//choses a 'random' title for the application.
            var name = names[index]; //gets chosen title 
            this.Text = name; //sets the chosen title
            try
            {
                if (!EventLog.SourceExists("ToolDeployment"))
                {
                    EventLog.CreateEventSource("ToolDeployment", "Application");
                }
                EventLog.WriteEntry("ToolDeployment", "Tooldeployment.exe loaded.", EventLogEntryType.Information);

            }
            catch (Exception x)
            {
                //
            }
        }
        private void updatelog(string x)//This adds a new line to the main log window, adds the text from string x, then scrolls to bottom
        {
            //I like to show the user that something is happening. 
            //I figured the easiest way to do this would be with a text window giving updates
            try
            {
                /*
                logwindow.Text += x + Environment.NewLine; //adds a new line
                 * */

                logwindow.AppendText("\n" + x);
                logwindow.SelectionStart = logwindow.Text.Length; //sets the selection to the start
                logwindow.ScrollToCaret(); //scrolls to the start. This makes the newest updates visible to the user

            }
            catch (Exception crash)
            {
                //eventlog("Unable to write to log window. The following error occurred:\n" + crash.Message +"\n\nBelow is the message that failed to write to log window:\n\n" + x);
                //just in case something can't write to the log window. Doesn't matter if it fails anyways.
                //Disabled because it could keep showing up over and over again

                //MessageBox.Show("Error while trying to update log;\n" + crash.Message);
            }
        }
        private void checkifshitisonline() //TODO check backup URLS if baseurl fails.
        {
            //Goes through each tool, and tries to get an HTTP resopnce.
            foreach (string w in toolnames)
            {
                string url = baseurl + w; //sets url
                Boolean isonline = true; //assumes is online unless otherwise
                HttpWebRequest request = WebRequest.Create(url.Trim()) as HttpWebRequest; //does the http request
                request.Method = "HEAD"; //sets request method.
                HttpWebResponse response = null;
                try
                {
                    response = request.GetResponse() as HttpWebResponse; //gets http responce
                }
                catch (WebException ex)
                {
                    isonline = false; //if there is an error, responce failed, file missing. 
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close(); //closes http connection
                    }
                }

                if (isonline && !reporteddone.Contains(w))
                {
                    filesavablibleonline.Add(w); //adds the file to list of files avalible online.
                }
                Application.DoEvents(); //Allows GUI to catch up. Much better user experince 
            }
            foreach (string y in filesavablibleonline)
            {
                if (!reporteddone.Contains(y))
                {
                    //after the loop above finishes, this goes through the list of files online, 
                    //filtering out already completed, then changes the buttons around. 
                    changebutton(y, null, "Avalible Online!", Color.Salmon, BTNDefaultBackColor, "true", "true", null, null, null, CHKBXDefaultForeColor, null, null);
                }
            }
        }
        private void stopdownloadingshit()//Stops all downloads, and cleans up partial downloads
        {
            updatelog("Stopping all active downloads. Please wait...");
            foreach (string p in activedownloads)
            {
                //for each file actively being downloaded:
                wascancled.Add(p); //adds to list of files that were canceled
                if (!filesavablibleonline.Contains(p))
                {
                    filesavablibleonline.Add(p);  //since we know its avalible online, lets add it to the list
                }
            }
            try
            {
                new Thread(new ThreadStart(deleteit)).Start();
            }
            catch (Exception x)
            {
                MessageBox.Show("Failed while deleting unfinished download: " + x.Message);
            }
            string g = "Cleaning up downloads, Please wait...";
            while (filesavablibleonline.Count > 0)
            {
                updatelog(g);
                g = g + "."; //shows the user that something is happening. 
                Application.DoEvents(); //Allows application to catch up
                System.Threading.Thread.Sleep(150); //no need to chew on CPU that much
            }

            //Resets the stage. 
            hidealllaunchbuttons();
            scanandreportdone();
            checkforsaveddata();
        }
        private void checkSMART()//Pulls smart data from all attached drives. TODO: Filter out CDs and Flash drives
        {
            try
            {
                // retrieve list of drives on computer (this will return both HDD's and CDROM's and Virtual CDROM's)                    
                var dicDrives = new Dictionary<int, HDD>();
                var wdSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

                // extract model and interface information
                int iDriveIndex = 0;
                foreach (ManagementObject drive in wdSearcher.Get())
                {
                    //Creates new object, and sets some varibles.
                    var hdd = new HDD();
                    hdd.Model = drive["Model"].ToString().Trim();
                    hdd.Type = drive["InterfaceType"].ToString().Trim();
                    dicDrives.Add(iDriveIndex, hdd); //Adds 
                    iDriveIndex++;
                }

                var pmsearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

                // retrieve hdd serial number
                iDriveIndex = 0;
                foreach (ManagementObject drive in pmsearcher.Get())
                {
                    // because all physical media will be returned we need to exit
                    // after the hard drives serial info is extracted
                    if (iDriveIndex >= dicDrives.Count)
                        break;

                    dicDrives[iDriveIndex].Serial = drive["SerialNumber"] == null ? "None" : drive["SerialNumber"].ToString().Trim();
                    iDriveIndex++;
                }

                // get wmi access to hdd 
                var searcher = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
                searcher.Scope = new ManagementScope(@"\root\wmi");
                // check if SMART reports the drive is failing
                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus");
                iDriveIndex = 0;
                foreach (ManagementObject drive in searcher.Get())
                {
                    dicDrives[iDriveIndex].IsOK = (bool)drive.Properties["PredictFailure"].Value == false;
                    iDriveIndex++;
                }
                // retrive attribute flags, value worste and vendor data information
                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictData");
                //MSStorageDriver_FailurePredictStatus
                //searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus");

                iDriveIndex = 0;
                foreach (ManagementObject data in searcher.Get())
                {
                    Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
                    for (int i = 0; i < 30; ++i)
                    {
                        try
                        {
                            int id = bytes[i * 12 + 2];

                            int flags = bytes[i * 12 + 4]; // least significant status byte, +3 most significant byte, but not used so ignored.
                            //bool advisory = (flags & 0x1) == 0x0;
                            bool failureImminent = (flags & 0x1) == 0x1;
                            //bool onlineDataCollection = (flags & 0x2) == 0x2;
                            int value = bytes[i * 12 + 5];
                            int worst = bytes[i * 12 + 6];
                            int vendordata = BitConverter.ToInt32(bytes, i * 12 + 7);
                            if (id == 0) continue;
                            var attr = dicDrives[iDriveIndex].Attributes[id];
                            attr.Current = value;
                            attr.Worst = worst;
                            attr.Data = vendordata;
                            attr.IsOK = failureImminent == false;
                        }
                        catch
                        {
                            // given key does not exist in attribute collection (attribute not in the dictionary of attributes)
                        }
                    }
                    iDriveIndex++;
                }
                // retreive threshold values foreach attribute
                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictThresholds");
                iDriveIndex = 0;
                foreach (ManagementObject data in searcher.Get())
                {
                    Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
                    for (int i = 0; i < 30; ++i)
                    {
                        try
                        {

                            int id = bytes[i * 12 + 2];
                            int thresh = bytes[i * 12 + 3];
                            if (id == 0) continue;

                            var attr = dicDrives[iDriveIndex].Attributes[id];
                            attr.Threshold = thresh;
                        }
                        catch
                        {
                            // given key does not exist in attribute collection (attribute not in the dictionary of attributes)
                        }
                    }
                    iDriveIndex++;
                }
                foreach (var drive in dicDrives)
                {

                    updatelog("-----------------------------------------------------");
                    updatelog(" DRIVE " + ((drive.Value.IsOK) ? "OK" : "BAD") + ": " + drive.Value.Serial + " - " + drive.Value.Model + " - " + drive.Value.Type);
                    if (!drive.Value.IsOK)
                    {
                        //Disabled because it kept showing up when USB flash drives were detected
                        //MessageBox.Show("S.M.A.R.T Error");
                    }

                    updatelog("Current\tWorst\tThreshold\tData\tStatus\tID");
                    foreach (var attr in drive.Value.Attributes)
                    {
                        if (attr.Value.HasData)
                        {
                            updatelog(attr.Value.Current + "\t" + attr.Value.Worst + "\t" + attr.Value.Threshold + "\t\t" + attr.Value.Data + "\t" + ((attr.Value.IsOK) ? "OK" : "") + "\t" + attr.Value.Attribute);
                            Application.DoEvents();
                        }
                    }
                    updatelog("DRIVE " + ((drive.Value.IsOK) ? "OK " : "BAD ") + drive.Value.Model);
                    updatelog("-----------------------------------------------------");
                }
            }
            catch (ManagementException e)
            {
                if (e.Message.Contains("Not supported"))
                {
                    updatelog("An error occurred while querying for WMI data: " + e.Message + ". Most likely a SSD or Raid");
                }
                else
                {
                    updatelog("An error occurred while querying for WMI data: " + e.Message);
                }
            }
        }
        private void copytosystemdrive()//Unused function - Copies all tools from application folder to C drive
        {
            updatelog("Copying to all files to system drive");
            if (!Directory.Exists(oldsaveto))
            {
                //If the folder doesn't exist, lets make it. 
                System.IO.Directory.CreateDirectory(oldsaveto);
            }
            if (localremotetools != oldsaveto) //Just make sure the application is not running from system drive
            {
                foreach (string x in toolnames)
                {
                    //For each file in the toolnames list, copy to system drive
                    System.Threading.Thread.Sleep(5000);
                    if (File.Exists(localremotetools + "\\" + x) && !File.Exists(oldsaveto + "\\" + x))
                    {
                        File.Copy(localremotetools + "\\" + x, oldsaveto + "\\" + x);
                    }
                }
                savetoo = oldsaveto; //sets working folder to system drive
                checkifshitexists(); //Scans for which files are avalible inworing folder.
                scanandreportdone(); //sets stuff avalible for launching
            }
        }
        private void checkifshitexists()//Looks for folder in application folder, and if so then set savetoo path to it 
        {
            updatelog("Checking if folders already exist");
            if (Directory.Exists(@localremotetools))
            {
                //If a folder exists in the same folder as this application
                //then lets assume its being ran on a flash drive, or something. 
                oldsaveto = savetoo.Trim(); //backups current savetoo
                savetoo = localremotetools; //sets working folder to application folder
                applicationFolderToolStripMenuItem.Checked = true; //Checks application thingy so the user can check which is being used
                systemDriveToolStripMenuItem.Checked = false; // unchecked the unused checkbox
            }
            updatelog("Using: " + savetoo);  //Lets tell the user whats going on
        }
        private void scanandreportdone() //This scans the working folder for each tool, if exits then reports it as done
        {
            foreach (string x in toolnames)
            {
                if (File.Exists(savetoo + "\\" + x) && !reporteddone.Contains(x))
                {
                    updatelog(x + " already exists. Reporting as done. ");
                    reportdone(x, false);
                }
                if (!File.Exists(savetoo + "\\" + x) && reporteddone.Contains(x))
                {
                    reporteddone.Remove(x);
                    changebutton(x, null, null, BTNDefaultForeColor, BTNDefaultBackColor, "false", "true", "false", null, null, CHKBXDefaultForeColor, "false", "false");
                }
                if (File.Exists(savetoo + "\\" + x) && reporteddone.Contains(x))
                {
                    //Do nothing.
                }
                if (!File.Exists(savetoo + "\\" + x) && !reporteddone.Contains(x))
                {
                    //Do nothing.
                }
            }
        }
        private Boolean checkifcancled(string x) //This takes a string, and checks if that is in a list. Not too srue why I put it in a method.
        {
            if (wascancled.Contains(x))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Boolean isalldownloadsdone()//This checks the activedownloads list aginst the reporteddone list, and reports its finding. 
        {
            Boolean isalldone = true;

            foreach (string x in activedownloads)
            {
                if (!reporteddone.Contains(x))
                {
                    isalldone = false;
                }
            }
            if (isalldone)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void runafterdownload()//Goes through the list of checked tools, and runs then
        {
            try
            {
                foreach (string p in toolnames)
                {
                    //For each item in toolnames list:
                    if (changebutton(p, "true", null, BTNDefaultForeColor,BTNDefaultBackColor , null, null, null, null, null, CHKBXDefaultForeColor, null, null)) //Check if the checkbox is checked
                    {
                        //If it is then add to a list
                        ischecked.Add(p);
                    }
                }
                System.Threading.Thread.Sleep(1000);
                foreach (string x in ischecked) //Checks list of checked aginst list of reported done, then runs each application in turn
                {
                    if (reporteddone.Contains(x))
                    {
                        string run = savetoo + "\\" + x;
                        System.Diagnostics.Process.Start(run).WaitForExit();
                    }
                }
            }
            catch (Exception crash)
            {
                eventlog("Method runafterdownload(); crashed with following message:\n" + crash.Message);
                //TODO: figure out how to update the below while in a thread
                //updatelog(crash.Message);
                //MessageBox.Show("Chrashed");
            }
        }
        private void deletethisshit()//Deletes all tools
        {
            updatelog("Deleting files...");
            try
            {
                System.IO.Directory.Delete(savetoo, true); //tries to delete the working folder, and all files inside.
                System.Threading.Thread.Sleep(1000); //Give it a second to take effect
                if (Directory.Exists(savetoo))
                {
                    //If folder is still there, then tell the user
                    updatelog("Unable to Delete: \n" + savetoo);
                }
                else
                {
                    //If folder is not there, tell the user
                    updatelog("Successfully Deleted:\n" + savetoo);
                    //hides everthing, and makes the GUI look like you just opened application
                    hidealllaunchbuttons();
                }
            }
            catch (Exception crash)
            {
                //Someting above crashed, tell user and then reset stage.
                updatelog("Crashed while deleting files:\n-" + crash.Message);
                eventlog("Method deletethisshit() crashed with following message:\n" + crash.Message);
                hidealllaunchbuttons();
                scanandreportdone();
                checkforsaveddata();
            }
        }
        private void downloadshit(string x)//Enables cancelall button, then starts the download thread 
        {
            if (Directory.Exists(savetoo))
            {
                if (File.Exists(savetoo + "\\" + x)) //Checks if file already exists
                {
                    if (!reporteddone.Contains(x) && !activedownloads.Contains(x))
                    {
                        //This checks if file was not reported as done, and is not in the list of active downloads.
                        //If so, then reports it as done. 
                        updatelog("Tried to download " + x + " but it already exists");
                        reportdone(x, false);
                    }
                }
                else
                {
                    //File not reported done and not active download
                    cancleallbtn.Enabled = true; //enables cancle all button
                    activedownloads.Add(x); //adds file to list of active downloads
                    updatelog("Downloading " + x); //tells user stuff is going on

                    //creats new form. This is left over from when this application spawned multiple windows for each download
                    listdownloads f2 = new listdownloads();

                    //Created to better notify user via systemtray if downloading has finished
                    isdownloadingstuff = true;

                    //This calls a function in the new form
                    //Passes the file name, where to save it to
                    //and where to download it from
                    f2.downloadwimfile(x, savetoo + "\\", baseurl, true);
                }
            }
        }
        private void enabmemsiinstaller() //This pushes some commands to CMD to enable the MSI installer in safemode w/networking. 
        {
            updatelog("Enabling MSI Installer");
            try
            {
                System.Diagnostics.Process.Start("cmd", "/c REG ADD \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Network\\MSIServer\\" + " /VE /T REG_SZ /F /D \"Service\"").WaitForExit();
                System.Diagnostics.Process.Start("cmd", "/c net start msiserver").WaitForExit(); //net start msiserver
            }
            catch (Exception crash)
            {
                eventlog("Unable to enable MSI installer. Error:\n" + crash.Message);
                updatelog("Error: " + crash.Message);
            }
        }
        private void opentools(string run)//Opens the file name in string run
        {
            string x = run.Replace(savetoo + "\\", "").Trim();
            changebutton(x, null, null, Color.Green, BTNDefaultBackColor, null, null, "false", null, null, CHKBXDefaultForeColor, null, null);
            if (filesavablibleonline.Contains(x))
            {
                //This checks if the file is in the list of files avalible from server
                //and if it is, then it re-marks the controls and then starts the download for this. 
                changebutton(x, null, "Avalible Online!", Color.Salmon, BTNDefaultBackColor, null, null, "true", null, null, CHKBXDefaultForeColor, null, null);
                filesavablibleonline.Remove(x);
                deploy();
                return;
            }
            else
            {
                try
                {
                    //Updates user on whats going on
                    updatelog("Running " + x);

                    if (!waslaunched.Contains(x))
                    {
                        //Keeps track of what files were opened.
                        waslaunched.Add(x);
                    }
                    //Saves data about which files were opened to the HDD
                    //Also saves CF6 notes
                    writesaveddata();

                    string switches = "";

                    //This checks if the file being opened has any switches or command line arguments
                    //if so, then it sets the string above to that switch
                    foreach (string i in automationlist)
                    {
                        if (i.Contains(x))
                        {
                            switches = i.Replace(x, "").Trim();
                        }
                    }

                    //Executes file
                    System.Diagnostics.Process.Start(run, switches);
                }
                catch (Exception crash)
                {
                    eventlog("Method opentools() crashed with following message:\n" + crash.Message);
                    updatelog(crash.Message);
                }
                return;
            }
        }
        private void checkforsaveddata()//if config file exists, loads and processes it
        {
            //Sets file path to config file. I should change it to an INI.
            string x = savetoo + "\\" + configfile;

            if (File.Exists(x))
            {
                //If the file exists, tell the user
                updatelog("Saved congifuration detected. Loading settings...");

                //then read all the lines in to a list
                string[] lines = File.ReadAllLines(savetoo + "\\" + configfile);

                //Go through that list and add it to another list
                foreach (string l in lines)
                {
                    if (!waslaunched.Contains(l))
                    {
                        waslaunched.Add(l);
                    }
                }

                //Saved data processing method
                processsaveddata();
            }
        }
        private void processsaveddata()//Changes buttons colors, and loads CF6 notess
        {
            string y = "";

            //This goes through each item in the waslaunched list
            foreach (string x in waslaunched)
            {
                //Looks for the line that defines the CF6 text field
                if (x.Contains("CF6NOTES"))
                {
                    //Saves it for later
                    y = x;
                }
                else
                {
                    //Changes button so user knows which files they already opened. Or tried to anyways.
                    updatelog("Updating " + x + "'s button");
                    changebutton(x, null, "Launch",Color.ForestGreen, BTNDefaultBackColor, null, null, null, null, null, CHKBXDefaultForeColor, null, null);
                }
            }

            //Restores CF6 field's text. 
            updatelog("Loading CF6 notes");
            CF6Notes.Text = y.Replace("CF6NOTES==", "");
        }
        private void writesaveddata()//Writes which files were opened, and CF6 notes. TODO: Add appliction location on screen
        {
            string u = "";

            //Goes through each item in the list 'waslaunched'
            foreach (string k in waslaunched)
            {
                if (k.Contains("CF6NOTES=="))
                {
                    //If this item is the CF6 notes, then lets save it for later
                    u = k;
                }
            }

            //Removes item from list
            waslaunched.Remove(u);

            //Replaces it with most up-to-date CF6 text
            waslaunched.Add("CF6NOTES==" + CF6Notes.Text);


            if (!Directory.Exists(savetoo))
            {
                //If the working folder doesn't exist, then create it
                System.IO.Directory.CreateDirectory(savetoo);
            }
            else if (File.Exists(savetoo + "\\" + configfile))
            {
                //If there already is a config file, lets assume its already loaded so we can delete this one
                File.Delete(savetoo + "\\" + configfile);
            }
            if (!File.Exists(savetoo + "\\" + configfile))
            {
                //Creates new config file, if old one is missing
                updatelog("Saving congifuration file");
                System.IO.File.WriteAllLines(@savetoo + "\\" + configfile, waslaunched.Cast<string>().ToArray());
            }
        }
        private void deleteit()//Deletes all files still in the list 'filesavalibleonline'
        {
            List<string> TEMPfilestoobedeleted = new List<string> { }; //creates a list. 
            foreach (string j in filesavablibleonline)
            {
                TEMPfilestoobedeleted.Add(j); //this basiclly copies one list to another. 
            }
            foreach (string p in TEMPfilestoobedeleted) //Using list created above, which is a copy of a nother list we are modifying. 
            {
                string deletefile = savetoo + "\\" + p; //makes file path as string. I like strings. 
                while (File.Exists(deletefile)) //creates loop that only breaks if file is deleted. 
                {
                    System.Threading.Thread.Sleep(500); //Lets not chew on the CPU too much. 
                    try
                    {
                        //Tell user we are deleting a file
                        updatelog("Deleting " + p);

                        //Lets not lie to the user 
                        File.Delete(deletefile);
                    }
                    catch
                    {
                        //ignore any errors. its in a loop and will simply tryagain. At some point this file can be deleted.... unless it cant....err...
                        //TODO: Add counter and break when gets too large
                    }
                }
                //file deleted, removed from files avalible online.
                filesavablibleonline.Remove(p);
            }
        }
        private void checkforupdates() //Unused method. I am still trying to figure out how to make this work
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 99; //sets a high connection limite. Lets take all the bandwidths! 
            int checkcount = 20;
            string d = applicationpath + "\\AutoUpdater.NET.dll";
            string filenameaun = "AutoUpdater.NET.dll";

            if (!File.Exists(d))
            {
                //If needed DLL doesn't exist, lets download it. 
                updatelog("Downloading updater");
                WebClient webClient = new WebClient(); //creates webclient
                webClient.DownloadFileAsync(new Uri(baseurl.Replace("_RemoteTools_/", "") + filenameaun), applicationpath + "\\"); //starts the download

            }
            while (activedownloads.Contains(filenameaun) && checkcount > 0)
            {
                //Lets wait for the file to finish downloads
                Application.DoEvents(); //Lets GUI catch up
                System.Threading.Thread.Sleep(500); //Pauses for 500ms 
                checkcount--; //one off counter
            }
            if (File.Exists(d))
            {
                //TODO Figure out how to dynamicly load and use a dll without crashing the application
                updatelog("Checking for updates");
            }
            else
            {
                //file missing, unable to check for updates
                updatelog("Unable to check for updates");
            }

        }
        private void md5scan() // Creats a md5 for each file in the list "toolnames"
        {
            md5hash.Clear(); //clears list
            foreach (string x in toolnames)
            {
                //For each item in the toolname list:
                using (var md5 = MD5.Create()) //Makes MD5 thingy
                {
                    if (File.Exists(savetoo + "\\" + x)) //checks if file exists 
                    {
                        using (var stream = File.OpenRead(savetoo + "\\" + x))//opens the file, and allows md5 function to do its thing
                        {
                            string f = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                            md5hash.Add(x + "==" + f);//adds file==md5hash to list.
                        }
                    }
                }
            }
        }
        private void md5compair()//Compairs the md5 of the tools currently on the computer to a txt file downloaded
        {
            string x = savetoo + "\\md5.txt";
            if (!File.Exists(x))
            {
                //If the md5.txt file doesn't exist in the working folder, then download it
                downloadshit("md5.txt");
            }
            while (activedownloads.Contains("md5.txt"))
            {
                //Waits for download to finish, meanwhile allowing GUI to update
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }
            if (File.Exists(x))
            {
                List<string> md5hashtemp = new List<string> { };
                List<string> founderror = new List<string> { };

                //Reads all lines in the text file in to a list
                string[] lines = File.ReadAllLines(savetoo + "\\md5.txt");

                //Goes through that list, one item at a time
                foreach (string l in lines)
                {
                    if (!md5hashtemp.Contains(l))
                    {
                        //Makes a copy of the list
                        md5hashtemp.Add(l);
                    }
                }

                //Goes through every file that has reported in
                foreach (string xa in reporteddone)
                {
                    string temp = "";
                    string temp2 = "";

                    //Compairs each value of the 'md5hashtemp' list
                    //to the current item in reporteddone list
                    foreach (string l in md5hashtemp)
                    {
                        if (l.Contains(xa))
                        {
                            //Found a match, lets add this to yet another list. Yay lists!
                            temp = l.Replace(xa + "==", "").Trim();
                        }
                    }
                    foreach (string l in md5hash)
                    {
                        if (l.Contains(xa))
                        {
                            //If the currently selected item in the list reporteddone
                            //is in the md5hash list, then add it to another list
                            temp2 = l.Replace(xa + "==", "").Trim();
                        }
                    }

                    //This basiclly takes the md5 of the file, and compairs it aginst the md5 in the downloaded txt file. 
                    if (temp != temp2)
                    {
                        //If wrong, add it to the list
                        founderror.Add("Error: " + xa + "\n-Scanned File: " + temp + "\n-Original File: " + temp2);
                    }
                    else
                    {
                        //if correct, tell user its fine
                        updatelog(xa + " md5 checked. Its fine. ");
                    }
                }
                if (founderror.Count > 0)
                {
                    foreach (string l in founderror)
                    {
                        //For each error found, tell the user
                        //this puts it at the end of the log window
                        updatelog(l);
                    }
                }
                else
                {
                    //Tells user we done
                    updatelog("All files check out");
                }
            }
        }
        private void md5write()//Writes the md5s gathered from md5scan() to a text file. I use this to update server
        {
            if (File.Exists(savetoo + "\\md5.txt"))
            {
                //If the txt file exits, then delete it
                File.Delete(savetoo + "\\md5.txt");
            }
            if (!File.Exists(savetoo + "\\md5.txt"))
            {
                //If the file doesn't exist, then tell use we are saving it
                updatelog("Saving md5s");

                //saves the txt file of
                System.IO.File.WriteAllLines(@savetoo + "\\md5.txt", md5hash.Cast<string>().ToArray());
                updatelog("Saved...");
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//Runs a little bit of code when the form closes
        {
            //Change title to let user know we are closeing
            notifyIcon1.BalloonTipText = "Closing, please wait...";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipTitle = "Please Wait";
            notifyIcon1.ShowBalloonTip(50);

            this.Text = "Closing, please wait...";
            updatelog("Closing. Please wait...");

            //Hides this window, instead of closing it
            this.Hide();

            //Stops any active downloads, which also cleans up any partial downloads
            stopdownloadingshit();

            //Now the form will close
        }
        private void loadeverything() //Called after correct password is entered
        {
            if (IntPtr.Size == 8)
            {
                is64bit = true;
                //updatelog("Is 64bit OS");
            }
            //Below I define which programs accept switchs, and what switches I want to use. 
            //This is automaticlly included when opening a file
            automationlist.Add(hitmanx32var + "/scan /noinstall");
            automationlist.Add(hitmanx64var + "/scan /noinstall");
            automationlist.Add(autorunsvar + "-e");
            automationlist.Add(hjtvar + "/autolog");
            automationlist.Add(mbamvar + "/SILENT /SUPRESSMSGBOXES /NORESTART");
            automationlist.Add(mbaevar + "/silent");
            automationlist.Add(uncheckvar + "-install");
            //automationlist.Add(AVG2014var + "/UILevel=silent /InstallToolbar=0 /ChangeBrowserSearchProvider=0 /SelectedLanguage=1033 /InstallSidebar=0 /ParticipateProductImprovement=0 /DontRestart /KillProcessesIfNeeded");
            automationlist.Add(abpievar + "/passive /norestart");
            automationlist.Add(callingcardvar + "/passive");
            automationlist.Add(tdssvar + "-accepteula -accepteulaksn -tdlfs -dcexact");
            //automationlist.Add(superaintivar + "/silent");
            //automationlist.Add(tweakingtoolsvar + "/silent");

            //Tells user we are doing stuff
            //And then adds each and every tool to a list
            //TODO: Figure out how to eleminating needing to manully adding tools
            updatelog("Initializing Components");
            toolnames.Add(ccleanervar);
            toolnames.Add(jrtvar);
            toolnames.Add(adwvar);
            toolnames.Add(mbamvar);
            toolnames.Add(hitmanx64var);
            toolnames.Add(hitmanx32var);
            toolnames.Add(nintievar);
            toolnames.Add(mbaevar);
            toolnames.Add(uncheckvar);
            toolnames.Add(abpievar);
            toolnames.Add(AVG2014var);
            toolnames.Add(callingcardvar);
            toolnames.Add(killemallvar);
            toolnames.Add(rkillvar);
            toolnames.Add(autorunsvar);
            toolnames.Add(hjtvar);
            toolnames.Add(teamvar);
            foreach (string g in toolnames)
            {
                //Adds all of the above to a the "standardtools" list
                standardtools.Add(g);
            }
            toolnames.Add(tdssvar);
            toolnames.Add(superaintivar);
            toolnames.Add(tweakingtoolsvar);
            toolnames.Add(avgremovalvar);
            toolnames.Add(sfcvar);
            toolnames.Add(revelationsvar);
            toolnames.Add(nortonremovalvar);
            toolnames.Add(mcafferemovalvar);
            toolnames.Add(roguekiller64var);
            toolnames.Add(roguekiller32var);
            toolnames.Add(Esetvar);
            toolnames.Add(produkeyvar);
            toolnames.Add(pcdecrapvar);
            toolnames.Add(revovar);
            toolnames.Add(chromevar);
            toolnames.Add(classicsrtvar);
            toolnames.Add(readervar);
            toolnames.Add(libraofficevar);

            foreach (string g in toolnames)
            {
                if (!standardtools.Contains(g))
                {
                    //Adds all of the above, minus the "standardtools" list, to the adcancedtools list
                    advancedtools.Add(g);
                }
            }

            //Saves application path for later, appending a folder that we might need
            localremotetools = applicationpath + "\\_RemoteTools_";

            //Sets the tool tip text to the string defined above, 
            //allows user to see where the files are being safed
            applicationFolderToolStripMenuItem.ToolTipText = localremotetools;
            systemDriveToolStripMenuItem.ToolTipText = oldsaveto;

            //Checks working folder for tools
            checkifshitexists();

            //Goes through folder and reports any found as done
            scanandreportdone();

            //Checks and loads config file
            checkforsaveddata();

            //uses info found in config file
            parsembamresults();

            //Checks SMART status of all drives in the computer
            //Disabled to speed up load time
            //checkSMART();
            //updatelog("");

            //Hey look! I did a thing!
            updatelog("System uptime " + (Environment.TickCount / 1000) / 60 / 60 + " hours");
            updatelog("Application loaded and ready for use");

        }
        private void unlockstuff()
        {
            toolsToolStripMenuItem.Visible = true;
            helpToolStripMenuItem.Visible = true;
            DeployBTN.Visible = true;
            cancleallbtn.Visible = true;
            DeployAllBTN.Visible = true;
            nuke.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            msiinstallerbtn.Visible = true;
            groupBox1.Visible = true;
            groupBox4.Visible = true;
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            Extras.Visible = true;
            logwindow.Visible = true;
            groupBox5.Visible = false;
            richTextBox1.Visible = false;
            loadeverything();
        }
        private void eventlog(string EventMessage)
        {
            if (!EventLog.SourceExists("ToolDeployment"))
            {
                EventLog.CreateEventSource("ToolDeployment", "Application");
            }

            EventLog.WriteEntry("ToolDeployment", EventMessage, EventLogEntryType.Warning);

        }
        private void Form1_Shown(object sender, EventArgs e) //Edit this if you want to add new files to be downloaded
        {
            if (bypasslogin)
            {
                unlockstuff();
            }
        }
        private void deletefileorcancle(string y)//Basicly this deletes a file . useful to add Delete tool option 
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                updatelog("Closing " + y);
                try
                {
                    System.Diagnostics.Process.Start("cmd", "/c taskkill /IM " + y + " /f");
                }
                catch (Exception crash)
                {
                    eventlog("Attempted to kill process " + y + ". Error:\n" + crash.Message);
                }
            }
            else
            {
                if (activedownloads.Contains(y))
                {
                    wascancled.Add(y);
                    changebutton(y, null, null, BTNDefaultForeColor, BTNDefaultBackColor, null, null, "false", null, null, CHKBXDefaultForeColor, "false", "false");
                }
                else
                {
                    //Change button back to default
                    changebutton(y, null, null, BTNDefaultForeColor, BTNDefaultBackColor, "false", null, "false", null, null, CHKBXDefaultForeColor, "false", "false");
                    //Makes a loop that only breaks when the file is missing
                    while (File.Exists(@savetoo + "\\" + y))
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(10);
                            //This deletes the file that was being downloaded
                            File.Delete(@savetoo + "\\" + y);
                            Application.DoEvents();
                        }
                        catch (Exception crash)
                        {
                            //aww... it crashed... and no one cares because its in a loop and will simply try again
                        }
                    }
                    //Clears a list
                    reporteddone.Remove(y);
                    //Clears a list
                    activedownloads.Remove(y);
                    //Clears a list
                    wascancled.Remove(y);
                    //Clears a list
                    filesavablibleonline.Remove(y);
                    //Clears a list
                    waslaunched.Remove(y);
                }
            }
        }
        public void reportdone(string y, Boolean notifyuser)//When a file is done, or already on HDD, then this function is called, and passed the file name 
        {
            //Gets info about the file
            FileInfo fInfo = new FileInfo(@savetoo + "\\" + y);
            //Gets size in bytes
            long size = fInfo.Length;
            if (size < 1)
            {
                //Tells user the download failed
                updatelog("Failed: " + y + " finished but only 0 bytes. Deleting.");
                //If the file is 0 bytes, then lets delete it. Most likely failed download. 
                wascancled.Add(y);
                activedownloads.Remove(y);
                deletefileorcancle(y);
                return;
            }

            try
            {
                List<string> tempremovefromactivedownloads = new List<string> { }; //Yay a list!

                //For each file that is being downloaded, add it to the temp list above
                foreach (string p in activedownloads)
                {
                    if (p == y)
                    {
                        tempremovefromactivedownloads.Add(y);
                    }
                }

                //for each file in the new temp list remove the same item from the list of active downloads
                foreach (string z in tempremovefromactivedownloads)
                {
                    activedownloads.Remove(y);
                }
                //Report the file as being finished
                reporteddone.Add(y);
                //tell user
                updatelog(y + " Reported done! Activating buttons");
                if (notifyuser)
                {
                    notifyIcon1.BalloonTipText = y + " Reported done!";
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipTitle = "Alert!";
                    notifyIcon1.ShowBalloonTip(100);
                }
                //Change the contols around to match a file being ready for launch
                changebutton(y, null, "Launch", BTNDefaultForeColor, BTNDefaultBackColor, "true", "true", null, null, null, CHKBXDefaultForeColor, "true", "false");
                //If the file that we just finished downloading was KillEmAll.exe, then lets make a whitelist for it
                if (y == killemallvar)
                {
                    string killemallconfigdir = savetoo;
                    try
                    {
                        //Defines the white list
                        List<string> white = new List<string> { "ToolDeployment.exe", "teamviewer.exe", "lmi_rescue.exe", "LMI_RE~2.exe", "CallingCard.exe", "CallingCard_srv.exe" };
                        foreach (string h in toolnames)
                        {
                            //Adds every tool to the white list
                            white.Add(h);
                        }
                        if (!Directory.Exists(killemallconfigdir))
                        {
                            //Makes a folder I think it needs. 
                            System.IO.Directory.CreateDirectory(killemallconfigdir);
                        }
                        if (File.Exists(killemallconfigdir + "\\KEA_Whitelist.txt"))
                        {
                            //If the white list already exists, then delete it
                            File.Delete(killemallconfigdir + "\\KEA_Whitelist.txt");

                            //Writes white list to HDD
                            System.IO.File.WriteAllLines(killemallconfigdir + "\\KEA_Whitelist.txt", white.Cast<string>().ToArray());
                        }
                        if (!File.Exists(killemallconfigdir + "\\KEA_Whitelist.txt"))
                        {
                            //If white list doesn't already exist, then write it to HDD
                            System.IO.File.WriteAllLines(@killemallconfigdir + "\\KEA_Whitelist.txt", white.Cast<string>().ToArray());
                        }
                    }
                    catch (Exception crash)
                    {
                        //Chrashed, try again
                        eventlog("Unable to modify " + killemallconfigdir + "//KEA_Whitelist.txt. Error:\n" + crash.Message);
                        updatelog(crash.Message);
                    }
                }

                //Checks if all files have finished downloading
                if (isalldownloadsdone())
                {
                    //disables the cancel all button
                    cancleallbtn.Enabled = false;
                    uncheckall();
                    if (isdownloadingstuff)
                    {

                        isdownloadingstuff = false;
                        notifyIcon1.BalloonTipText = "All downloads completed!";
                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIcon1.BalloonTipTitle = "Alert!";
                        notifyIcon1.ShowBalloonTip(50);
                        checkforsaveddata();

                    }

                    if (runAllAfterDownloadToolStripMenuItem.Checked == true)
                    {
                        //if user selected the option to run after download, then this runs the files. all that were checked
                        try
                        {
                            new Thread(new ThreadStart(runafterdownload)).Start();
                        }
                        catch (Exception x)
                        {
                            updatelog("Unable to run after download. Error: " + x.Message);
                        }
                    }
                }
            }
            catch (Exception crash)
            {
                eventlog("Method reporteddone(); crased with following message:\n" + crash.Message);
                updatelog("reporteddone(); crashed:\n-" + crash.Message);
            }
        }
        private void checkregulartools() //Checks every tool in the standardtools list
        {
            updatelog("Checking all standard tools");
            foreach (string g in standardtools)
            {
                changebutton(g, null, null, BTNDefaultForeColor, BTNDefaultBackColor, null, null, "true", null, null, CHKBXDefaultForeColor, null, null);
            }
        }
        private void checkalladvanced()//Checks every tool in the advancedtools list
        {
            updatelog("Checking all advanced tools");
            foreach (string g in advancedtools)
            {
                changebutton(g, null, null, BTNDefaultForeColor, BTNDefaultBackColor, null, null, "true", null, null, CHKBXDefaultForeColor, null, null);
            }
        }
        private void uncheckall() //Unchecks all tools in "toolname" list
        {
            updatelog("Unchecking all tools");
            foreach (string s in toolnames)
            {
                changebutton(s, null, null,BTNDefaultForeColor,BTNDefaultBackColor, null, null, "false", null, null, CHKBXDefaultForeColor, null, null);
            }
        }
        private void hidealllaunchbuttons() //resets stage, as if nothing happened
        {
            updatelog("Resetting stage...");

            foreach (string x in toolnames)
            {
                changebutton(x, null, "Launch", BTNDefaultForeColor, BTNDefaultBackColor, "false", null, null, null, null, CHKBXDefaultForeColor, "false", "false");
            }
            //Clears a list
            reporteddone.Clear();
            //unchecks all checkboxes
            uncheckall();
            //Clears a list
            activedownloads.Clear();
            //Clears a list
            wascancled.Clear();
            //Clears a list
            filesavablibleonline.Clear();
            //Clears a list
            waslaunched.Clear();
        }
        private void parsembamresults() //Pulls MBAM log from clipbaord, and outputs the good bits to the user
        {
            //Gets text out of clipboard
            string x = Clipboard.GetText();

            //Checks if the clipbaord contained mbam logs
            if (x.Contains("Malwarebytes Anti-Malware"))
            {
                List<string> adduplater = new List<string> { };

                //Splits clipboard in to an array
                string[] result = x.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                //Goes through array, and checks each item if it congains the string we are looking for
                //if it does, then it adds it to a list without anything else. 
                foreach (string y in result)
                {
                    if (y.Contains("Processes:"))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Processes: ", "").Trim());
                    }
                    if (y.Contains("Modules:"))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Modules: ", "").Trim());
                    }
                    if (y.Contains("Registry "))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Registry ", "").Replace("Keys: ", "").Replace("Values: ", "").Replace("Data: ", "").Trim());
                    }
                    if (y.Contains("Folders:"))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Folders: ", "").Trim());
                    }
                    if (y.Contains("Files:"))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Files: ", "").Trim());
                    }
                    if (y.Contains("Physical "))
                    {
                        updatelog(y);
                        adduplater.Add(y.Replace("Physical Sectors: ", "").Trim());
                    }
                }
                int total = 0;
                //Adds up each number in the adduplater list, then tells user the total
                foreach (string ma in adduplater)
                {
                    int tempx = 0;
                    try
                    {
                        Int32.TryParse(ma.Trim(), out tempx);
                    }
                    catch (Exception crash)
                    {
                        eventlog("Was unable to parse Clipboard data. Error:\n" + crash.Message);
                        updatelog("Unable to parse Clipboard data: " + crash.Message);
                    }
                    total = total + tempx;
                }
                updatelog("Mbam total: " + total);
            }
            else
            {
                updatelog("Mbam results not found in clipboard");
            }
        }
        private void deploy() //Edit this if you want to add new files to be downloaded
        { //This goes through each group of controls, checks the following:
            //1) if the checkbox is checked
            //2) if the checkbox is enabled
            //3) checks if the file was already reported done
            //If all of the above pass, then the file download is started. and some controls are changed
            updatelog("Starting Deployment...");
            if (!Directory.Exists(savetoo))
            {
                System.IO.Directory.CreateDirectory(savetoo);
            }
            if (CcleanerChkBX.Checked == true && CcleanerChkBX.Enabled == true && !reporteddone.Contains(ccleanervar))
            {
                downloadshit(ccleanervar);
                downloadshit("portable.dat");
                downloadshit("ccleaner.ini");
                downloadshit("branding.dll");
                downloadshit("CCleaner.dat");

                cclcanclebtn.Visible = true;
                CCprgsbr.Visible = true;
            }
            if (JRTChkBX.Checked == true && JRTChkBX.Enabled == true && !reporteddone.Contains(jrtvar))
            {
                downloadshit(jrtvar);
                JRTcanclebtn.Visible = true;
                JRTprgsbr.Visible = true;
            }
            if (ADWChkBX.Checked == true & ADWChkBX.Enabled == true && !reporteddone.Contains(adwvar))
            {
                downloadshit(adwvar);
                ADWcanclebtn.Visible = true;
                ADWprgsbr.Visible = true;
            }
            if (MbamChkBx.Checked == true && MbamChkBx.Enabled == true && !reporteddone.Contains(mbamvar))
            {
                downloadshit(mbamvar);
                MBAMcnaclebtn.Visible = true;
                MBAMprgsbr.Visible = true;
            }
            if (HitmanChkBX.Checked == true && HitmanChkBX.Enabled == true)
            {
                if (is64bit == true)
                {
                    if (!reporteddone.Contains(hitmanx64var))
                    {
                        downloadshit(hitmanx64var);
                        Hitmancanclebtn.Visible = true;
                        Hitmanprgsbr.Visible = true;
                    }
                }
                else
                {
                    if (!reporteddone.Contains(hitmanx32var))
                    {
                        downloadshit(hitmanx32var);
                        Hitmancanclebtn.Visible = true;
                        Hitmanprgsbr.Visible = true;
                    }
                }
            }
            if (NiniteChkBX.Checked == true && NiniteChkBX.Enabled == true && !reporteddone.Contains(nintievar))
            {
                downloadshit(nintievar);
                ninitecanclebtn.Visible = true;
                niniteprgsbr.Visible = true;
            }
            if (MBAEChkBx.Checked == true && MBAEChkBx.Enabled == true && !reporteddone.Contains(mbaevar))
            {
                downloadshit(mbaevar);
                MBAEcancelbutton.Visible = true;
                MBAEprgsbr.Visible = true;
            }
            if (UncheckyChkBX.Checked == true && UncheckyChkBX.Enabled == true && !reporteddone.Contains(uncheckvar))
            {
                downloadshit(uncheckvar);
                Uncheckycanclebtn.Visible = true;
                unchkprgsbr.Visible = true;
            }
            if (ABPIEChkBX.Checked == true && ABPIEChkBX.Enabled == true && !reporteddone.Contains(abpievar))
            {
                downloadshit(abpievar);
                ABPcanclebtn.Visible = true;
                APBprgsbr.Visible = true;
            }
            if (AVG2014ChkBX.Checked == true && AVG2014ChkBX.Enabled == true && !reporteddone.Contains(AVG2014var))
            {
                downloadshit(AVG2014var);
                AVGcanclebtn.Visible = true;
                AVGprgsbr.Visible = true;
            }
            if (callingcardchkbx.Checked == true && callingcardchkbx.Enabled == true && !reporteddone.Contains(callingcardvar))
            {
                downloadshit(callingcardvar);
                callingcardcancelbtn.Visible = true;
                callingcardprgsbr.Visible = true;
            }
            if (killemallchkbx.Checked == true && killemallchkbx.Enabled == true && !reporteddone.Contains(killemallvar))
            {
                downloadshit(killemallvar);
                killemallcancelbtn.Visible = true;
                killemallprgsbr.Visible = true;
            }
            if (rkillchkbx.Checked == true && rkillchkbx.Enabled == true && !reporteddone.Contains(rkillvar))
            {
                downloadshit(rkillvar);
                rkillcancelbtn.Visible = true;
                rkillprgsbr.Visible = true;
            }
            if (autorunschkbx.Checked == true && autorunschkbx.Enabled == true && !reporteddone.Contains(autorunsvar))
            {
                downloadshit(autorunsvar);
                autorunscancelbtn.Visible = true;
                autorunsprgsbr.Visible = true;
            }
            if (tdsschkbx.Checked == true && tdsschkbx.Enabled == true && !reporteddone.Contains(tdssvar))
            {
                downloadshit(tdssvar);
                tdsscancelbtn.Visible = true;
                tdssprgsbr.Visible = true;
            }
            if (superantichkbx.Checked == true && superantichkbx.Enabled == true && !reporteddone.Contains(superaintivar))
            {
                downloadshit(superaintivar);
                superanticancelbtn.Visible = true;
                superantiprgsbr.Visible = true;
            }
            if (tweakikngtookschkbx.Checked == true && superanticancelbtn.Enabled == true && !reporteddone.Contains(tweakingtoolsvar))
            {
                downloadshit(tweakingtoolsvar);
                tweakingtoolscancelbtn.Visible = true;
                tweakingtoolsprgsbr.Visible = true;
            }
            if (avgremovalchkbx.Checked == true && avgremovalchkbx.Enabled == true && !reporteddone.Contains(avgremovalvar))
            {
                downloadshit(avgremovalvar);
                avgremovalcancelbtn.Visible = true;
                avgremovalprgsbr.Visible = true;
            }
            if (sfcchkbx.Checked == true && sfcchkbx.Enabled == true && !reporteddone.Contains(sfcvar))
            {
                downloadshit(sfcvar);
                sfccancelbtn.Visible = true;
                sfcprgsbr.Visible = true;
            }
            if (revelationchkbx.Checked == true && revelationchkbx.Enabled == true && !reporteddone.Contains(revelationsvar))
            {
                downloadshit(revelationsvar);
                revelationscancelbtn.Visible = true;
                revelationprgsbr.Visible = true;
            }
            if (nortonremovalchkbx.Checked == true && nortonremovalchkbx.Enabled == true && !reporteddone.Contains(nortonremovalvar))
            {
                downloadshit(nortonremovalvar);
                nortonremovalcancelbtn.Visible = true;
                nortonremovalprgsbr.Visible = true;
            }
            if (mcafferemovalchkbx.Checked == true && mcafferemovalchkbx.Enabled == true && !reporteddone.Contains(mcafferemovalvar))
            {
                downloadshit(mcafferemovalvar);
                mcafferemovalcancelbtn.Visible = true;
                mcafferemovalprgsbr.Visible = true;
            }
            if (roguekillerchkbx.Checked == true && roguekillerchkbx.Enabled == true)
            {
                if (is64bit == true)
                {
                    if (!reporteddone.Contains(roguekiller64var))
                    {
                        downloadshit(roguekiller64var);
                        roguekillercancelbtn.Visible = true;
                        roguekillerprgsbr.Visible = true;
                    }
                }
                else
                {
                    if (!reporteddone.Contains(roguekiller32var))
                    {
                        downloadshit(roguekiller32var);
                        roguekillercancelbtn.Visible = true;
                        roguekillerprgsbr.Visible = true;
                    }
                }
            }
            if (esetchkbx.Checked == true && esetchkbx.Enabled == true && !reporteddone.Contains(Esetvar))
            {
                downloadshit(Esetvar);
                esetcancelbtn.Visible = true;
                esetprgsbr.Visible = true;
            }
            if (produkeychkbx.Checked == true && produkeychkbx.Enabled == true && !reporteddone.Contains(produkeyvar))
            {
                downloadshit(produkeyvar);
                produkeycancelbtn.Visible = true;
                produkeyprgsbr.Visible = true;
            }
            if (hjtchkbx.Checked == true && hjtchkbx.Enabled == true && !reporteddone.Contains(hjtvar))
            {
                downloadshit(hjtvar);
                hjtcancelbtn.Visible = true;
                hjtprgsbr.Visible = true;
            }
            if (pcdecrapchkbx.Checked == true && pcdecrapchkbx.Enabled == true && !reporteddone.Contains(pcdecrapvar))
            {
                downloadshit(pcdecrapvar);
                pcdecrapcancelbtn.Visible = true;
                pcdecrapprgsbr.Visible = true;
            }
            if (revochkbx.Checked == true && revochkbx.Enabled == true && !reporteddone.Contains(revovar))
            {
                downloadshit(revovar);
                revocancelbtn.Visible = true;
                revoprgsbr.Visible = true;
            }
            if (chromechkbx.Checked == true && chromechkbx.Enabled == true && !reporteddone.Contains(chromevar))
            {
                downloadshit(chromevar);
                chromecancelbtn.Visible = true;
                chromeprgsbr.Visible = true;
            }
            if (classicchkbx.Checked == true && classicchkbx.Enabled == true && !reporteddone.Contains(classicsrtvar))
            {
                downloadshit(classicsrtvar);
                classiccancelbtn.Visible = true;
                classicprgsbr.Visible = true;
            }
            if (teamchkbx.Checked == true && teamchkbx.Enabled == true && !reporteddone.Contains(teamvar))
            {
                downloadshit(teamvar);
                teamcancelbr.Visible = true;
                teamprgsbr.Visible = true;
            }
            if (readerchkbx.Checked == true && readerchkbx.Enabled == true && !reporteddone.Contains(readervar))
            {
                downloadshit(readervar);
                readercancelbtn.Visible = true;
                readerprgsbr.Visible = true;
            }
            if (librachkbx.Checked == true && librachkbx.Enabled == true && !reporteddone.Contains(libraofficevar))
            {
                downloadshit(libraofficevar);
                librecancelbtn.Visible = true;
                libreprgsbr.Visible = true;
            }
        }
        private void checkpassword()
        {
            if (textBox1.Text == password)
            {
                unlockstuff();
            }
            else if (textBox1.Text.Contains("cake") && textBox1.Text.Contains("is") && textBox1.Text.Contains("lie"))
            {
                darktheme();
            } else if (textBox1.Text.Contains("Rainbow") && textBox1.Text.Contains("Dash")) 
            {
                unlockstuff();
                thememaker(ColorTranslator.FromHtml("#9EDBF9"), Color.Purple, ColorTranslator.FromHtml("#77B0E0"), Color.DarkGreen);
                hidealllaunchbuttons();
                scanandreportdone();
                checkforsaveddata();
            }
            else
            {
                eventlog("Wrong password entered, user entered: " + textBox1.Text);
                notifyIcon1.BalloonTipText = "Wrong Password!";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                notifyIcon1.BalloonTipTitle = "Alert!";
                notifyIcon1.ShowBalloonTip(50);

            }
        }
        private void darktheme()
        {
            unlockstuff();
            thememaker(Color.Black, Color.White, Color.DarkGray, Color.WhiteSmoke);
            hidealllaunchbuttons();
            scanandreportdone();
            checkforsaveddata();

        }
        private void lighttheme()
        {

        }
        private void thememaker(Color FormBackColor, Color BTNForColor, Color BTNBackColor, Color CHKBXForeColor)
        {
            BTNDefaultForeColor = BTNForColor;
            BTNDefaultBackColor = BTNBackColor;
            CHKBXDefaultForeColor = CHKBXForeColor;

            //richTextBox1.Clear();
            //richTextBox1.Visible = true;
            //richTextBox1.BackColor = Color.Orange;

            this.BackColor = FormBackColor;

            DeployBTN.ForeColor = BTNForColor;
            DeployBTN.BackColor = BTNBackColor;
            DeployAllBTN.ForeColor = BTNForColor;
            DeployAllBTN.BackColor = BTNBackColor;
            toolsToolStripMenuItem.ForeColor = BTNForColor;
            helpToolStripMenuItem.ForeColor = BTNForColor;
            groupBox4.ForeColor = BTNForColor;
            groupBox1.ForeColor = BTNForColor;
            groupBox2.ForeColor = BTNForColor;
            Extras.ForeColor = BTNForColor;
            groupBox3.ForeColor = BTNForColor;

            //groupBox4.BackColor = FormBackColor;
            //groupBox1.BackColor = FormBackColor;
            //groupBox2.BackColor = FormBackColor;
            //Extras.BackColor = FormBackColor;
            //groupBox3.BackColor = FormBackColor;

            cancleallbtn.ForeColor = BTNForColor;
            cancleallbtn.BackColor = BTNBackColor;
            nuke.ForeColor = BTNForColor;
            nuke.BackColor = BTNBackColor;
            button1.ForeColor = BTNForColor;
            button1.BackColor = BTNBackColor;
            button2.ForeColor = BTNForColor;
            button2.BackColor = BTNBackColor;
            msiinstallerbtn.ForeColor = BTNForColor;
            msiinstallerbtn.BackColor = BTNBackColor;
            logwindow.ForeColor = BTNForColor;
            logwindow.BackColor = BTNBackColor;
            
            foreach (string x in toolnames)
            {
                changebutton(x, null, null,BTNDefaultForeColor, BTNDefaultBackColor, null, null, null, null, null, CHKBXDefaultForeColor, null, null);
            }
            processsaveddata();
             
        }
        public Boolean updateprgsbr(int x, string y) //Edit this if you want to add new files to be downloaded
        {
            //This takes the string, and finds the correct progress bar for it
            //Once found, it sets that bar's value to the number passed
            //This is mostly called from form2, when the file is being downloaded
            if (y == ccleanervar)
            {
                CCprgsbr.Value = x;
            }
            if (y == jrtvar)
            {
                JRTprgsbr.Value = x;
            }
            if (y == adwvar)
            {
                ADWprgsbr.Value = x;
            }
            if (y == mbamvar)
            {
                MBAMprgsbr.Value = x;
            }
            if (y == hitmanx32var || y == hitmanx64var)
            {
                Hitmanprgsbr.Value = x;
            }
            if (y == nintievar)
            {
                niniteprgsbr.Value = x;
            }
            if (y == mbaevar)
            {
                MBAEprgsbr.Value = x;
            }
            if (y == uncheckvar)
            {
                unchkprgsbr.Value = x;
            }
            if (y == abpievar)
            {
                APBprgsbr.Value = x;
            }
            if (y == AVG2014var)
            {
                AVGprgsbr.Value = x;
            }
            if (y == callingcardvar)
            {
                callingcardprgsbr.Value = x;
            }
            if (y == killemallvar)
            {
                killemallprgsbr.Value = x;
            }
            if (y == rkillvar)
            {
                rkillprgsbr.Value = x;
            }
            if (y == autorunsvar)
            {
                autorunsprgsbr.Value = x;
            }
            if (y == tdssvar)
            {
                tdssprgsbr.Value = x;
            }
            if (y == superaintivar)
            {
                superantiprgsbr.Value = x;
            }
            if (y == tweakingtoolsvar)
            {
                tweakingtoolsprgsbr.Value = x;
            }
            if (y == avgremovalvar)
            {
                avgremovalprgsbr.Value = x;
            }
            if (y == sfcvar)
            {
                sfcprgsbr.Value = x;
            }
            if (y == revelationsvar)
            {
                revelationprgsbr.Value = x;
            }
            if (y == nortonremovalvar)
            {
                nortonremovalprgsbr.Value = x;
            }
            if (y == mcafferemovalvar)
            {
                mcafferemovalprgsbr.Value = x;
            }
            if (y == roguekiller32var || y == roguekiller64var)
            {
                roguekillerprgsbr.Value = x;
            }
            if (y == Esetvar)
            {
                esetprgsbr.Value = x;
            }
            if (y == produkeyvar)
            {
                produkeyprgsbr.Value = x;
            }
            if (y == hjtvar)
            {
                hjtprgsbr.Value = x;
            }
            if (y == pcdecrapvar)
            {
                pcdecrapprgsbr.Value = x;
            }
            if (y == revovar)
            {
                revoprgsbr.Value = x;
            }
            if (y == chromevar)
            {
                chromeprgsbr.Value = x;
            }
            if (y == classicsrtvar)
            {
                classicprgsbr.Value = x;
            }
            if (y == teamvar)
            {
                teamprgsbr.Value = x;
            }
            if (y == readervar)
            {
                readerprgsbr.Value = x;
            }
            if (y == libraofficevar)
            {
                libreprgsbr.Value = x;
            }
            if (checkifcancled(y) == true)
            {
                activedownloads.Remove(y);
                return true;
            }
            return false;
        }

        private Boolean changebutton(
            string fileexename,
            string returnischecked,
            string BTNtext,
            Color BTNForeColor,
            Color BTNBackColor,
            string BTNEnabled,
            string BTNVisible,
            string CHKBXChecked,
            string CHKBXEnabled,
            string CHKBXText,
            Color CHKBXForeColor,
            string CancelBTNVisable,
            string prgsbrVisable)///Edit this if you want to add new files to be downloaded
        {
            Boolean checkedvar = true;
            Boolean enablebutton = false;
            Boolean showcancelBTN = false;
            Boolean btnisvisible = true;
            Boolean CHKBXEnabledDisabled = true;
            Boolean prgsbrVisableHidden = true;
            Color btnforecolor = Color.Black;
            Color btnbackcolor = Color.Black;

            #region I needed some way to pass either a 'true' 'false' or a 'null' to leave unchanged. Thats where this came in.
            btnforecolor = BTNForeColor;

            //TODO NEED to add more colors, or something, idk
            btnbackcolor = BTNBackColor;

            if (BTNEnabled == "true")
            {
                enablebutton = true;
            }
            else if (BTNEnabled == "false")
            {
                enablebutton = false;
            }
            if (BTNVisible == "true")
            {
                btnisvisible = true;
            }
            else if (BTNVisible == "false")
            {
                btnisvisible = false;
            }
            if (CHKBXChecked == "true")
            {
                checkedvar = true;
            }
            else if (CHKBXChecked == "false")
            {
                checkedvar = false;
            }
            if (CHKBXEnabled == "true")
            {
                CHKBXEnabledDisabled = true;
            }
            else if (CHKBXEnabled == "false")
            {
                CHKBXEnabledDisabled = false;
            }
            if (CancelBTNVisable == "true")
            {
                showcancelBTN = true;
            }
            else if (CancelBTNVisable == "false")
            {
                showcancelBTN = false;
            }
            if (prgsbrVisable == "true")
            {
                prgsbrVisableHidden = true;
            }
            else if (prgsbrVisable == "false")
            {
                prgsbrVisableHidden = false;
            }
            #endregion
            /*
             * This is where I can change groups of controls with a single method call. Very useful for loops.
             * Each group below does the following: 
             * 
             * 1) Checks if btncolornull is null, if not then changes the launch button's color to that of btncolor
             * 2) Checks if btntextnull is null, if not then changes the launch button's text to that of btnntextnull
             * 3) Checks if btnenabledisablednull is null, if not then enables or disables the button accordently
             * 4) Checks if btnvisiblehiddennull is null, if not then sets the visibility accordenly
             * 5) Checks if chkbcenableddisablednull is null, then sets the checkbox = to that of chkbxenableddisabled
             * 6) Checks if returnischecked is null, if not then it checks if the corresponding checkbox is checked, and returns true or false
             * 7) Checks if cancelbtnvisablehiddennull is null, if not then sets visibility to the value of showcancelbtn
             * 8) Checks if prgsbrvisablehiddennull is null, if not then sets the corresponding visibility to that of prgsbrVisableHidden
             * */

            if (fileexename == ccleanervar)
            {
                if (BTNForeColor != null) { ccbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { ccbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { ccbtn.Text = BTNtext; }
                if (BTNEnabled != null) { ccbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { ccbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { ccbtn.Text = CHKBXText; }
                if (CHKBXChecked != null) { CcleanerChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { CcleanerChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { CcleanerChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (CcleanerChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { cclcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { CCprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == jrtvar)
            {
                if (BTNForeColor != null) { JRTbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { JRTbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { JRTbtn.Text = BTNtext; }
                if (BTNEnabled != null) { JRTbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { JRTbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { JRTChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { JRTChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { JRTChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { JRTChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (JRTChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { JRTcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { JRTprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == adwvar)
            {

                if (BTNForeColor != null) { ADWbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { ADWbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { ADWbtn.Text = BTNtext; }
                if (BTNEnabled != null) { ADWbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { ADWbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { ADWChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { ADWChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { ADWChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { ADWChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (ADWChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { ADWcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { ADWprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mbamvar)
            {
                if (BTNForeColor != null) { MBAMbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { MBAMbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { MBAMbtn.Text = BTNtext; }
                if (BTNEnabled != null) { MBAMbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { MBAMbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { MbamChkBx.Text = CHKBXText; }
                if (CHKBXChecked != null) { MbamChkBx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { MbamChkBx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { MbamChkBx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (MbamChkBx.Checked) { return true; } }
                if (CancelBTNVisable != null) { MBAMcnaclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { MBAMprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == hitmanx32var || fileexename == hitmanx64var)
            {
                if (BTNForeColor != null) { Hitmanbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { Hitmanbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { Hitmanbtn.Text = BTNtext; }
                if (BTNEnabled != null) { Hitmanbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { Hitmanbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { HitmanChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { HitmanChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { HitmanChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { HitmanChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (HitmanChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { Hitmancanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { Hitmanprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == nintievar)
            {
                if (BTNForeColor != null) { ninitebtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { ninitebtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { ninitebtn.Text = BTNtext; }
                if (BTNEnabled != null) { ninitebtn.Enabled = enablebutton; }
                if (BTNVisible != null) { ninitebtn.Visible = btnisvisible; }
                if (CHKBXText != null) { NiniteChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { NiniteChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { NiniteChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { NiniteChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (NiniteChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { ninitecanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { niniteprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mbaevar)
            {
                if (BTNForeColor != null) { MBAEbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { MBAEbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { MBAEbtn.Text = BTNtext; }
                if (BTNEnabled != null) { MBAEbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { MBAEbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { MBAEChkBx.Text = CHKBXText; }
                if (CHKBXChecked != null) { MBAEChkBx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { MBAEChkBx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { MBAEChkBx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (MBAEChkBx.Checked) { return true; } }
                if (CancelBTNVisable != null) { MBAEcancelbutton.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { MBAEprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == uncheckvar)
            {
                if (BTNForeColor != null) { Unchkbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { Unchkbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { Unchkbtn.Text = BTNtext; }
                if (BTNEnabled != null) { Unchkbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { Unchkbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { UncheckyChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { UncheckyChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { UncheckyChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { UncheckyChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (UncheckyChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { Uncheckycanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { unchkprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == abpievar)
            {
                if (BTNForeColor != null) { ABPbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { ABPbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { ABPbtn.Text = BTNtext; }
                if (BTNEnabled != null) { ABPbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { ABPbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { ABPIEChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { ABPIEChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { ABPIEChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { ABPIEChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (ABPIEChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { ABPcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { APBprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == AVG2014var)
            {
                if (BTNForeColor != null) { AVGbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { AVGbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { AVGbtn.Text = BTNtext; }
                if (BTNEnabled != null) { AVGbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { AVGbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { AVG2014ChkBX.Text = CHKBXText; }
                if (CHKBXChecked != null) { AVG2014ChkBX.Checked = checkedvar; }
                if (CHKBXEnabled != null) { AVG2014ChkBX.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { AVG2014ChkBX.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (AVG2014ChkBX.Checked) { return true; } }
                if (CancelBTNVisable != null) { AVGcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { AVGprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == callingcardvar)
            {
                if (BTNForeColor != null) { callingcardbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { callingcardbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { callingcardbtn.Text = BTNtext; }
                if (BTNEnabled != null) { callingcardbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { callingcardbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { callingcardchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { callingcardchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { callingcardchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { callingcardchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (callingcardchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { callingcardcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { callingcardprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == killemallvar)
            {
                if (BTNForeColor != null) { killemallbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { killemallbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { killemallbtn.Text = BTNtext; }
                if (BTNEnabled != null) { killemallbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { killemallbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { killemallchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { killemallchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { killemallchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { killemallchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (killemallchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { killemallcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { killemallprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == rkillvar)
            {
                if (BTNForeColor != null) { rkillbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { rkillbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { rkillbtn.Text = BTNtext; }
                if (BTNEnabled != null) { rkillbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { rkillbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { rkillchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { rkillchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { rkillchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { rkillchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (rkillchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { rkillcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { rkillprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == autorunsvar)
            {
                if (BTNForeColor != null) { autorunsbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { autorunsbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { autorunsbtn.Text = BTNtext; }
                if (BTNEnabled != null) { autorunsbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { autorunsbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { autorunschkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { autorunschkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { autorunschkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { autorunschkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (autorunschkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { autorunscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { autorunsprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == tdssvar)
            {
                if (BTNForeColor != null) { tdssbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { tdssbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { tdssbtn.Text = BTNtext; }
                if (BTNEnabled != null) { tdssbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { tdssbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { tdsschkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { tdsschkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { tdsschkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { tdsschkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (tdsschkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { tdsscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { tdssprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == superaintivar)
            {
                if (BTNForeColor != null) { superantibtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { superantibtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { superantibtn.Text = BTNtext; }
                if (BTNEnabled != null) { superantibtn.Enabled = enablebutton; }
                if (BTNVisible != null) { superantibtn.Visible = btnisvisible; }
                if (CHKBXText != null) { superantichkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { superantichkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { superantichkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { superantichkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (superantichkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { superanticancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { superantiprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == tweakingtoolsvar)
            {
                if (BTNForeColor != null) { tweakingtoolsbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { tweakingtoolsbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { tweakingtoolsbtn.Text = BTNtext; }
                if (BTNEnabled != null) { tweakingtoolsbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { tweakingtoolsbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { tweakikngtookschkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { tweakikngtookschkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { tweakikngtookschkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { tweakikngtookschkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (tweakikngtookschkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { tweakingtoolscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { tweakingtoolsprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == avgremovalvar)
            {
                if (BTNForeColor != null) { avgremovalbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { avgremovalbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { avgremovalbtn.Text = BTNtext; }
                if (BTNEnabled != null) { avgremovalbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { avgremovalbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { avgremovalchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { avgremovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { avgremovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { avgremovalchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (avgremovalchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { avgremovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { avgremovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == sfcvar)
            {
                if (BTNForeColor != null) { sfcbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { sfcbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { sfcbtn.Text = BTNtext; }
                if (BTNEnabled != null) { sfcbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { sfcbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { sfcchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { sfcchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { sfcchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { sfcchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (sfcchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { sfccancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { sfcprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == revelationsvar)
            {
                if (BTNForeColor != null) { revelationbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { revelationbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { revelationbtn.Text = BTNtext; }
                if (BTNEnabled != null) { revelationbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { revelationbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { revelationchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { revelationchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { revelationchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { revelationchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (revelationchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { revelationscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { revelationprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == nortonremovalvar)
            {
                if (BTNForeColor != null) { nortonremovalbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { nortonremovalbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { nortonremovalbtn.Text = BTNtext; }
                if (BTNEnabled != null) { nortonremovalbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { nortonremovalbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { nortonremovalchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { nortonremovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { nortonremovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { nortonremovalchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (nortonremovalchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { nortonremovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { nortonremovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mcafferemovalvar)
            {
                if (BTNForeColor != null) { mcafeeremovalbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { mcafeeremovalbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { mcafeeremovalbtn.Text = BTNtext; }
                if (BTNEnabled != null) { mcafeeremovalbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { mcafeeremovalbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { mcafferemovalchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { mcafferemovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { mcafferemovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { mcafferemovalchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (mcafferemovalchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { mcafferemovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { mcafferemovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == roguekiller32var || fileexename == roguekiller64var)
            {
                if (BTNForeColor != null) { roguekillerbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { roguekillerbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { roguekillerbtn.Text = BTNtext; }
                if (BTNEnabled != null) { roguekillerbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { roguekillerbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { roguekillerchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { roguekillerchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { roguekillerchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { roguekillerchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (roguekillerchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { roguekillercancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { roguekillerprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == Esetvar)
            {
                if (BTNForeColor != null) { esetbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { esetbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { esetbtn.Text = BTNtext; }
                if (BTNEnabled != null) { esetbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { esetbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { esetchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { esetchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { esetchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { esetchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (esetchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { esetcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { esetprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == produkeyvar)
            {
                if (BTNForeColor != null) { produkeybtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { produkeybtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { produkeybtn.Text = BTNtext; }
                if (BTNEnabled != null) { produkeybtn.Enabled = enablebutton; }
                if (BTNVisible != null) { produkeybtn.Visible = btnisvisible; }
                if (CHKBXText != null) { produkeychkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { produkeychkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { produkeychkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { produkeychkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (produkeychkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { produkeycancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { produkeyprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == hjtvar)
            {
                if (BTNForeColor != null) { hjtbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { hjtbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { hjtbtn.Text = BTNtext; }
                if (BTNEnabled != null) { hjtbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { hjtbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { hjtchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { hjtchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { hjtchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { hjtchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (hjtchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { hjtcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { hjtprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == pcdecrapvar)
            {
                if (BTNForeColor != null) { pcdecrapbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { pcdecrapbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { pcdecrapbtn.Text = BTNtext; }
                if (BTNEnabled != null) { pcdecrapbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { pcdecrapbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { pcdecrapchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { pcdecrapchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { pcdecrapchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { pcdecrapchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (pcdecrapchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { pcdecrapcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { pcdecrapprgsbr.Visible = prgsbrVisableHidden; }
            }

            if (fileexename == revovar)
            {

                if (BTNForeColor != null) { revobtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { revobtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { revobtn.Text = BTNtext; }
                if (BTNEnabled != null) { revobtn.Enabled = enablebutton; }
                if (BTNVisible != null) { revobtn.Visible = btnisvisible; }
                if (CHKBXText != null) { revochkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { revochkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { revochkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { revochkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (revochkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { revocancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { revoprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == chromevar)
            {

                if (BTNForeColor != null) { chromebtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { chromebtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { chromebtn.Text = BTNtext; }
                if (BTNEnabled != null) { chromebtn.Enabled = enablebutton; }
                if (BTNVisible != null) { chromebtn.Visible = btnisvisible; }
                if (CHKBXText != null) { chromechkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { chromechkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { chromechkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { chromechkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (chromechkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { chromecancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { chromeprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == classicsrtvar)
            {

                if (BTNForeColor != null) { classicbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { classicbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { classicbtn.Text = BTNtext; }
                if (BTNEnabled != null) { classicbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { classicbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { classicchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { classicchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { classicchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { classicchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (classicchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { classiccancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { classicprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == teamvar)
            {

                if (BTNForeColor != null) { teambtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { teambtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { teambtn.Text = BTNtext; }
                if (BTNEnabled != null) { teambtn.Enabled = enablebutton; }
                if (BTNVisible != null) { teambtn.Visible = btnisvisible; }
                if (CHKBXText != null) { teamchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { teamchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { teamchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { teamchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (teamchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { teamcancelbr.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { teamprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == readervar)
            {

                if (BTNForeColor != null) { readerbtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { readerbtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { readerbtn.Text = BTNtext; }
                if (BTNEnabled != null) { readerbtn.Enabled = enablebutton; }
                if (BTNVisible != null) { readerbtn.Visible = btnisvisible; }
                if (CHKBXText != null) { readerchkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { readerchkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { readerchkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { readerchkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (readerchkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { readercancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { readerprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == libraofficevar)
            {

                if (BTNForeColor != null) { librebtn.ForeColor = btnforecolor; }
                if (BTNBackColor != null) { librebtn.BackColor = btnbackcolor; }
                if (BTNtext != null) { librebtn.Text = BTNtext; }
                if (BTNEnabled != null) { librebtn.Enabled = enablebutton; }
                if (BTNVisible != null) { librebtn.Visible = btnisvisible; }
                if (CHKBXText != null) { librachkbx.Text = CHKBXText; }
                if (CHKBXChecked != null) { librachkbx.Checked = checkedvar; }
                if (CHKBXEnabled != null) { librachkbx.Enabled = CHKBXEnabledDisabled; }
                if (CHKBXForeColor != null) { librachkbx.ForeColor = CHKBXForeColor; }
                if (returnischecked != null) { if (librachkbx.Checked) { return true; } }
                if (CancelBTNVisable != null) { librecancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisable != null) { libreprgsbr.Visible = prgsbrVisableHidden; }
            }
            return false;
        }
        #region Cancel buttons
        /*
         * Here you will find all cancel buttons.
         * Each one does the following in order:
         * 1) Adds the file name to the wascancled list (yes, mispelled, who cares)
         * 2) Hides the Cancel button
         * 3) Hides the corresponding progress bar
         * 4) Unchecks the corresponding checkbox
         * 
         * Thats it. It does this for each tool 30+ of them. 
         * TODO: figure out how to get the name of the control sender, 
         * and assign the correct tool associated with the button.  
         * */
        private void button1_Click_1(object sender, EventArgs e)
        {
            deletefileorcancle(callingcardvar);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            deletefileorcancle(ccleanervar);
        }

        private void JRTcanclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(jrtvar);
        }

        private void ADWcanclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(adwvar);
        }

        private void MBAMcnaclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(mbamvar);

        }

        private void Hitmancanclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(hitmanx32var);
            deletefileorcancle(hitmanx64var);
        }

        private void ninitecanclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(nintievar);
        }

        private void MBAEcancelbutton_Click(object sender, EventArgs e)
        {
            deletefileorcancle(mbaevar);
        }

        private void Uncheckycanclebtn_Click(object sender, EventArgs e)
        {

            deletefileorcancle(uncheckvar);
        }

        private void AVGcanclebtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(AVG2014var);
        }


        private void ABPcanclebtn_Click(object sender, EventArgs e)
        {

            deletefileorcancle(abpievar);
        }

        private void tdsscancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(tdssvar);
        }

        private void superanticancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(superaintivar);
        }

        private void tweakingtoolscancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(tweakingtoolsvar);
        }

        private void simplesystemcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(avgremovalvar);
        }

        private void sfccancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(sfcvar);
        }

        private void revelationscancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(revelationsvar);
        }

        private void nortonremovalcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(nortonremovalvar);
        }

        private void mcafferemovalcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(mcafferemovalvar);
        }

        private void roguekillercancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(roguekiller32var);
            deletefileorcancle(roguekiller64var);
        }

        private void esetcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(Esetvar);
        }

        private void produkeycancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(produkeyvar);
        }

        private void killemallcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(killemallvar);
        }

        private void rkillcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(rkillvar);
        }

        private void autorunscancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(autorunsvar);
        }
        private void hjtcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(hjtvar);
        }
        private void pcdecrapcancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(pcdecrapvar);
        }
        private void revocancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(revovar);
        }

        private void chromecancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(chromevar);
        }

        private void readercancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(readervar);
        }

        private void librecancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(libraofficevar);
        }

        private void classiccancelbtn_Click(object sender, EventArgs e)
        {
            deletefileorcancle(classicsrtvar);
        }

        private void teamcancelbr_Click(object sender, EventArgs e)
        {
            deletefileorcancle(teamvar);
        }
        #endregion
        #region Launch buttons

        /*
         * This is where all the "Launch"/"Avalible Online" buttons events are.
         * The buttons do the following, in order:
         * 
         * 1) checks if the button's text is "Avalible Online", if not then turns button green
         * 2) Combines the savetoo file path with a slash, and the file's name
         * 3) Runs the method "opentools" and sends the combined text from step 2
         * 
         * TODO: See if there is a way to get the file's name from ((Control)sender)'s varible name
         * ~maybe something like this: string buttonName = ((Control)sender).Name.Replace("btn","");
         * ~then maybe a: foreach (string x in toolnames){if (x.contains(buttonname)){//then do stuff;}}
         * */
        private void Hitmanbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; }; if (is64bit == true)
            {
                string run = savetoo + "\\" + hitmanx64var;
                opentools(run);
            }
            else
            {
                string run = savetoo + "\\" + hitmanx32var;
                opentools(run);

            }
        }
        private void ccbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + ccleanervar;
            opentools(run);
        }

        private void JRTbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + jrtvar;
            opentools(run);
        }

        private void ADWbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + adwvar;
            opentools(run);
        }

        private void MBAMbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + mbamvar;
            opentools(run);
        }
        private void ninitebtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + nintievar;
            opentools(run);
        }

        private void MBAEbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + mbaevar;
            opentools(run);
        }

        private void Unchkbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + uncheckvar;
            opentools(run);
        }

        private void AVGbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + AVG2014var;
            opentools(run);
        }

        private void ABPbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + abpievar;
            opentools(run);
        }
        private void callingcardbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + callingcardvar;
            opentools(run);
        }

        private void tdssbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + tdssvar;
            opentools(run);
        }

        private void superantibtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + superaintivar;
            opentools(run);
        }

        private void tweakingtoolsbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + tweakingtoolsvar;
            opentools(run);
        }

        private void simplesystembtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + avgremovalvar;
            opentools(run);
        }

        private void sfcbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + sfcvar;
            opentools(run);
        }

        private void revelationbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + revelationsvar;
            opentools(run);
        }

        private void nortonremovalbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + nortonremovalvar;
            opentools(run);
        }

        private void mcafeeremovalbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + mcafferemovalvar;
            opentools(run);
        }

        private void roguekillerbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run;
            if (is64bit)
            {
                run = savetoo + "\\" + roguekiller64var;
            }
            else
            {
                run = savetoo + "\\" + roguekiller32var;
            }
            opentools(run);
        }

        private void esetbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + Esetvar;
            opentools(run);
        }

        private void produkeybtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + produkeyvar;
            opentools(run);
        }

        private void killemallbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + killemallvar;
            opentools(run);
        }

        private void rkillbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + rkillvar;
            opentools(run);
        }

        private void autorunsbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + autorunsvar;
            opentools(run);
        }
        private void hjtbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + hjtvar;
            opentools(run);
        }
        private void pcdecrapbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + pcdecrapvar;
            opentools(run);
        }
        private void teambtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + teamvar;
            opentools(run);
        }

        private void revobtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + revovar;
            opentools(run);
        }

        private void chromebtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + chromevar;
            opentools(run);
        }

        private void readerbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + readervar;
            opentools(run);
        }

        private void librebtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + libraofficevar;
            opentools(run);
        }

        private void classicbtn_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Text != "Avalible Online!") { ((Control)sender).ForeColor = Color.Green; };
            string run = savetoo + "\\" + classicsrtvar;
            opentools(run);
        }

        #endregion
        #region Main stage buttons, excluding launch buttons
        /*
         * Here is were the main stage buttons are. 
         * Deploy all checked
         * Deploy All standard tools
         * Deploy All advanced tools
         * Deploy All tools
         * Cancel button
         * and Enabled MSI installer
         * */
        private void nuke_Click(object sender, EventArgs e)
        {
            stopdownloadingshit();
            deletethisshit();
        }
        private void DeployAllBTN_Click(object sender, EventArgs e)
        {
            checkregulartools();
            deploy();
        }

        private void DeployBTN_Click(object sender, EventArgs e)
        {
            deploy();
        }
        private void msiinstallerbtn_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(enabmemsiinstaller)).Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            checkregulartools();
            checkalladvanced();
            deploy();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            checkalladvanced();
            deploy();
        }
        private void cancleallbtn_Click(object sender, EventArgs e)
        {
            try
            {
                stopdownloadingshit();
            }
            catch (Exception x)
            {
                MessageBox.Show("Cancle all button crashed: " + x.Message);
            }
        }
        #endregion
        #region Tool Strip menu item events
        /*
         * Here are all the random buttons I created in the "File" menu
         * */
        private void CF6Notes_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {
                //cmdPath = CF6Notes.Text;
                //new Thread(new ThreadStart(cmd)).Start();

                this.ActiveControl = logwindow;
                writesaveddata();
            }
        }
        private void passwordenterbutton(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {
                checkpassword();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            checkpassword();
        }
        private void resetStageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activedownloads.Count > 0)
            {
                updatelog("Unable to reset stage while there are active downloads");
            }
            else
            {
                hidealllaunchbuttons();
                scanandreportdone();
                checkforsaveddata();
                updatelog("Stage reset");
            }
        }

        private void scanServerBETAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activedownloads.Count > 0)
            {
                updatelog("Unable to scan server while there are active downloads");
            }
            else
            {
                checkifshitisonline();
            }
        }

        private void loadGreenButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkforsaveddata();
        }

        private void saveGreenButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writesaveddata();
        }

        private void showCF6NotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CF6Notes.Visible == false)
            {
                CF6Notes.Visible = true;
            }
            else
            {
                CF6Notes.Visible = false;
                writesaveddata();
            }
        }
        private void toolsToolStripMenuItem_DoubleClick(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem_DoubleClick(object sender, EventArgs e)
        {

        }

        private void deleteConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string x = savetoo + "\\" + configfile;

            if (File.Exists(x))
            {
                try
                {
                    File.Delete(x);
                    hidealllaunchbuttons();
                    scanandreportdone();
                }
                catch (Exception crash)
                {
                    eventlog("Unable to delete config file. Error:\n" + crash.Message);
                    updatelog(crash.Message);
                }
            }


        }

        private void resetCF6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CF6Notes.Text = CF6NotesDefaultText;
        }

        private void resetHitmanTrialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(systemrootdrive + "ProgramData\\HitmanPro") && !Directory.Exists(systemrootdrive + "Users\\All Users\\HitmanPro"))
            {
                updatelog("Nothing to reset");
                return;
            }

            updatelog("Resetting Hitman trial");
            try
            {
                System.IO.File.Delete(systemrootdrive + "ProgramData\\HitmanPro\\HitmanPro.key");
                System.IO.File.Delete(systemrootdrive + "ProgramData\\HitmanPro\\HitmanPro.lic");
                System.IO.File.Delete(systemrootdrive + "Users\\All Users\\HitmanPro\\HitmanPro.key");
                System.IO.File.Delete(systemrootdrive + "Users\\All Users\\HitmanPro\\HitmanPro.lic");
                System.Threading.Thread.Sleep(1000);
                if (File.Exists(systemrootdrive + "ProgramData\\HitmanPro\\HitmanPro.lic") || File.Exists(systemrootdrive + "ProgramData\\HitmanPro\\HitmanPro.key"))
                {
                    updatelog("Unable to reset Hitman trial. Please make sure Hitman is closed and try again.");
                }
                else
                {
                    updatelog("Successfully reset Hitman");
                }
            }
            catch (Exception crash)
            {
                eventlog("Reset Hitman:\n" + crash.Message);
                updatelog(crash.Message + "\nPlease try again");
            }
        }

        private void md5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            md5scan();
            md5compair();
        }

        private void mD5SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            md5scan();
            md5write();
        }

        private void checkSMARTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkSMART();
        }

        private void clipboardTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parsembamresults();
        }

        private void mRebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown", "/r /f /t 300");
        }
        private void googleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://google.com");
        }

        private void sRebootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown", "/r /f /t 30");
        }



        private void systemDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!systemDriveToolStripMenuItem.Checked == true && applicationFolderToolStripMenuItem.Checked == true)
            {
                if (activedownloads.Count > 0)
                {
                    updatelog("Unable to switch tool folder while there are active downloads");
                }
                else
                {
                    systemDriveToolStripMenuItem.Checked = true;
                    applicationFolderToolStripMenuItem.Checked = false;
                    applicationFolderToolStripMenuItem.Checked = false;
                    updatelog("Switching to System Drive");
                    savetoo = oldsaveto;
                    hidealllaunchbuttons();
                    updatelog("Scanning new folder");
                    scanandreportdone();
                    checkforsaveddata();
                    updatelog("Switch to System Drive completed");
                }
            }
        }
        private void applicationFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!applicationFolderToolStripMenuItem.Checked == true && systemDriveToolStripMenuItem.Checked == true)
            {
                if (activedownloads.Count > 0)
                {
                    updatelog("Unable to switch tool folder while there are active downloads");
                    applicationFolderToolStripMenuItem.Checked = false;
                }
                else
                {
                    applicationFolderToolStripMenuItem.Checked = true;
                    systemDriveToolStripMenuItem.Checked = false;
                    updatelog("Switching to Application folder");
                    savetoo = localremotetools;
                    hidealllaunchbuttons();
                    updatelog("Scanning new folder");
                    scanandreportdone();
                    checkforsaveddata();
                    updatelog("Switch to Application folder completed");
                }
            }
        }
        private void runafterdownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(runafterdownload)).Start();
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uncheckall();
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkregulartools();
            checkalladvanced();
        }

        #endregion



        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Form1_MouseDown(object sender,
        System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                //Hide();
            }
        }
        private void notifyIcon1_DoubleClick(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void setURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Uri.EscapeDataString
            baseurl = "http://" + CF6Notes.Text.Trim().Replace(" ", "%20");
            updatelog("baseURL is now: " + baseurl);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private void downloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkregulartools();
            checkalladvanced();
            deploy();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                cmdwindow f2 = new cmdwindow();
                f2.Show();

            }
            else
            {
                if (!myBGWorker.IsBusy)
                {
                    backgroundworkeriscancled = false;
                    updatelog("MD5 Calculator will start " + idlethreashhold + " munites after computer goes idle.");
                    watcher();
                }
                else
                {
                    updatelog("MD5 Calculator canceled.");
                    backgroundworkeriscancled = true;
                    myBGWorker.CancelAsync();

                }

            }
        }
        private void watcher()
        {
            myBGWorker.DoWork += new DoWorkEventHandler(myBGWorker_DoWork);
            myBGWorker.ProgressChanged += new ProgressChangedEventHandler(myBGWorker_ProgressChanged);
            myBGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myBGWorker_RunWorkerCompleted);
            myBGWorker.WorkerSupportsCancellation = true;
            myBGWorker.WorkerReportsProgress = true;
            myBGWorker.RunWorkerAsync();
        }
        private void myBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            double seconds = 0.0;
            while (!myBGWorker.CancellationPending)
            {

                // Get the system uptime
                int systemUptime = Environment.TickCount;
                // The tick at which the last input was recorded
                int LastInputTicks = 0;
                // The number of ticks that passed since last input
                int IdleTicks = 0;

                // Set the struct
                LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
                LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
                LastInputInfo.dwTime = 0;

                // If we have a value from the function
                if (GetLastInputInfo(ref LastInputInfo))
                {
                    // Get the number of ticks at the point when the last activity was seen
                    LastInputTicks = (int)LastInputInfo.dwTime;

                    // Number of idle ticks = system uptime ticks - number of ticks at last input
                    IdleTicks = systemUptime - LastInputTicks;
                }

                // Set the labels; divide by 1000 to transform the milliseconds to seconds
                seconds = IdleTicks / 1000;
                System.Threading.Thread.Sleep(1000);

                int percentagethingy = (int)((seconds / (idlethreashhold * 60)) * 100);

                if (seconds != 0)
                {
                    double u = (seconds / (idlethreashhold * 60)) * 100;
                    myBGWorker.ReportProgress(Convert.ToInt32(u));
                }
                else
                {
                    myBGWorker.ReportProgress(0);
                }
                if (seconds == idlethreashhold * 60)
                {
                    progressBar1.Value = 0;
                    idlethreashhold += idlethreashhold;
                }
            }
        }
        private void myBGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private void myBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            if (!backgroundworkeriscancled)
            {
                cmdwindow f2 = new cmdwindow();
                f2.Show();
            }
            //watcher();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}

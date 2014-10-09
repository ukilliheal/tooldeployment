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

/*
 * Its a mess, yes. I am still in the process of learning C sharp . 
 * I am working on adding more comments to better explain the code. Bare with me here.
 * */
namespace ToolDeployment
{
    public partial class Form1 : Form
    {

        #region Vars being defined //Edit to add new Tools (Buttons, Progress bars, etc)

        #region Edit to add new Tools (Buttons, Progress bars, etc)
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

        string ahkvar = "AutoHotkey.exe";

        string CF6NotesDefaultText = "MS:?|Apz:?|TMP:?|REG:?|JRT:?|ADW:?|MB:?|SFC:?|CD:?|HM:? || APB:?|Unchk:?|Mbae:?|AVG:?|WU:?|NN:?";
        #endregion
        string applicationpath = Application.StartupPath;
        string localremotetools = "";
        string baseurl = "http://www.example.com/"; //If your file was located at www.example.com/file.exe then you want this: http://www.example.com/
        string secondurl = ""; //not being used right now. 

        string savetoo = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string oldsaveto = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string systemrootdrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
        Boolean is64bit = false;
        Random random = new Random();
        private const uint WM_COMMAND = 0x0111;
        private const int BN_CLICKED = 245;
        private const int IDOK = 1;
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

        public Form1()
        {


            Shown += Form1_Shown;
            this.FormClosing += Form1_FormClosing;

            InitializeComponent();


        }
        private void Form1_Load(object sender, EventArgs e)
        {

            int index = random.Next(names.Count);
            var name = names[index];
            this.Text = name;
        }
        private void updatelog(string x)
        {
            try
            {
                logwindow.Text += x + Environment.NewLine;
                logwindow.SelectionStart = logwindow.Text.Length;
                logwindow.ScrollToCaret();
            }
            catch (Exception crash)
            {
            }
        }
        private void checkifshitisonline() //TODO check backup URLS if baseurl fails.
        {
            foreach (string w in toolnames)
            {
                string url = baseurl + w;
                Boolean isonline = true;
                HttpWebRequest request = WebRequest.Create(url.Trim()) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = null;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    isonline = false;
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }

                if (isonline && !reporteddone.Contains(w))
                {
                    filesavablibleonline.Add(w);
                }
                Application.DoEvents();
            }
            foreach (string y in filesavablibleonline)
            {
                if (!reporteddone.Contains(y))
                {
                    changebutton(y, null, "Avalible Online!", "Red", "true", "true", null, null, null, null);
                }
            }
        }
        private void stopdownloadingshit()
        {
            updatelog("Stopping all downloads. Please wait...");
            foreach (string p in activedownloads)
            {
                wascancled.Add(p);
                filesavablibleonline.Add(p);
            }
            new Thread(new ThreadStart(deleteit)).Start();
            string g = "Cleaning up downloads, Please wait...";
            while (filesavablibleonline.Count > 0)
            {
                updatelog(g);
                g = g + ".";
                Application.DoEvents();
                System.Threading.Thread.Sleep(1000);
            }
            hidealllaunchbuttons();
            scanandreportdone();
            checkforsaveddata();
        }
        private void checkSMART()
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
                    var hdd = new HDD();
                    hdd.Model = drive["Model"].ToString().Trim();
                    hdd.Type = drive["InterfaceType"].ToString().Trim();
                    dicDrives.Add(iDriveIndex, hdd);
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
                    updatelog("- - - -");
                    if (!drive.Value.IsOK)
                    {
                        MessageBox.Show("S.M.A.R.T Error");
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
                    updatelog("- - - -");
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

        private void copytosystemdrive()
        {
            updatelog("Copying to all files to system drive");
            if (!Directory.Exists(oldsaveto))
            {
                System.IO.Directory.CreateDirectory(oldsaveto);
            }
            if (localremotetools != oldsaveto)
            {
                foreach (string x in toolnames)
                {
                    System.Threading.Thread.Sleep(5000);
                    if (File.Exists(localremotetools + "\\" + x) && !File.Exists(oldsaveto + "\\" + x))
                    {
                        File.Copy(localremotetools + "\\" + x, oldsaveto + "\\" + x);
                    }
                }
                savetoo = oldsaveto;
                checkifshitexists();
                scanandreportdone();
            }
        }
        private void checkifshitexists()
        {
            updatelog("Checking if folders already exist");
            if (Directory.Exists(@localremotetools))
            {
                oldsaveto = savetoo.Trim();
                savetoo = localremotetools;
                applicationFolderToolStripMenuItem.Checked = true;
                systemDriveToolStripMenuItem.Checked = false;
            }
            updatelog("Using: " + savetoo);
        }
        private void scanandreportdone()
        {
            foreach (string x in toolnames)
            {
                if (File.Exists(savetoo + "\\" + x) && !reporteddone.Contains(x))
                {
                    updatelog(x + " already exists. Reporting as done. ");
                    reportdone(x);
                }
            }
        }
        private Boolean checkifcancled(string x)
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
        private Boolean isalldownloadsdone()
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
        private void runafterdownload()
        {
            try
            {
                foreach (string p in toolnames)
                {
                    if (changebutton(p, "true", null, null, null, null, null, null, null, null))
                    {
                        ischecked.Add(p);
                    }
                }
                System.Threading.Thread.Sleep(1000);
                foreach (string x in ischecked)
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
                //TODO: figure out how to update the below while in a thread
                //updatelog(crash.Message);
                //MessageBox.Show("Chrased");
            }
        }
        private void deletethisshit()
        {
            updatelog("Deleting files...");
            try
            {
                System.IO.Directory.Delete(savetoo, true);
                System.Threading.Thread.Sleep(1000);
                if (Directory.Exists(savetoo))
                {
                    updatelog("Unable to Delete: \n" + savetoo);
                }
                else
                {
                    updatelog("Successfully Deleted:\n" + savetoo);
                    hidealllaunchbuttons();
                }
            }
            catch (Exception crash)
            {
                updatelog(crash.Message);
                hidealllaunchbuttons();
                scanandreportdone();
                checkforsaveddata();
            }
        }
        private void downloadshit(string x)
        {
            if (Directory.Exists(savetoo))
            {
                if (File.Exists(savetoo + "\\" + x))
                {
                    if (!reporteddone.Contains(x) && !activedownloads.Contains(x))
                    {
                        reportdone(x);
                    }
                }
                else
                {
                    cancleallbtn.Enabled = true;
                    activedownloads.Add(x);
                    updatelog("Downloading " + x);
                    listdownloads f2 = new listdownloads();
                    f2.downloadwimfile(x, savetoo + "\\", baseurl, true);
                }
            }
        }
        private void enabmemsiinstaller()
        {
            updatelog("Enabling MSI Installer");
            try
            {
                System.Diagnostics.Process.Start("cmd", "/c REG ADD \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Network\\MSIServer\\" + " /VE /T REG_SZ /F /D \"Service\"").WaitForExit();
                System.Diagnostics.Process.Start("cmd", "/c net start msiserver").WaitForExit(); //net start msiserver
            }
            catch (Exception crash)
            {
                updatelog(crash.Message);
            }
        }
        private void opentools(string run)
        {
            string x = run.Replace(savetoo + "\\", "").Trim();
            changebutton(x, null, null, null, null, null, "false", null, null, null);
            if (filesavablibleonline.Contains(x))
            {
                changebutton(x, null, "Avalible Online!", "Red", null, null, "true", null, null, null);
                filesavablibleonline.Remove(x);
                deploy();
                return;
            }
            else
            {
                try
                {
                    updatelog("Running " + x);

                    if (!waslaunched.Contains(x))
                    {
                        waslaunched.Add(x);
                    }
                    writesaveddata();
                    ///
                    string switches = "";
                    foreach (string i in automationlist)
                    {
                        if (i.Contains(x))
                        {
                            switches = i.Replace(x, "").Trim();
                        }
                    }

                    System.Diagnostics.Process.Start(run, switches);
                }
                catch (Exception crash)
                {
                    updatelog(crash.Message);
                }
                return;
            }
        }
        public void checkforsaveddata()
        {
            string x = savetoo + "\\tdconfig.txt";
            if (File.Exists(x))
            {
                updatelog("Saved congifuration detected. Loading settings...");
                string[] lines = File.ReadAllLines(savetoo + "\\tdconfig.txt");
                foreach (string l in lines)
                {
                    if (!waslaunched.Contains(l))
                    {
                        waslaunched.Add(l);
                    }
                }
                processsaveddata();
            }
        }
        public void processsaveddata()
        {
            string y = "";
            foreach (string x in waslaunched)
            {
                if (x.Contains("CF6NOTES"))
                {

                    y = x;
                }
                else
                {
                    updatelog("Updating " + x + "'s button");
                    changebutton(x, null, "Launch", "Green", null, null, null, null, null, null);
                }
            }
            updatelog("Loading CF6 notes");
            CF6Notes.Text = y.Replace("CF6NOTES==", "");
        }
        public void writesaveddata()
        {
            string u = "";
            foreach (string k in waslaunched)
            {
                if (k.Contains("CF6NOTES=="))
                {
                    u = k;
                }
            }
            waslaunched.Remove(u);
            waslaunched.Add("CF6NOTES==" + CF6Notes.Text);
            if (!Directory.Exists(savetoo))
            {
                System.IO.Directory.CreateDirectory(savetoo);
            }
            else if (File.Exists(savetoo + "\\tdconfig.txt"))
            {
                File.Delete(savetoo + "\\tdconfig.txt");
            }
            if (!File.Exists(savetoo + "\\tdconfig.txt"))
            {
                updatelog("Saving congifuration file");
                System.IO.File.WriteAllLines(@savetoo + "\\tdconfig.txt", waslaunched.Cast<string>().ToArray());
            }
        }
        private void deleteit()
        {
            List<string> TEMPfilestoobedeleted = new List<string> { };
            foreach (string j in filesavablibleonline)
            {
                TEMPfilestoobedeleted.Add(j);
            }
            foreach (string p in TEMPfilestoobedeleted)
            {
                string deletefile = savetoo + "\\" + p;
                while (File.Exists(deletefile))
                {
                    System.Threading.Thread.Sleep(1000);
                    try
                    {
                        updatelog("Deleting " + p);
                        File.Delete(deletefile);
                    }
                    catch
                    {
                    }
                }
                filesavablibleonline.Remove(p);
            }
        }
        public void checkforupdates()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 40;
            int checkcount = 20;
            string d = applicationpath + "\\AutoUpdater.NET.dll";
            string filenameaun = "AutoUpdater.NET.dll";

            if (!File.Exists(d))
            {
                updatelog("Downloading updater");
                WebClient webClient = new WebClient();
                webClient.DownloadFileAsync(new Uri(baseurl.Replace("_RemoteTools_/", "") + filenameaun), applicationpath + "\\");

            }
            while (activedownloads.Contains(filenameaun) && checkcount > 0)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
                checkcount--;
            }
            if (File.Exists(d))
            {
                updatelog("Checking for updates");
            }
            else
            {
                updatelog("Unable to check for updates");
            }

        }
        private void md5scan()
        {
            md5hash.Clear();
            foreach (string x in toolnames)
            {
                using (var md5 = MD5.Create())
                {
                    if (File.Exists(savetoo + "\\" + x))
                    {
                        using (var stream = File.OpenRead(savetoo + "\\" + x))
                        {
                            string f = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                            md5hash.Add(x + "==" + f);
                        }
                    }
                }
            }
        }
        private void md5compair()
        {
            string x = savetoo + "\\md5.txt";
            if (!File.Exists(x))
            {
                downloadshit("md5.txt");
            }
            while (activedownloads.Contains("md5.txt"))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }
            if (File.Exists(x))
            {
                List<string> md5hashtemp = new List<string> { };
                List<string> founderror = new List<string> { };
                string[] lines = File.ReadAllLines(savetoo + "\\md5.txt");
                foreach (string l in lines)
                {
                    if (!md5hashtemp.Contains(l))
                    {
                        md5hashtemp.Add(l);
                    }
                }
                foreach (string xa in reporteddone)
                {
                    string temp = "";
                    string temp2 = "";
                    foreach (string l in md5hashtemp)
                    {
                        if (l.Contains(xa))
                        {
                            temp = l.Replace(xa + "==", "").Trim();
                        }
                    }
                    foreach (string l in md5hash)
                    {
                        if (l.Contains(xa))
                        {
                            temp2 = l.Replace(xa + "==", "").Trim();
                        }
                    }
                    if (temp != temp2)
                    {
                        founderror.Add("Error: " + xa + "\n-Scanned File: " + temp + "\n-Original File: " + temp2);
                    }
                    else
                    {
                        updatelog(xa + " md5 checked");
                    }
                }
                if (founderror.Count > 0)
                {
                    foreach (string l in founderror)
                    {
                        updatelog(l);
                    }
                }
                else
                {
                    updatelog("All files check out");
                }
            }
        }
        private void md5write()
        {
            if (File.Exists(savetoo + "\\md5.txt"))
            {
                File.Delete(savetoo + "\\md5.txt");
            }
            if (!File.Exists(savetoo + "\\md5.txt"))
            {
                updatelog("Saving md5s");
                System.IO.File.WriteAllLines(@savetoo + "\\md5.txt", md5hash.Cast<string>().ToArray());
                updatelog("Saved...");
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Text = "Closing, please wait...";
            updatelog("Closing. Please wait...");
            this.Hide();
            stopdownloadingshit();
        }
        private void Form1_Shown(object sender, EventArgs e) //Edit this if you want to add new files to be downloaded
        {
            if (IntPtr.Size == 8)
            {
                is64bit = true;
                //updatelog("Is 64bit OS");
            }
            automationlist.Add(hitmanx32var + "/scan /noinstall");
            automationlist.Add(hitmanx64var + "/scan /noinstall");
            automationlist.Add(autorunsvar + "-e");
            automationlist.Add(hjtvar + "/autolog");
            automationlist.Add(mbaevar + "/silent");
            automationlist.Add(uncheckvar + "-install");
            //automationlist.Add(AVG2014var + "/UILevel=silent /InstallToolbar=0 /ChangeBrowserSearchProvider=0 /SelectedLanguage=1033 /InstallSidebar=0 /ParticipateProductImprovement=0 /DontRestart /KillProcessesIfNeeded");
            automationlist.Add(abpievar + "/passive /norestart");
            automationlist.Add(callingcardvar + "/passive");
            automationlist.Add(tdssvar + "-accepteula -accepteulaksn -tdlfs -dcexact");
            //automationlist.Add(superaintivar + "/silent");
            //automationlist.Add(tweakingtoolsvar + "/silent");

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
                    advancedtools.Add(g);
                }
            }
            localremotetools = applicationpath + "\\_RemoteTools_";
            applicationFolderToolStripMenuItem.ToolTipText = localremotetools;
            systemDriveToolStripMenuItem.ToolTipText = oldsaveto;
            checkifshitexists();
            scanandreportdone();
            checkforsaveddata();
            parsembamresults();
            checkSMART();
            updatelog("Written by Ukilliheal on 9/11/2014. \nhttp://code.google.com/u/117050513231464874742/ \nhttp://www.youtube.com/user/ukilliheal");
            updatelog("Application loaded and ready for use");
        }
        public void reportdone(string y)
        {
            try
            {
                string o = "Launch";
                List<string> tempremovefromactivedownloads = new List<string> { };
                foreach (string p in activedownloads)
                {
                    if (p == y)
                    {
                        tempremovefromactivedownloads.Add(y);
                    }
                }
                foreach (string z in tempremovefromactivedownloads)
                {
                    activedownloads.Remove(y);
                }
                reporteddone.Add(y);
                updatelog(y + " Reported done! Activating buttons");
                changebutton(y, null, o, "Black", "true", "true", null, null, "false", "false");
                if (y == killemallvar)
                {
                    string killemallconfigdir = savetoo;
                    try
                    {
                        List<string> lines = new List<string> { "ToolDeployment.exe", "teamviewer.exe", "lmi_rescue.exe", "LMI_RE~2.exe", "CallingCard.exe", "CallingCard_srv.exe" };
                        foreach (string h in toolnames)
                        {
                            lines.Add(h);
                        }
                        if (!Directory.Exists(killemallconfigdir))
                        {
                            System.IO.Directory.CreateDirectory(killemallconfigdir);
                        }
                        if (File.Exists(killemallconfigdir + "\\KEA_Whitelist.txt"))
                        {
                            File.Delete(killemallconfigdir + "\\KEA_Whitelist.txt");
                            System.IO.File.WriteAllLines(killemallconfigdir + "\\KEA_Whitelist.txt", lines.Cast<string>().ToArray());
                        }
                        if (!File.Exists(killemallconfigdir + "\\KEA_Whitelist.txt"))
                        {
                            System.IO.File.WriteAllLines(@killemallconfigdir + "\\KEA_Whitelist.txt", lines.Cast<string>().ToArray());
                        }
                    }
                    catch (Exception crash)
                    {
                        updatelog(crash.Message);
                    }
                }
                if (isalldownloadsdone())
                {
                    cancleallbtn.Enabled = false;
                    if (runAllAfterDownloadToolStripMenuItem.Checked == true)
                    {
                        new Thread(new ThreadStart(runafterdownload)).Start();
                    }
                }
                #region original reportdone blocks of if/then statements
                #endregion
            }
            catch (Exception crash)
            {
                updatelog(crash.Message);
            }
        }
        private void checkregulartools()
        {
            updatelog("Checking all standard tools");
            foreach (string g in standardtools)
            {
                changebutton(g, null, null, null, null, null, "true", null, null, null);
            }
        }
        private void checkalladvanced()
        {
            updatelog("Checking all advanced tools");
            foreach (string g in advancedtools)
            {
                changebutton(g, null, null, null, null, null, "true", null, null, null);
            }
        }
        private void uncheckall()
        {
            updatelog("Unchecking all tools");
            foreach (string s in toolnames)
            {
                changebutton(s, null, null, null, null, null, "false", null, null, null);
            }
        }
        private void hidealllaunchbuttons()
        {
            updatelog("Resetting stage...");

            foreach (string x in toolnames)
            {
                changebutton(x, null, "Launch", "Black", "false", null, null, null, "false", "false");
            }
            reporteddone.Clear();
            uncheckall();
            activedownloads.Clear();
            wascancled.Clear();
            filesavablibleonline.Clear();
            waslaunched.Clear();
        }
        private void parsembamresults()
        {
            string x = Clipboard.GetText();
            if (x.Contains("Malwarebytes Anti-Malware"))
            {
                List<string> adduplater = new List<string> { };
                string[] result = x.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
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
                foreach (string ma in adduplater)
                {
                    int tempx = 0;
                    try
                    {
                        Int32.TryParse(ma.Trim(), out tempx);
                    }
                    catch (Exception crash)
                    {
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
        {
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
        public Boolean updateprgsbr(int x, string y) //Edit this if you want to add new files to be downloaded
        {
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
        /* // This is just my attempt at using Windows API... still trying to figure that one out.
        private void testautomation()
        {
            int hwnd = 0;
            IntPtr hwndChild = IntPtr.Zero;

            //Get a handle for the Calculator Application main window
            hwnd = FindWindow(null, "Calculator").ToInt32();
            if (hwnd == 0)
            {
                if (MessageBox.Show("Couldn't find the calculator" +
                                   " application. Do you want to start it?",
                                   "TestWinAPI",
                                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("Calc");
                }
            }
            else
            {

                //Get a handle for the "1" button
                hwndChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "1");

                //send BN_CLICKED message
                SendMessage(hwndChild, WM_COMMAND, (BN_CLICKED << 16) | IDOK, hwndChild);
                //Get a handle for the "+" button
                hwndChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "+");

                //send BN_CLICKED message
                SendMessage(hwndChild, WM_COMMAND, (BN_CLICKED << 16) | IDOK, hwndChild);
                //Get a handle for the "2" button
                hwndChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "2");

                //send BN_CLICKED message
                SendMessage(hwndChild, WM_COMMAND, (BN_CLICKED << 16) | IDOK, hwndChild);
                //Get a handle for the "=" button
                hwndChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "=");

                //send BN_CLICKED message
                SendMessage(hwndChild, WM_COMMAND, (BN_CLICKED << 16) | IDOK, hwndChild);
            }
        }
        */
        private Boolean changebutton(
            string fileexename,
            string returnischecked,
            string BTNtextNull,
            string BTNcolorNull,
            string BTNEnabledDisabledNull,
            string BTNVisibleHiddenNull,
            string CHKBXCheckUncheckNullNull,
            string CHKBXEnabledDisabledNull,
            string CancelBTNVisableHiddenNull,
            string prgsbrVisableHiddenNull)///Edit this if you want to add new files to be downloaded
        {
            Boolean checkedvar = true;
            Boolean enablebutton = false;
            Boolean showcancelBTN = false;
            Boolean btnisvisible = true;
            Boolean CHKBXEnabledDisabled = true;
            Boolean prgsbrVisableHidden = true;
            Color btncolor = Color.Black;

            if (BTNcolorNull == "Black")
            {
                btncolor = Color.Black;
            }
            else if (BTNcolorNull == "Green")
            {
                btncolor = Color.Green;
            }
            else if (BTNcolorNull == "Red")
            {
                btncolor = Color.IndianRed;
            }

            if (BTNEnabledDisabledNull == "true")
            {
                enablebutton = true;
            }
            else if (BTNEnabledDisabledNull == "false")
            {
                enablebutton = false;
            }
            if (BTNVisibleHiddenNull == "true")
            {
                btnisvisible = true;
            }
            else if (BTNVisibleHiddenNull == "false")
            {
                btnisvisible = false;
            }
            if (CHKBXCheckUncheckNullNull == "true")
            {
                checkedvar = true;
            }
            else if (CHKBXCheckUncheckNullNull == "false")
            {
                checkedvar = false;
            }
            if (CHKBXEnabledDisabledNull == "true")
            {
                CHKBXEnabledDisabled = true;
            }
            else if (CHKBXEnabledDisabledNull == "false")
            {
                CHKBXEnabledDisabled = false;
            }
            if (CancelBTNVisableHiddenNull == "true")
            {
                showcancelBTN = true;
            }
            else if (CancelBTNVisableHiddenNull == "false")
            {
                showcancelBTN = false;
            }
            if (prgsbrVisableHiddenNull == "true")
            {
                prgsbrVisableHidden = true;
            }
            else if (prgsbrVisableHiddenNull == "false")
            {
                prgsbrVisableHidden = false;
            }
            if (fileexename == ccleanervar)
            {
                if (BTNcolorNull != null) { ccbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { ccbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { ccbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { ccbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { CcleanerChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { CcleanerChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (CcleanerChkBX.Checked) { return true; } }

                if (CancelBTNVisableHiddenNull != null) { cclcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { CCprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == jrtvar)
            {
                if (BTNcolorNull != null) { JRTbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { JRTbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { JRTbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { JRTbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { JRTChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { JRTChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (JRTChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { JRTcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { JRTprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == adwvar)
            {

                if (BTNcolorNull != null) { ADWbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { ADWbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { ADWbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { ADWbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { ADWChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { ADWChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (ADWChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { ADWcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { ADWprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mbamvar)
            {
                if (BTNcolorNull != null) { MBAMbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { MBAMbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { MBAMbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { MBAMbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { MbamChkBx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { MbamChkBx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (MbamChkBx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { MBAMcnaclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { MBAMprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == hitmanx32var || fileexename == hitmanx64var)
            {
                if (BTNcolorNull != null) { Hitmanbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { Hitmanbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { Hitmanbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { Hitmanbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { HitmanChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { HitmanChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (HitmanChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { Hitmancanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { Hitmanprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == nintievar)
            {
                if (BTNcolorNull != null) { ninitebtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { ninitebtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { ninitebtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { ninitebtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { NiniteChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { NiniteChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (NiniteChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { ninitecanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { niniteprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mbaevar)
            {
                if (BTNcolorNull != null) { MBAEbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { MBAEbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { MBAEbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { MBAEbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { MBAEChkBx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { MBAEChkBx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (MBAEChkBx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { MBAEcancelbutton.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { MBAEprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == uncheckvar)
            {
                if (BTNcolorNull != null) { Unchkbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { Unchkbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { Unchkbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { Unchkbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { UncheckyChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { UncheckyChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (UncheckyChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { Uncheckycanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { unchkprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == abpievar)
            {
                if (BTNcolorNull != null) { ABPbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { ABPbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { ABPbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { ABPbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { ABPIEChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { ABPIEChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (ABPIEChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { ABPcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { APBprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == AVG2014var)
            {
                if (BTNcolorNull != null) { AVGbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { AVGbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { AVGbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { AVGbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { AVG2014ChkBX.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { AVG2014ChkBX.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (AVG2014ChkBX.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { AVGcanclebtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { AVGprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == callingcardvar)
            {
                if (BTNcolorNull != null) { callingcardbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { callingcardbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { callingcardbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { callingcardbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { callingcardchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { callingcardchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (callingcardchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { callingcardcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { callingcardprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == killemallvar)
            {
                if (BTNcolorNull != null) { killemallbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { killemallbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { killemallbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { killemallbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { killemallchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { killemallchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (killemallchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { killemallcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { killemallprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == rkillvar)
            {
                if (BTNcolorNull != null) { rkillbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { rkillbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { rkillbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { rkillbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { rkillchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { rkillchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (rkillchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { rkillcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { rkillprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == autorunsvar)
            {
                if (BTNcolorNull != null) { autorunsbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { autorunsbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { autorunsbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { autorunsbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { autorunschkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { autorunschkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (autorunschkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { autorunscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { autorunsprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == tdssvar)
            {
                if (BTNcolorNull != null) { tdssbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { tdssbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { tdssbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { tdssbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { tdsschkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { tdsschkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (tdsschkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { tdsscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { tdssprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == superaintivar)
            {
                if (BTNcolorNull != null) { superantibtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { superantibtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { superantibtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { superantibtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { superantichkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { superantichkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (superantichkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { superanticancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { superantiprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == tweakingtoolsvar)
            {
                if (BTNcolorNull != null) { tweakingtoolsbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { tweakingtoolsbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { tweakingtoolsbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { tweakingtoolsbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { tweakikngtookschkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { tweakikngtookschkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (tweakikngtookschkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { tweakingtoolscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { tweakingtoolsprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == avgremovalvar)
            {
                if (BTNcolorNull != null) { avgremovalbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { avgremovalbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { avgremovalbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { avgremovalbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { avgremovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { avgremovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (avgremovalchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { avgremovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { avgremovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == sfcvar)
            {
                if (BTNcolorNull != null) { sfcbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { sfcbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { sfcbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { sfcbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { sfcchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { sfcchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (sfcchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { sfccancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { sfcprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == revelationsvar)
            {
                if (BTNcolorNull != null) { revelationbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { revelationbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { revelationbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { revelationbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { revelationchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { revelationchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (revelationchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { revelationscancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { revelationprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == nortonremovalvar)
            {
                if (BTNcolorNull != null) { nortonremovalbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { nortonremovalbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { nortonremovalbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { nortonremovalbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { nortonremovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { nortonremovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (nortonremovalchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { nortonremovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { nortonremovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == mcafferemovalvar)
            {
                if (BTNcolorNull != null) { mcafeeremovalbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { mcafeeremovalbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { mcafeeremovalbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { mcafeeremovalbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { mcafferemovalchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { mcafferemovalchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (mcafferemovalchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { mcafferemovalcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { mcafferemovalprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == roguekiller32var || fileexename == roguekiller64var)
            {
                if (BTNcolorNull != null) { roguekillerbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { roguekillerbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { roguekillerbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { roguekillerbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { roguekillerchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { roguekillerchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (roguekillerchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { roguekillercancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { roguekillerprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == Esetvar)
            {
                if (BTNcolorNull != null) { esetbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { esetbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { esetbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { esetbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { esetchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { esetchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (esetchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { esetcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { esetprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == produkeyvar)
            {
                if (BTNcolorNull != null) { produkeybtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { produkeybtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { produkeybtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { produkeybtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { produkeychkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { produkeychkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (produkeychkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { produkeycancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { produkeyprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == hjtvar)
            {
                if (BTNcolorNull != null) { hjtbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { hjtbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { hjtbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { hjtbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { hjtchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { hjtchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (hjtchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { hjtcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { hjtprgsbr.Visible = prgsbrVisableHidden; }

            }
            if (fileexename == pcdecrapvar)
            {
                if (BTNcolorNull != null) { pcdecrapbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { pcdecrapbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { pcdecrapbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { pcdecrapbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { pcdecrapchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { pcdecrapchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (pcdecrapchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { pcdecrapcancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { pcdecrapprgsbr.Visible = prgsbrVisableHidden; }
            }

            if (fileexename == revovar)
            {

                if (BTNcolorNull != null) { revobtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { revobtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { revobtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { revobtn.Visible = btnisvisible; }

                if (CHKBXCheckUncheckNullNull != null) { revochkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { revochkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (revochkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { revocancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { revoprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == chromevar)
            {

                if (BTNcolorNull != null) { chromebtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { chromebtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { chromebtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { chromebtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { chromechkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { chromechkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (chromechkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { chromecancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { chromeprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == classicsrtvar)
            {

                if (BTNcolorNull != null) { classicbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { classicbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { classicbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { classicbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { classicchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { classicchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (classicchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { classiccancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { classicprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == teamvar)
            {

                if (BTNcolorNull != null) { teambtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { teambtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { teambtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { teambtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { teamchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { teamchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (teamchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { teamcancelbr.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { teamprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == readervar)
            {

                if (BTNcolorNull != null) { readerbtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { readerbtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { readerbtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { readerbtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { readerchkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { readerchkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (readerchkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { readercancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { readerprgsbr.Visible = prgsbrVisableHidden; }
            }
            if (fileexename == libraofficevar)
            {

                if (BTNcolorNull != null) { librebtn.ForeColor = btncolor; }
                if (BTNtextNull != null) { librebtn.Text = BTNtextNull; }
                if (BTNEnabledDisabledNull != null) { librebtn.Enabled = enablebutton; }
                if (BTNVisibleHiddenNull != null) { librebtn.Visible = btnisvisible; }
                if (CHKBXCheckUncheckNullNull != null) { librachkbx.Checked = checkedvar; }
                if (CHKBXEnabledDisabledNull != null) { librachkbx.Enabled = CHKBXEnabledDisabled; }
                if (returnischecked != null) { if (librachkbx.Checked) { return true; } }
                if (CancelBTNVisableHiddenNull != null) { librecancelbtn.Visible = showcancelBTN; }
                if (prgsbrVisableHiddenNull != null) { libreprgsbr.Visible = prgsbrVisableHidden; }
            }
            return false;

        }

        #region Buttons and shit
        private void DeployAllBTN_Click(object sender, EventArgs e)
        {
            checkregulartools();
            deploy();
        }

        private void DeployBTN_Click(object sender, EventArgs e)
        {
            deploy();
        }

        private void Hitmanbtn_Click(object sender, EventArgs e)
        {
            Hitmanbtn.ForeColor = Color.Green;
            if (is64bit == true)
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


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

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
        #region Cancle buttons
        private void button1_Click_1(object sender, EventArgs e)
        {
            wascancled.Add(callingcardvar);
            ((Control)sender).Visible = false;
            callingcardprgsbr.Visible = false;
            callingcardchkbx.Checked = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            wascancled.Add(ccleanervar);
            ((Control)sender).Visible = false;
            CCprgsbr.Visible = false;
            CcleanerChkBX.Checked = false;
        }

        private void JRTcanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(jrtvar);
            ((Control)sender).Visible = false;
            JRTprgsbr.Visible = false;
            JRTChkBX.Checked = false;
        }

        private void ADWcanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(adwvar);
            ((Control)sender).Visible = false;
            ADWprgsbr.Visible = false;
            ADWChkBX.Checked = false;
        }

        private void MBAMcnaclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(mbamvar);
            ((Control)sender).Visible = false;
            MBAMprgsbr.Visible = false;
            MbamChkBx.Checked = false;

        }

        private void Hitmancanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(hitmanx32var);
            wascancled.Add(hitmanx64var);
            ((Control)sender).Visible = false;
            Hitmanprgsbr.Visible = false;
            HitmanChkBX.Checked = false;
        }

        private void ninitecanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(nintievar);
            ((Control)sender).Visible = false;
            niniteprgsbr.Visible = false;
            NiniteChkBX.Checked = false;
        }

        private void MBAEcancelbutton_Click(object sender, EventArgs e)
        {
            wascancled.Add(mbaevar);
            ((Control)sender).Visible = false;
            MBAEprgsbr.Visible = false;
            MBAEChkBx.Checked = false;
        }

        private void Uncheckycanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(uncheckvar);
            ((Control)sender).Visible = false;
            unchkprgsbr.Visible = false;
            UncheckyChkBX.Checked = false;
        }

        private void AVGcanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(AVG2014var);
            ((Control)sender).Visible = false;
            AVGprgsbr.Visible = false;
            AVG2014ChkBX.Checked = false;
        }


        private void ABPcanclebtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(abpievar);
            ((Control)sender).Visible = false;
            APBprgsbr.Visible = false;
            ABPIEChkBX.Checked = false;
        }

        private void tdsscancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(tdssvar);
            ((Control)sender).Visible = false;
            tdssprgsbr.Visible = false;
            tdsschkbx.Checked = false;
        }

        private void superanticancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(superaintivar);
            ((Control)sender).Visible = false;
            superantiprgsbr.Visible = false;
            superantichkbx.Checked = false;
        }

        private void tweakingtoolscancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(tweakingtoolsvar);
            ((Control)sender).Visible = false;
            tweakingtoolsprgsbr.Visible = false;
        }

        private void simplesystemcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(avgremovalvar);
            ((Control)sender).Visible = false;
            avgremovalprgsbr.Visible = false;
            avgremovalchkbx.Checked = false;
        }

        private void sfccancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(sfcvar);
            ((Control)sender).Visible = false;
        }

        private void revelationscancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(revelationsvar);
            ((Control)sender).Visible = false;
            revelationprgsbr.Visible = false;
            revelationchkbx.Checked = false;
        }

        private void nortonremovalcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(nortonremovalvar);
            ((Control)sender).Visible = false;
            nortonremovalprgsbr.Visible = false;
            nortonremovalchkbx.Checked = false;
        }

        private void mcafferemovalcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(mcafferemovalvar);
            ((Control)sender).Visible = false;
            mcafferemovalprgsbr.Visible = false;
            mcafferemovalchkbx.Checked = false;
        }

        private void roguekillercancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(roguekiller32var);
            wascancled.Add(roguekiller64var);
            ((Control)sender).Visible = false;
            roguekillerprgsbr.Visible = false;
            roguekillerchkbx.Checked = false;
        }

        private void esetcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(Esetvar);
            ((Control)sender).Visible = false;
            esetprgsbr.Visible = false;
            esetchkbx.Checked = false;
        }

        private void produkeycancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(produkeyvar);
            ((Control)sender).Visible = false;
            produkeyprgsbr.Visible = false;
            produkeychkbx.Checked = false;
        }

        private void killemallcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(killemallvar);
            ((Control)sender).Visible = false;
            killemallprgsbr.Visible = false;
        }

        private void rkillcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(rkillvar);
            ((Control)sender).Visible = false;
            rkillprgsbr.Visible = false;
            rkillchkbx.Checked = false;
        }

        private void autorunscancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(autorunsvar);
            ((Control)sender).Visible = false;
            autorunsprgsbr.Visible = false;
            autorunschkbx.Checked = false;
        }
        private void hjtcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(hjtvar);
            ((Control)sender).Visible = false;
            hjtprgsbr.Visible = false;
        }
        private void pcdecrapcancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(pcdecrapvar);
            ((Control)sender).Visible = false;
            pcdecrapprgsbr.Visible = false;
        }
        private void revocancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(revovar);
            ((Control)sender).Visible = false;
            revoprgsbr.Visible = false;
        }

        private void chromecancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(chromevar);
            ((Control)sender).Visible = false;
            chromeprgsbr.Visible = false;
        }

        private void readercancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(readervar);
            ((Control)sender).Visible = false;
            readerprgsbr.Visible = false;
        }

        private void librecancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(libraofficevar);
            ((Control)sender).Visible = false;
            libreprgsbr.Visible = false;
        }

        private void classiccancelbtn_Click(object sender, EventArgs e)
        {
            wascancled.Add(classicsrtvar);
            ((Control)sender).Visible = false;
            classicprgsbr.Visible = false;
        }

        private void teamcancelbr_Click(object sender, EventArgs e)
        {
            wascancled.Add(teamvar);
            ((Control)sender).Visible = false;
            teamprgsbr.Visible = false;
        }
        #endregion
        #region Launch buttons n crap
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
        private void cancleallbtn_Click(object sender, EventArgs e)
        {
            stopdownloadingshit();
        }

        private void msiinstallerbtn_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(enabmemsiinstaller)).Start();
        }

        private void nuke_Click(object sender, EventArgs e)
        {
            stopdownloadingshit();
            deletethisshit();
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

        private void CF6Notes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.ActiveControl = logwindow;
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
            string x = savetoo + "\\tdconfig.txt";

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
                    updatelog(crash.Message);
                }
            }


        }

        private void resetCF6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CF6Notes.Text = CF6NotesDefaultText;
        }

        private void button3_Click(object sender, EventArgs e)
        {
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







    }

        #endregion

}

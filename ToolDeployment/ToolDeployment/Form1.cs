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
        //This is where I list each tool, and define the file name. 
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
        string ahkvar = "AutoHotkey.exe"; //not being used right now. 

        //I use the cf6 notes to help me keep track of the computer repair. 
        string CF6NotesDefaultText = "";

        #endregion

        string applicationpath = Application.StartupPath;
        string localremotetools = "";
        string baseurl = "http://www.example.com/"; //If your file was located at www.example.com/file.exe then you want this: http://www.example.com/
        string secondurl = ""; //not being used right now. TODO: make function to check if server is up, switch to new server if down. 

        string savetoo = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string oldsaveto = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "_RemoteTools_";
        string systemrootdrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
        Boolean is64bit = false;
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

        public Form1()
        {

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
        } 
        private void updatelog(string x)//This adds a new line to the main log window, adds the text from string x, then scrolls to bottom
        {
            //I like to show the user that something is happening. 
            //I figured the easiest way to do this would be with a text window giving updates
            try
            {
                logwindow.Text += x + Environment.NewLine; //adds a new line
                logwindow.SelectionStart = logwindow.Text.Length; //sets the selection to the start
                logwindow.ScrollToCaret(); //scrolls to the start. This makes the newest updates visible to the user
            }
            catch (Exception crash)
            {
                //just in case something can't write to the log window. Doesn't matter if it fails anyways.
                MessageBox.Show("Unable to write to log window;\n" + crash.Message);
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
                    changebutton(y, null, "Avalible Online!", "Red", "true", "true", null, null, null, null);
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
            new Thread(new ThreadStart(deleteit)).Start();
            string g = "Cleaning up downloads, Please wait...";
            while (filesavablibleonline.Count > 0)
            {
                updatelog(g);
                g = g + "."; //shows the user that something is happening. 
                Application.DoEvents(); //Allows application to catch up
                System.Threading.Thread.Sleep(1000); //no need to chew on CPU that much
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
                    reportdone(x);
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
                    if (changebutton(p, "true", null, null, null, null, null, null, null, null)) //Check if the checkbox is checked
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
                //TODO: figure out how to update the below while in a thread
                //updatelog(crash.Message);
                //MessageBox.Show("Chrashed");
            }
        }
        private void deletethisshit()//Deletes the tools
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
                        reportdone(x);
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
                updatelog("Error: " + crash.Message);
            }
        }
        private void opentools(string run)//Opens the file name in string run
        {
            string x = run.Replace(savetoo + "\\", "").Trim();
            changebutton(x, null, null, null, null, null, "false", null, null, null);
            if (filesavablibleonline.Contains(x))
            {
                //This checks if the file is in the list of files avalible from server
                //and if it is, then it re-marks the controls and then starts the download for this. 
                changebutton(x, null, "Avalible Online!", "Red", null, null, "true", null, null, null);
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

                    //Exicutes file
                    System.Diagnostics.Process.Start(run, switches);
                }
                catch (Exception crash)
                {
                    updatelog(crash.Message);
                }
                return;
            }
        }
        public void checkforsaveddata()//if config file exists, loads and processes it
        {
            //Sets file path to config file. I should change it to an INI.
            string x = savetoo + "\\tdconfig.txt";

            if (File.Exists(x))
            {
                //If the file exists, tell the user
                updatelog("Saved congifuration detected. Loading settings...");

                //then read all the lines in to a list
                string[] lines = File.ReadAllLines(savetoo + "\\tdconfig.txt");

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
        public void processsaveddata()//Changes buttons colors, and loads CF6 notes
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
                    changebutton(x, null, "Launch", "Green", null, null, null, null, null, null);
                }
            }

            //Restores CF6 field's text. 
            updatelog("Loading CF6 notes");
            CF6Notes.Text = y.Replace("CF6NOTES==", "");
        }
        public void writesaveddata()//Writes which files were opened, and CF6 notes. TODO: Add appliction location on screen
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
            else if (File.Exists(savetoo + "\\tdconfig.txt"))
            {
                //If there already is a config file, lets assume its already loaded so we can delete this one
                File.Delete(savetoo + "\\tdconfig.txt");
            }
            if (!File.Exists(savetoo + "\\tdconfig.txt"))
            {
                //Creates new config file, if old one is missing
                updatelog("Saving congifuration file");
                System.IO.File.WriteAllLines(@savetoo + "\\tdconfig.txt", waslaunched.Cast<string>().ToArray());
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
                    System.Threading.Thread.Sleep(750); //Lets not chew on the CPU too much. 
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
        public void checkforupdates() //Unused method. I am still trying to figure out how to make this work
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
            this.Text = "Closing, please wait...";
            updatelog("Closing. Please wait...");

            //Hides this window, instead of closing it
            this.Hide();

            //Stops any active downloads, which also cleans up any partial downloads
            stopdownloadingshit();

            //Now the form will close
        }
        private void Form1_Shown(object sender, EventArgs e) //Edit this if you want to add new files to be downloaded
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
            checkSMART();
            updatelog("");

            //Hey look! I did a thing!
            updatelog("Written by Ukilliheal on 9/11/2014. \nhttps://code.google.com/p/tooldeployment/");
            updatelog("Application loaded and ready for use");
        }
        private void cleanupfaileddownload(string y)//Basicly this deletes a file that was reported as done. useful to add Delete tool option 
        {

            //Tells user the download failed
            updatelog("Failed: Unable to download" + y + " Please try again");
            //Adds to cancled list 
            wascancled.Add(y);
            //Change button back to default
            changebutton(y, null, null, null, null, null, "false", null, "false", "false");

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
            if (activedownloads.Count == 0)
            {
                //TODO: Reset stage after its all done. But I think that happens anyways. 
                // hidealllaunchbuttons();
                //scanandreportdone();
                //checkforsaveddata();
            }

        }

        public void reportdone(string y)//When a file is done, or already on HDD, then this function is called, and passed the file name 
        {
            //Gets info about the file
            FileInfo fInfo = new FileInfo(@savetoo + "\\" + y);
            //Gets size in bytes
            long size = fInfo.Length;
            if (size < 1)
            {
                //If the file is 0 bytes, then lets delete it. Most likely failed download. 
                cleanupfaileddownload(y);
                return;
            }

            try
            {
                string o = "Launch";
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
                //Change the contols around to match a file being ready for launch
                changebutton(y, null, o, "Black", "true", "true", null, null, "false", "false");

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
                        updatelog(crash.Message);
                    }
                }

                //Checks if all files have finished downloading
                if (isalldownloadsdone())
                {
                    //disables the cancel all button
                    cancleallbtn.Enabled = false;
                    if (runAllAfterDownloadToolStripMenuItem.Checked == true)
                    {
                        //if user selected the option to run after download, then this runs the files. all that were checked
                        new Thread(new ThreadStart(runafterdownload)).Start();
                    }
                }
            }
            catch (Exception crash)
            {
                updatelog("reporteddone(); crashed:\n-" + crash.Message);
            }
        }
        private void checkregulartools() //Checks every tool in the standardtools list
        {
            updatelog("Checking all standard tools");
            foreach (string g in standardtools)
            {
                changebutton(g, null, null, null, null, null, "true", null, null, null);
            }
        }
        private void checkalladvanced()//Checks every tool in the advancedtools list
        {
            updatelog("Checking all advanced tools");
            foreach (string g in advancedtools)
            {
                changebutton(g, null, null, null, null, null, "true", null, null, null);
            }
        }
        private void uncheckall() //Unchecks all tools in "toolname" list
        {
            updatelog("Unchecking all tools");
            foreach (string s in toolnames)
            {
                changebutton(s, null, null, null, null, null, "false", null, null, null);
            }
        }
        private void hidealllaunchbuttons() //resets stage, as if nothing happened
        {
            updatelog("Resetting stage...");

            foreach (string x in toolnames)
            {
                changebutton(x, null, "Launch", "Black", "false", null, null, null, "false", "false");
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
        private void parsembamresults() //Pulls MBAM log from clipbaord, and outputs the good buts to the user
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

            #region I needed some way to pass either a 'true' 'false' or a 'null' to leave unchanged. Thats where this came in.
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
            stopdownloadingshit();
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
                this.ActiveControl = logwindow;
                writesaveddata();
            }
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
        #region So much fail
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
        #endregion
    }
}

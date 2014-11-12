using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ToolDeployment
{
    public partial class cmdwindow : Form
    {
        public string filepath = "cmd";
        public string filearguments = "/c " + "dir" + " " + "/B /S C:\\";
        Process proc;
        List<string> thingstowritetodrive = new List<string> { };

        public cmdwindow()
        {
            this.FormClosing += Form1_FormClosing; //adds closing event listener
            InitializeComponent();
        }
        private void cmd()
        {
            try
            {
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filepath,
                        Arguments = filearguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                //proc.Kill();
                string md5s = "";
                while (!proc.StandardOutput.EndOfStream)
                {
                    try
                    {
                        string line = proc.StandardOutput.ReadLine();
                        //For each item in the toolname list:
                        using (var md5 = MD5.Create()) //Makes MD5 thingy
                        {
                            if (File.Exists(line.Trim())) //checks if file exists 
                            {
                                using (var stream = File.OpenRead(line.Trim()))//opens the file, and allows md5 function to do its thing
                                {
                                    string f = BitConverter.ToString(md5.ComputeHash(stream));
                                    md5s = f;
                                }
                            }
                        }
                        string x = DateTime.Now.ToString();
                        string line2 = x.Replace(" ", "-").Replace("/", "-").Trim() + "," + md5s + "," + line;
                        updatelog(line2);
                        thingstowritetodrive.Add(line2);
                        Application.DoEvents();
                        //Random r = new Random();
                        //int rInt = r.Next(0, 30); //for ints
                        System.Threading.Thread.Sleep(0);
                    }
                    catch (Exception crash)
                    {
                        updatelog(crash.Message);
                    }
                }
                updatelog("\nCommand completed. Close at any time.");
                //this.Close();
            }
            catch (Exception crash)
            {
                updatelog(crash.Message);
            }
        }
        public void updatelog(string x)//This adds a new line to the main log window, adds the text from string x, then scrolls to bottom
        {
            //I like to show the user that something is happening. 
            //I figured the easiest way to do this would be with a text window giving updates
            try
            {
                logwindow.AppendText("\n" + x);
                /*
                logwindow.Text += x + Environment.NewLine; //adds a new line
                Application.DoEvents();
                logwindow.SelectionStart = logwindow.Text.Length; //sets the selection to the start
                Application.DoEvents();
                logwindow.ScrollToCaret(); //scrolls to the start. This makes the newest updates visible to the user
                 * */
                Application.DoEvents();
            }
            catch (Exception crash)
            {
                //just in case something can't write to the log window. Doesn't matter if it fails anyways.
                //Disabled because it could keep showing up over and over again

                //MessageBox.Show("Error while trying to update log;\n" + crash.Message);
            }
        }
        private void cmdwindow_Load(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//Runs a little bit of code when the form closes
        {
            try
            {
                proc.Kill();
            }
            catch (Exception crash)
            {
                if (!EventLog.SourceExists("ToolDeployment"))
                {
                    EventLog.CreateEventSource("ToolDeployment", "Application");
                }

                EventLog.WriteEntry("ToolDeployment", crash.Message, EventLogEntryType.Error);
            }
        }
        private void Form1_Shown(object sender, EventArgs e) //Edit this if you want to add new files to be downloaded
        {
            cmd();
        }
    }
}

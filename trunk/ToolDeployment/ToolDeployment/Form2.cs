using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ToolDeployment;

namespace ToolDeployment
{
    public partial class listdownloads : Form
    {

        //I copied this code from some website that I can't remember. I then modified it to suit my needs. 

        //Creates stopwatch and webclient. Stopwatch is to caculate download speed. Webclient is the downloader
        Stopwatch sw = new Stopwatch();
        WebClient webClient = new WebClient();

        /* Just some random code I am playing around with
         * 
        NetworkCredential nc = new NetworkCredential("mylog", "mypassword");
        webClient.Credentials = nc;
         used to check if there were any errors in transfering the files
        Exception err = null;
         * 
         * */

        //Defining some crap to be used later. 
        Boolean cancledl = false;
        String ppath = "";
        String x = "";
        Boolean completedDL = false;
        string filebeingdownloaded = "";
        public listdownloads()
        {
            //This does a thing. 
            InitializeComponent();
        }
        public void downloadwimfile(string filename, string savetoo, string baseURL, Boolean overrideconnectionlimit) // filebeingdownloaded = x
        {
           //Defining stuff
            filebeingdownloaded = filename;
            string url = baseURL + filename;

            //creates a new event, triggered when the file is done/cancled/errors out
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed2);

            //creats a new event, triggered when webclient does a thing. idc
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged2);

            //This sets the connection limit. just because it makes things go faster. I think. 
            if (overrideconnectionlimit)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 99999999;
            }

            //starts the stopwatch
            sw.Start();

            //this was older code used when this program spawned different windows for each download. 
            label1.Text = "Downloading " + filebeingdownloaded;

            //Defines and starts the download. 
            ppath = savetoo + filebeingdownloaded;
            webClient.DownloadFileAsync(new Uri(url), ppath);
        }
        private void ProgressChanged2(object sender, DownloadProgressChangedEventArgs e)
        {
            //this was older code used when this program spawned different windows for each download. 

            //int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
            //progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
            //labelSpeed.Text = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("000.00"));
            //progressBar1.Value = e.ProgressPercentage;
            //labelDownloaded.Text = string.Format("{0} MB's / {1} MB's",
            //(e.BytesReceived / 1024d / 1024d).ToString("0.00"),
            //(e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

            //Checks if original window is open
            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {
                //This updates the correct progress bar in the main form. The fuction also checks if the file was cancled. 
                cancledl = (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).updateprgsbr(e.ProgressPercentage, filebeingdownloaded);
            }
            if (cancledl == true)
            {
                //stops downloading things. 
                cancledownload();
            }
        }
        #region Download complete/Check for Cancel
        private void Completed2(object sender, AsyncCompletedEventArgs e)
        {
            //Download was cancled/completed/errord out, so this starts cleaning stuff up and closing stuff out. 
            sw.Stop();
            if (cancledl == true)
            {
                //if cancled then close this form.
                this.Close();
            }
            else
            {
                completedDL = true;
                //button1.Enabled = false;
                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    //reports a file as being done. main form handles the rest. 
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).reportdone(filebeingdownloaded);
                }

                //Dear Form2; Go away, no one likes you!
                this.Close();
            }
        }
        #endregion
        private void cancledownload()
        {
            //checks if form1 is open
            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {
                //updates progress to 0, resetting it. 
                cancledl = (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).updateprgsbr(0, filebeingdownloaded);
            }
            if (completedDL == false)
            {
                //File was cancled, close downlolader and delete file. 
                cancledl = true;
                webClient.CancelAsync();
                new Thread(new ThreadStart(deletefile)).Start();
            }
        }
        private void deletefile()
        {

            while (File.Exists(ppath))
            {
                try
                {

                    System.Threading.Thread.Sleep(2000);
                    //This deletes the file that was being downloaded
                    File.Delete(ppath);
                }
                catch (Exception crash)
                {
                    //aww... it crashed... and no one cares. 
                }
            }


        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            //Left over from when this form use to spwan visible windows. 
            cancledownload();
        }
    }
}

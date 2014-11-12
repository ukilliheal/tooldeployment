namespace ToolDeployment
{
    partial class cmdwindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logwindow = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // logwindow
            // 
            this.logwindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logwindow.BackColor = System.Drawing.Color.Black;
            this.logwindow.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.logwindow.DetectUrls = false;
            this.logwindow.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logwindow.ForeColor = System.Drawing.Color.White;
            this.logwindow.Location = new System.Drawing.Point(3, 2);
            this.logwindow.Name = "logwindow";
            this.logwindow.ReadOnly = true;
            this.logwindow.Size = new System.Drawing.Size(660, 301);
            this.logwindow.TabIndex = 0;
            this.logwindow.Text = "";
            // 
            // cmdwindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 304);
            this.Controls.Add(this.logwindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "cmdwindow";
            this.Text = "MD5Hash Caculator";
            this.Load += new System.EventHandler(this.cmdwindow_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logwindow;
    }
}
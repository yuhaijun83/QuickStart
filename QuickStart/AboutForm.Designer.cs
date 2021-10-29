
namespace QuickStart
{
    partial class aboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(aboutForm));
            this.button_AboutForm_Close = new System.Windows.Forms.Button();
            this.lbl_BigTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_BuildVersion = new System.Windows.Forms.Label();
            this.linkLabel_GitHub = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_AboutForm_Close
            // 
            resources.ApplyResources(this.button_AboutForm_Close, "button_AboutForm_Close");
            this.button_AboutForm_Close.Name = "button_AboutForm_Close";
            this.button_AboutForm_Close.UseVisualStyleBackColor = true;
            this.button_AboutForm_Close.Click += new System.EventHandler(this.button_AboutFormClose_Click);
            // 
            // lbl_BigTitle
            // 
            resources.ApplyResources(this.lbl_BigTitle, "lbl_BigTitle");
            this.lbl_BigTitle.Name = "lbl_BigTitle";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lbl_BuildVersion
            // 
            resources.ApplyResources(this.lbl_BuildVersion, "lbl_BuildVersion");
            this.lbl_BuildVersion.Name = "lbl_BuildVersion";
            // 
            // linkLabel_GitHub
            // 
            resources.ApplyResources(this.linkLabel_GitHub, "linkLabel_GitHub");
            this.linkLabel_GitHub.Name = "linkLabel_GitHub";
            this.linkLabel_GitHub.TabStop = true;
            this.linkLabel_GitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_GitHub_LinkClicked);
            // 
            // aboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel_GitHub);
            this.Controls.Add(this.lbl_BuildVersion);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbl_BigTitle);
            this.Controls.Add(this.button_AboutForm_Close);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "aboutForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_AboutForm_Close;
        private System.Windows.Forms.Label lbl_BigTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_BuildVersion;
        private System.Windows.Forms.LinkLabel linkLabel_GitHub;
    }
}
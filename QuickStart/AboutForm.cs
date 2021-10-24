using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickStart
{
    public partial class aboutForm : Form
    {
        public aboutForm()
        {
            InitializeComponent();
        }

        private void button_AboutFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel_GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String strTargetUrl = "https://github.com/yuhaijun83/QuickStart";
            System.Diagnostics.Process.Start("iexplore.exe", strTargetUrl);
        }
    }
}

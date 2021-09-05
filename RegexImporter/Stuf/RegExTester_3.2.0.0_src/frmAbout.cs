using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace RegExTester
{
    public partial class frmAbout : Form
    {

        public frmAbout()
        {
            InitializeComponent();
        }

        private void llHomepage1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.davidemauri.it/");
        }

        private void llProject1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.sourceforge.net/projects/regextest");
        }

        private void llProject2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.codeproject.com/KB/cs/dotnetregextest.aspx");
        }

        private void llProject3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.sourceforge.net/projects/regextester");
        }

        private void llTCPArticle3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.codeproject.com/KB/string/regextester.aspx");
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto://opablo@gmail.com");
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            titleLabel.Text += " - v" + Application.ProductVersion.ToString();
        }
    }
}

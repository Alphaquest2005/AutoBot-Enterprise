using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RegExTracer
{
	public partial class FormHelpAbout : Form
	{
		public FormHelpAbout()
		{
			InitializeComponent();
		}

		private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start((string)((Control)sender).Tag);
		}

		private void ButtonOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
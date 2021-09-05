using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace RegExTester
{
    public partial class CustomRichTextBox : RichTextBox
    {
        public CustomRichTextBox() : base()
        {
            InitializeComponent();
            //base.ContextMenuStrip = contextMenuStrip;
        }

        #region Custom extension of the SuspendLayout feature

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public new void SuspendLayout()
        {
            base.SuspendLayout();
            SendMessage(base.Handle, WM_SETREDRAW, false, 0);
        }

        public new void ResumeLayout()
        {
            this.ResumeLayout(true);
        }

        public new void ResumeLayout(bool performLayout)
        {
            SendMessage(base.Handle, WM_SETREDRAW, true, 0);
            base.ResumeLayout(performLayout);
            base.Refresh();
        }

        #endregion

        #region ContextMenu feature

        private string _lastFindString = "";

        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            base.Undo();
        }

        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            base.Redo();
        }

        private void tsmiCut_Click(object sender, EventArgs e)
        {
            base.Cut();
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            base.Copy();
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            base.Paste();
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            // I had to use this approach because the rtb doen't have a Selection-Delete function.
            SendKeys.Send("{DEL}"); // Delete key
        }

        private void tsmiFind_Click(object sender, EventArgs e)
        {
            string findString = Microsoft.VisualBasic.Interaction.InputBox("Insert text to find...", "Find Function", _lastFindString, -1, -1);

            if (string.IsNullOrEmpty(findString)) return;

            int position = base.Find(findString, 0, RichTextBoxFinds.None);
            if (position == -1)
            {
                MessageBox.Show("Text not found", "Find Function", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                base.Select(position, findString.Length);
                _lastFindString = findString;
            }
        }

        private void tsmiFindNext_Click(object sender, EventArgs e)
        {
            string findString = _lastFindString;

            if (string.IsNullOrEmpty(findString)) return;

            int position = base.Find(findString, base.SelectionStart + base.SelectionLength, RichTextBoxFinds.None);
            if (position == -1)
            {
                MessageBox.Show("No more occurrences found", "Find Next Function", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                base.Select(position, findString.Length);
            }
        }

        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            base.SelectAll();
        }


        private void tsmiWordWrap_CheckStateChanged(object sender, EventArgs e)
        {
            base.WordWrap = tsmiWordWrap.Checked;
        }

        #endregion

    }
}

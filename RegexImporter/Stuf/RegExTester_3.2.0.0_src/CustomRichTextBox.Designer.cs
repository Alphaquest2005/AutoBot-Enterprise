namespace RegExTester
{
    partial class CustomRichTextBox
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFind = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiWordWrap = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUndo,
            this.tsmiRedo,
            this.tsmiSeparator1,
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.tsmiDelete,
            this.tsmiSeparator2,
            this.tsmiFind,
            this.tsmiFindNext,
            this.tsmiSeparator3,
            this.tsmiSelectAll,
            this.tsmiSeparator4,
            this.tsmiWordWrap});
            this.contextMenuStrip.Name = "rtbTextContextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(138, 248);
            // 
            // tsmiUndo
            // 
            this.tsmiUndo.Image = global::RegExTester.Properties.Resources.Undo;
            this.tsmiUndo.Name = "tsmiUndo";
            this.tsmiUndo.Size = new System.Drawing.Size(137, 22);
            this.tsmiUndo.Text = "&Undo";
            this.tsmiUndo.Click += new System.EventHandler(this.tsmiUndo_Click);
            // 
            // tsmiRedo
            // 
            this.tsmiRedo.Image = global::RegExTester.Properties.Resources.Redo;
            this.tsmiRedo.Name = "tsmiRedo";
            this.tsmiRedo.Size = new System.Drawing.Size(137, 22);
            this.tsmiRedo.Text = "&Redo";
            this.tsmiRedo.Click += new System.EventHandler(this.tsmiRedo_Click);
            // 
            // tsmiSeparator1
            // 
            this.tsmiSeparator1.Name = "tsmiSeparator1";
            this.tsmiSeparator1.Size = new System.Drawing.Size(134, 6);
            // 
            // tsmiCut
            // 
            this.tsmiCut.Image = global::RegExTester.Properties.Resources.Cut;
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.Size = new System.Drawing.Size(137, 22);
            this.tsmiCut.Text = "Cu&t";
            this.tsmiCut.Click += new System.EventHandler(this.tsmiCut_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Image = global::RegExTester.Properties.Resources.Copy;
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(137, 22);
            this.tsmiCopy.Text = "&Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Image = global::RegExTester.Properties.Resources.Paste;
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(137, 22);
            this.tsmiPaste.Text = "&Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Image = global::RegExTester.Properties.Resources.Delete;
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(137, 22);
            this.tsmiDelete.Text = "&Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiSeparator2
            // 
            this.tsmiSeparator2.Name = "tsmiSeparator2";
            this.tsmiSeparator2.Size = new System.Drawing.Size(134, 6);
            // 
            // tsmiFind
            // 
            this.tsmiFind.Image = global::RegExTester.Properties.Resources.Find;
            this.tsmiFind.Name = "tsmiFind";
            this.tsmiFind.Size = new System.Drawing.Size(137, 22);
            this.tsmiFind.Text = "&Find...";
            this.tsmiFind.Click += new System.EventHandler(this.tsmiFind_Click);
            // 
            // tsmiFindNext
            // 
            this.tsmiFindNext.Image = global::RegExTester.Properties.Resources.FindNext;
            this.tsmiFindNext.Name = "tsmiFindNext";
            this.tsmiFindNext.Size = new System.Drawing.Size(137, 22);
            this.tsmiFindNext.Text = "Find &Next";
            this.tsmiFindNext.Click += new System.EventHandler(this.tsmiFindNext_Click);
            // 
            // tsmiSeparator3
            // 
            this.tsmiSeparator3.Name = "tsmiSeparator3";
            this.tsmiSeparator3.Size = new System.Drawing.Size(134, 6);
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Image = global::RegExTester.Properties.Resources.Plain;
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(137, 22);
            this.tsmiSelectAll.Text = "Select &All";
            this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAll_Click);
            // 
            // tsmiSeparator4
            // 
            this.tsmiSeparator4.Name = "tsmiSeparator4";
            this.tsmiSeparator4.Size = new System.Drawing.Size(134, 6);
            // 
            // tsmiWordWrap
            // 
            this.tsmiWordWrap.Checked = true;
            this.tsmiWordWrap.CheckOnClick = true;
            this.tsmiWordWrap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiWordWrap.Image = global::RegExTester.Properties.Resources.WordWrap;
            this.tsmiWordWrap.Name = "tsmiWordWrap";
            this.tsmiWordWrap.Size = new System.Drawing.Size(137, 22);
            this.tsmiWordWrap.Text = "&WordWrap";
            this.tsmiWordWrap.CheckStateChanged += new System.EventHandler(this.tsmiWordWrap_CheckStateChanged);
            // 
            // CustomRichTextBox
            // 
            this.ContextMenuStrip = this.contextMenuStrip;
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmiRedo;
        private System.Windows.Forms.ToolStripSeparator tsmiSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripSeparator tsmiSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiFind;
        private System.Windows.Forms.ToolStripMenuItem tsmiFindNext;
        private System.Windows.Forms.ToolStripSeparator tsmiSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.ToolStripSeparator tsmiSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsmiWordWrap;
    }
}

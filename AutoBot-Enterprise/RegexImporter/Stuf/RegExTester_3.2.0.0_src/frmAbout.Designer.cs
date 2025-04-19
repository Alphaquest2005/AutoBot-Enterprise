using System.Windows.Forms;

namespace RegExTester
{
    /// <summary>
    /// Summary description for FormAbout.
    /// </summary>
    partial class frmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.llHomepage1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.llProject1 = new System.Windows.Forms.LinkLabel();
            this.closeButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.llProject2 = new System.Windows.Forms.LinkLabel();
            this.llProject3 = new System.Windows.Forms.LinkLabel();
            this.llTCPArticle3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // llHomepage1
            // 
            this.llHomepage1.AutoSize = true;
            this.llHomepage1.Location = new System.Drawing.Point(367, 155);
            this.llHomepage1.Name = "llHomepage1";
            this.llHomepage1.Size = new System.Drawing.Size(112, 13);
            this.llHomepage1.TabIndex = 5;
            this.llHomepage1.TabStop = true;
            this.llHomepage1.Text = "Davide Homepage";
            this.llHomepage1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llHomepage1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llHomepage1_LinkClicked);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(506, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(11, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(508, 32);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "RegEx Tester";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Regular Expression Tester";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Originally written by Davide Mauri (2003)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // llProject1
            // 
            this.llProject1.AutoSize = true;
            this.llProject1.Location = new System.Drawing.Point(265, 155);
            this.llProject1.Name = "llProject1";
            this.llProject1.Size = new System.Drawing.Size(96, 13);
            this.llProject1.TabIndex = 4;
            this.llProject1.TabStop = true;
            this.llProject1.Text = "Project Website";
            this.llProject1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llProject1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProject1_LinkClicked);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.closeButton.Location = new System.Drawing.Point(438, 290);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(247, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Enhanced by Kurt Griffiths (2006)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(247, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Turbocharged by Pablo Osés (2009)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // llProject2
            // 
            this.llProject2.AutoSize = true;
            this.llProject2.Location = new System.Drawing.Point(265, 178);
            this.llProject2.Name = "llProject2";
            this.llProject2.Size = new System.Drawing.Size(96, 13);
            this.llProject2.TabIndex = 7;
            this.llProject2.TabStop = true;
            this.llProject2.Text = "Project Website";
            this.llProject2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llProject2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProject2_LinkClicked);
            // 
            // llProject3
            // 
            this.llProject3.AutoSize = true;
            this.llProject3.Location = new System.Drawing.Point(265, 201);
            this.llProject3.Name = "llProject3";
            this.llProject3.Size = new System.Drawing.Size(96, 13);
            this.llProject3.TabIndex = 9;
            this.llProject3.TabStop = true;
            this.llProject3.Text = "Project Website";
            this.llProject3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llProject3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llProject3_LinkClicked);
            // 
            // llTCPArticle3
            // 
            this.llTCPArticle3.AutoSize = true;
            this.llTCPArticle3.Location = new System.Drawing.Point(367, 201);
            this.llTCPArticle3.Name = "llTCPArticle3";
            this.llTCPArticle3.Size = new System.Drawing.Size(146, 13);
            this.llTCPArticle3.TabIndex = 10;
            this.llTCPArticle3.TabStop = true;
            this.llTCPArticle3.Text = "The Code Project Article";
            this.llTCPArticle3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llTCPArticle3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llTCPArticle3_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(265, 224);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(159, 13);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "E-mail: opablo@gmail.com";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(497, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "Feature contributions: Eric Lebetsamer (2010)";
            // 
            // frmAbout
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(531, 325);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.llTCPArticle3);
            this.Controls.Add(this.llProject3);
            this.Controls.Add(this.llProject2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.llProject1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.llHomepage1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.LinkLabel llHomepage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel llProject1;
        private System.Windows.Forms.Button closeButton;
        private Label label5;
        private Label label6;
        private LinkLabel llProject2;
        private LinkLabel llProject3;
        private LinkLabel llTCPArticle3;
        private LinkLabel linkLabel1;
        private Label label2;

    }
}

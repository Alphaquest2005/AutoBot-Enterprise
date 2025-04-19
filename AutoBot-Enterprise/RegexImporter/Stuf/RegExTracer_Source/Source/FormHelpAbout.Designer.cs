namespace RegExTracer
{
	partial class FormHelpAbout
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHelpAbout));
			this._pictureLogoType = new System.Windows.Forms.PictureBox();
			this._labelProduct = new System.Windows.Forms.Label();
			this._buttonOK = new System.Windows.Forms.Button();
			this._labelCopyright = new System.Windows.Forms.Label();
			this._linkSupport = new System.Windows.Forms.LinkLabel();
			this._linkRsdnForum = new System.Windows.Forms.LinkLabel();
			this._linkRsdnArticle = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this._pictureLogoType)).BeginInit();
			this.SuspendLayout();
			// 
			// _pictureLogoType
			// 
			this._pictureLogoType.Image = global::RegExTracer.Properties.Resources.LogoType300x210;
			this._pictureLogoType.Location = new System.Drawing.Point(12, 12);
			this._pictureLogoType.Name = "_pictureLogoType";
			this._pictureLogoType.Size = new System.Drawing.Size(300, 210);
			this._pictureLogoType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._pictureLogoType.TabIndex = 0;
			this._pictureLogoType.TabStop = false;
			this._pictureLogoType.WaitOnLoad = true;
			// 
			// _labelProduct
			// 
			this._labelProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelProduct.BackColor = System.Drawing.Color.Transparent;
			this._labelProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelProduct.Location = new System.Drawing.Point(224, 12);
			this._labelProduct.Name = "_labelProduct";
			this._labelProduct.Size = new System.Drawing.Size(395, 64);
			this._labelProduct.TabIndex = 0;
			this._labelProduct.Text = "RegEx Tracer v3.1\r\n.Net Regular Expression Debugger";
			this._labelProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _buttonOK
			// 
			this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._buttonOK.Location = new System.Drawing.Point(502, 202);
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.Size = new System.Drawing.Size(120, 30);
			this._buttonOK.TabIndex = 3;
			this._buttonOK.Text = "OK";
			this._buttonOK.UseVisualStyleBackColor = true;
			this._buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
			// 
			// _labelCopyright
			// 
			this._labelCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelCopyright.BackColor = System.Drawing.Color.Transparent;
			this._labelCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelCopyright.Location = new System.Drawing.Point(318, 143);
			this._labelCopyright.Name = "_labelCopyright";
			this._labelCopyright.Size = new System.Drawing.Size(304, 56);
			this._labelCopyright.TabIndex = 1;
			this._labelCopyright.Text = "Copyright © 2006, TrifleSoft\r\nThis software is provided \"AS IS\" without any warra" +
					"nty.\r\nUse it on your own risk.";
			this._labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _linkSupport
			// 
			this._linkSupport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._linkSupport.Location = new System.Drawing.Point(319, 76);
			this._linkSupport.Name = "_linkSupport";
			this._linkSupport.Size = new System.Drawing.Size(304, 23);
			this._linkSupport.TabIndex = 4;
			this._linkSupport.TabStop = true;
			this._linkSupport.Tag = "mailto:adontz@mail.ru?subject=RegEx%20Tracer";
			this._linkSupport.Text = "Customer support";
			this._linkSupport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_LinkClicked);
			// 
			// _linkRsdnForum
			// 
			this._linkRsdnForum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._linkRsdnForum.LinkArea = new System.Windows.Forms.LinkArea(0, 27);
			this._linkRsdnForum.Location = new System.Drawing.Point(321, 110);
			this._linkRsdnForum.Name = "_linkRsdnForum";
			this._linkRsdnForum.Size = new System.Drawing.Size(150, 23);
			this._linkRsdnForum.TabIndex = 5;
			this._linkRsdnForum.TabStop = true;
			this._linkRsdnForum.Tag = "http://www.rsdn.ru/forum/message.1884510.aspx";
			this._linkRsdnForum.Text = "Discussion on RSDN.ru";
			this._linkRsdnForum.UseCompatibleTextRendering = true;
			this._linkRsdnForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_LinkClicked);
			// 
			// _linkRsdnArticle
			// 
			this._linkRsdnArticle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._linkRsdnArticle.LinkArea = new System.Windows.Forms.LinkArea(0, 27);
			this._linkRsdnArticle.Location = new System.Drawing.Point(473, 110);
			this._linkRsdnArticle.Name = "_linkRsdnArticle";
			this._linkRsdnArticle.Size = new System.Drawing.Size(150, 23);
			this._linkRsdnArticle.TabIndex = 6;
			this._linkRsdnArticle.TabStop = true;
			this._linkRsdnArticle.Tag = "http://http://www.rsdn.ru/article/files/Progs/RegexTrc.xml";
			this._linkRsdnArticle.Text = "Article on RSDN.ru";
			this._linkRsdnArticle.UseCompatibleTextRendering = true;
			// 
			// FormHelpAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(634, 244);
			this.Controls.Add(this._linkRsdnArticle);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this._linkRsdnForum);
			this.Controls.Add(this._linkSupport);
			this.Controls.Add(this._labelCopyright);
			this.Controls.Add(this._labelProduct);
			this.Controls.Add(this._pictureLogoType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHelpAbout";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this._pictureLogoType)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureLogoType;
		private System.Windows.Forms.Label _labelProduct;
		private System.Windows.Forms.Label _labelCopyright;
		private System.Windows.Forms.LinkLabel _linkSupport;
		private System.Windows.Forms.LinkLabel _linkRsdnForum;
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.LinkLabel _linkRsdnArticle;
	}
}
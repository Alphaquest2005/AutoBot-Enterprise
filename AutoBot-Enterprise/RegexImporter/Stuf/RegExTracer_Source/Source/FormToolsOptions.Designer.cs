namespace RegExTracer
{
	partial class FormToolsOptions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToolsOptions));
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonOK = new System.Windows.Forms.Button();
			this._buttonColorChoose = new System.Windows.Forms.Button();
			this._buttonColorRemove = new System.Windows.Forms.Button();
			this._buttonColorAdd = new System.Windows.Forms.Button();
			this._listColorList = new System.Windows.Forms.ListBox();
			this._colorDialog = new System.Windows.Forms.ColorDialog();
			this._groupGeneral = new System.Windows.Forms.GroupBox();
			this._numericTextSize = new System.Windows.Forms.NumericUpDown();
			this._buttonForeColor = new System.Windows.Forms.Button();
			this._buttonBackColor = new System.Windows.Forms.Button();
			this._labelColor = new System.Windows.Forms.Label();
			this._groupHighlighting = new System.Windows.Forms.GroupBox();
			this._comboHighlightingType = new System.Windows.Forms.ComboBox();
			this._labelHighlightingType = new System.Windows.Forms.Label();
			this._labelTextSize = new System.Windows.Forms.Label();
			this._groupGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._numericTextSize)).BeginInit();
			this._groupHighlighting.SuspendLayout();
			this.SuspendLayout();
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.Location = new System.Drawing.Point(530, 407);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(90, 23);
			this._buttonCancel.TabIndex = 3;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			// 
			// _buttonOK
			// 
			this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOK.Location = new System.Drawing.Point(434, 407);
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.Size = new System.Drawing.Size(90, 23);
			this._buttonOK.TabIndex = 2;
			this._buttonOK.Text = "OK";
			this._buttonOK.UseVisualStyleBackColor = true;
			this._buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
			// 
			// _buttonColorChoose
			// 
			this._buttonColorChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonColorChoose.Location = new System.Drawing.Point(416, 251);
			this._buttonColorChoose.Name = "_buttonColorChoose";
			this._buttonColorChoose.Size = new System.Drawing.Size(90, 23);
			this._buttonColorChoose.TabIndex = 4;
			this._buttonColorChoose.Text = "Choose…";
			this._buttonColorChoose.UseVisualStyleBackColor = true;
			this._buttonColorChoose.Click += new System.EventHandler(this.ButtonColorChoose_Click);
			// 
			// _buttonColorRemove
			// 
			this._buttonColorRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonColorRemove.Location = new System.Drawing.Point(512, 251);
			this._buttonColorRemove.Name = "_buttonColorRemove";
			this._buttonColorRemove.Size = new System.Drawing.Size(90, 23);
			this._buttonColorRemove.TabIndex = 5;
			this._buttonColorRemove.Text = "Remove";
			this._buttonColorRemove.UseVisualStyleBackColor = true;
			this._buttonColorRemove.Click += new System.EventHandler(this.ButtonColorRemove_Click);
			// 
			// _buttonColorAdd
			// 
			this._buttonColorAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonColorAdd.Location = new System.Drawing.Point(320, 251);
			this._buttonColorAdd.Name = "_buttonColorAdd";
			this._buttonColorAdd.Size = new System.Drawing.Size(90, 23);
			this._buttonColorAdd.TabIndex = 3;
			this._buttonColorAdd.Text = "Add…";
			this._buttonColorAdd.UseVisualStyleBackColor = true;
			this._buttonColorAdd.Click += new System.EventHandler(this.ButtonColorAdd_Click);
			// 
			// _listColorList
			// 
			this._listColorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._listColorList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this._listColorList.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._listColorList.FormattingEnabled = true;
			this._listColorList.IntegralHeight = false;
			this._listColorList.Location = new System.Drawing.Point(6, 46);
			this._listColorList.Name = "_listColorList";
			this._listColorList.Size = new System.Drawing.Size(596, 199);
			this._listColorList.TabIndex = 2;
			this._listColorList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListColorList_DrawItem);
			this._listColorList.DoubleClick += new System.EventHandler(this.ListColorList_DoubleClick);
			this._listColorList.SelectedIndexChanged += new System.EventHandler(this.ListColorList_SelectedIndexChanged);
			this._listColorList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.ListColorList_MeasureItem);
			// 
			// _colorDialog
			// 
			this._colorDialog.AnyColor = true;
			this._colorDialog.FullOpen = true;
			// 
			// _groupGeneral
			// 
			this._groupGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._groupGeneral.Controls.Add(this._labelTextSize);
			this._groupGeneral.Controls.Add(this._numericTextSize);
			this._groupGeneral.Controls.Add(this._buttonForeColor);
			this._groupGeneral.Controls.Add(this._buttonBackColor);
			this._groupGeneral.Controls.Add(this._labelColor);
			this._groupGeneral.Location = new System.Drawing.Point(12, 12);
			this._groupGeneral.Name = "_groupGeneral";
			this._groupGeneral.Size = new System.Drawing.Size(608, 103);
			this._groupGeneral.TabIndex = 0;
			this._groupGeneral.TabStop = false;
			this._groupGeneral.Text = "General";
			// 
			// _numericTextSize
			// 
			this._numericTextSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._numericTextSize.DecimalPlaces = 1;
			this._numericTextSize.Location = new System.Drawing.Point(548, 77);
			this._numericTextSize.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
			this._numericTextSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this._numericTextSize.Name = "_numericTextSize";
			this._numericTextSize.Size = new System.Drawing.Size(55, 20);
			this._numericTextSize.TabIndex = 3;
			this._numericTextSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._numericTextSize.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
			this._numericTextSize.ValueChanged += new System.EventHandler(this.NumericTextSize_ValueChanged);
			// 
			// _buttonForeColor
			// 
			this._buttonForeColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonForeColor.Location = new System.Drawing.Point(482, 19);
			this._buttonForeColor.Name = "_buttonForeColor";
			this._buttonForeColor.Size = new System.Drawing.Size(120, 23);
			this._buttonForeColor.TabIndex = 1;
			this._buttonForeColor.Text = "Foreground Color…";
			this._buttonForeColor.UseVisualStyleBackColor = true;
			this._buttonForeColor.Click += new System.EventHandler(this.ButtonForeColor_Click);
			// 
			// _buttonBackColor
			// 
			this._buttonBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonBackColor.Location = new System.Drawing.Point(482, 48);
			this._buttonBackColor.Name = "_buttonBackColor";
			this._buttonBackColor.Size = new System.Drawing.Size(120, 23);
			this._buttonBackColor.TabIndex = 2;
			this._buttonBackColor.Text = "Background Color…";
			this._buttonBackColor.UseVisualStyleBackColor = true;
			this._buttonBackColor.Click += new System.EventHandler(this.ButtonBackColor_Click);
			// 
			// _labelColor
			// 
			this._labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._labelColor.Font = new System.Drawing.Font("Lucida Console", 10F);
			this._labelColor.Location = new System.Drawing.Point(6, 19);
			this._labelColor.Margin = new System.Windows.Forms.Padding(3);
			this._labelColor.Name = "_labelColor";
			this._labelColor.Size = new System.Drawing.Size(470, 78);
			this._labelColor.TabIndex = 0;
			this._labelColor.Text = "The quick brown fox jumps over the lazy dog.\r\nÑúåøü åù¸ ýòèõ ìÿãêèõ ôðàíöóçñêèõ á" +
					"óëîê, äà âûïåé æå ÷àþ.";
			this._labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _groupHighlighting
			// 
			this._groupHighlighting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._groupHighlighting.Controls.Add(this._comboHighlightingType);
			this._groupHighlighting.Controls.Add(this._labelHighlightingType);
			this._groupHighlighting.Controls.Add(this._listColorList);
			this._groupHighlighting.Controls.Add(this._buttonColorRemove);
			this._groupHighlighting.Controls.Add(this._buttonColorAdd);
			this._groupHighlighting.Controls.Add(this._buttonColorChoose);
			this._groupHighlighting.Location = new System.Drawing.Point(12, 121);
			this._groupHighlighting.Name = "_groupHighlighting";
			this._groupHighlighting.Size = new System.Drawing.Size(608, 280);
			this._groupHighlighting.TabIndex = 1;
			this._groupHighlighting.TabStop = false;
			this._groupHighlighting.Text = "Highlighting";
			// 
			// _comboHighlightingType
			// 
			this._comboHighlightingType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._comboHighlightingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboHighlightingType.FormattingEnabled = true;
			this._comboHighlightingType.Items.AddRange(new object[] {
            "Constant text color, different background color.",
            "Different text color, constant background color."});
			this._comboHighlightingType.Location = new System.Drawing.Point(52, 19);
			this._comboHighlightingType.Name = "_comboHighlightingType";
			this._comboHighlightingType.Size = new System.Drawing.Size(550, 21);
			this._comboHighlightingType.TabIndex = 1;
			this._comboHighlightingType.SelectedIndexChanged += new System.EventHandler(this.ComboHighlightingType_SelectedIndexChanged);
			// 
			// _labelHighlightingType
			// 
			this._labelHighlightingType.Location = new System.Drawing.Point(6, 16);
			this._labelHighlightingType.Name = "_labelHighlightingType";
			this._labelHighlightingType.Size = new System.Drawing.Size(40, 23);
			this._labelHighlightingType.TabIndex = 0;
			this._labelHighlightingType.Text = "Type";
			this._labelHighlightingType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _labelTextSize
			// 
			this._labelTextSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._labelTextSize.Location = new System.Drawing.Point(482, 74);
			this._labelTextSize.Name = "_labelTextSize";
			this._labelTextSize.Size = new System.Drawing.Size(60, 23);
			this._labelTextSize.TabIndex = 4;
			this._labelTextSize.Text = "Text Size";
			this._labelTextSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormToolsOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 442);
			this.Controls.Add(this._groupHighlighting);
			this.Controls.Add(this._groupGeneral);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this._buttonCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormToolsOptions";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this._groupGeneral.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._numericTextSize)).EndInit();
			this._groupHighlighting.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox _groupGeneral;
		private System.Windows.Forms.Button _buttonForeColor;
		private System.Windows.Forms.Button _buttonBackColor;
		private System.Windows.Forms.Label _labelColor;
		private System.Windows.Forms.GroupBox _groupHighlighting;
		private System.Windows.Forms.ListBox _listColorList;
		private System.Windows.Forms.Button _buttonColorChoose;
		private System.Windows.Forms.Button _buttonColorRemove;
		private System.Windows.Forms.Button _buttonColorAdd;
		
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.ColorDialog _colorDialog;
		private System.Windows.Forms.ComboBox _comboHighlightingType;
		private System.Windows.Forms.Label _labelHighlightingType;
		private System.Windows.Forms.NumericUpDown _numericTextSize;
		private System.Windows.Forms.Label _labelTextSize;
	}
}
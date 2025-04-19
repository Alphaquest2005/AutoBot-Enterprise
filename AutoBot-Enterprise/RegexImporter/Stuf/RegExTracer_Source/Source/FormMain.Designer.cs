namespace RegExTracer
{
	partial class FormMain
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this._split_Re_IR = new System.Windows.Forms.SplitContainer();
			this._tableLayoutOptions = new System.Windows.Forms.TableLayoutPanel();
			this._checkOptionECMAScript = new System.Windows.Forms.CheckBox();
			this._checkOptionIgnoreWhitespace = new System.Windows.Forms.CheckBox();
			this._checkOptionRightToLeft = new System.Windows.Forms.CheckBox();
			this._checkOptionIgnoreCase = new System.Windows.Forms.CheckBox();
			this._checkOptionSingleLine = new System.Windows.Forms.CheckBox();
			this._checkOptionCultureInvariant = new System.Windows.Forms.CheckBox();
			this._checkOptionMultiline = new System.Windows.Forms.CheckBox();
			this._checkOptionExplicitCapture = new System.Windows.Forms.CheckBox();
			this._comboRegExHighlight = new System.Windows.Forms.ComboBox();
			this._textRegEx = new Nabu.Forms.RichTextBoxEx(this.components);
			this._labelRegExp = new System.Windows.Forms.Label();
			this._split_I_R = new System.Windows.Forms.SplitContainer();
			this._comboInputHighlight = new System.Windows.Forms.ComboBox();
			this._textInput = new Nabu.Forms.RichTextBoxEx(this.components);
			this._labelInput = new System.Windows.Forms.Label();
			this._textResult = new Nabu.Forms.RichTextBoxEx(this.components);
			this._labelResult = new System.Windows.Forms.Label();
			this._menuMain = new System.Windows.Forms.MenuStrip();
			this._menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemFileNew = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this._menuSeparatorFile1 = new System.Windows.Forms.ToolStripSeparator();
			this._menuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this._menuSeparatorFile2 = new System.Windows.Forms.ToolStripSeparator();
			this._menuItemFilePrint = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemFilePrintPreview = new System.Windows.Forms.ToolStripMenuItem();
			this._menuSeparatorFile3 = new System.Windows.Forms.ToolStripSeparator();
			this._menuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditUndo = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditRedo = new System.Windows.Forms.ToolStripMenuItem();
			this._menuSeparatorEdit1 = new System.Windows.Forms.ToolStripSeparator();
			this._menuItemEditCut = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditCopy = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditCopyCodeFriendly = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditCopyCSharpCode = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditCopyVBNetCode = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemEditPaste = new System.Windows.Forms.ToolStripMenuItem();
			this._menuSeparatorEdit2 = new System.Windows.Forms.ToolStripSeparator();
			this._menuItemEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemTools = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this._menuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
			this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this._split_Re_IR.Panel1.SuspendLayout();
			this._split_Re_IR.Panel2.SuspendLayout();
			this._split_Re_IR.SuspendLayout();
			this._tableLayoutOptions.SuspendLayout();
			this._split_I_R.Panel1.SuspendLayout();
			this._split_I_R.Panel2.SuspendLayout();
			this._split_I_R.SuspendLayout();
			this._menuMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// _split_Re_IR
			// 
			this._split_Re_IR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._split_Re_IR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._split_Re_IR.Location = new System.Drawing.Point(12, 27);
			this._split_Re_IR.Name = "_split_Re_IR";
			this._split_Re_IR.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _split_Re_IR.Panel1
			// 
			this._split_Re_IR.Panel1.Controls.Add(this._tableLayoutOptions);
			this._split_Re_IR.Panel1.Controls.Add(this._comboRegExHighlight);
			this._split_Re_IR.Panel1.Controls.Add(this._textRegEx);
			this._split_Re_IR.Panel1.Controls.Add(this._labelRegExp);
			this._split_Re_IR.Panel1.Padding = new System.Windows.Forms.Padding(4);
			// 
			// _split_Re_IR.Panel2
			// 
			this._split_Re_IR.Panel2.Controls.Add(this._split_I_R);
			this._split_Re_IR.Size = new System.Drawing.Size(768, 523);
			this._split_Re_IR.SplitterDistance = 154;
			this._split_Re_IR.TabIndex = 1;
			// 
			// _tableLayoutOptions
			// 
			this._tableLayoutOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutOptions.ColumnCount = 4;
			this._tableLayoutOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutOptions.Controls.Add(this._checkOptionECMAScript, 0, 0);
			this._tableLayoutOptions.Controls.Add(this._checkOptionIgnoreWhitespace, 3, 1);
			this._tableLayoutOptions.Controls.Add(this._checkOptionRightToLeft, 0, 1);
			this._tableLayoutOptions.Controls.Add(this._checkOptionIgnoreCase, 3, 0);
			this._tableLayoutOptions.Controls.Add(this._checkOptionSingleLine, 1, 0);
			this._tableLayoutOptions.Controls.Add(this._checkOptionCultureInvariant, 2, 1);
			this._tableLayoutOptions.Controls.Add(this._checkOptionMultiline, 1, 1);
			this._tableLayoutOptions.Controls.Add(this._checkOptionExplicitCapture, 2, 0);
			this._tableLayoutOptions.Location = new System.Drawing.Point(4, 100);
			this._tableLayoutOptions.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutOptions.Name = "_tableLayoutOptions";
			this._tableLayoutOptions.RowCount = 2;
			this._tableLayoutOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutOptions.Size = new System.Drawing.Size(758, 48);
			this._tableLayoutOptions.TabIndex = 11;
			// 
			// _checkOptionECMAScript
			// 
			this._checkOptionECMAScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionECMAScript.Location = new System.Drawing.Point(3, 3);
			this._checkOptionECMAScript.Name = "_checkOptionECMAScript";
			this._checkOptionECMAScript.Size = new System.Drawing.Size(183, 18);
			this._checkOptionECMAScript.TabIndex = 4;
			this._checkOptionECMAScript.Text = "ECMA script syntax.";
			this._checkOptionECMAScript.UseVisualStyleBackColor = true;
			this._checkOptionECMAScript.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionIgnoreWhitespace
			// 
			this._checkOptionIgnoreWhitespace.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionIgnoreWhitespace.Location = new System.Drawing.Point(570, 27);
			this._checkOptionIgnoreWhitespace.Name = "_checkOptionIgnoreWhitespace";
			this._checkOptionIgnoreWhitespace.Size = new System.Drawing.Size(185, 18);
			this._checkOptionIgnoreWhitespace.TabIndex = 6;
			this._checkOptionIgnoreWhitespace.Text = "Ignore whitespace.";
			this._checkOptionIgnoreWhitespace.UseVisualStyleBackColor = true;
			this._checkOptionIgnoreWhitespace.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionRightToLeft
			// 
			this._checkOptionRightToLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionRightToLeft.Location = new System.Drawing.Point(3, 27);
			this._checkOptionRightToLeft.Name = "_checkOptionRightToLeft";
			this._checkOptionRightToLeft.Size = new System.Drawing.Size(183, 18);
			this._checkOptionRightToLeft.TabIndex = 7;
			this._checkOptionRightToLeft.Text = "Right to left search.";
			this._checkOptionRightToLeft.UseVisualStyleBackColor = true;
			this._checkOptionRightToLeft.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionIgnoreCase
			// 
			this._checkOptionIgnoreCase.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionIgnoreCase.Location = new System.Drawing.Point(570, 3);
			this._checkOptionIgnoreCase.Name = "_checkOptionIgnoreCase";
			this._checkOptionIgnoreCase.Size = new System.Drawing.Size(185, 18);
			this._checkOptionIgnoreCase.TabIndex = 3;
			this._checkOptionIgnoreCase.Text = "Ignore case.";
			this._checkOptionIgnoreCase.UseVisualStyleBackColor = true;
			this._checkOptionIgnoreCase.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionSingleLine
			// 
			this._checkOptionSingleLine.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionSingleLine.Location = new System.Drawing.Point(192, 3);
			this._checkOptionSingleLine.Name = "_checkOptionSingleLine";
			this._checkOptionSingleLine.Size = new System.Drawing.Size(183, 18);
			this._checkOptionSingleLine.TabIndex = 9;
			this._checkOptionSingleLine.Text = "Single line input.";
			this._checkOptionSingleLine.UseVisualStyleBackColor = true;
			this._checkOptionSingleLine.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionCultureInvariant
			// 
			this._checkOptionCultureInvariant.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionCultureInvariant.Location = new System.Drawing.Point(381, 27);
			this._checkOptionCultureInvariant.Name = "_checkOptionCultureInvariant";
			this._checkOptionCultureInvariant.Size = new System.Drawing.Size(183, 18);
			this._checkOptionCultureInvariant.TabIndex = 10;
			this._checkOptionCultureInvariant.Text = "Culture invariant.";
			this._checkOptionCultureInvariant.UseVisualStyleBackColor = true;
			this._checkOptionCultureInvariant.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionMultiline
			// 
			this._checkOptionMultiline.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionMultiline.Location = new System.Drawing.Point(192, 27);
			this._checkOptionMultiline.Name = "_checkOptionMultiline";
			this._checkOptionMultiline.Size = new System.Drawing.Size(183, 18);
			this._checkOptionMultiline.TabIndex = 5;
			this._checkOptionMultiline.Text = "Multiline regular expression.";
			this._checkOptionMultiline.UseVisualStyleBackColor = true;
			this._checkOptionMultiline.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _checkOptionExplicitCapture
			// 
			this._checkOptionExplicitCapture.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkOptionExplicitCapture.Location = new System.Drawing.Point(381, 3);
			this._checkOptionExplicitCapture.Name = "_checkOptionExplicitCapture";
			this._checkOptionExplicitCapture.Size = new System.Drawing.Size(183, 18);
			this._checkOptionExplicitCapture.TabIndex = 8;
			this._checkOptionExplicitCapture.Text = "Explicit group capture.";
			this._checkOptionExplicitCapture.UseVisualStyleBackColor = true;
			this._checkOptionExplicitCapture.CheckedChanged += new System.EventHandler(this.CheckOption_CheckedChanged);
			// 
			// _comboRegExHighlight
			// 
			this._comboRegExHighlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._comboRegExHighlight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboRegExHighlight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._comboRegExHighlight.FormattingEnabled = true;
			this._comboRegExHighlight.Items.AddRange(new object[] {
            "None",
            "Braces",
            "Groups"});
			this._comboRegExHighlight.Location = new System.Drawing.Point(609, 4);
			this._comboRegExHighlight.Name = "_comboRegExHighlight";
			this._comboRegExHighlight.Size = new System.Drawing.Size(150, 24);
			this._comboRegExHighlight.TabIndex = 1;
			this._comboRegExHighlight.SelectedIndexChanged += new System.EventHandler(this.ComboRegExHighlight_SelectedIndexChanged);
			// 
			// _textRegEx
			// 
			this._textRegEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._textRegEx.DetectUrls = false;
			this._textRegEx.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textRegEx.HideSelection = false;
			this._textRegEx.Location = new System.Drawing.Point(7, 30);
			this._textRegEx.MaxLength = 32768;
			this._textRegEx.Name = "_textRegEx";
			this._textRegEx.Size = new System.Drawing.Size(752, 67);
			this._textRegEx.TabIndex = 2;
			this._textRegEx.Text = "";
			this._textRegEx.WordWrap = false;
			this._textRegEx.Leave += new System.EventHandler(this.TextRegEx_Leave);
			this._textRegEx.Enter += new System.EventHandler(this.TextRegEx_Enter);
			this._textRegEx.TextChanged += new System.EventHandler(this.TextRegEx_TextChanged);
			// 
			// _labelRegExp
			// 
			this._labelRegExp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelRegExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelRegExp.Location = new System.Drawing.Point(7, 4);
			this._labelRegExp.Name = "_labelRegExp";
			this._labelRegExp.Size = new System.Drawing.Size(596, 23);
			this._labelRegExp.TabIndex = 0;
			this._labelRegExp.Text = "Regular Expression";
			// 
			// _split_I_R
			// 
			this._split_I_R.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._split_I_R.Dock = System.Windows.Forms.DockStyle.Fill;
			this._split_I_R.Location = new System.Drawing.Point(0, 0);
			this._split_I_R.Name = "_split_I_R";
			this._split_I_R.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _split_I_R.Panel1
			// 
			this._split_I_R.Panel1.Controls.Add(this._comboInputHighlight);
			this._split_I_R.Panel1.Controls.Add(this._textInput);
			this._split_I_R.Panel1.Controls.Add(this._labelInput);
			this._split_I_R.Panel1.Padding = new System.Windows.Forms.Padding(4);
			// 
			// _split_I_R.Panel2
			// 
			this._split_I_R.Panel2.Controls.Add(this._textResult);
			this._split_I_R.Panel2.Controls.Add(this._labelResult);
			this._split_I_R.Panel2.Padding = new System.Windows.Forms.Padding(4);
			this._split_I_R.Size = new System.Drawing.Size(768, 365);
			this._split_I_R.SplitterDistance = 175;
			this._split_I_R.TabIndex = 0;
			// 
			// _comboInputHighlight
			// 
			this._comboInputHighlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._comboInputHighlight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboInputHighlight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._comboInputHighlight.FormattingEnabled = true;
			this._comboInputHighlight.Items.AddRange(new object[] {
            "None",
            "Unicolor",
            "Multicolor"});
			this._comboInputHighlight.Location = new System.Drawing.Point(609, 4);
			this._comboInputHighlight.Name = "_comboInputHighlight";
			this._comboInputHighlight.Size = new System.Drawing.Size(150, 24);
			this._comboInputHighlight.TabIndex = 1;
			this._comboInputHighlight.SelectedIndexChanged += new System.EventHandler(this.ComboInputHighlight_SelectedIndexChanged);
			// 
			// _textInput
			// 
			this._textInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._textInput.DetectUrls = false;
			this._textInput.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textInput.HideSelection = false;
			this._textInput.Location = new System.Drawing.Point(7, 30);
			this._textInput.MaxLength = 32768;
			this._textInput.Name = "_textInput";
			this._textInput.Size = new System.Drawing.Size(752, 136);
			this._textInput.TabIndex = 2;
			this._textInput.Text = "";
			this._textInput.WordWrap = false;
			this._textInput.Leave += new System.EventHandler(this.TextInput_Leave);
			this._textInput.Enter += new System.EventHandler(this.TextInput_Enter);
			this._textInput.TextChanged += new System.EventHandler(this.TextInput_TextChanged);
			// 
			// _labelInput
			// 
			this._labelInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelInput.Location = new System.Drawing.Point(7, 4);
			this._labelInput.Name = "_labelInput";
			this._labelInput.Size = new System.Drawing.Size(596, 23);
			this._labelInput.TabIndex = 0;
			this._labelInput.Text = "Input";
			// 
			// _textResult
			// 
			this._textResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._textResult.DetectUrls = false;
			this._textResult.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textResult.HideSelection = false;
			this._textResult.Location = new System.Drawing.Point(7, 30);
			this._textResult.MaxLength = 32768;
			this._textResult.Name = "_textResult";
			this._textResult.ReadOnly = true;
			this._textResult.Size = new System.Drawing.Size(752, 147);
			this._textResult.TabIndex = 1;
			this._textResult.Text = "";
			this._textResult.WordWrap = false;
			// 
			// _labelResult
			// 
			this._labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._labelResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelResult.Location = new System.Drawing.Point(7, 4);
			this._labelResult.Name = "_labelResult";
			this._labelResult.Size = new System.Drawing.Size(752, 23);
			this._labelResult.TabIndex = 0;
			this._labelResult.Text = "Result";
			// 
			// _menuMain
			// 
			this._menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemFile,
            this._menuItemEdit,
            this._menuItemTools,
            this._menuItemHelp});
			this._menuMain.Location = new System.Drawing.Point(0, 0);
			this._menuMain.Name = "_menuMain";
			this._menuMain.Size = new System.Drawing.Size(792, 24);
			this._menuMain.TabIndex = 0;
			// 
			// _menuItemFile
			// 
			this._menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemFileNew,
            this._menuItemFileOpen,
            this._menuSeparatorFile1,
            this._menuItemFileSave,
            this._menuItemFileSaveAs,
            this._menuSeparatorFile2,
            this._menuItemFilePrint,
            this._menuItemFilePrintPreview,
            this._menuSeparatorFile3,
            this._menuItemFileExit});
			this._menuItemFile.Name = "_menuItemFile";
			this._menuItemFile.Size = new System.Drawing.Size(40, 20);
			this._menuItemFile.Text = "&File";
			// 
			// _menuItemFileNew
			// 
			this._menuItemFileNew.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFileNew.Image")));
			this._menuItemFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemFileNew.Name = "_menuItemFileNew";
			this._menuItemFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this._menuItemFileNew.Size = new System.Drawing.Size(179, 22);
			this._menuItemFileNew.Text = "&New";
			this._menuItemFileNew.ToolTipText = "Create new document.";
			this._menuItemFileNew.Click += new System.EventHandler(this.MenuItemFileNew_Click);
			// 
			// _menuItemFileOpen
			// 
			this._menuItemFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFileOpen.Image")));
			this._menuItemFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemFileOpen.Name = "_menuItemFileOpen";
			this._menuItemFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this._menuItemFileOpen.Size = new System.Drawing.Size(179, 22);
			this._menuItemFileOpen.Text = "&Open …";
			this._menuItemFileOpen.ToolTipText = "Open existing document.";
			this._menuItemFileOpen.Click += new System.EventHandler(this.MenuItemFileOpen_Click);
			// 
			// _menuSeparatorFile1
			// 
			this._menuSeparatorFile1.Name = "_menuSeparatorFile1";
			this._menuSeparatorFile1.Size = new System.Drawing.Size(176, 6);
			// 
			// _menuItemFileSave
			// 
			this._menuItemFileSave.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFileSave.Image")));
			this._menuItemFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemFileSave.Name = "_menuItemFileSave";
			this._menuItemFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this._menuItemFileSave.Size = new System.Drawing.Size(179, 22);
			this._menuItemFileSave.Text = "&Save";
			this._menuItemFileSave.ToolTipText = "Save current document.";
			this._menuItemFileSave.Click += new System.EventHandler(this.MenuItemFileSave_Click);
			// 
			// _menuItemFileSaveAs
			// 
			this._menuItemFileSaveAs.Name = "_menuItemFileSaveAs";
			this._menuItemFileSaveAs.Size = new System.Drawing.Size(179, 22);
			this._menuItemFileSaveAs.Text = "Save &As …";
			this._menuItemFileSaveAs.ToolTipText = "Save current document under new name.";
			this._menuItemFileSaveAs.Click += new System.EventHandler(this.MenuItemFileSaveAs_Click);
			// 
			// _menuSeparatorFile2
			// 
			this._menuSeparatorFile2.Name = "_menuSeparatorFile2";
			this._menuSeparatorFile2.Size = new System.Drawing.Size(176, 6);
			// 
			// _menuItemFilePrint
			// 
			this._menuItemFilePrint.Enabled = false;
			this._menuItemFilePrint.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFilePrint.Image")));
			this._menuItemFilePrint.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemFilePrint.Name = "_menuItemFilePrint";
			this._menuItemFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this._menuItemFilePrint.Size = new System.Drawing.Size(179, 22);
			this._menuItemFilePrint.Text = "&Print";
			this._menuItemFilePrint.ToolTipText = "Print current document.";
			this._menuItemFilePrint.Click += new System.EventHandler(this.MenuItemFilePrint_Click);
			// 
			// _menuItemFilePrintPreview
			// 
			this._menuItemFilePrintPreview.Enabled = false;
			this._menuItemFilePrintPreview.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemFilePrintPreview.Image")));
			this._menuItemFilePrintPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemFilePrintPreview.Name = "_menuItemFilePrintPreview";
			this._menuItemFilePrintPreview.Size = new System.Drawing.Size(179, 22);
			this._menuItemFilePrintPreview.Text = "Print Pre&view";
			this._menuItemFilePrintPreview.ToolTipText = "Print preview current document.";
			this._menuItemFilePrintPreview.Click += new System.EventHandler(this.MenuItemFilePrintPreview_Click);
			// 
			// _menuSeparatorFile3
			// 
			this._menuSeparatorFile3.Name = "_menuSeparatorFile3";
			this._menuSeparatorFile3.Size = new System.Drawing.Size(176, 6);
			// 
			// _menuItemFileExit
			// 
			this._menuItemFileExit.Name = "_menuItemFileExit";
			this._menuItemFileExit.Size = new System.Drawing.Size(179, 22);
			this._menuItemFileExit.Text = "E&xit";
			this._menuItemFileExit.ToolTipText = "Exit application.";
			this._menuItemFileExit.Click += new System.EventHandler(this.MenuItemFileExit_Click);
			// 
			// _menuItemEdit
			// 
			this._menuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemEditUndo,
            this._menuItemEditRedo,
            this._menuSeparatorEdit1,
            this._menuItemEditCut,
            this._menuItemEditCopy,
            this._menuItemEditCopyCodeFriendly,
            this._menuItemEditCopyCSharpCode,
            this._menuItemEditCopyVBNetCode,
            this._menuItemEditPaste,
            this._menuSeparatorEdit2,
            this._menuItemEditSelectAll});
			this._menuItemEdit.Name = "_menuItemEdit";
			this._menuItemEdit.Size = new System.Drawing.Size(41, 20);
			this._menuItemEdit.Text = "&Edit";
			// 
			// _menuItemEditUndo
			// 
			this._menuItemEditUndo.Name = "_menuItemEditUndo";
			this._menuItemEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this._menuItemEditUndo.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditUndo.Tag = "Undo {0}";
			this._menuItemEditUndo.Text = "&Undo";
			this._menuItemEditUndo.Click += new System.EventHandler(this.MenuItemEditUndo_Click);
			// 
			// _menuItemEditRedo
			// 
			this._menuItemEditRedo.Name = "_menuItemEditRedo";
			this._menuItemEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this._menuItemEditRedo.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditRedo.Tag = "Redo {0}";
			this._menuItemEditRedo.Text = "&Redo";
			this._menuItemEditRedo.Click += new System.EventHandler(this.MenuItemEditRedo_Click);
			// 
			// _menuSeparatorEdit1
			// 
			this._menuSeparatorEdit1.Name = "_menuSeparatorEdit1";
			this._menuSeparatorEdit1.Size = new System.Drawing.Size(196, 6);
			// 
			// _menuItemEditCut
			// 
			this._menuItemEditCut.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemEditCut.Image")));
			this._menuItemEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemEditCut.Name = "_menuItemEditCut";
			this._menuItemEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this._menuItemEditCut.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditCut.Text = "Cu&t";
			this._menuItemEditCut.ToolTipText = "Cut selection to clipboard.";
			this._menuItemEditCut.Click += new System.EventHandler(this.MenuItemEditCut_Click);
			// 
			// _menuItemEditCopy
			// 
			this._menuItemEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemEditCopy.Image")));
			this._menuItemEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemEditCopy.Name = "_menuItemEditCopy";
			this._menuItemEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this._menuItemEditCopy.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditCopy.Text = "&Copy";
			this._menuItemEditCopy.ToolTipText = "Copy selection to clipboard.";
			this._menuItemEditCopy.Click += new System.EventHandler(this.MenuItemEditCopy_Click);
			// 
			// _menuItemEditCopyCodeFriendly
			// 
			this._menuItemEditCopyCodeFriendly.Name = "_menuItemEditCopyCodeFriendly";
			this._menuItemEditCopyCodeFriendly.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditCopyCodeFriendly.Text = "Copy C&ode Friendly";
			this._menuItemEditCopyCodeFriendly.ToolTipText = "Copy to clipboard with \'\\\' replaced to \'\\\\\'.";
			this._menuItemEditCopyCodeFriendly.Click += new System.EventHandler(this.MenuItemEditCopyCodeFriendly_Click);
			// 
			// _menuItemEditCopyCSharpCode
			// 
			this._menuItemEditCopyCSharpCode.Name = "_menuItemEditCopyCSharpCode";
			this._menuItemEditCopyCSharpCode.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditCopyCSharpCode.Text = "Copy C# Code";
			this._menuItemEditCopyCSharpCode.Click += new System.EventHandler(this.MenuItemEditCopyCSharpCode_Click);
			// 
			// _menuItemEditCopyVBNetCode
			// 
			this._menuItemEditCopyVBNetCode.Name = "_menuItemEditCopyVBNetCode";
			this._menuItemEditCopyVBNetCode.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditCopyVBNetCode.Text = "Copy VB.Net Code";
			this._menuItemEditCopyVBNetCode.Click += new System.EventHandler(this.MenuItemEditCopyVBNetCode_Click);
			// 
			// _menuItemEditPaste
			// 
			this._menuItemEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("_menuItemEditPaste.Image")));
			this._menuItemEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._menuItemEditPaste.Name = "_menuItemEditPaste";
			this._menuItemEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this._menuItemEditPaste.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditPaste.Text = "&Paste";
			this._menuItemEditPaste.ToolTipText = "Paste from clipboard.";
			this._menuItemEditPaste.Click += new System.EventHandler(this.MenuItemEditPaste_Click);
			// 
			// _menuSeparatorEdit2
			// 
			this._menuSeparatorEdit2.Name = "_menuSeparatorEdit2";
			this._menuSeparatorEdit2.Size = new System.Drawing.Size(196, 6);
			// 
			// _menuItemEditSelectAll
			// 
			this._menuItemEditSelectAll.Name = "_menuItemEditSelectAll";
			this._menuItemEditSelectAll.Size = new System.Drawing.Size(199, 22);
			this._menuItemEditSelectAll.Text = "Select &All";
			this._menuItemEditSelectAll.ToolTipText = "Select whole text.";
			this._menuItemEditSelectAll.Click += new System.EventHandler(this.MenuItemEditSelectAll_Click);
			// 
			// _menuItemTools
			// 
			this._menuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemToolsOptions});
			this._menuItemTools.Name = "_menuItemTools";
			this._menuItemTools.Size = new System.Drawing.Size(51, 20);
			this._menuItemTools.Text = "Tools";
			// 
			// _menuItemToolsOptions
			// 
			this._menuItemToolsOptions.Name = "_menuItemToolsOptions";
			this._menuItemToolsOptions.Size = new System.Drawing.Size(147, 22);
			this._menuItemToolsOptions.Text = "Options …";
			this._menuItemToolsOptions.ToolTipText = "Change application parameters.";
			this._menuItemToolsOptions.Click += new System.EventHandler(this.MenuItemToolsOptions_Click);
			// 
			// _menuItemHelp
			// 
			this._menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuItemHelpAbout});
			this._menuItemHelp.Name = "_menuItemHelp";
			this._menuItemHelp.Size = new System.Drawing.Size(45, 20);
			this._menuItemHelp.Text = "&Help";
			// 
			// _menuItemHelpAbout
			// 
			this._menuItemHelpAbout.Name = "_menuItemHelpAbout";
			this._menuItemHelpAbout.Size = new System.Drawing.Size(122, 22);
			this._menuItemHelpAbout.Text = "&About";
			this._menuItemHelpAbout.ToolTipText = "Authorship and copyright information.";
			this._menuItemHelpAbout.Click += new System.EventHandler(this.MenuItemHelpAbout_Click);
			// 
			// _openFileDialog
			// 
			this._openFileDialog.DefaultExt = "regex";
			this._openFileDialog.FileName = "new";
			this._openFileDialog.Filter = "RegEx files|*.regex|All files|*.*";
			this._openFileDialog.RestoreDirectory = true;
			this._openFileDialog.SupportMultiDottedExtensions = true;
			// 
			// _saveFileDialog
			// 
			this._saveFileDialog.DefaultExt = "regex";
			this._saveFileDialog.FileName = "new";
			this._saveFileDialog.Filter = "RegEx files|*.regex|All files|*.*";
			this._saveFileDialog.RestoreDirectory = true;
			this._saveFileDialog.SupportMultiDottedExtensions = true;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 562);
			this.Controls.Add(this._split_Re_IR);
			this.Controls.Add(this._menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this._menuMain;
			this.Name = "FormMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RegEx Tracer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this._split_Re_IR.Panel1.ResumeLayout(false);
			this._split_Re_IR.Panel2.ResumeLayout(false);
			this._split_Re_IR.ResumeLayout(false);
			this._tableLayoutOptions.ResumeLayout(false);
			this._split_I_R.Panel1.ResumeLayout(false);
			this._split_I_R.Panel2.ResumeLayout(false);
			this._split_I_R.ResumeLayout(false);
			this._menuMain.ResumeLayout(false);
			this._menuMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer _split_Re_IR;
		private System.Windows.Forms.SplitContainer _split_I_R;
		private System.Windows.Forms.Label _labelRegExp;
		private System.Windows.Forms.ComboBox _comboRegExHighlight;
		private Nabu.Forms.RichTextBoxEx _textRegEx;
		private System.Windows.Forms.Label _labelInput;
		private System.Windows.Forms.ComboBox _comboInputHighlight;
		private Nabu.Forms.RichTextBoxEx _textInput;
		private System.Windows.Forms.Label _labelResult;
		private Nabu.Forms.RichTextBoxEx _textResult;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
		private System.Windows.Forms.SaveFileDialog _saveFileDialog;
		private System.Windows.Forms.MenuStrip _menuMain;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFile;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFileNew;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFileOpen;
		private System.Windows.Forms.ToolStripSeparator _menuSeparatorFile1;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFileSave;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFileSaveAs;
		private System.Windows.Forms.ToolStripSeparator _menuSeparatorFile2;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFilePrint;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFilePrintPreview;
		private System.Windows.Forms.ToolStripSeparator _menuSeparatorFile3;
		private System.Windows.Forms.ToolStripMenuItem _menuItemFileExit;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEdit;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditUndo;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditRedo;
		private System.Windows.Forms.ToolStripSeparator _menuSeparatorEdit1;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditCut;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditCopy;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditCopyCSharpCode;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditCopyVBNetCode;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditCopyCodeFriendly;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditPaste;
		private System.Windows.Forms.ToolStripSeparator _menuSeparatorEdit2;
		private System.Windows.Forms.ToolStripMenuItem _menuItemEditSelectAll;
		private System.Windows.Forms.ToolStripMenuItem _menuItemTools;
		private System.Windows.Forms.ToolStripMenuItem _menuItemToolsOptions;
		private System.Windows.Forms.ToolStripMenuItem _menuItemHelp;
		private System.Windows.Forms.ToolStripMenuItem _menuItemHelpAbout;
		private System.Windows.Forms.CheckBox _checkOptionECMAScript;
		private System.Windows.Forms.CheckBox _checkOptionIgnoreCase;
		private System.Windows.Forms.CheckBox _checkOptionSingleLine;
		private System.Windows.Forms.CheckBox _checkOptionExplicitCapture;
		private System.Windows.Forms.CheckBox _checkOptionRightToLeft;
		private System.Windows.Forms.CheckBox _checkOptionIgnoreWhitespace;
		private System.Windows.Forms.CheckBox _checkOptionMultiline;
		private System.Windows.Forms.CheckBox _checkOptionCultureInvariant;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutOptions;
		
	}
}


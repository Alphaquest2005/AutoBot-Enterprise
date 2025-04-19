using System.Windows.Forms;

namespace RegExTester
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    partial class frmMain
    {


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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.matchesStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.executionTimeStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.contextStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
            this.topAndMiddleSplitContainer = new System.Windows.Forms.SplitContainer();
            this.aboutButton = new System.Windows.Forms.Button();
            this.regExLibraryButton = new System.Windows.Forms.Button();
            this.regExCheatSheetButton = new System.Windows.Forms.Button();
            this.replaceModeButton = new System.Windows.Forms.Button();
            this.indentedInputButton = new System.Windows.Forms.Button();
            this.singleLineButton = new System.Windows.Forms.Button();
            this.multiLineButton = new System.Windows.Forms.Button();
            this.cultureInvariantButton = new System.Windows.Forms.Button();
            this.ignoreCaseButton = new System.Windows.Forms.Button();
            this.replaceModeCheckBox = new System.Windows.Forms.CheckBox();
            this.indentedInputCheckBox = new System.Windows.Forms.CheckBox();
            this.copyButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.singleLineCheckBox = new System.Windows.Forms.CheckBox();
            this.multiLineCheckBox = new System.Windows.Forms.CheckBox();
            this.ignoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.cultureInvariantCheckBox = new System.Windows.Forms.CheckBox();
            this.regExAndRepExSplitContainer = new System.Windows.Forms.SplitContainer();
            this.regExLabel = new System.Windows.Forms.Label();
            this.regExTextBox = new System.Windows.Forms.TextBox();
            this.repExLabel = new System.Windows.Forms.Label();
            this.repExTextBox = new System.Windows.Forms.TextBox();
            this.middleAndBottomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.textAndResultsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.textLabel = new System.Windows.Forms.Label();
            this.textRichTextBox = new RegExTester.CustomRichTextBox();
            this.resultsLabel = new System.Windows.Forms.Label();
            this.resultsRichTextBox = new RegExTester.CustomRichTextBox();
            this.exportResultsButton = new System.Windows.Forms.Button();
            this.resultListView = new System.Windows.Forms.ListView();
            this.matchColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.positionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.lengthColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.resultsListLabel = new System.Windows.Forms.Label();
            this.copyButtonContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyGeneric0MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGeneric1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGeneric2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGeneric3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.statusStatusBarPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchesStatusBarPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.executionTimeStatusBarPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contextStatusBarPanel)).BeginInit();
            this.topAndMiddleSplitContainer.Panel1.SuspendLayout();
            this.topAndMiddleSplitContainer.Panel2.SuspendLayout();
            this.topAndMiddleSplitContainer.SuspendLayout();
            this.regExAndRepExSplitContainer.Panel1.SuspendLayout();
            this.regExAndRepExSplitContainer.Panel2.SuspendLayout();
            this.regExAndRepExSplitContainer.SuspendLayout();
            this.middleAndBottomSplitContainer.Panel1.SuspendLayout();
            this.middleAndBottomSplitContainer.Panel2.SuspendLayout();
            this.middleAndBottomSplitContainer.SuspendLayout();
            this.textAndResultsSplitContainer.Panel1.SuspendLayout();
            this.textAndResultsSplitContainer.Panel2.SuspendLayout();
            this.textAndResultsSplitContainer.SuspendLayout();
            this.copyButtonContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 435);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusStatusBarPanel,
            this.matchesStatusBarPanel,
            this.executionTimeStatusBarPanel,
            this.contextStatusBarPanel});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(592, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusBar";
            // 
            // statusStatusBarPanel
            // 
            this.statusStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusStatusBarPanel.Name = "sbpStatus";
            this.statusStatusBarPanel.Text = "Nothing searched yet.";
            this.statusStatusBarPanel.Width = 546;
            // 
            // matchesStatusBarPanel
            // 
            this.matchesStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.matchesStatusBarPanel.MinWidth = 0;
            this.matchesStatusBarPanel.Name = "sbpMatches";
            this.matchesStatusBarPanel.Width = 10;
            // 
            // executionTimeStatusBarPanel
            // 
            this.executionTimeStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.executionTimeStatusBarPanel.MinWidth = 0;
            this.executionTimeStatusBarPanel.Name = "sbpExecutionTime";
            this.executionTimeStatusBarPanel.Width = 10;
            // 
            // contextStatusBarPanel
            // 
            this.contextStatusBarPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.contextStatusBarPanel.MinWidth = 0;
            this.contextStatusBarPanel.Name = "sbpContext";
            this.contextStatusBarPanel.Width = 10;
            // 
            // topAndMiddleSplitContainer
            // 
            this.topAndMiddleSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.topAndMiddleSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topAndMiddleSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.topAndMiddleSplitContainer.IsSplitterFixed = true;
            this.topAndMiddleSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.topAndMiddleSplitContainer.Name = "topAndMiddleSplitContainer";
            this.topAndMiddleSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // topAndMiddleSplitContainer.Panel1
            // 
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.aboutButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.regExLibraryButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.regExCheatSheetButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.replaceModeButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.indentedInputButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.singleLineButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.multiLineButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.cultureInvariantButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.ignoreCaseButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.replaceModeCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.indentedInputCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.copyButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.testButton);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.singleLineCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.multiLineCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.ignoreCaseCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.cultureInvariantCheckBox);
            this.topAndMiddleSplitContainer.Panel1.Controls.Add(this.regExAndRepExSplitContainer);
            this.topAndMiddleSplitContainer.Panel1MinSize = 100;
            // 
            // topAndMiddleSplitContainer.Panel2
            // 
            this.topAndMiddleSplitContainer.Panel2.Controls.Add(this.middleAndBottomSplitContainer);
            this.topAndMiddleSplitContainer.Size = new System.Drawing.Size(592, 435);
            this.topAndMiddleSplitContainer.SplitterDistance = 100;
            this.topAndMiddleSplitContainer.TabIndex = 0;
            // 
            // aboutButton
            // 
            this.aboutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aboutButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutButton.ForeColor = System.Drawing.Color.Blue;
            this.aboutButton.Location = new System.Drawing.Point(438, 1);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(137, 25);
            this.aboutButton.TabIndex = 31;
            this.aboutButton.Text = "About This Program";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // regExLibraryButton
            // 
            this.regExLibraryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.regExLibraryButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.regExLibraryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.regExLibraryButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regExLibraryButton.ForeColor = System.Drawing.Color.Blue;
            this.regExLibraryButton.Location = new System.Drawing.Point(328, 1);
            this.regExLibraryButton.Name = "regExLibraryButton";
            this.regExLibraryButton.Size = new System.Drawing.Size(102, 25);
            this.regExLibraryButton.TabIndex = 30;
            this.regExLibraryButton.Text = "RegEx Library";
            this.regExLibraryButton.UseVisualStyleBackColor = true;
            this.regExLibraryButton.Click += new System.EventHandler(this.regExLibraryButton_Click);
            // 
            // regExCheatSheetButton
            // 
            this.regExCheatSheetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.regExCheatSheetButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.regExCheatSheetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.regExCheatSheetButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regExCheatSheetButton.ForeColor = System.Drawing.Color.Blue;
            this.regExCheatSheetButton.Location = new System.Drawing.Point(186, 1);
            this.regExCheatSheetButton.Name = "regExCheatSheetButton";
            this.regExCheatSheetButton.Size = new System.Drawing.Size(127, 25);
            this.regExCheatSheetButton.TabIndex = 28;
            this.regExCheatSheetButton.Text = "RegEx CheatSheet";
            this.regExCheatSheetButton.UseVisualStyleBackColor = true;
            this.regExCheatSheetButton.Click += new System.EventHandler(this.regExCheatSheetButton_Click);
            // 
            // replaceModeButton
            // 
            this.replaceModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.replaceModeButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.replaceModeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replaceModeButton.ForeColor = System.Drawing.Color.Blue;
            this.replaceModeButton.Location = new System.Drawing.Point(375, 73);
            this.replaceModeButton.Name = "replaceModeButton";
            this.replaceModeButton.Size = new System.Drawing.Size(22, 22);
            this.replaceModeButton.TabIndex = 24;
            this.replaceModeButton.Text = "?";
            this.replaceModeButton.UseVisualStyleBackColor = true;
            this.replaceModeButton.Click += new System.EventHandler(this.replaceModeButton_Click);
            // 
            // indentedInputButton
            // 
            this.indentedInputButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.indentedInputButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.indentedInputButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.indentedInputButton.ForeColor = System.Drawing.Color.Blue;
            this.indentedInputButton.Location = new System.Drawing.Point(380, 51);
            this.indentedInputButton.Name = "indentedInputButton";
            this.indentedInputButton.Size = new System.Drawing.Size(22, 22);
            this.indentedInputButton.TabIndex = 23;
            this.indentedInputButton.Text = "?";
            this.indentedInputButton.UseVisualStyleBackColor = true;
            this.indentedInputButton.Click += new System.EventHandler(this.indentedInputButton_Click);
            // 
            // singleLineButton
            // 
            this.singleLineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.singleLineButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.singleLineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.singleLineButton.ForeColor = System.Drawing.Color.Blue;
            this.singleLineButton.Location = new System.Drawing.Point(243, 73);
            this.singleLineButton.Name = "singleLineButton";
            this.singleLineButton.Size = new System.Drawing.Size(22, 22);
            this.singleLineButton.TabIndex = 22;
            this.singleLineButton.Text = "?";
            this.singleLineButton.UseVisualStyleBackColor = true;
            this.singleLineButton.Click += new System.EventHandler(this.singleLineButton_Click);
            // 
            // multiLineButton
            // 
            this.multiLineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.multiLineButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.multiLineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.multiLineButton.ForeColor = System.Drawing.Color.Blue;
            this.multiLineButton.Location = new System.Drawing.Point(232, 51);
            this.multiLineButton.Name = "multiLineButton";
            this.multiLineButton.Size = new System.Drawing.Size(22, 22);
            this.multiLineButton.TabIndex = 21;
            this.multiLineButton.Text = "?";
            this.multiLineButton.UseVisualStyleBackColor = true;
            this.multiLineButton.Click += new System.EventHandler(this.multiLineButton_Click);
            // 
            // cultureInvariantButton
            // 
            this.cultureInvariantButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cultureInvariantButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.cultureInvariantButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cultureInvariantButton.ForeColor = System.Drawing.Color.Blue;
            this.cultureInvariantButton.Location = new System.Drawing.Point(130, 73);
            this.cultureInvariantButton.Name = "cultureInvariantButton";
            this.cultureInvariantButton.Size = new System.Drawing.Size(22, 22);
            this.cultureInvariantButton.TabIndex = 20;
            this.cultureInvariantButton.Text = "?";
            this.cultureInvariantButton.UseVisualStyleBackColor = true;
            this.cultureInvariantButton.Click += new System.EventHandler(this.cultureInvariantButton_Click);
            // 
            // ignoreCaseButton
            // 
            this.ignoreCaseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ignoreCaseButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.ignoreCaseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ignoreCaseButton.ForeColor = System.Drawing.Color.Blue;
            this.ignoreCaseButton.Location = new System.Drawing.Point(102, 51);
            this.ignoreCaseButton.Name = "ignoreCaseButton";
            this.ignoreCaseButton.Size = new System.Drawing.Size(22, 22);
            this.ignoreCaseButton.TabIndex = 19;
            this.ignoreCaseButton.Text = "?";
            this.ignoreCaseButton.UseVisualStyleBackColor = true;
            this.ignoreCaseButton.Click += new System.EventHandler(this.ignoreCaseButton_Click);
            // 
            // replaceModeCheckBox
            // 
            this.replaceModeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.replaceModeCheckBox.CausesValidation = false;
            this.replaceModeCheckBox.Checked = true;
            this.replaceModeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.replaceModeCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceModeCheckBox.Location = new System.Drawing.Point(275, 73);
            this.replaceModeCheckBox.Name = "replaceModeCheckBox";
            this.replaceModeCheckBox.Size = new System.Drawing.Size(127, 22);
            this.replaceModeCheckBox.TabIndex = 15;
            this.replaceModeCheckBox.Text = "Replace mode";
            this.replaceModeCheckBox.CheckedChanged += new System.EventHandler(this.replaceModeCheckBox_CheckedChanged);
            // 
            // indentedInputCheckBox
            // 
            this.indentedInputCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.indentedInputCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.indentedInputCheckBox.Location = new System.Drawing.Point(275, 51);
            this.indentedInputCheckBox.Name = "indentedInputCheckBox";
            this.indentedInputCheckBox.Size = new System.Drawing.Size(127, 22);
            this.indentedInputCheckBox.TabIndex = 13;
            this.indentedInputCheckBox.Text = "Indented Input";
            this.indentedInputCheckBox.CheckedChanged += new System.EventHandler(this.indentedInputCheckBox_CheckedChanged);
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.copyButton.Location = new System.Drawing.Point(405, 56);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 34);
            this.copyButton.TabIndex = 17;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testButton.Location = new System.Drawing.Point(486, 56);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(92, 34);
            this.testButton.TabIndex = 18;
            this.testButton.Text = "Test [F5]";
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // singleLineCheckBox
            // 
            this.singleLineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.singleLineCheckBox.Checked = true;
            this.singleLineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.singleLineCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.singleLineCheckBox.Location = new System.Drawing.Point(160, 73);
            this.singleLineCheckBox.Name = "singleLineCheckBox";
            this.singleLineCheckBox.Size = new System.Drawing.Size(109, 22);
            this.singleLineCheckBox.TabIndex = 11;
            this.singleLineCheckBox.Text = "Single Line";
            // 
            // multiLineCheckBox
            // 
            this.multiLineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.multiLineCheckBox.Checked = true;
            this.multiLineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.multiLineCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.multiLineCheckBox.Location = new System.Drawing.Point(160, 51);
            this.multiLineCheckBox.Name = "multiLineCheckBox";
            this.multiLineCheckBox.Size = new System.Drawing.Size(109, 22);
            this.multiLineCheckBox.TabIndex = 9;
            this.multiLineCheckBox.Text = "Multi Line";
            // 
            // ignoreCaseCheckBox
            // 
            this.ignoreCaseCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ignoreCaseCheckBox.Checked = true;
            this.ignoreCaseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreCaseCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ignoreCaseCheckBox.Location = new System.Drawing.Point(11, 51);
            this.ignoreCaseCheckBox.Name = "ignoreCaseCheckBox";
            this.ignoreCaseCheckBox.Size = new System.Drawing.Size(142, 22);
            this.ignoreCaseCheckBox.TabIndex = 5;
            this.ignoreCaseCheckBox.Text = "Ignore Case";
            // 
            // cultureInvariantCheckBox
            // 
            this.cultureInvariantCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cultureInvariantCheckBox.Checked = true;
            this.cultureInvariantCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cultureInvariantCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cultureInvariantCheckBox.Location = new System.Drawing.Point(11, 73);
            this.cultureInvariantCheckBox.Name = "cultureInvariantCheckBox";
            this.cultureInvariantCheckBox.Size = new System.Drawing.Size(142, 22);
            this.cultureInvariantCheckBox.TabIndex = 7;
            this.cultureInvariantCheckBox.Text = "Culture Invariant";
            // 
            // regExAndRepExSplitContainer
            // 
            this.regExAndRepExSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.regExAndRepExSplitContainer.Location = new System.Drawing.Point(11, 5);
            this.regExAndRepExSplitContainer.Name = "regExAndRepExSplitContainer";
            // 
            // regExAndRepExSplitContainer.Panel1
            // 
            this.regExAndRepExSplitContainer.Panel1.Controls.Add(this.regExLabel);
            this.regExAndRepExSplitContainer.Panel1.Controls.Add(this.regExTextBox);
            // 
            // regExAndRepExSplitContainer.Panel2
            // 
            this.regExAndRepExSplitContainer.Panel2.Controls.Add(this.repExLabel);
            this.regExAndRepExSplitContainer.Panel2.Controls.Add(this.repExTextBox);
            this.regExAndRepExSplitContainer.Size = new System.Drawing.Size(567, 42);
            this.regExAndRepExSplitContainer.SplitterDistance = 283;
            this.regExAndRepExSplitContainer.TabIndex = 4;
            // 
            // regExLabel
            // 
            this.regExLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.regExLabel.Location = new System.Drawing.Point(0, 3);
            this.regExLabel.Name = "regExLabel";
            this.regExLabel.Size = new System.Drawing.Size(283, 16);
            this.regExLabel.TabIndex = 1;
            this.regExLabel.Text = "Regular Expression";
            // 
            // regExTextBox
            // 
            this.regExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.regExTextBox.HideSelection = false;
            this.regExTextBox.Location = new System.Drawing.Point(0, 21);
            this.regExTextBox.Name = "regExTextBox";
            this.regExTextBox.Size = new System.Drawing.Size(283, 21);
            this.regExTextBox.TabIndex = 0;
            // 
            // repExLabel
            // 
            this.repExLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.repExLabel.Location = new System.Drawing.Point(0, 3);
            this.repExLabel.Name = "repExLabel";
            this.repExLabel.Size = new System.Drawing.Size(284, 16);
            this.repExLabel.TabIndex = 2;
            this.repExLabel.Text = "Replacement Expression";
            // 
            // repExTextBox
            // 
            this.repExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.repExTextBox.HideSelection = false;
            this.repExTextBox.Location = new System.Drawing.Point(0, 21);
            this.repExTextBox.Name = "repExTextBox";
            this.repExTextBox.Size = new System.Drawing.Size(280, 21);
            this.repExTextBox.TabIndex = 0;
            // 
            // middleAndBottomSplitContainer
            // 
            this.middleAndBottomSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.middleAndBottomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.middleAndBottomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.middleAndBottomSplitContainer.Name = "middleAndBottomSplitContainer";
            this.middleAndBottomSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // middleAndBottomSplitContainer.Panel1
            // 
            this.middleAndBottomSplitContainer.Panel1.Controls.Add(this.textAndResultsSplitContainer);
            this.middleAndBottomSplitContainer.Panel1MinSize = 61;
            // 
            // middleAndBottomSplitContainer.Panel2
            // 
            this.middleAndBottomSplitContainer.Panel2.Controls.Add(this.exportResultsButton);
            this.middleAndBottomSplitContainer.Panel2.Controls.Add(this.resultListView);
            this.middleAndBottomSplitContainer.Panel2.Controls.Add(this.resultsListLabel);
            this.middleAndBottomSplitContainer.Panel2MinSize = 89;
            this.middleAndBottomSplitContainer.Size = new System.Drawing.Size(592, 331);
            this.middleAndBottomSplitContainer.SplitterDistance = 191;
            this.middleAndBottomSplitContainer.TabIndex = 0;
            // 
            // textAndResultsSplitContainer
            // 
            this.textAndResultsSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textAndResultsSplitContainer.Location = new System.Drawing.Point(11, 9);
            this.textAndResultsSplitContainer.Name = "textAndResultsSplitContainer";
            // 
            // textAndResultsSplitContainer.Panel1
            // 
            this.textAndResultsSplitContainer.Panel1.Controls.Add(this.textLabel);
            this.textAndResultsSplitContainer.Panel1.Controls.Add(this.textRichTextBox);
            // 
            // textAndResultsSplitContainer.Panel2
            // 
            this.textAndResultsSplitContainer.Panel2.Controls.Add(this.resultsLabel);
            this.textAndResultsSplitContainer.Panel2.Controls.Add(this.resultsRichTextBox);
            this.textAndResultsSplitContainer.Size = new System.Drawing.Size(567, 166);
            this.textAndResultsSplitContainer.SplitterDistance = 283;
            this.textAndResultsSplitContainer.TabIndex = 1;
            // 
            // textLabel
            // 
            this.textLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLabel.Location = new System.Drawing.Point(-3, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(286, 16);
            this.textLabel.TabIndex = 1;
            this.textLabel.Text = "Test Text";
            // 
            // textRichTextBox
            // 
            this.textRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textRichTextBox.HideSelection = false;
            this.textRichTextBox.Location = new System.Drawing.Point(0, 19);
            this.textRichTextBox.Name = "textRichTextBox";
            this.textRichTextBox.Size = new System.Drawing.Size(283, 147);
            this.textRichTextBox.TabIndex = 0;
            this.textRichTextBox.Text = "";
            // 
            // resultsLabel
            // 
            this.resultsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsLabel.Location = new System.Drawing.Point(-3, 0);
            this.resultsLabel.Name = "resultsLabel";
            this.resultsLabel.Size = new System.Drawing.Size(287, 16);
            this.resultsLabel.TabIndex = 2;
            this.resultsLabel.Text = "Test Results";
            // 
            // resultsRichTextBox
            // 
            this.resultsRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsRichTextBox.HideSelection = false;
            this.resultsRichTextBox.Location = new System.Drawing.Point(0, 19);
            this.resultsRichTextBox.Name = "resultsRichTextBox";
            this.resultsRichTextBox.Size = new System.Drawing.Size(280, 147);
            this.resultsRichTextBox.TabIndex = 0;
            this.resultsRichTextBox.Text = "";
            // 
            // exportResultsButton
            // 
            this.exportResultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportResultsButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.exportResultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportResultsButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportResultsButton.ForeColor = System.Drawing.Color.Blue;
            this.exportResultsButton.Location = new System.Drawing.Point(473, 1);
            this.exportResultsButton.Name = "exportResultsButton";
            this.exportResultsButton.Size = new System.Drawing.Size(102, 25);
            this.exportResultsButton.TabIndex = 32;
            this.exportResultsButton.Text = "Export Results (CSV)";
            this.exportResultsButton.UseVisualStyleBackColor = true;
            this.exportResultsButton.Click += new System.EventHandler(this.exportResultsButton_Click);
            // 
            // resultListView
            // 
            this.resultListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.matchColumnHeader,
            this.positionColumnHeader,
            this.lengthColumnHeader});
            this.resultListView.FullRowSelect = true;
            this.resultListView.GridLines = true;
            this.resultListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.resultListView.HideSelection = false;
            this.resultListView.Location = new System.Drawing.Point(11, 27);
            this.resultListView.MultiSelect = false;
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(567, 91);
            this.resultListView.TabIndex = 1;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.SelectedIndexChanged += new System.EventHandler(this.resultListView_SelectedIndexChanged);
            // 
            // matchColumnHeader
            // 
            this.matchColumnHeader.Text = "Match";
            this.matchColumnHeader.Width = 350;
            // 
            // positionColumnHeader
            // 
            this.positionColumnHeader.Text = "Position";
            // 
            // lengthColumnHeader
            // 
            this.lengthColumnHeader.Text = "Length";
            // 
            // resultsListLabel
            // 
            this.resultsListLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsListLabel.Location = new System.Drawing.Point(8, 9);
            this.resultsListLabel.Name = "resultsListLabel";
            this.resultsListLabel.Size = new System.Drawing.Size(570, 15);
            this.resultsListLabel.TabIndex = 0;
            this.resultsListLabel.Text = "Test Results";
            // 
            // copyButtonContextMenu
            // 
            this.copyButtonContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGeneric0MenuItem,
            this.copyGeneric1MenuItem,
            this.copyGeneric2MenuItem,
            this.copyGeneric3MenuItem});
            this.copyButtonContextMenu.Name = "btnCopyContextMenuStrip";
            this.copyButtonContextMenu.Size = new System.Drawing.Size(163, 92);
            // 
            // copyGeneric0MenuItem
            // 
            this.copyGeneric0MenuItem.Image = global::RegExTester.Properties.Resources.CSharpSnippet;
            this.copyGeneric0MenuItem.Name = "copyGeneric0MenuItem";
            this.copyGeneric0MenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyGeneric0MenuItem.Tag = "csharp snippet";
            this.copyGeneric0MenuItem.Text = "C# code &snippet";
            this.copyGeneric0MenuItem.Click += new System.EventHandler(this.copyGeneric0MenuItem_Click);
            // 
            // copyGeneric1MenuItem
            // 
            this.copyGeneric1MenuItem.Image = global::RegExTester.Properties.Resources.CSharp;
            this.copyGeneric1MenuItem.Name = "copyGeneric1MenuItem";
            this.copyGeneric1MenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyGeneric1MenuItem.Tag = "csharp";
            this.copyGeneric1MenuItem.Text = "&C# escaped string";
            this.copyGeneric1MenuItem.Click += new System.EventHandler(this.copyGeneric0MenuItem_Click);
            // 
            // copyGeneric2MenuItem
            // 
            this.copyGeneric2MenuItem.Image = global::RegExTester.Properties.Resources.Html;
            this.copyGeneric2MenuItem.Name = "copyGeneric2MenuItem";
            this.copyGeneric2MenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyGeneric2MenuItem.Tag = "html";
            this.copyGeneric2MenuItem.Text = "&HTML encoded";
            this.copyGeneric2MenuItem.Click += new System.EventHandler(this.copyGeneric0MenuItem_Click);
            // 
            // copyGeneric3MenuItem
            // 
            this.copyGeneric3MenuItem.Image = global::RegExTester.Properties.Resources.Plain;
            this.copyGeneric3MenuItem.Name = "copyGeneric3MenuItem";
            this.copyGeneric3MenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyGeneric3MenuItem.Tag = "plain";
            this.copyGeneric3MenuItem.Text = "&Plain text";
            this.copyGeneric3MenuItem.Click += new System.EventHandler(this.copyGeneric0MenuItem_Click);
            // 
            // exportSaveFileDialog
            // 
            this.exportSaveFileDialog.DefaultExt = "csv";
            this.exportSaveFileDialog.Filter = "Comma Separated Values|*.csv|All files|*.*";
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(592, 457);
            this.Controls.Add(this.topAndMiddleSplitContainer);
            this.Controls.Add(this.statusBar);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(600, 390);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegEx Tester";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.statusStatusBarPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchesStatusBarPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.executionTimeStatusBarPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contextStatusBarPanel)).EndInit();
            this.topAndMiddleSplitContainer.Panel1.ResumeLayout(false);
            this.topAndMiddleSplitContainer.Panel2.ResumeLayout(false);
            this.topAndMiddleSplitContainer.ResumeLayout(false);
            this.regExAndRepExSplitContainer.Panel1.ResumeLayout(false);
            this.regExAndRepExSplitContainer.Panel1.PerformLayout();
            this.regExAndRepExSplitContainer.Panel2.ResumeLayout(false);
            this.regExAndRepExSplitContainer.Panel2.PerformLayout();
            this.regExAndRepExSplitContainer.ResumeLayout(false);
            this.middleAndBottomSplitContainer.Panel1.ResumeLayout(false);
            this.middleAndBottomSplitContainer.Panel2.ResumeLayout(false);
            this.middleAndBottomSplitContainer.ResumeLayout(false);
            this.textAndResultsSplitContainer.Panel1.ResumeLayout(false);
            this.textAndResultsSplitContainer.Panel2.ResumeLayout(false);
            this.textAndResultsSplitContainer.ResumeLayout(false);
            this.copyButtonContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private StatusBar statusBar;
        private StatusBarPanel statusStatusBarPanel;
        private StatusBarPanel contextStatusBarPanel;
        private SplitContainer topAndMiddleSplitContainer;
        private SplitContainer middleAndBottomSplitContainer;
        private Button copyButton;
        private Button testButton;
        private CheckBox cultureInvariantCheckBox;
        private CheckBox singleLineCheckBox;
        private CheckBox multiLineCheckBox;
        private CheckBox ignoreCaseCheckBox;
        private Label resultsListLabel;
        private ListView resultListView;
        private ColumnHeader matchColumnHeader;
        private ColumnHeader positionColumnHeader;
        private ColumnHeader lengthColumnHeader;
        private CheckBox indentedInputCheckBox;
        private System.ComponentModel.IContainer components;
        private ContextMenuStrip copyButtonContextMenu;
        private ToolStripMenuItem copyGeneric1MenuItem;
        private ToolStripMenuItem copyGeneric2MenuItem;
        private ToolStripMenuItem copyGeneric3MenuItem;
        private CheckBox replaceModeCheckBox;
        private SplitContainer textAndResultsSplitContainer;
        private CustomRichTextBox resultsRichTextBox;
        private CustomRichTextBox textRichTextBox;
        private SplitContainer regExAndRepExSplitContainer;
        private TextBox regExTextBox;
        private TextBox repExTextBox;
        private ToolStripMenuItem copyGeneric0MenuItem;
        private Label textLabel;
        private Label resultsLabel;
        private Label regExLabel;
        private Label repExLabel;
        private StatusBarPanel matchesStatusBarPanel;
        private StatusBarPanel executionTimeStatusBarPanel;
        private Button ignoreCaseButton;
        private Button cultureInvariantButton;
        private Button multiLineButton;
        private Button singleLineButton;
        private Button replaceModeButton;
        private Button indentedInputButton;
        private Button aboutButton;
        private Button regExLibraryButton;
        private Button regExCheatSheetButton;
		private SaveFileDialog exportSaveFileDialog;
        private Button exportResultsButton;

    }
}
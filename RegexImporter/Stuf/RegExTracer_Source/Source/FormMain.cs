using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Nabu.Forms.TextObjectModel;

namespace RegExTracer
{
	public partial class FormMain : Form
	{
		#region Types
		
		/// <summary>
		/// Regular Expression parser state.
		/// </summary>
		private enum RegExParserState
		{
			/// <summary>
			/// Default state.
			/// </summary>
			Default,
			/// <summary>
			/// Group begins with '(' and ends with ')' with exception of '\(' '\)' escape sequences.
			/// </summary>
			Group,
			/// <summary>
			/// Escape sequence. '\{AnySymbol}'.
			/// </summary>
			EscapeSequense,
			/// <summary>
			/// Symbol set begins with '[' and ends with ']' with exception of '\[' '\]' escape sequences.
			/// </summary>
			SymbolSet
		}
		
		private enum HighlightingRegEx
		{
			None,
			Braces,
			Groups
		}

		private enum HighlightingInput
		{
			None,
			Unicolor,
			Multicolor
		}
		
		private enum BuildReason
		{
			Document,
			RegEx,
			Input
		}

		#endregion
		
		#region Variables
		
		/// <summary>
		/// Path of current document file.
		/// </summary>
		private string _documentFile = null;
		/// <summary>
		/// True if document has unsaved changes. False otherwise.
		/// </summary>
		private bool _documentIsUnsaved = false;
		/// <summary>
		/// True if highlighting is in progress.
		/// </summary>
		private bool _isHighlighting = false;
		/// <summary>
		/// RichTextBox with currently has focus or null if no RichTextBox has focus.
		/// </summary>
		private RichTextBox _textBoxCurrent = null;
		/// <summary>
		/// Application settings.
		/// </summary>
		private Settings _settings = new Settings();
		
		#endregion
		
		#region Methods

		#region RegEx

		private bool BuildMatchList(string textRegEx, string textInput, out int[][] matchList, out string errorMessage)
		{
			matchList = null;
			errorMessage = null;
			
			try
			{
				Regex regEx = new Regex(
					textRegEx,
					(this._checkOptionECMAScript.Checked       ? RegexOptions.ECMAScript : 0) |
					(this._checkOptionSingleLine.Checked       ? RegexOptions.Singleline : 0) |
					(this._checkOptionExplicitCapture.Checked  ? RegexOptions.ExplicitCapture : 0) |
					(this._checkOptionIgnoreCase.Checked       ? RegexOptions.IgnoreCase : 0) |
					(this._checkOptionRightToLeft.Checked      ? RegexOptions.RightToLeft : 0) |
					(this._checkOptionMultiline.Checked        ? RegexOptions.Multiline : 0) |
					(this._checkOptionCultureInvariant.Checked ? RegexOptions.CultureInvariant : 0) |
					(this._checkOptionIgnoreWhitespace.Checked ? RegexOptions.IgnorePatternWhitespace : 0));
				
				MatchCollection matchCollection = regEx.Matches(textInput);

				matchList = new int[matchCollection.Count][];
				
				for (int indexMatch = 0; indexMatch < matchCollection.Count; ++indexMatch)
				{
					Match match = matchCollection[indexMatch];
					
					matchList[indexMatch] = new int[2 * match.Groups.Count];
					
					for (int indexGroup = 0; indexGroup < match.Groups.Count; ++indexGroup)
					{
						matchList[indexMatch][2 * indexGroup + 0] = match.Groups[indexGroup].Index;
						matchList[indexMatch][2 * indexGroup + 1] = match.Groups[indexGroup].Length;	
					}
				}
			}
			catch (Exception exception)
			{
				errorMessage = exception.Message;
			}
			
			return errorMessage == null;
		}
		
		#endregion

		#region Highlighting

		#region General

		private void BuildResult(BuildReason buildReason)
		{
			string textRegEx = this._textRegEx.Text.Replace("\r", "").Replace("\n", "");
			string textInput = this._textInput.Text;
			int[][] matchList;
			string errorMessage;

			if (BuildMatchList(textRegEx, textInput, out matchList, out errorMessage))
			{
				if ((this._comboRegExHighlight.SelectedIndex != 0) && ((buildReason == BuildReason.Document) || (buildReason == BuildReason.RegEx)))
				{
					ApplyHighlightRegEx();
				}
				
				if ((this._comboInputHighlight.SelectedIndex != 0) && ((buildReason == BuildReason.Document) || (buildReason == BuildReason.Input)))
				{
					ApplyHighlightInput(textInput, matchList);
				}

				ApplyHighlightResult(textInput, matchList);
			}
			else
			{
				ClearHighlightRegEx();
				ClearHighlightInput();
				ClearHighlightResult(errorMessage);
			}
		}
		
		private int RtfColor(Color color)
		{
			return (color.B << 16) | (color.G << 8) | (color.R);
		}

		#endregion

		#region RegEx

		private void ClearHighlightRegEx()
		{
			ITextDocument textDocument = this._textRegEx.TextDocument;
			ITextRange textRange = textDocument.Range(0, this._textRegEx.TextLength);

			textRange.Para.SpaceBefore = 0.0F;
			textRange.Para.SpaceAfter = 0.0F;
			
			ITextFont textFont = textRange.Font;

			textFont.BackColor = RtfColor(this._textRegEx.BackColor);
			textFont.ForeColor = RtfColor(this._textRegEx.ForeColor);
			textFont.Bold = FormatFlags.False;
		}

		private void ApplyHighlightRegEx()
		{
			ClearHighlightRegEx();

			HighlightingRegEx highlightingMode =
				(new HighlightingRegEx[] { HighlightingRegEx.None, HighlightingRegEx.Braces, HighlightingRegEx.Groups })[this._comboRegExHighlight.SelectedIndex];
			ITextDocument textDocument = this._textRegEx.TextDocument;

			textDocument.Freeze();

			ITextRange textRangeAll = textDocument.Range(0, this._textRegEx.TextLength);

			textRangeAll.Para.SpaceBefore = 4.0F;
			textRangeAll.Para.SpaceAfter = 4.0F;
			
			string textRegEx = this._textRegEx.Text;

			Stack<RegExParserState> stackParserState = new Stack<RegExParserState>();

			stackParserState.Push(RegExParserState.Default);

			int groupDepth = 0;

			for (int index = 0; index < textRegEx.Length; ++index)
			{
				Color symbolColor = Color.Transparent;

				switch (stackParserState.Peek())
				{
					case RegExParserState.Default:
						switch (textRegEx[index])
						{
							case '\\':
								stackParserState.Push(RegExParserState.EscapeSequense);
								break;
							case '[':
								stackParserState.Push(RegExParserState.SymbolSet);
								break;
							case '(':
								if (highlightingMode == HighlightingRegEx.Braces)
								{
									symbolColor = this._settings.HighlightColorList[0];
								}

								stackParserState.Push(RegExParserState.Group);
								++groupDepth;
								break;
						}
						break;
					case RegExParserState.Group:
						switch (textRegEx[index])
						{
							case '\\':
								stackParserState.Push(RegExParserState.EscapeSequense);
								break;
							case '[':
								stackParserState.Push(RegExParserState.SymbolSet);
								break;
							case '(':
								if (highlightingMode == HighlightingRegEx.Braces)
								{
									symbolColor = this._settings.HighlightColorList[0];
								}

								stackParserState.Push(RegExParserState.Group);
								++groupDepth;
								break;
							case ')':
								if (highlightingMode == HighlightingRegEx.Braces)
								{
									symbolColor = this._settings.HighlightColorList[0];
								}
								else if (highlightingMode == HighlightingRegEx.Groups)
								{
									symbolColor = this._settings.HighlightColorList[(groupDepth - 1) % this._settings.HighlightColorList.Count];
								}

								stackParserState.Pop();
								--groupDepth;
								break;
						}
						break;
					case RegExParserState.EscapeSequense:
						stackParserState.Pop();
						break;
					case RegExParserState.SymbolSet:
						switch (textRegEx[index])
						{
							case '\\':
								stackParserState.Push(RegExParserState.EscapeSequense);
								break;
							case '[':
								stackParserState.Push(RegExParserState.SymbolSet);
								break;
							case ']':
								stackParserState.Pop();
								break;
						}
						break;
				}

				ITextRange textRange = textDocument.Range(index, index + 1);

				if (groupDepth == 0)
				{
					if (symbolColor == Color.Transparent)
					{
						switch (this._settings.HighlighingType)
						{
							case HighlighingType.BackGround:
								symbolColor = this._textRegEx.BackColor;
								break;
							case HighlighingType.ForeGround:
								symbolColor = this._textRegEx.ForeColor;
								break;
						}
					}
				}
				else
				{
					if (symbolColor == Color.Transparent)
					{
						if (highlightingMode == HighlightingRegEx.Braces)
						{
							switch (this._settings.HighlighingType)
							{
								case HighlighingType.BackGround:
									symbolColor = this._textRegEx.BackColor;
									break;
								case HighlighingType.ForeGround:
									symbolColor = this._textRegEx.ForeColor;
									break;
							}
						}
						else if (highlightingMode == HighlightingRegEx.Groups)
						{
							symbolColor = this._settings.HighlightColorList[(groupDepth - 1) % this._settings.HighlightColorList.Count];
						}
					}
				}

				switch (this._settings.HighlighingType)
				{
					case HighlighingType.BackGround:
						textRange.Font.BackColor = RtfColor(symbolColor);
						break;
					case HighlighingType.ForeGround:
						textRange.Font.ForeColor = RtfColor(symbolColor);
						break;
				}
			}
			
			textDocument.Unfreeze();
		}

		#endregion

		#region Input

		private void ClearHighlightInput()
		{
			ITextDocument textDocument = this._textInput.TextDocument;
			ITextRange textRange = textDocument.Range(0, this._textInput.TextLength);

			textRange.Para.SpaceBefore = 0.0F;
			textRange.Para.SpaceAfter = 0.0F;
			
			ITextFont textFont = textRange.Font;

			textFont.BackColor = RtfColor(this._textInput.BackColor);
			textFont.ForeColor = RtfColor(this._textInput.ForeColor);
			textFont.Bold = FormatFlags.False;
		}

		private void ApplyHighlightInput(string textInput, int[][] matchList)
		{
			ClearHighlightInput();
			
			HighlightingInput highlightingMode =
				(new HighlightingInput[] { HighlightingInput.None, HighlightingInput.Unicolor, HighlightingInput.Multicolor })[this._comboInputHighlight.SelectedIndex];
			ITextDocument textDocument = this._textInput.TextDocument;
			
			textDocument.Freeze();

			ITextRange textRangeAll = textDocument.Range(0, this._textInput.TextLength);

			textRangeAll.Para.SpaceBefore = 4.0F;
			textRangeAll.Para.SpaceAfter = 4.0F;
			
			for (int indexMatch = 0; indexMatch < matchList.Length; ++indexMatch)
			{
				for (int indexGroup = 1; indexGroup < matchList[indexMatch].Length / 2; ++indexGroup)
				{
					ITextRange textRange = textDocument.Range(
						matchList[indexMatch][2 * indexGroup + 0],
						matchList[indexMatch][2 * indexGroup + 0] + matchList[indexMatch][2 * indexGroup + 1]);

					Color symbolColor = Color.Transparent;
					
					if (highlightingMode == HighlightingInput.Unicolor)
					{
						symbolColor = this._settings.HighlightColorList[0];
					}
					else if (highlightingMode == HighlightingInput.Multicolor)
					{
						symbolColor = this._settings.HighlightColorList[(indexGroup - 1) % this._settings.HighlightColorList.Count];
					}

					switch (this._settings.HighlighingType)
					{
						case HighlighingType.BackGround:
							textRange.Font.BackColor = RtfColor(symbolColor);
							break;
						case HighlighingType.ForeGround:
							textRange.Font.ForeColor = RtfColor(symbolColor);
							break;
					}
				}
			}
			
			textDocument.Unfreeze();
		}

		#endregion
		
		#region Result

		private void ClearHighlightResult(string errorMessage)
		{
			this._textResult.ReadOnly = false;
			
			ITextDocument textDocument = this._textResult.TextDocument;
			ITextRange textRange = textDocument.Range(0, this._textResult.TextLength);

			textRange.Para.SpaceBefore = 0.0F;
			textRange.Para.SpaceAfter = 0.0F;
			
			ITextFont textFont = textRange.Font;

			textFont.BackColor = RtfColor(this._textResult.BackColor);
			textFont.ForeColor = RtfColor(this._textResult.ForeColor);
			textFont.Bold = FormatFlags.False;

			this._textResult.ReadOnly = true;
		}

		private void ApplyHighlightResult(string textInput, int[][] matchList)
		{
			this._textResult.ReadOnly = false;
			
			ITextDocument textDocument = this._textResult.TextDocument;
			
			textDocument.Freeze();
			textDocument.New();
			
			ITextRange textSelection = textDocument.Selection;
			
			textSelection.Para.SpaceBefore = 4.0F;
			textSelection.Para.SpaceAfter = 4.0F;

			for (int indexMatch = 0; indexMatch < matchList.Length; ++indexMatch)
			{
				textSelection.Text += textInput.Substring(matchList[indexMatch][0], matchList[indexMatch][1]) + "\r\n";
				textSelection.Font.BackColor = RtfColor(this._textResult.BackColor);
				textSelection.Font.Size = 1.2F * this._textResult.Font.Size;
				textSelection.Start = textSelection.End;
				textSelection.Text += "\t";
				textSelection.Font.Size = this._textResult.Font.Size;
				textSelection.Start = textSelection.End;
					
				for (int indexGroup = 1; indexGroup < matchList[indexMatch].Length / 2; ++indexGroup)
				{
					Color symbolColor = this._settings.HighlightColorList[(indexGroup - 1) % this._settings.HighlightColorList.Count];
					string text = textInput.Substring(matchList[indexMatch][2 * indexGroup + 0], matchList[indexMatch][2 * indexGroup + 1]);

					if (text == string.Empty)
					{
						text = "<empty>";
					}
					
					textSelection.Text += " " + text + " ";
					
					switch (this._settings.HighlighingType)
					{
						case HighlighingType.BackGround:
							textSelection.Font.BackColor = RtfColor(symbolColor);
							break;
						case HighlighingType.ForeGround:
							textSelection.Font.ForeColor = RtfColor(symbolColor);
							break;
					}

					textSelection.Font.Size = this._textResult.Font.Size;
					textSelection.Start = textSelection.End;
				}

				textSelection.Text += "\r\n";
				textSelection.Start = textSelection.End;
			}
			
			textDocument.Unfreeze();

			this._textResult.ReadOnly = true;
		}

		#endregion
		
		#endregion
		
		#region I/O

		private bool DocumentOpen(string filename)
		{
			if (filename == null)
			{
				if (this._openFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					filename = this._openFileDialog.FileName;
				}
				else
				{
					return false;
				}
			}

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(RegExDocument));

			using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				RegExDocument regExDocument = (RegExDocument)xmlSerializer.Deserialize(stream);

				this._textRegEx.Lines = regExDocument.RegEx.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				this._textInput.Lines = regExDocument.Input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				this._checkOptionECMAScript.Checked = regExDocument.OptionECMAScript;
				this._checkOptionSingleLine.Checked = regExDocument.OptionSingleLine;
				this._checkOptionExplicitCapture.Checked = regExDocument.OptionExplicitCapture;
				this._checkOptionIgnoreCase.Checked = regExDocument.OptionIgnoreCase;
				this._checkOptionRightToLeft.Checked = regExDocument.OptionRightToLeft;
				this._checkOptionMultiline.Checked = regExDocument.OptionMultiline;
				this._checkOptionCultureInvariant.Checked = regExDocument.OptionCultureInvariant;
				this._checkOptionIgnoreWhitespace.Checked = regExDocument.OptionIgnoreWhitespace;
			}

			this._documentFile = filename;
			
			return true;
		}

		private bool DocumentSave(string filename)
		{
			if (filename == null)
			{
				if (this._saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					filename = this._saveFileDialog.FileName;
				}
				else
				{
					return false;
				}
			}

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(RegExDocument));

			using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				RegExDocument regExDocument = new RegExDocument(
					this._textRegEx.Text,
					this._textInput.Text,
					this._checkOptionECMAScript.Checked,
					this._checkOptionSingleLine.Checked,
					this._checkOptionExplicitCapture.Checked,
					this._checkOptionIgnoreCase.Checked,
					this._checkOptionRightToLeft.Checked,
					this._checkOptionMultiline.Checked,
					this._checkOptionCultureInvariant.Checked,
					this._checkOptionIgnoreWhitespace.Checked);

				xmlSerializer.Serialize(stream, regExDocument);
			}

			this._documentFile = filename;
			
			return true;
		}

		#endregion
		
		#region Menu
		
		#region File

		private void MenuItemFileNew_Click(object sender, EventArgs e)
		{
			this._textRegEx.Text = string.Empty;
			this._textInput.Text = string.Empty;
			this._checkOptionECMAScript.Checked = false;
			this._checkOptionSingleLine.Checked = false;
			this._checkOptionExplicitCapture.Checked = false;
			this._checkOptionIgnoreCase.Checked = false;
			this._checkOptionRightToLeft.Checked = false;
			this._checkOptionMultiline.Checked = false;
			this._checkOptionCultureInvariant.Checked = false;
			this._checkOptionIgnoreWhitespace.Checked = false;
		}

		private void MenuItemFileOpen_Click(object sender, EventArgs e)
		{
			this._isHighlighting = true;
			DocumentOpen(null);
			BuildResult(BuildReason.Document);
			this._isHighlighting = false;
			this._documentIsUnsaved = false;
		}

		private void MenuItemFileSave_Click(object sender, EventArgs e)
		{
			DocumentSave(this._documentFile);
			this._documentIsUnsaved = false;
		}

		private void MenuItemFileSaveAs_Click(object sender, EventArgs e)
		{
			DocumentSave(null);
			this._documentIsUnsaved = false;
		}

		private void MenuItemFilePrint_Click(object sender, EventArgs e)
		{
		}

		private void MenuItemFilePrintPreview_Click(object sender, EventArgs e)
		{
		}
		
		private void MenuItemFileExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		
		#endregion
		
		#region Edit

		private void MenuItemEditUndo_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();

			if (textBox != null)
			{
				textBox.Undo();
			}
		}

		private void MenuItemEditRedo_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();

			if (textBox != null)
			{
				textBox.Redo();
			}
		}

		private void MenuItemEditCut_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();
			
			if (textBox != null)
			{
				if (textBox.SelectionLength > 0)
				{
					Clipboard.SetDataObject(textBox.SelectedText);
					textBox.SelectedText = string.Empty;
				}
			}
		}

		private void MenuItemEditCopy_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();

			if (textBox != null)
			{
				if (textBox.SelectionLength > 0)
				{
					Clipboard.SetDataObject(textBox.SelectedText);
				}
				else
				{
					Clipboard.SetDataObject(textBox.Text);
				}
			}
		}

		private void MenuItemEditCopyCodeFriendly_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = this._textRegEx;

			if (textBox != null)
			{
				string text = string.Empty;
				
				if (textBox.SelectionLength > 0)
				{
					text = textBox.SelectedText;
				}
				else
				{
					text = textBox.Text;
				}

				text = text.Replace("\r", "").Replace("\n", "").Replace("\\", "\\\\");
				
				Clipboard.SetDataObject(text);
			}
		}

		private void MenuItemEditCopyCSharpCode_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = this._textRegEx;

			if (textBox != null)
			{
				string text = string.Empty;

				if (textBox.SelectionLength > 0)
				{
					text = textBox.SelectedText;
				}
				else
				{
					text = textBox.Text;
				}

				text = text.Replace("\r", "").Replace("\n", "").Replace("\\", "\\\\");
				
				string options = "0";
				
				if (this._checkOptionECMAScript.Checked)       options += " | RegexOptions.ECMAScript";
				if (this._checkOptionSingleLine.Checked)       options += " | RegexOptions.Singleline";
				if (this._checkOptionExplicitCapture.Checked)  options += " | RegexOptions.ExplicitCapture";
				if (this._checkOptionIgnoreCase.Checked)       options += " | RegexOptions.IgnoreCase";
				if (this._checkOptionRightToLeft.Checked)      options += " | RegexOptions.RightToLeft";
				if (this._checkOptionMultiline.Checked)        options += " | RegexOptions.Multiline";
				if (this._checkOptionCultureInvariant.Checked) options += " | RegexOptions.CultureInvariant";
				if (this._checkOptionIgnoreWhitespace.Checked) options += " | RegexOptions.IgnorePatternWhitespace";
				
				if (options.StartsWith("0 | "))
				{
					options = options.Substring(4);
				}

				Clipboard.SetDataObject(string.Format("Regex regEx = new Regex(\"{0}\", {1});", text, options));
			}
		}

		private void MenuItemEditCopyVBNetCode_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = this._textRegEx;

			if (textBox != null)
			{
				string text = string.Empty;

				if (textBox.SelectionLength > 0)
				{
					text = textBox.SelectedText;
				}
				else
				{
					text = textBox.Text;
				}

				text = text.Replace("\r", "").Replace("\n", "");

				string options = "0";

				if (this._checkOptionECMAScript.Checked) options += " Or RegexOptions.ECMAScript";
				if (this._checkOptionSingleLine.Checked) options += " Or RegexOptions.Singleline";
				if (this._checkOptionExplicitCapture.Checked) options += " Or RegexOptions.ExplicitCapture";
				if (this._checkOptionIgnoreCase.Checked) options += " Or RegexOptions.IgnoreCase";
				if (this._checkOptionRightToLeft.Checked) options += " Or RegexOptions.RightToLeft";
				if (this._checkOptionMultiline.Checked) options += " Or RegexOptions.Multiline";
				if (this._checkOptionCultureInvariant.Checked) options += " Or RegexOptions.CultureInvariant";
				if (this._checkOptionIgnoreWhitespace.Checked) options += " Or RegexOptions.IgnorePatternWhitespace";

				if (options.StartsWith("0 Or "))
				{
					options = options.Substring(5);
				}

				Clipboard.SetDataObject(string.Format("Dim regEx As New Regex(\"{0}\", {1});", text, options));
			}
		}

		private void MenuItemEditPaste_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();

			if (textBox != null)
			{
				if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
				{
					textBox.SelectedText = Clipboard.GetText(TextDataFormat.UnicodeText);
				}
				else if (Clipboard.ContainsText(TextDataFormat.Text))
				{
					textBox.SelectedText = Clipboard.GetText(TextDataFormat.Text);
				}
			}
		}

		private void MenuItemEditSelectAll_Click(object sender, EventArgs e)
		{
			RichTextBox textBox = GetCurrentTextBox();

			if (textBox != null)
			{
				textBox.SelectionStart = 0;
				textBox.SelectionLength = textBox.TextLength;
			}
		}
		
		#endregion
		
		#region Tools

		private void MenuItemToolsOptions_Click(object sender, EventArgs e)
		{
			if ((new FormToolsOptions(this._settings)).ShowDialog(this) == DialogResult.OK)
			{
				this._textRegEx.ForeColor = this._settings.ForeColor;
				this._textRegEx.BackColor = this._settings.BackColor;
				this._textRegEx.Font = new Font(this._textRegEx.Font.FontFamily, this._settings.TextSize);
				this._textInput.ForeColor = this._settings.ForeColor;
				this._textInput.BackColor = this._settings.BackColor;
				this._textInput.Font = new Font(this._textInput.Font.FontFamily, this._settings.TextSize);
				this._textResult.ForeColor = this._settings.ForeColor;
				this._textResult.BackColor = this._settings.BackColor;
				this._textResult.Font = new Font(this._textResult.Font.FontFamily, this._settings.TextSize);
				this._isHighlighting = true;
				BuildResult(BuildReason.Document);
				this._isHighlighting = false;
			}
		}

		#endregion
		
		#region Help
		
		private void MenuItemHelpAbout_Click(object sender, EventArgs e)
		{
			(new FormHelpAbout()).ShowDialog(this);
		}
		
		#endregion
		
		#endregion
		
		#region Form

		public FormMain()
		{
			InitializeComponent();
			SetCurrentTextBox(null);
			this._textRegEx.ForeColor = this._settings.ForeColor;
			this._textRegEx.BackColor = this._settings.BackColor;
			this._textRegEx.Font = new Font(this._textRegEx.Font.FontFamily, this._settings.TextSize);
			this._textInput.ForeColor = this._settings.ForeColor;
			this._textInput.BackColor = this._settings.BackColor;
			this._textInput.Font = new Font(this._textInput.Font.FontFamily, this._settings.TextSize);
			this._textResult.ForeColor = this._settings.ForeColor;
			this._textResult.BackColor = this._settings.BackColor;
			this._textResult.Font = new Font(this._textResult.Font.FontFamily, this._settings.TextSize);
			this._isHighlighting = true;
			this._comboRegExHighlight.SelectedIndex = 1;
			this._comboInputHighlight.SelectedIndex = 2;
			this._isHighlighting = false;
		}

		private void SetCurrentTextBox(RichTextBox textBox)
		{
			this._textBoxCurrent = textBox;
			
			if (this._textBoxCurrent != null)
			{
				this._menuItemEditUndo.Enabled = true;
				this._menuItemEditRedo.Enabled = true;
				this._menuItemEditCut.Enabled = true;
				this._menuItemEditCopy.Enabled = true;
				this._menuItemEditPaste.Enabled = true;
				this._menuItemEditSelectAll.Enabled = true;
				this._menuItemEditUndo.ToolTipText = string.Format((string)this._menuItemEditUndo.Tag, this._textBoxCurrent.UndoActionName);
				this._menuItemEditRedo.ToolTipText = string.Format((string)this._menuItemEditRedo.Tag, this._textBoxCurrent.RedoActionName);
			}
			else
			{
				this._menuItemEditUndo.Enabled = false;
				this._menuItemEditRedo.Enabled = false;
				this._menuItemEditCut.Enabled = false;
				this._menuItemEditCopy.Enabled = false;
				this._menuItemEditPaste.Enabled = false;
				this._menuItemEditSelectAll.Enabled = false;
				this._menuItemEditUndo.ToolTipText = string.Empty;
				this._menuItemEditRedo.ToolTipText = string.Empty;
			}
		}

		private RichTextBox GetCurrentTextBox()
		{
			return this._textBoxCurrent;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this._documentIsUnsaved)
			{
				DialogResult result = MessageBox.Show(this, "Do you want to save changes you made?", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

				if (result == DialogResult.Yes)
				{
					this._menuItemFileSave.PerformClick();
				}
				else if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}
		
		#endregion
		
		#region Controls

		private void CheckOption_CheckedChanged(object sender, EventArgs e)
		{
			if (!this._isHighlighting)
			{
				this._isHighlighting = true;
				BuildResult(BuildReason.Document);
				this._isHighlighting = false;
			}

			this._documentIsUnsaved = true;
		}

		private void TextRegEx_TextChanged(object sender, EventArgs e)
		{
			if (!this._isHighlighting)
			{
				this._isHighlighting = true;
				BuildResult(BuildReason.Document);
				this._isHighlighting = false;
			}

			this._documentIsUnsaved = true;

			this._menuItemEditUndo.ToolTipText = string.Format((string)this._menuItemEditUndo.Tag, this._textRegEx.UndoActionName);
			this._menuItemEditRedo.ToolTipText = string.Format((string)this._menuItemEditRedo.Tag, this._textRegEx.RedoActionName);
		}

		private void TextRegEx_Enter(object sender, EventArgs e)
		{
			SetCurrentTextBox(this._textRegEx);
		}

		private void TextRegEx_Leave(object sender, EventArgs e)
		{
			SetCurrentTextBox(null);
		}

		private void TextInput_TextChanged(object sender, EventArgs e)
		{
			if (!this._isHighlighting)
			{
				this._isHighlighting = true;
				BuildResult(BuildReason.Input);
				this._isHighlighting = false;
			}
			
			this._documentIsUnsaved = true;

			this._menuItemEditUndo.ToolTipText = string.Format((string)this._menuItemEditUndo.Tag, this._textInput.UndoActionName);
			this._menuItemEditRedo.ToolTipText = string.Format((string)this._menuItemEditRedo.Tag, this._textInput.RedoActionName);
		}

		private void TextInput_Enter(object sender, EventArgs e)
		{
			SetCurrentTextBox(this._textInput);
		}

		private void TextInput_Leave(object sender, EventArgs e)
		{
			SetCurrentTextBox(null);
		}

		private void ComboRegExHighlight_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this._isHighlighting)
			{
				this._isHighlighting = true;

				if (this._comboRegExHighlight.SelectedIndex == 0)
				{
					ClearHighlightRegEx();
				}

				BuildResult(BuildReason.RegEx);
				this._isHighlighting = false;
			}
		}
		
		private void ComboInputHighlight_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this._isHighlighting)
			{
				this._isHighlighting = true;

				if (this._comboInputHighlight.SelectedIndex == 0)
				{
					ClearHighlightInput();
				}

				BuildResult(BuildReason.Input);

				this._isHighlighting = false;
			}
		}

		#endregion

		#endregion
	}
}
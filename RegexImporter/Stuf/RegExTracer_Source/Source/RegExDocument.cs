using System;
using System.Xml.Serialization;

namespace RegExTracer
{
	[XmlRoot(ElementName = "regex_document")]
	public class RegExDocument
	{
		private string _regex;
		private string _input;
		private bool _optionECMAScript;
		private bool _optionSingleLine;
		private bool _optionExplicitCapture;
		private bool _optionIgnoreCase;
		private bool _optionRightToLeft;
		private bool _optionMultiline;
		private bool _optionCultureInvariant;
		private bool _optionIgnoreWhitespace;

		public RegExDocument()
		{
		}

		public RegExDocument(
			string regex,
			string input,
			bool optionECMAScript,
			bool optionSingleLine,
			bool optionExplicitCapture,
			bool optionIgnoreCase,
			bool optionRightToLeft,
			bool optionMultiline,
			bool optionCultureInvariant,
			bool optionIgnoreWhitespace)
		{
			this._regex = regex;
			this._input = input;
			this._optionECMAScript = optionECMAScript;
			this._optionSingleLine = optionSingleLine;
			this._optionExplicitCapture = optionExplicitCapture;
			this._optionIgnoreCase = optionIgnoreCase;
			this._optionRightToLeft = optionRightToLeft;
			this._optionMultiline = optionMultiline;
			this._optionCultureInvariant = optionCultureInvariant;
			this._optionIgnoreWhitespace = optionIgnoreWhitespace;
		}

		[XmlElement("regex")]
		public string RegEx
		{
			get { return this._regex; }
			set { this._regex = value; }
		}

		[XmlElement("input")]
		public string Input
		{
			get { return this._input; }
			set { this._input = value; }
		}

		[XmlElement("option_ecma_script")]
		public bool OptionECMAScript
		{
			get { return this._optionECMAScript; }
			set { this._optionECMAScript = value; }
		}

		[XmlElement("option_single_line")]
		public bool OptionSingleLine
		{
			get { return this._optionSingleLine; }
			set { this._optionSingleLine = value; }
		}

		[XmlElement("option_explicit_capture")]
		public bool OptionExplicitCapture
		{
			get { return this._optionExplicitCapture; }
			set { this._optionExplicitCapture = value; }
		}

		[XmlElement("option_ignore_case")]
		public bool OptionIgnoreCase
		{
			get { return this._optionIgnoreCase; }
			set { this._optionIgnoreCase = value; }
		}

		[XmlElement("option_right_to_left")]
		public bool OptionRightToLeft
		{
			get { return this._optionRightToLeft; }
			set { this._optionRightToLeft = value; }
		}

		[XmlElement("option_multiline")]
		public bool OptionMultiline
		{
			get { return this._optionMultiline; }
			set { this._optionMultiline = value; }
		}

		[XmlElement("option_culture_invariant")]
		public bool OptionCultureInvariant
		{
			get { return this._optionCultureInvariant; }
			set { this._optionCultureInvariant = value; }
		}

		[XmlElement("option_ignore_whitespace")]
		public bool OptionIgnoreWhitespace
		{
			get { return this._optionIgnoreWhitespace; }
			set { this._optionIgnoreWhitespace = value; }
		}
	}
}

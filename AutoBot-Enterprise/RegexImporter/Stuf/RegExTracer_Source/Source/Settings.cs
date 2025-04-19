using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace RegExTracer
{
	public class Settings
	{
		#region Constants

		/// <summary>
		/// Lightness = 75
		/// </summary>
		private static Color[] HighlightColorListDefault = 
		{
			/* Dark Blue */ Color.FromArgb(0x7F, 0xCF, 0xFF),
			/* Red       */ Color.FromArgb(0xFF, 0x7F, 0x7F),
			/* Green     */ Color.FromArgb(0x7F, 0xFF, 0x7F),
			/* Orange    */ Color.FromArgb(0xFF, 0xCF, 0x7F),
			/* Blue      */ Color.FromArgb(0x7F, 0xFF, 0xFF),
			/* Yellow    */ Color.FromArgb(0xFF, 0xFF, 0x7F),
			/* Violet    */ Color.FromArgb(0x7F, 0x7F, 0xFF),
			/* Gray      */ Color.FromArgb(0xBF, 0xBF, 0xBF)
		};
		
		private static Color BackColorDefault = Color.FromArgb(0xEF, 0xEF, 0xEF);
		
		private static Color ForeColorDefault = Color.FromArgb(0x00, 0x00, 0x00);
		
		#endregion
		
		#region Types
		
		[Serializable]
		public class SettingsData
		{
			private Color _foreColor = ForeColorDefault;
			private Color _backColor = BackColorDefault;
			private float _textSize = 10.0F;
			private HighlighingType _highlighingType = HighlighingType.BackGround;
			private IList<Color> _highlightColorList = new List<Color>(HighlightColorListDefault);
			
			public SettingsData()
			{
			}

			public Color ForeColor
			{
				get { return this._foreColor; }
				set { this._foreColor = value; }
			}

			public Color BackColor
			{
				get { return this._backColor; }
				set { this._backColor = value; }
			}

			public float TextSize
			{
				get { return this._textSize; }
				set { this._textSize = value; }
			}

			public HighlighingType HighlighingType
			{
				get { return this._highlighingType; }
				set { this._highlighingType = value; }
			}
			
			public IList<Color> HighlightColorList
			{
				get { return this._highlightColorList; }
				set { this._highlightColorList = value; }
			}
		}
		
		#endregion
		
		#region Variables

		private SettingsData _settingsData = new SettingsData();
		private string _settingsFilename = Application.UserAppDataPath + "\\Settings.bin";
		
		#endregion
		
		#region Methods

		public Settings()
		{
			Load();
		}

		public void Load()
		{
			if (File.Exists(this._settingsFilename))
			{
				BinaryFormatter formatter = new BinaryFormatter();

				using (Stream stream = new FileStream(this._settingsFilename, FileMode.Open, FileAccess.Read))
				{
					this._settingsData = (SettingsData)formatter.Deserialize(stream);

					stream.Close();
				}
			}
		}

		public void Save()
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (Stream stream = new FileStream(this._settingsFilename, FileMode.Create, FileAccess.Write))
			{
				formatter.Serialize(stream, this._settingsData);

				stream.Close();
			}
		}

		#endregion
		
		#region Properties

		public Color ForeColor
		{
			get { return this._settingsData.ForeColor; }
			set { this._settingsData.ForeColor = value; }
		}

		public Color BackColor
		{
			get { return this._settingsData.BackColor; }
			set { this._settingsData.BackColor = value; }
		}

		public float TextSize
		{
			get { return this._settingsData.TextSize; }
			set { this._settingsData.TextSize = value; }
		}

		public HighlighingType HighlighingType
		{
			get { return this._settingsData.HighlighingType; }
			set { this._settingsData.HighlighingType = value; }
		}

		public IList<Color> HighlightColorList
		{
			get { return this._settingsData.HighlightColorList; }
			set { this._settingsData.HighlightColorList = value; }
		}

		#endregion
	}

	public enum HighlighingType
	{
		BackGround,
		ForeGround
	}
}

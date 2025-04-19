using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RegExTracer
{
	public partial class FormToolsOptions : Form
	{
		#region Variables

		private Settings _settings;

		#endregion
		
		#region Methods

		public FormToolsOptions(Settings settings)
		{
			InitializeComponent();

			this._settings = settings;

			this._labelColor.ForeColor = this._settings.ForeColor;
			this._labelColor.BackColor = this._settings.BackColor;
			this._numericTextSize.Value = (decimal)this._settings.TextSize;
			
			switch (this._settings.HighlighingType)
			{
				case HighlighingType.BackGround:
					this._comboHighlightingType.SelectedIndex = 0;
					break;
				case HighlighingType.ForeGround:
					this._comboHighlightingType.SelectedIndex = 1;
					break;
			}

			this._listColorList.Items.Clear();

			foreach (Color color in this._settings.HighlightColorList)
			{
				this._listColorList.Items.Add(color);
			}

			this._listColorList.Refresh();
		}

		private void ButtonForeColor_Click(object sender, EventArgs e)
		{
			this._colorDialog.Color = this._labelColor.ForeColor;

			if (this._colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this._labelColor.ForeColor = this._colorDialog.Color;
				this._listColorList.Refresh();
			}
		}

		private void ButtonBackColor_Click(object sender, EventArgs e)
		{
			this._colorDialog.Color = this._labelColor.BackColor;

			if (this._colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this._labelColor.BackColor = this._colorDialog.Color;
				this._listColorList.Refresh();
			}
		}

		private void NumericTextSize_ValueChanged(object sender, EventArgs e)
		{
			this._labelColor.Font = new Font(this._labelColor.Font.FontFamily, (float)this._numericTextSize.Value);
		}
		
		private void ComboHighlightingType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this._listColorList.Refresh();
		}
		
		private void ListColorList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this._buttonColorChoose.Enabled = this._listColorList.SelectedIndex != -1;
			this._buttonColorRemove.Enabled = (this._listColorList.SelectedIndex != -1) && (this._listColorList.Items.Count > 1);
		}

		private void ListColorList_DoubleClick(object sender, EventArgs e)
		{
			this._buttonColorChoose.PerformClick();
		}

		private void ListColorList_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemWidth = this._listColorList.ClientRectangle.Width;
			e.ItemHeight = 12 + 2 * this._listColorList.Font.Height;
		}

		private void ListColorList_DrawItem(object sender, DrawItemEventArgs e)
		{
			Brush brushItemColor = new SolidBrush((Color)this._listColorList.Items[e.Index]);
			Brush brushItemForeColor = new SolidBrush(this._labelColor.ForeColor);
			Brush brushItemBackColor = new SolidBrush(this._labelColor.BackColor);
			Brush brushListForeColor = new SolidBrush(this._listColorList.ForeColor);
			Brush brushListBackColor = new SolidBrush(this._listColorList.BackColor);

			Rectangle rectItem = e.Bounds;

			rectItem.Inflate(-3, -3);

			RectangleF rectText = new RectangleF(rectItem.X, rectItem.Y, rectItem.Width, rectItem.Height);

			rectText.Inflate(-3, -3);

			switch (this._comboHighlightingType.SelectedIndex)
			{
				case 0:
					e.Graphics.FillRectangle(brushItemColor, rectItem);
					e.Graphics.DrawString(this._labelColor.Text, e.Font, brushItemForeColor, rectText);
					break;
				case 1:
					e.Graphics.FillRectangle(brushItemBackColor, rectItem);
					e.Graphics.DrawString(this._labelColor.Text, e.Font, brushItemColor, rectText);
					break;
			}
			
			rectItem = e.Bounds;

			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				rectItem.Inflate(-1, -1);
				e.Graphics.DrawRectangle(new Pen(brushListForeColor, 1), rectItem);
				rectItem.Inflate(-1, -1);
				e.Graphics.DrawRectangle(new Pen(brushListBackColor, 1), rectItem);
			}
			else
			{
				rectItem.Inflate(-2, -2);
				e.Graphics.DrawRectangle(new Pen(brushListBackColor, 4), rectItem);
			}
		}

		private void ButtonColorAdd_Click(object sender, EventArgs e)
		{
			if (this._colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this._listColorList.Items.Add(this._colorDialog.Color);
			}
		}

		private void ButtonColorChoose_Click(object sender, EventArgs e)
		{
			this._colorDialog.Color = (Color)this._listColorList.Items[this._listColorList.SelectedIndex];

			if (this._colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this._listColorList.Items[this._listColorList.SelectedIndex] = this._colorDialog.Color;
			}
		}

		private void ButtonColorRemove_Click(object sender, EventArgs e)
		{
			this._listColorList.Items.RemoveAt(this._listColorList.SelectedIndex);
		}
		
		private void ButtonOK_Click(object sender, EventArgs e)
		{
			this._settings.ForeColor = this._labelColor.ForeColor;
			this._settings.BackColor = this._labelColor.BackColor;
			this._settings.TextSize = (float)this._numericTextSize.Value;
			
			switch (this._comboHighlightingType.SelectedIndex)
			{
				case 0:
					this._settings.HighlighingType = HighlighingType.BackGround;
					break;
				case 1:
					this._settings.HighlighingType = HighlighingType.ForeGround;
					break;
			}
			
			this._settings.HighlightColorList.Clear();

			foreach (Color color in this._listColorList.Items)
			{
				this._settings.HighlightColorList.Add(color);
			}

			this._settings.Save();

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		
		private void ButtonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		#endregion
	}
}
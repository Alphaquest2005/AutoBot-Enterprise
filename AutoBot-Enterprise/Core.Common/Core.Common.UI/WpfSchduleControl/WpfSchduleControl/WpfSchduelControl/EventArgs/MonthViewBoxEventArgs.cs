using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace WpfSchduleControl
{
    /// <summary>
    /// Holds data about a box of text to be rendered on the month view
    /// </summary>
    public class MonthViewBoxEventArgs
    {
        #region Fields
        private DrawingContext _DrawingContext;
        private Color _textColor;
        private Color _backgroundColor;
        private string _text;
        private Color _borderColor;
        private Rect _bounds;
        private FontFamily _font;
        private HorizontalAlignment _TextFlags;
        #endregion

        #region Ctor

        internal MonthViewBoxEventArgs(DrawingContext g, Rect bounds, string text, HorizontalAlignment textAlign, Color textColor, Color backColor, Color borderColor)
        {
            _DrawingContext = g;
            _bounds = bounds;
            Text = text;
            TextColor = textColor;
            BackgroundColor = backColor;
            BorderColor = borderColor;

            switch (textAlign)
            {
                case HorizontalAlignment.Center:
                    TextFlags |= HorizontalAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    TextFlags |= HorizontalAlignment.Right;
                    break;
                case HorizontalAlignment.Left:
                    TextFlags |= HorizontalAlignment.Left;
                    break;
                default:
                    break;
            }

          //  TextFlags |= VerticalAlignment.Center;
        }

        internal MonthViewBoxEventArgs(DrawingContext g, Rect bounds, string text, Color textColor)
            : this(g, bounds, text, HorizontalAlignment.Center, textColor, new Color(), new Color())
        {}

        internal MonthViewBoxEventArgs(DrawingContext g, Rect bounds, string text, Color textColor, Color backColor)
            : this(g, bounds, text, HorizontalAlignment.Center, textColor, backColor, new Color())
        {}

        internal MonthViewBoxEventArgs(DrawingContext g, Rect bounds, string text, HorizontalAlignment textAlign, Color textColor, Color backColor)
            : this(g, bounds, text, textAlign, textColor, backColor, new Color())
        { }

        internal MonthViewBoxEventArgs(DrawingContext g, Rect bounds, string text, HorizontalAlignment textAlign, Color textColor)
            : this(g, bounds, text, textAlign, textColor, new Color(), new Color())
        { }

        #endregion

        #region Props

        /// <summary>
        /// Gets or sets the bounds of the box
        /// </summary>
        public Rect Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Gets or sets the font of the text. If null, default will be used.
        /// </summary>
        public FontFamily Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Gets or sets the DrawingContext object where to draw
        /// </summary>
        public DrawingContext DrawingContext
        {
            get { return _DrawingContext; }
        }

        /// <summary>
        /// Gets or sets the border color of the box
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Gets or sets the text of the box
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets the background color of the box
        /// </summary>
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the text color of the box
        /// </summary>
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        /// <summary>
        /// Gets or sets the flags of the text
        /// </summary>
        public HorizontalAlignment TextFlags
        {
            get { return _TextFlags; }
            set { _TextFlags = value; }
        }


        #endregion
    }
}

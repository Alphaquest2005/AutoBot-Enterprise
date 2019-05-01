using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace WpfSchduleControl
{
    /// <summary>
    /// Provides color information of calendar graphical elements
    /// </summary>
    public class CalendarColorTable
    {
        #region Static

        /// <summary>
        /// Returns the result of combining the specified colors
        /// </summary>
        /// <param name="c1">First color to combine</param>
        /// <param name="c2">Second olor to combine</param>
        /// <returns>Average of both colors</returns>
        public static Color Combine(Color c1, Color c2)
        {
            return Color.FromArgb(
                Convert.ToByte((c1.R + c2.R) / 2),
                Convert.ToByte((c1.G + c2.G) / 2),
                Convert.ToByte((c1.B + c2.B) / 2),
                Convert.ToByte((c1.A + c2.A) / 2)
                );
        }

        /// <summary>
        /// Converts the hex formatted color to a <see cref="Color"/>
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            var convertFromString = ColorConverter.ConvertFromString(hex);
            if (convertFromString != null) return (Color) convertFromString;
            return new Color();
        }

        #endregion

        #region Colors

        /// <summary>
        /// Background color of calendar
        /// </summary>
        public Color Background = SystemColors.ControlColor;

        /// <summary>
        /// Background color of days in even months
        /// </summary>
        public Color DayBackgroundEven = SystemColors.ControlColor;

        /// <summary>
        /// Background color of days in odd months
        /// </summary>
        public Color DayBackgroundOdd = SystemColors.ControlLightColor;

        /// <summary>
        /// Background color of selected days
        /// </summary>
        public Color DayBackgroundSelected = SystemColors.HighlightColor;

        /// <summary>
        /// Border of 
        /// </summary>
        public Color DayBorder = SystemColors.ControlDarkColor;

        /// <summary>
        /// Background color of day headers.
        /// </summary>
        public Color DayHeaderBackground = Combine(SystemColors.ControlDarkColor, SystemColors.ControlColor);

        /// <summary>
        /// Color of text of day headers
        /// </summary>
        public Color DayHeaderText = SystemColors.GrayTextColor;

        /// <summary>
        /// Color of secondary text in headers
        /// </summary>
        public Color DayHeaderSecondaryText = SystemColors.GrayTextColor;

        /// <summary>
        /// Color of border of the top part of the days
        /// </summary>
        /// <remarks>
        /// The DayTop is the zone of the calendar where items that lasts all or more are placed.
        /// </remarks>
        public Color DayTopBorder = SystemColors.ControlDarkColor;

        /// <summary>
        /// Color of border of the top parth of the days when selected
        /// </summary>
        /// <remarks>
        /// The DayTop is the zone of the calendar where items that lasts all or more are placed.
        /// </remarks>
        public Color DayTopSelectedBorder = SystemColors.ControlDarkColor;

        /// <summary>
        /// Background color of day tops.
        /// </summary>
        /// <remarks>
        /// The DayTop is the zone of the calendar where items that lasts all or more are placed.
        /// </remarks>
        public Color DayTopBackground = SystemColors.ControlLightColor;

        /// <summary>
        /// Background color of selected day tops.
        /// </summary>
        /// <remarks>
        /// The DayTop is the zone of the calendar where items that lasts all or more are placed.
        /// </remarks>
        public Color DayTopSelectedBackground = SystemColors.HighlightColor;

        /// <summary>
        /// Color of items borders
        /// </summary>
        public Color ItemBorder = SystemColors.ControlTextColor;

        /// <summary>
        /// Background color of items
        /// </summary>
        public Color ItemBackground = SystemColors.WindowColor;

        /// <summary>
        /// Forecolor of items
        /// </summary>
        public Color ItemText = SystemColors.WindowTextColor;

        /// <summary>
        /// Color of secondary text on items (Dates and times)
        /// </summary>
        public Color ItemSecondaryText = SystemColors.GrayTextColor;

        /// <summary>
        /// Color of items shadow
        /// </summary>
        public Color ItemShadow = Color.FromArgb(50,Colors.Black.R, Colors.Black.G, Colors.Black.B);

        /// <summary>
        /// Color of items selected border
        /// </summary>
        public Color ItemSelectedBorder = SystemColors.HighlightColor;

        /// <summary>
        /// Background color of selected items
        /// </summary>
        public Color ItemSelectedBackground = Combine(SystemColors.HighlightColor, SystemColors.HighlightTextColor);

        /// <summary>
        /// Forecolor of selected items
        /// </summary>
        public Color ItemSelectedText = SystemColors.WindowTextColor;

        /// <summary>
        /// Background color of week headers
        /// </summary>
        public Color WeekHeaderBackground = Combine(SystemColors.ControlDarkColor, SystemColors.ControlColor);

        /// <summary>
        /// Border color of week headers
        /// </summary>
        public Color WeekHeaderBorder = SystemColors.ControlDarkColor;

        /// <summary>
        /// Forecolor of week headers
        /// </summary>
        public Color WeekHeaderText = SystemColors.ControlTextColor;

        /// <summary>
        /// Forecolor of day names
        /// </summary>
        public Color WeekDayName = SystemColors.ControlTextColor;

        /// <summary>
        /// Border color of today day
        /// </summary>
        public Color TodayBorder = Colors.Orange;

        /// <summary>
        /// Background color of today's DayTop
        /// </summary>
        public Color TodayTopBackground = Colors.Orange;

        /// <summary>
        /// Color of lines in timescale
        /// </summary>
        public Color TimeScaleLine = SystemColors.ControlDarkColor;

        /// <summary>
        /// Color of text representing hours on the timescale
        /// </summary>
        public Color TimeScaleHours = SystemColors.GrayTextColor;

        /// <summary>
        /// Color of text representing minutes on the timescale
        /// </summary>
        public Color TimeScaleMinutes = SystemColors.GrayTextColor;

        /// <summary>
        /// Background color of time units
        /// </summary>
        public Color TimeUnitBackground = SystemColors.ControlColor;

        /// <summary>
        /// Background color of highlighted time units
        /// </summary>
        public Color TimeUnitHighlightedBackground = Combine(SystemColors.ControlColor, SystemColors.ControlLightLightColor);

        /// <summary>
        /// Background color of selected time units
        /// </summary>
        public Color TimeUnitSelectedBackground = SystemColors.HighlightColor;

        /// <summary>
        /// Color of light border of time units
        /// </summary>
        public Color TimeUnitBorderLight = SystemColors.ControlDarkColor;

        /// <summary>
        /// Color of dark border of time units
        /// </summary>
        public Color TimeUnitBorderDark = SystemColors.ControlDarkDarkColor;

        /// <summary>
        /// Border color of the overflow indicators
        /// </summary>
        public Color DayOverflowBorder = Colors.White;

        /// <summary>
        /// Background color of the overflow indicators
        /// </summary>
        public Color DayOverflowBackground = SystemColors.ControlLightColor;

        /// <summary>
        /// Background color of selected overflow indicators
        /// </summary>
        public Color DayOverflowSelectedBackground = Colors.Orange;

        #endregion
    }
}


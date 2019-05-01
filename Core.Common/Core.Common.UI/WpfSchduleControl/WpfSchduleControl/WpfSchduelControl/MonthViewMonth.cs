using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WpfSchduleControl
{
    /// <summary>
    /// Represents a month displayed on <see cref="CalendarMonth"/>
    /// </summary>
    public class MonthViewMonth
    {
        #region Fields
        private DateTime _date;
        private Rect monthNameBounds;
        private Rect[] dayNamesBounds;
        private MonthViewDay[] days;
        private string[] _dayHeaders;
        private Size _size;
        private Point _location;
        private MonthView _monthview;
        #endregion

        #region Ctor

        internal MonthViewMonth(MonthView monthView, DateTime date)
        {
            if (date.Day != 1)
            {
                date = new DateTime(date.Year, date.Month, 1);
            }


            _monthview = monthView;
            _date = date;

            int preDays = (new int[] { 0, 1, 2, 3, 4, 5, 6 })[(int)date.DayOfWeek] - (int)MonthView.FirstDayOfWeek;
            days = new MonthViewDay[6 * 7];
            DateTime curDate = date.AddDays(-preDays);
            DayHeaders = new string[7];

            for (int i = 0; i < days.Length; i++)
            {
                days[i] = new MonthViewDay(this, curDate);

                if (i < 7)
                {
                    DayHeaders[i] = curDate.ToString(MonthView.DayNamesFormat).Substring(0, MonthView.DayNamesLength);
                }

                curDate = curDate.AddDays(1);
            }
        }

        #endregion

        #region Props

        public Rect Bounds
        {
            get { return new Rect(Location, Size); }
        }

        public MonthView MonthView
        {
            get { return _monthview; }
        }

        public Point Location
        {
            get { return _location; }
        }

        public Size Size
        {
            get { return MonthView.MonthSize; }
        }

        public MonthViewDay[] Days
        {
            get { return days; }
            set { days = value; }
        }

        public Rect[] DayNamesBounds
        {
            get { return dayNamesBounds; }
            set { dayNamesBounds = value; }
        }

        public string[] DayHeaders
        {
            get { return _dayHeaders; }
            set { _dayHeaders = value; }
        }

        public Rect MonthNameBounds
        {
            get { return monthNameBounds; }
            set { monthNameBounds = value; }
        }

        /// <summary>
        /// Gets or sets the date of the first day of the month
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the value of the <see cref="Location"/> property
        /// </summary>
        /// <param name="location"></param>
        internal void SetLocation(Point location)
        {

            int startX = location.X;
            int startY = location.Y;
            int curX = startX;
            int curY = startY;

            _location = location;

            monthNameBounds = new Rect(location, new Size(Size.Width,MonthView.DaySize.Height));

            if (MonthView.DayNamesVisible)
            {
                dayNamesBounds = new Rect[7];
                curY = location.Y + MonthView.DaySize.Height;
                for (int i = 0; i < dayNamesBounds.Length; i++)
                {
                    DayNamesBounds[i] = new Rect(curX, curY, MonthView.DaySize.Width, MonthView.DaySize.Height);

                    curX += MonthView.DaySize.Width;
                }

            }
            else
            {
                dayNamesBounds = new Rect[] { };
            }

            curX = startX;
            curY = startY + MonthView.DaySize.Height * 2;

            for (int i = 0; i < Days.Length; i++)
            {
                Days[i].SetBounds(new Rect(new Point(curX, curY), MonthView.DaySize));

                curX += MonthView.DaySize.Width;

                if ((i + 1) % 7 == 0)
                {
                    curX = startX;
                    curY += MonthView.DaySize.Height;
                }
            }

        }

        #endregion
    }
}

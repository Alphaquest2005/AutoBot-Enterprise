using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfSchduleControl
{
    public class CalendarProfessionalRenderer
        : CalendarSystemRenderer
    {
        #region Fields

        public Color HeaderA = FromHex("#E4ECF6");
        public Color HeaderB = FromHex("#D6E2F1");
        public Color HeaderC = FromHex("#C2D4EB");
        public Color HeaderD = FromHex("#D0DEEF");

        public Color TodayA = FromHex("#F8D478");
        public Color TodayB = FromHex("#F8D478");
        public Color TodayC = FromHex("#F2AA36");
        public Color TodayD = FromHex("#F7C966");

        #endregion

        #region Ctor

        public CalendarProfessionalRenderer(Calendar c)
            : base(c)
        {
            
            ColorTable.Background = FromHex("#E3EFFF");
            ColorTable.DayBackgroundEven = FromHex("#A5BFE1");
            ColorTable.DayBackgroundOdd = FromHex("#FFFFFF");
            ColorTable.DayBackgroundSelected = FromHex("#E6EDF7");
            ColorTable.DayBorder = FromHex("#5D8CC9");
            ColorTable.DayHeaderBackground = FromHex("#DFE8F5");
            ColorTable.DayHeaderText = Colors.Black;
            ColorTable.DayHeaderSecondaryText = Colors.Black;
            ColorTable.DayTopBorder = FromHex("#5D8CC9");
            ColorTable.DayTopSelectedBorder = FromHex("#5D8CC9");
            ColorTable.DayTopBackground = FromHex("#A5BFE1");
            ColorTable.DayTopSelectedBackground = FromHex("#294C7A");
            ColorTable.ItemBorder = FromHex("#5D8CC9");
            ColorTable.ItemBackground = FromHex("#C0D3EA");
            ColorTable.ItemText = Colors.Black;
            ColorTable.ItemSecondaryText = FromHex("#294C7A");
            ColorTable.ItemSelectedBorder = Colors.Black;
            ColorTable.ItemSelectedBackground = FromHex("#C0D3EA");
            ColorTable.ItemSelectedText = Colors.Black;
            ColorTable.WeekHeaderBackground = FromHex("#DFE8F5");
            ColorTable.WeekHeaderBorder = FromHex("#5D8CC9");
            ColorTable.WeekHeaderText = FromHex("#5D8CC9");
            ColorTable.TodayBorder = FromHex("#EE9311");
            ColorTable.TodayTopBackground = FromHex("#EE9311");
            ColorTable.TimeScaleLine = FromHex("#6593CF");
            ColorTable.TimeScaleHours = FromHex("#6593CF");
            ColorTable.TimeScaleMinutes = FromHex("#6593CF");
            ColorTable.TimeUnitBackground = FromHex("#E6EDF7");
            ColorTable.TimeUnitHighlightedBackground = Colors.White;
            ColorTable.TimeUnitSelectedBackground = FromHex("#294C7A");
            ColorTable.TimeUnitBorderLight = FromHex("#D5E1F1");
            ColorTable.TimeUnitBorderDark = FromHex("#A5BFE1");
            ColorTable.WeekDayName = FromHex("#6593CF");

            SelectedItemBorder = 2f;
            ItemRoundness = 5;
        }

        #endregion

        #region Private Method

        public static void GradientRect(DrawingContext g, Rect bounds, Color a, Color b)
        {
            var br = new LinearGradientBrush(b, a, -90);
            g.DrawRect(br,new Pen() , bounds);
            
        }

        public static void GlossyRect(DrawingContext g, Rect bounds, Color a, Color b, Color c, Color d)
        {
            Rect top = new Rect(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 2);
            Rect bot = new Rect(bounds.Left, top.Bottom, bounds.Right, bounds.Bottom);

            GradientRect(g, top, a, b);
            GradientRect(g, bot, c, d);

        }

        /// <summary>
        /// Shortcut to one on CalendarColorTable
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Color FromHex(string color)
        {
            return CalendarColorTable.FromHex(color);
        }

        #endregion

        #region Overrides

        public override void OnInitialize(CalendarRendererEventArgs e)
        {
            base.OnInitialize(e);

            e.Calendar.FontFamily = SystemFonts.CaptionFontFamily;
        }

        public override void OnDrawDayHeaderBackground(CalendarRendererDayEventArgs e)
        {
            Rect r = e.Day.HeaderBounds;

            if (e.Day.Date == DateTime.Today)
            {
                GlossyRect(e.DrawingContext, e.Day.HeaderBounds, TodayA, TodayB, TodayC, TodayD);
            }
            else
            {
                GlossyRect(e.DrawingContext, e.Day.HeaderBounds, HeaderA, HeaderB, HeaderC, HeaderD);
            }

            if (e.Calendar.DaysMode == CalendarDaysMode.Short)
            {

                Pen p = new Pen(new SolidColorBrush(ColorTable.DayBorder),1 );
                
                    e.DrawingContext.DrawLine(p, new Point(r.Left, r.Top),new Point( r.Right, r.Top));
                    e.DrawingContext.DrawLine(p,new Point( r.Left, r.Bottom), new Point( r.Right, r.Bottom));
                
            }
        }

        private int penThickness = 1;
        public override void OnDrawItemBorder(CalendarRendererItemBoundsEventArgs e)
        {
            base.OnDrawItemBorder(e);
            Pen p = new Pen(new SolidColorBrush(Color.FromArgb( 150, Colors.White.R, Colors.White.G, Colors.White.B)),penThickness );
            
                e.DrawingContext.DrawLine(p, new Point(e.Bounds.Left + ItemRoundness, e.Bounds.Top + 1),new Point(e.Bounds.Right - ItemRoundness, e.Bounds.Top + 1)); 
            

            if (e.Item.Selected && !e.Item.IsDragging)
            {
                bool horizontal = false;
                bool vertical = false;
                Rect r1 = new Rect(0, 0, 5, 5);
                Rect r2 = new Rect(0, 0, 5, 5);

                horizontal = e.Item.IsOnDayTop;
                vertical = !e.Item.IsOnDayTop && e.Calendar.DaysMode == CalendarDaysMode.Expanded;

                if (horizontal)
                {
                    r1.X = e.Bounds.Left - 2;
                    r2.X = e.Bounds.Right - r1.Width + 2;
                    r1.Y = e.Bounds.Top + (e.Bounds.Height - r1.Height) / 2;
                    r2.Y = r1.Y;
                }

                if (vertical)
                {
                    r1.Y = e.Bounds.Top - 2;
                    r2.Y = e.Bounds.Bottom - r1.Height + 2;
                    r1.X = e.Bounds.Left + (e.Bounds.Width - r1.Width) / 2;
                    r2.X = r1.X;
                }

                if ((horizontal || vertical) && Calendar.AllowItemResize)
                {
                    if (!e.Item.IsOpenStart && e.IsFirst)
                    {
                        e.DrawingContext.DrawRect(Brushes.White, new Pen(), r1);
                        e.DrawingContext.DrawRect(Brushes.Black, new Pen(), r1);
                    }

                    if (!e.Item.IsOpenEnd && e.IsLast)
                    {
                        e.DrawingContext.DrawRect(Brushes.White, new Pen(), r2);
                        e.DrawingContext.DrawRect(Brushes.Black, new Pen(), r2);
                    }
                } 
            }
        }

        #endregion
    }
}

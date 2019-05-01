using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Core.Common.UI
{
    public class SliderPanel : Panel, INotifyPropertyChanged
    {
        public SliderPanel()
        {
            MouseLeftButtonDown += new MouseButtonEventHandler(SliderPanel_MouseLeftButtonDown);
            MouseLeftButtonUp += new MouseButtonEventHandler(SliderPanel_MouseLeftButtonUp);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Point MouseStart, MouseNow, MouseFirst, MouseFinal;

        int _Counter;
        public int Counter
        {
            get
            {
                return _Counter;
            }
            set
            {
                _Counter = value;
                SendPropertyChanged("Counter");
            }
        }

        



        void SliderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseOver == true)
            {
                CaptureMouse();
                canmove = true;
                MouseStart = e.GetPosition(this);
                MouseNow = MouseStart;
                MouseFirst = MouseStart;

                MouseMove += new MouseEventHandler(SliderPanel_MouseMove);
                e.Handled = true;
            }
        }

        bool canmove = false;

        void SliderPanel_MouseMove(object sender, MouseEventArgs e)
        {

            if (canmove == true)
            {
                MouseNow = e.GetPosition(this);

                MoveControls();

                MouseStart = MouseNow;
                e.Handled = true;
            }
        }

        public void MoveControls()
        {
            if (canmove == true)
            {
                for (var i = 0; i < Children.Count; i++)
                {

                    if (Orientation == "Horizontal")
                    {
                        var yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + (MouseNow.X - MouseStart.X), 0);
                        Children[i].RenderTransform = yu;
                    }
                    else
                    {
                        var yu = new TranslateTransform(0, Children[i].RenderTransform.Value.OffsetY + (MouseNow.Y - MouseStart.Y));
                        Children[i].RenderTransform = yu;
                    }
                }

               
            }
        }

        public void MoveControls(double value)
        {
            if (Orientation == "Horizontal")
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var pFrom = Children[i].RenderTransform.Value.OffsetX;
                    var pTo = Children[i].RenderTransform.Value.OffsetX + value;
                    var da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));

                    var yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + value, 0);
                    Children[i].RenderTransform = yu;

                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
                }
            }
            else
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var pFrom = Children[i].RenderTransform.Value.OffsetY;
                    var pTo = Children[i].RenderTransform.Value.OffsetY + value;
                    var da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));

                    var yu = new TranslateTransform(0, Children[i].RenderTransform.Value.OffsetY + value);
                    Children[i].RenderTransform = yu;

                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.YProperty, da);
                }
            }
        }

        int _SelectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                _SelectedIndex = value;
                SendPropertyChanged("SelectedIndex");
                AnimateBySelectedIndex(value);
            }
        }

        void AnimateBySelectedIndex(int index)
        {
            if (index < 0 || index > Children.Count - 1 || index * -1 == Counter)
                return;

            index *= -1;
            double pTo, pFrom;

            if (Orientation == "Horizontal")
            {
                pTo = index * DesiredSize.Width;
                pFrom = index > Counter ? (pTo - DesiredSize.Width) : (pTo + DesiredSize.Width);
            }
            else
            {
                pTo = index * DesiredSize.Height;
                pFrom = index > Counter ? (pTo - DesiredSize.Height) : (pTo + DesiredSize.Height);
            }
            Counter = index;

            for (var i = 0; i < Children.Count; i++)
            {
                var da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));
                if (Orientation == "Horizontal")
                {
                    var yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX, 0);
                    Children[i].RenderTransform = yu;

                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
                }
                else
                {
                    var yu = new TranslateTransform(0, Children[i].RenderTransform.Value.OffsetY);
                    Children[i].RenderTransform = yu;

                    ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.YProperty, da);
                }
            }
        }

        void SliderPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MouseMove -= new MouseEventHandler(SliderPanel_MouseMove);

                MouseFinal = e.GetPosition(this);
                ReleaseMouseCapture();
                if (Orientation == "Horizontal")
                {
                    if ((MouseFinal.X - MouseFirst.X) > 0)
                    {
                        if (Math.Abs(MouseFinal.X - MouseFirst.X) > 50)
                            Counter = Counter + 1;
                    }
                    else
                    {
                        if (Math.Abs(MouseFinal.X - MouseFirst.X) > 50)
                            Counter = Counter - 1;
                    }
                }
                else
                {
                    if ((MouseFinal.Y - MouseFirst.Y) > 0)
                    {
                        if (Math.Abs(MouseFinal.Y - MouseFirst.Y) > 50)
                            Counter = Counter + 1;
                    }
                    else
                    {
                        if (Math.Abs(MouseFinal.Y - MouseFirst.Y) > 50)
                            Counter = Counter - 1;
                    }
                }

                double pTo, pFrom;

                if (Orientation == "Horizontal")
                {
                    pTo = Counter * DesiredSize.Width;
                    pFrom = (MouseFinal.X - MouseFirst.X) > 0 ? (pTo - DesiredSize.Width) + (MouseFinal.X - MouseFirst.X) : (pTo + DesiredSize.Width) + (MouseFinal.X - MouseFirst.X);

                    if (Math.Abs(MouseFinal.X - MouseFirst.X) < 50)
                        pFrom = pTo + (MouseFinal.X - MouseFirst.X);

                    if (Counter > 0)
                    {
                        //  pTo = (Counter - 1) * this.DesiredSize.Width;
                        pTo = (Counter - 1) * (MouseFinal.X - MouseFirst.X);
                        Counter = Counter - 1;
                    }
                    else if (Counter <= Children.Count * -1)
                    {
                        // pTo = (Counter + 1) * this.DesiredSize.Width;
                        pTo = (Counter + 1) * (MouseFirst.X - MouseFinal.X);
                        Counter = Counter + 1;
                    }
                }
                else
                {
                    pTo = Counter * DesiredSize.Height;
                    pFrom = (MouseFinal.Y - MouseFirst.Y) > 0 ? (pTo - DesiredSize.Height) + (MouseFinal.Y - MouseFirst.Y) : (pTo + DesiredSize.Height) + (MouseFinal.Y - MouseFirst.Y);

                    if (Math.Abs(MouseFinal.Y - MouseFirst.Y) < 50)
                        pFrom = pTo + (MouseFinal.Y - MouseFirst.Y);

                    if (Counter > 0)
                    {
                        //  pTo = (Counter - 1) * this.DesiredSize.Width;
                        pTo = (Counter - 1) * (MouseFinal.Y - MouseFirst.Y);
                        Counter = Counter - 1;
                    }
                    else if (Counter <= Children.Count * -1)
                    {
                        // pTo = (Counter + 1) * this.DesiredSize.Width;
                        pTo = (Counter + 1) * (MouseFirst.Y - MouseFinal.Y);
                        Counter = Counter + 1;
                    }
                }


                for (var i = 0; i < Children.Count; i++)
                {
                    var da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));


                    //((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
                }

                SelectedIndex = Math.Abs(Counter);
                e.Handled = true;
            }
            catch { }
            finally
            {
                canmove = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                resultSize.Width = Math.Max(resultSize.Width, DesiredSize.Width);
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            resultSize.Width =
                double.IsPositiveInfinity(availableSize.Width) ?
                resultSize.Width : availableSize.Width;

            resultSize.Height =
                double.IsPositiveInfinity(availableSize.Height) ?
                resultSize.Height : availableSize.Height;

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect((i * finalSize.Width), (double)0, finalSize.Width, finalSize.Height));
            }

            Clip = new RectangleGeometry(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));

            return base.ArrangeOverride(finalSize);
        }
        static Stack<string> pctl = new Stack<string>();

        public void MoveToPreviousCtl()
        {
            var ctl = pctl.Pop();
            while (ctl == ppctl && pctl.Count > 0)
            {
                ctl = pctl.Pop();
            }
            //if (ctl == ppctl && pctl.Count > 0) 
            MoveTo(ctl);
        }

        static string ppctl = "";
        private double navWidth = 25;
        private double navHeight = 40;
        public void MoveTo(string ctl)
        {
            var slid = this;

            Expander slidcontents;
            if (ctl == "ReportBRD")
            {
                var mainsales = Common.FindChild<Grid>(slid, "MainSalesContents");
                slidcontents = Common.FindChild<Expander>(mainsales, ctl);
            }
            else
            {
                slidcontents = Common.FindChild<Expander>(slid, ctl);
            }


            if (slidcontents == null)
            {
                MessageBox.Show("Control Not Found:" + ctl);
                return;
            }

            if (Orientation == "Horizontal")
            {
                var sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).X * -1;
                slid.MoveControls(sl + navWidth);
            }
            else
            {
                var sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).Y * -1;
                slid.MoveControls(sl + navHeight);
            }
            slidcontents.IsExpanded = true;
            ppctl = ctl;
            pctl.Push(ctl);
        }
        public void BringIntoView(string ctl)
        {
            var slid = this;

            Expander slidcontents;
            if (ctl == "ReportBRD")
            {
                var mainsales = Common.FindChild<Grid>(slid, "MainSalesContents");
                slidcontents = Common.FindChild<Expander>(mainsales, ctl);
            }
            else
            {
                slidcontents = Common.FindChild<Expander>(slid, ctl);
            }

            BringIntoView(slidcontents);



        }

        public void BringIntoView(Expander slidcontents)
        {
            var slid = this;
            if (slidcontents == null) return;
            if (slidcontents.IsExpanded == false) return;

            var sp = (FrameworkElement)slid.Parent;


            var exp = ((FrameworkElement)slidcontents.Content);

            if (slid.Orientation == "Horizontal")
            {
                var sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).X * -1;
                if (sl * -1 > (sp.ActualWidth - exp.ActualWidth) && sp.ActualWidth > exp.ActualWidth)
                {
                    slid.MoveControls(sl + sp.ActualWidth - exp.ActualWidth);
                }
                if (sl * -1 < 0 || sp.ActualWidth < exp.ActualWidth)
                {
                    slid.MoveControls(sl + navWidth);
                }
            }
            else
            {
                var sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).Y * -1;
                if (sl * -1 > (sp.ActualHeight - exp.ActualHeight))
                {
                    // slid.MoveControls(sl + sp.ActualHeight - exp.ActualHeight + nav);
                    slid.MoveControls(sl + navHeight);
                }
                if (sl * -1 < 0)
                {
                    slid.MoveControls(sl + navHeight);
                }
            }
            ppctl = slidcontents.Name;
            pctl.Push(slidcontents.Name);
        }

        public string Orientation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InsightPresentationLayer
{
    public class SliderPanel : Panel, INotifyPropertyChanged
    {
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

        public SliderPanel()
        {
            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(SliderPanel_MouseLeftButtonUp);

        }



        void SliderPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseOver == true)
            {
                this.CaptureMouse();
                canmove = true;
                this.MouseStart = e.GetPosition(this);
                this.MouseNow = this.MouseStart;
                this.MouseFirst = this.MouseStart;

                this.MouseMove += new System.Windows.Input.MouseEventHandler(SliderPanel_MouseMove);
            }
        }

        bool canmove = false;

        void SliderPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (canmove == true)
            {
                this.MouseNow = e.GetPosition(this);

                MoveControls();

                MouseStart = MouseNow;
                e.Handled = true;
            }
        }

        public void MoveControls()
        {
            if (canmove == true)
            {
                for (int i = 0; i < Children.Count; i++)
                {


                    TranslateTransform yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + (MouseNow.X - MouseStart.X), 0);
                    Children[i].RenderTransform = yu;

                }
            }
        }

        public void MoveControls(double value)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                double pFrom = Children[i].RenderTransform.Value.OffsetX;
                double pTo = Children[i].RenderTransform.Value.OffsetX + value;
                DoubleAnimation da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));

                TranslateTransform yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX + value, 0);
                Children[i].RenderTransform = yu;

                ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
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
            if (index < 0 || index > this.Children.Count - 1 || index * -1 == Counter)
                return;

            index *= -1;
            double pTo, pFrom;
            pTo = index * this.DesiredSize.Width;
            pFrom = index > Counter ? (pTo - this.DesiredSize.Width) : (pTo + this.DesiredSize.Width);

            Counter = index;

            for (int i = 0; i < Children.Count; i++)
            {
                DoubleAnimation da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));

                TranslateTransform yu = new TranslateTransform(Children[i].RenderTransform.Value.OffsetX, 0);
                Children[i].RenderTransform = yu;

                ((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
            }
        }

        void SliderPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.MouseMove -= new System.Windows.Input.MouseEventHandler(SliderPanel_MouseMove);

                this.MouseFinal = e.GetPosition(this);
                this.ReleaseMouseCapture();

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

                double pTo, pFrom;

                pTo = Counter * this.DesiredSize.Width;
                pFrom = (MouseFinal.X - MouseFirst.X) > 0 ? (pTo - this.DesiredSize.Width) + (MouseFinal.X - MouseFirst.X) : (pTo + this.DesiredSize.Width) + (MouseFinal.X - MouseFirst.X);

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



                for (int i = 0; i < Children.Count; i++)
                {
                    DoubleAnimation da = new DoubleAnimation(pFrom, pTo, new Duration(TimeSpan.FromSeconds(0.3)));


                    //((TranslateTransform)Children[i].RenderTransform).BeginAnimation(TranslateTransform.XProperty, da);
                }

                SelectedIndex = Math.Abs(Counter);
            }
            catch { }
            finally
            {
                canmove = false;
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in this.Children)
            {
                child.Measure(availableSize);
                resultSize.Width = Math.Max(resultSize.Width, this.DesiredSize.Width);
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

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect((i * finalSize.Width), (double)0, finalSize.Width, finalSize.Height));
            }

            this.Clip = new RectangleGeometry(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));

            return base.ArrangeOverride(finalSize);
        }
        static Stack<string> pctl = new Stack<string>();

        public void MoveToPreviousCtl()
        {
            string ctl = pctl.Pop();
            while (ctl == ppctl && pctl.Count > 0) ctl = pctl.Pop();
            MoveTo(ctl);
        }

        static string ppctl = "";
        private double navWidth = 25;
        public void MoveTo(string ctl)
        {
            SliderPanel slid = this;//Common.FindChild<SliderPanel>(MainView, "Slider");

            Expander slidcontents;
            if (ctl == "ReportBRD")
            {
                Grid mainsales = Common.FindChild<Grid>(slid, "MainSalesContents");
                slidcontents = Common.FindChild<Expander>(mainsales, ctl);
            }
            else
            {
                slidcontents = Common.FindChild<Expander>(slid, ctl);
            }




            // double sl = VisualTreeHelper.GetOffset(slidcontents).X*-1;
            double sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).X * -1;
            // Canvas.SetLeft(slidcontents, -1800);
            //slid.MouseStart = new Point(0, 0);
            //slid.MouseNow = new Point(-1800, 0);
            slid.MoveControls(sl + navWidth);
            slidcontents.IsExpanded = true;
            ppctl = ctl;
            pctl.Push(ctl);
        }
        public void BringIntoView(string ctl)
        {
            SliderPanel slid = this;//Common.FindChild<SliderPanel>(MainView, "Slider");

            Expander slidcontents;
            if (ctl == "ReportBRD")
            {
                Grid mainsales = Common.FindChild<Grid>(slid, "MainSalesContents");
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
            SliderPanel slid = this;//Common.FindChild<SliderPanel>(MainView, "Slider");

            if (slidcontents.IsExpanded == false) return;


            // double sl = VisualTreeHelper.GetOffset(slidcontents).X*-1;
            double sl = slidcontents.TransformToAncestor(slid.Parent as Visual).Transform(new Point(0, 0)).X * -1;
            // Canvas.SetLeft(slidcontents, -1800);
            //slid.MouseStart = new Point(0, 0);
            //slid.MouseNow = new Point(-1800, 0);
            FrameworkElement sp = (FrameworkElement)slid.Parent;


            FrameworkElement exp = ((FrameworkElement)slidcontents.Content);
            if (sl * -1 > (sp.ActualWidth - exp.ActualWidth))
            {
                slid.MoveControls(sl + sp.ActualWidth - ((FrameworkElement)slidcontents.Content).ActualWidth);
            }
            if (sl * -1 < 0)
            {
                slid.MoveControls(sl + navWidth);
            }
            ppctl = slidcontents.Name;
            pctl.Push(slidcontents.Name);
        }
    }
}

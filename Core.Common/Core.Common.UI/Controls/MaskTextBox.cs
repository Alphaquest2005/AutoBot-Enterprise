using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MOWPFCustomControls
{
    public class MaskTextBox : TextBox
    {
        public static DependencyProperty DataTypeProperty =
   DependencyProperty.Register("DataType", typeof(string), typeof(MaskTextBox), new PropertyMetadata("string"));
        public static DependencyProperty RegExProperty =
  DependencyProperty.Register("RegEx", typeof(string), typeof(MaskTextBox), new PropertyMetadata("string"));


        public string DataType
        {
            get { return (string)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public string RegEx
        {
            get { return (string)GetValue(RegExProperty); }
            set { SetValue(RegExProperty, value); }
        }

        public MaskTextBox()
            : base()
        {
            EventManager.RegisterClassHandler(
                typeof(MaskTextBox),
                DataObject.PastingEvent,
                (DataObjectPastingEventHandler)((sender, e) =>
                                                     {
                                                         if (!IsDataValid(e.DataObject))
                                                         {
                                                             var data = new DataObject();
                                                             data.SetText(String.Empty);
                                                             e.DataObject = data;
                                                             e.Handled = false;
                                                         }
                                                     }));
            AddHandler(PreviewKeyDownEvent, new RoutedEventHandler(PreviewKeyDownEventHandler));
            AddHandler(LostFocusEvent, new RoutedEventHandler(LostFocusEventHandler));
        }

        public void LostFocusEventHandler(object sender, RoutedEventArgs e)
        {
            if (Text == "-")
                Text = string.Empty;
        }

        public void PreviewKeyDownEventHandler(object sender, RoutedEventArgs e)
        {
            var ke = e as KeyEventArgs;
            if (ke.Key == Key.Space)
            {
                ke.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = !IsDataValid(e.Data);
            base.OnDrop(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (!IsDataValid(e.Data))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.None;
            }
            base.OnDragEnter(e);
        }

        private Boolean IsDataValid(
            IDataObject data)
        {
            var isValid = false;
            if (data != null)
            {
                var Text = data.GetData(DataFormats.Text) as String;
                if (!String.IsNullOrEmpty(Text == null ? null : Text.Trim()))
                {
                    switch (DataType)
                    {
                        case "INT":
                            var result = -1;
                            if (Int32.TryParse(Text.Trim(), out result))
                            {
                                if (result > 0)
                                {
                                    isValid = true;
                                }
                            }
                            break;

                        case "FLOAT":
                            float floatResult = -1;
                            if (float.TryParse(Text.Trim(), out floatResult))
                            {
                                if (floatResult > 0)
                                {
                                    isValid = true;
                                }
                            }
                            break;
                        case "RegEx":
                            if (Regex.IsMatch(Text, RegEx))
                            {
                                isValid = true;
                            }
                            break;

                    }
                }
            }
            return isValid;
        }


        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var Text = this.Text;
            Text = Text.Insert(CaretIndex, e.Text);
            switch (DataType)
            {
                case "INT":
                    var result = -1;
                    if (!Int32.TryParse(Text.Trim(), out result))
                    {
                        if (!Text.Equals("-"))
                            e.Handled = true;
                    }
                    break;

                case "FLOAT":
                    float floatResult = -1;
                    if (!float.TryParse(Text.Trim(), out floatResult))
                    {
                        if (!Text.Equals("-"))
                            e.Handled = true;
                    }
                    break;
                case "RegEx":
                    if (!Regex.IsMatch(Text, RegEx))
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

    }
}
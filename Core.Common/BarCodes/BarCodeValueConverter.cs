using System;
using System.Windows.Data;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace BarCodes
{
    public class BarCodeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long val = 0;
            if ( value != null && long.TryParse(value.ToString(), out val))
            {
                string sval = val.ToString().PadLeft(12, '0');
                BarCodes.UPCA.cUPCA upc = new UPCA.cUPCA();
                BitmapSource bs = upc.CreateBarCodeBitmapSource(sval, 1);
                return bs;

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

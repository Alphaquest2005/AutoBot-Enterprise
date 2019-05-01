using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;


namespace BarCodes.UPCA
{
    public class cUPCA
    {
        private Bitmap newBitmap = new Bitmap(120,80);
        private Graphics g;
        private int barCodeHeight = 80;
        private int placeMarker = 0;
        private int imageWidth = 0;
        private float imageScale = 1;
        private string UPCABegin = "0000000000000101";
        private string[] UPCALeft = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string UPCAMiddle = "01010";
        private string[] UPCARight = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
        private string UPCAEnd = "1010000000000000";

       

        public Image CreateBarCode(string txt, int scale)
        {
            Image img = null;

            txt = DoBarCode(txt, scale);
            img = Image.FromHbitmap(newBitmap.GetHbitmap());
            return img;
        }

        public System.Windows.Media.Imaging.BitmapSource CreateBarCodeBitmapSource(string txt, int scale)
        {
          
            string result = DoBarCode(txt, scale);
           
                IntPtr hBitmap = newBitmap.GetHbitmap();

                System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                return bitmapSource;
           
        }

        private string DoBarCode(string txt, int scale)
        {
            imageWidth = 120;
            imageScale = scale;
            imageWidth = System.Convert.ToInt32(imageWidth * imageScale);
            int imageHeightHolder = System.Convert.ToInt32(barCodeHeight * imageScale);
            string incomingString = txt.ToUpper();
            //if (incomingString.Replace("0","") == "")
            //{
            //    return null;
            //}
            int numberOfChars = incomingString.Length;
            newBitmap = new Bitmap((imageWidth), imageHeightHolder, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(newBitmap);
            g.ScaleTransform(imageScale, imageScale);
            Rectangle newRec = new Rectangle(0, 0, (imageWidth), imageHeightHolder);
            g.FillRectangle(new SolidBrush(Color.White), newRec);
            placeMarker = 0;
            txt = txt.Substring(0, 11) + GetCheckSum(txt).ToString();
            int wholeSet = 0;
            for (wholeSet = 1; wholeSet <= System.Convert.ToInt32(incomingString.Length); wholeSet++)
            {
                int currentASCII = (int)Convert.ToChar((incomingString.Substring(wholeSet - 1, 1))) - 48;
                string currentLetter = "";
                if (wholeSet == 1)
                {
                    DrawSet(UPCABegin, placeMarker, 0);
                    DrawSet(UPCALeft[currentASCII], placeMarker, 0);
                }
                else if (wholeSet <= 5)
                {
                    DrawSet(UPCALeft[currentASCII], placeMarker, 6);
                }
                else if (wholeSet == 6)
                {
                    DrawSet(UPCALeft[currentASCII], placeMarker, 6);
                    DrawSet(UPCAMiddle, placeMarker, 0);
                }
                else if (wholeSet <= 11)
                {
                    DrawSet(UPCARight[currentASCII], placeMarker, 6);
                }
                else if (wholeSet == 12)
                {
                    DrawSet(UPCARight[currentASCII], placeMarker, 0);
                    DrawSet(UPCAEnd, placeMarker, 0);
                }
            }

            System.Drawing.Font font = new System.Drawing.Font("Courier New, Bold", 8);
            try
            {
                g.DrawString(txt.Substring(0, 1), font, Brushes.Black, new System.Drawing.PointF(0, 67));
                g.DrawString(txt.Substring(1, 5), font, Brushes.Black, new System.Drawing.PointF(22, 67));
                g.DrawString(txt.Substring(6, 5), font, Brushes.Black, new System.Drawing.PointF(60, 67));
                g.DrawString(txt.Substring(11, 1), font, Brushes.Black, new System.Drawing.PointF(108, 67));
            }
            finally
            {
                font.Dispose();
            }
            return txt;
        }

        public int GetCheckSum(string barCode)
        {
            if (barCode.Length < 11)
            {
                barCode = barCode.PadLeft(11, '0');
            }
            string leftSideOfBarCode = barCode.Substring(0, 11);
            int total = 0;
            int currentDigit = 0;
            int i = 0;
            for (i = 0; i <= leftSideOfBarCode.Length - 1; i++)
            {
                currentDigit = Convert.ToInt32(leftSideOfBarCode.Substring(i, 1));
                if (((i - 1) % 2 == 0))
                {
                    total += currentDigit * 1;
                }
                else
                {
                    total += currentDigit * 3;
                }
            }
            int iCheckSum = (10 - (total % 10)) % 10;
            return iCheckSum;
        }

        private void DrawSet(string upcCode, int drawLocation, int barHeight)
        {
            int[] currentLetterArray = new int[upcCode.Length];
            placeMarker += upcCode.Length;
            barHeight = barCodeHeight - barHeight;
            int i = 0;
            for (i = 0; i <= upcCode.Length - 1; i++)
            {
                currentLetterArray[i] = Convert.ToInt16(upcCode.Substring(i, 1));
            }
            for (i = 0; i <= upcCode.Length - 1; i++)
            {
                if (currentLetterArray[i] == 0)
                {
                    g.DrawLine(Pens.White, i + (drawLocation), 0, i + (drawLocation), barHeight - 6);
                }
                else if (currentLetterArray[i] == 1)
                {
                    g.DrawLine(Pens.Black, i + (drawLocation), 0, i + (drawLocation), barHeight - 6);
                }
            }
        }

    }
}

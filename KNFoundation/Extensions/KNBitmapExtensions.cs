using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;

namespace KNFoundation {
    public static class KNBitmapExtensions {

        public static BitmapSource ToBitmapSource(this Bitmap b) {

            IntPtr hBitmap = b.GetHbitmap();

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }

    }
}

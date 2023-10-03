using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace PhoneBook
{
    internal static class Utils
    {
        public static void LoadSafely<T>(object obj, out T? result) 
        {
            if (DBNull.Value.Equals(obj)) result = default;
            else result = (T)obj;
        }
        public static ImageBrush ConvertImage(System.Drawing.Image img)
        {
            var bitmap = new System.Drawing.Bitmap(img);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return new ImageBrush(bitmapSource);
        }
        public static ImageBrush ConvertImage(string path)
        {
            var img = Image.FromFile(path);
            return ConvertImage(img);
        }
    }
}

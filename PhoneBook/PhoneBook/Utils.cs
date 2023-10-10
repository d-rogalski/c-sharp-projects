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
using System.Collections.Immutable;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Media.Media3D;
using System.IO;

namespace PhoneBook
{
    internal static class Utils
    {
        public static void LoadSafely<T>(object obj, out T? result) 
        {
            if (DBNull.Value.Equals(obj)) result = default;
            else result = (T)obj;
        }
        public static ImageSource ImageToImageSource(byte[] img)
        {
            var bitmap = new Bitmap(Image.FromStream(new MemoryStream(img)));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return bitmapSource;
        }
        public static ImageSource ImageToImageSource(string filePath)
        {
            var bitmap = new Bitmap(Image.FromFile(filePath));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return bitmapSource;
        }
        public static ImageBrush ImageToImageBrush(Image img)
        {
            var bitmap = new System.Drawing.Bitmap(img);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return new ImageBrush(bitmapSource);
        }
        public static ImageBrush ImageToImageBrush(byte[] img)
        {
            var bitmap = new Bitmap(Image.FromStream(new MemoryStream(img)));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return new ImageBrush(bitmapSource);
        }
        public static ImageBrush ImageToImageBrush(string path)
        {
            var img = Image.FromFile(path);
            return ImageToImageBrush(img);
        }
        public static byte[] ImageToBytes(Image img)
        {
            ImageConverter imgConv = new ImageConverter();
            byte[] bytes = (byte[])imgConv.ConvertTo(img, typeof(byte[]));
            return bytes;
        }
    }
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            T[] res = new T[length];
            Array.Copy(array, index, res, 0, length);
            return res;
        }
    }
}

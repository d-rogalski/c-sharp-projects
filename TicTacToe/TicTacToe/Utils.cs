using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace TicTacToe
{
    public static class Utils
    {
        /// <summary>
        /// Converts an image loaded from the given path to an ImageBrush object
        /// </summary>
        /// <param name="filePath">Path to the image.</param>
        /// <returns>An ImageBrush object corresponding to the given image.</returns>
        public static ImageBrush ImageToImageBrush(string path)
        {
            var img = Image.FromFile(path);
            var bitmap = new Bitmap(img);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            img.Dispose();
            return new ImageBrush(bitmapSource);
        }
    }
    public static class Extensions
    {
        public static T GetValue<T>(this T[,] array, (int, int) indices) => array[indices.Item1, indices.Item2];
        public static void SetValue<T>(this T[,] array, T value, (int, int) indices) => array[indices.Item1, indices.Item2] = value;
    }
}

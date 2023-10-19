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
    /// <summary>
    /// Static class of utility methods using throughout the project.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Parses an object imported from the database in a safe way - in case of NULL value in the table replaces it with a default value of the given type.
        /// </summary>
        /// <typeparam name="T">Type of the loaded object.</typeparam>
        /// <param name="obj">The loaded object.</param>
        /// <param name="result">Variable for the object after parsing.</param>
        public static void LoadSafely<T>(object obj, out T? result) 
        {
            if (DBNull.Value.Equals(obj)) result = default;
            else result = (T)obj;
        }

        /// <summary>
        /// Converts an image stored as an array of bytes to an ImageSource object.
        /// </summary>
        /// <param name="img">Bytes of the image.</param>
        /// <returns>An ImageSource object corresponding to the given image.</returns>
        public static ImageSource ImageToImageSource(byte[] img)
        {
            var bitmap = new Bitmap(Image.FromStream(new MemoryStream(img)));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return bitmapSource;
        }

        /// <summary>
        /// Converts an image loaded from the given path to an ImageSource object
        /// </summary>
        /// <param name="filePath">Path to the image.</param>
        /// <returns>An ImageSource object corresponding to the given image.</returns>
        public static ImageSource ImageToImageSource(string filePath)
        {
            var bitmap = new Bitmap(Image.FromFile(filePath));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return bitmapSource;
        }

        /// <summary>
        /// Converts an image stored as an array of bytes to an ImageBrush object.
        /// </summary>
        /// <param name="img">Bytes of the image.</param>
        /// <returns>An ImageBrush object corresponding to the given image.</returns>
        public static ImageBrush ImageToImageBrush(byte[] img)
        {
            var bitmap = new Bitmap(Image.FromStream(new MemoryStream(img)));
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return new ImageBrush(bitmapSource);
        }

        /// <summary>
        /// Converts an image loaded from the given path to an ImageBrush object
        /// </summary>
        /// <param name="filePath">Path to the image.</param>
        /// <returns>An ImageBrush object corresponding to the given image.</returns>
        public static ImageBrush ImageToImageBrush(string path)
        {
            var img = Image.FromFile(path);
            var bitmap = new System.Drawing.Bitmap(img);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            img.Dispose();
            return new ImageBrush(bitmapSource);
        }
    }


    /// <summary>
    /// Class of extensions used in the project
    /// </summary>
    internal static class PhoneBookExtensions
    {
        /// <summary>
        /// Creates a sub array from the given array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="startIndex">Index to start array from.</param>
        /// <param name="count">Number of consecutive elements to take.</param>
        /// <returns>A new array with the elements from the part of the original one.</returns>
        public static T[] SubArray<T>(this T[] values, int startIndex, int count)
        {
            T[] res = new T[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = values[startIndex + i];
            }
            return res;
        }
    }


    /// <summary>
    /// A custom exception indicating the necessity to close the app.
    /// </summary>
    public class ClosingException : Exception
    {
        public ClosingException() { }
        public ClosingException(string message) : base(message) { }
        public ClosingException(string message, Exception inner) : base(message, inner) { }
    }
}

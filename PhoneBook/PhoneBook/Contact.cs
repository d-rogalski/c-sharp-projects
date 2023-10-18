using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using Microsoft.Identity.Client;
using System.Windows.Media;

namespace PhoneBook
{
    public class Contact
    {
        public int Id { get; set; }
        public byte[]? Icon { get; set; }
        public ImageBrush IconBrush
        { 
            get
            {
                if (Icon == null) return Utils.ImageToImageBrush("images/default_icon.png");
                else return Utils.ImageToImageBrush(Icon);
            } 
        }
        public ImageSource IconSource
        {
            get
            {
                if (Icon == null) return Utils.ImageToImageSource("images/default_icon.png");
                else return Utils.ImageToImageSource(Icon);
            }
        }
        public string DisplayName { get; }
        public bool Favourite { get; }
        public Contact() 
        {
            Id = -1;
            Icon = null;
            DisplayName = "";
            Favourite = false;
        }
        public Contact(object[] objects)
        {
            Id = (int)objects[0];
            DisplayName = (string)objects[1];

            Utils.LoadSafely(objects[2], out byte[]? bytes);
            Icon = bytes;

            Utils.LoadSafely(objects[3], out bool tmp);
            Favourite = tmp;
        }
        public override string ToString()
        {
            return $"{DisplayName} (Id: {Id})";
        }
        public void SetIcon(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                Icon = new byte[fs.Length];
                fs.Read(Icon);
            }        
        }
    }
}

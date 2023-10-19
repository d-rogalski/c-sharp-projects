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
    /// <summary>
    /// A class partially corresponding to the Contact entity in the PhoneBook database. Used to display and operate on the contacts in the app.
    /// </summary>
    public class Contact
    {
        // Fields and properties
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Id of the contact.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Icon of the contact as raw bytes.
        /// </summary>
        public byte[]? Icon { get => _icon; }

        /// <summary>
        /// Icon of the contact as an ImageBrush object.
        /// </summary>
        public ImageBrush IconBrush
        { 
            get
            {
                if (_icon == null) return Utils.ImageToImageBrush("images/default_icon.png");
                else return Utils.ImageToImageBrush(_icon);
            } 
        }

        /// <summary>
        /// Icon of the contact as an ImageSource object.
        /// </summary>
        public ImageSource IconSource
        {
            get
            {
                if (_icon == null) return Utils.ImageToImageSource("images/default_icon.png");
                else return Utils.ImageToImageSource(_icon);
            }
        }

        /// <summary>
        /// Display name of the contact - first name and last name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Information whether the contact is favourite or not.
        /// </summary>
        public bool Favourite { get; }

        private byte[]? _icon;

        // Methods
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Main constructor, creates an empty object.
        /// </summary>
        public Contact() 
        {
            Id = -1;
            _icon = null;
            DisplayName = "";
            Favourite = false;
        }

        /// <summary>
        /// Auxiliary connstructor, create an object out of the data improted from the database.
        /// </summary>
        /// <param name="objects"></param>
        public Contact(object[] objects)
        {
            Id = (int)objects[0];
            DisplayName = (string)objects[1];

            Utils.LoadSafely(objects[2], out byte[]? bytes);
            _icon = bytes;

            Utils.LoadSafely(objects[3], out bool tmp);
            Favourite = tmp;
        }

        /// <summary>
        /// Implicit conversion of the object to a string.
        /// </summary>
        /// <returns>Display name of the Contact and its id.</returns>
        public override string ToString()
        {
            return $"{DisplayName} (Id: {Id})";
        }

        /// <summary>
        /// Sets the icon of the Contact from the file.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public void SetIcon(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open);
            _icon = new byte[fs.Length];
            fs.Read(_icon);
        }

        /// <summary>
        /// Removes the icon of the Contact
        /// </summary>
        public void RemoveIcon()
        {
            _icon = null;
        }
    }
}

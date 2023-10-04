using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PhoneBook
{
    public class Contact
    {
        public int Id { get; }
        public Image? Icon { get; set; }
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
            Icon = bytes != null ? Image.FromStream(new MemoryStream(bytes)) : null;

            Utils.LoadSafely(objects[3], out bool tmp);
            Favourite = tmp;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace NoteApp
{
    public class Note
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public string DateCreated { get => dateCreated.ToString("g"); set { dateCreated = DateTime.Parse(value); } }

        private DateTime dateCreated;

        public Note(string text="", string? title=null)
        {
            Text = text;
            dateCreated = DateTime.Now;
            if (title != null) Title = title;
            else Title = "Untitled";
        }

        public static Note ReadJson(string filePath)
        {
            return new Note("");
        }

        public override string ToString()
        {
            return $"{Title}\n{DateCreated}\n{Text}";
        }
    }
}

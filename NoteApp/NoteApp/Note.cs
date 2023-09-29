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
    /// <summary>
    /// Class representing a note in the app.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Content of the note.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Title of the note.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Date of the note's creation.
        /// </summary>
        public string DateCreated { get => dateCreated.ToString("g"); set { dateCreated = DateTime.Parse(value); } }


        private DateTime dateCreated;


        /// <summary>
        /// Create a new note with the given text and title.
        /// </summary>
        /// <param name="text">Note's content</param>
        /// <param name="title">Note's title. "Untitled" if not specified.</param>
        public Note(string text="", string? title=null)
        {
            Text = text;
            dateCreated = DateTime.Now;
            if (title != null) Title = title;
            else Title = "Untitled";
        }
    }
}

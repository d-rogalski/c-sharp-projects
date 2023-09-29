using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace NoteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Note> notes; // List of notes
        private bool closeWithoutSaving = false; // Prevents losing the data in case of an error while loading
        public MainWindow()
        {
            InitializeComponent();           
        }

        // Window's events

        /// <summary>
        /// Saving the notes to file when closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!closeWithoutSaving)
            {
                using var writer = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, "notes.data"));
                var jsonString = JsonSerializer.Serialize(notes);
                writer.Write(jsonString);
            }
        }
        /// <summary>
        /// Loading the notes to file when loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var reader = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "notes.data"));
                notes = JsonSerializer.Deserialize<List<Note>>(reader.ReadToEnd());
                RefreshList();
            }
            catch (Exception)
            {
                var dialogResult = MessageBox.Show("Could not load the notes from the file. Click 'Ok' if you want to start a new notebook or 'Cancel' to close the app.", "Loading error", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK) notes = new List<Note>();
                else
                {
                    closeWithoutSaving = true;
                    this.Close();
                }
            }
        }

        // Controls' events

        /// <summary>
        /// Add a note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNoteButton_Click(object sender, RoutedEventArgs e)
        {
            notes.Insert(0, new Note());
            RefreshList();
        }
        /// <summary>
        /// Pick a note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (noteList.SelectedIndex != -1)
            {
                var i = noteList.SelectedIndex;
                noteTitleTextBox.Text = notes[i].Title;
                noteTextBox.Text = notes[i].Text;
                noteDateTextBlock.Text = notes[i].DateCreated;
            }
        }
        /// <summary>
        /// Update the selected note's title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noteList.SelectedIndex < 0)
            {
                addNoteButton_Click(sender, e);
            }
            notes[noteList.SelectedIndex].Title = noteTitleTextBox.Text;
        }
        /// <summary>
        /// Update the selected note's content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noteList.SelectedIndex < 0)
            {
                addNoteButton_Click(sender, e);
            }
            notes[noteList.SelectedIndex].Text = noteTextBox.Text;
        }
        /// <summary>
        /// Delete a note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteList_MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            notes.RemoveAt(noteList.SelectedIndex);
            RefreshList();
            noteList.SelectedIndex = 0;
        }

        // Utility functions

        /// <summary>
        /// Refreshes the list of notes.
        /// </summary>
        private void RefreshList()
        {
            noteList.Items.Clear();
            foreach (var note in notes)
            {
                noteList.Items.Add(note.Title);
                Console.WriteLine(note.Title);
            }
            noteList.SelectedIndex = 0;
        }
    }
}

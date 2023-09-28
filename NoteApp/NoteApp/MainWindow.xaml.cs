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
        public List<Note> notes;
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void RefreshList()
        {
            noteList.Items.Clear();
            foreach (var note in notes)
            {
                noteList.Items.Add(note.Title);
                Console.WriteLine(note.Title);
            }
            noteList.SelectedIndex = noteList.Items.Count - 1;
        }

        private void addNoteButton_Click(object sender, RoutedEventArgs e)
        {
            notes.Add(new Note());
            RefreshList();
        }

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

        private void noteTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noteList.SelectedIndex < 0)
            {
                addNoteButton_Click(sender, e);
            }
            notes[noteList.SelectedIndex].Title = noteTitleTextBox.Text;
        }

        private void noteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (noteList.SelectedIndex < 0)
            {
                addNoteButton_Click(sender, e);
            }
            notes[noteList.SelectedIndex].Text = noteTextBox.Text;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (var writer = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, "notes.data")))
            {
                var jsonString = JsonSerializer.Serialize(notes);
                writer.Write(jsonString);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var reader = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "notes.data")))
                {
                    notes = JsonSerializer.Deserialize<List<Note>>(reader.ReadToEnd());
                }
            }
            catch (Exception)
            {
                notes = new List<Note>();
            }
            RefreshList();
        }
        private void addNoteButton_MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            notes.RemoveAt(noteList.SelectedIndex);
            RefreshList();
            noteList.SelectedIndex = 0;
        }
    }
}

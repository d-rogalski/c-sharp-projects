using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Identity.Client;
using System.Configuration;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PhoneBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Fields and properties
        // ---------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Connection string for connecting with the SQL Server.
        /// </summary>
        private string sqlConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;
            Initial Catalog=master;
            Integrated Security=True;
            Connect Timeout=30;
            Encrypt=False;
            Trust Server Certificate=False;
            Application Intent=ReadWrite;
            Multi Subnet Failover=False;
            Initial Catalog=PhoneBook";

        /// <summary>
        /// <c>DataBaseHandler</c> object for easier operations with the data base.
        /// </summary>
        private PhoneBookDatabase db;

        /// <summary>
        /// List of contacts stored as the <c>Contact</c> objects. It is a Items source of the ListView control.
        /// </summary>
        public List<Contact> contacts = new();

        /// <summary>
        /// Current contact property. Returns a currently selected contact if there is one or a <c>_newContact</c> variable otherwise.
        /// </summary>
        public Contact CurrentContact 
        { 
            get
            {
                if (contactList.SelectedIndex != -1) return contacts[contactList.SelectedIndex];
                else return _newContact; 
            } 
        }

        /// <summary>
        /// Logger of the application.
        /// </summary>
        internal Logger logger;

        /// <summary>
        /// New Contact object used in the process of adding a new contact to the PhoneBook. Returned by <c>CurrentContact</c> property if the ListView has no selection.
        /// </summary>
        private Contact _newContact;


        // ModelView operations - controls' events
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialization of the window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            int res;
            string msg;

            // Initiating logger
            (res, msg) = TryWithMessageBox(
                () => logger = new Logger(), 
                "Failed to initate log. The app will be closed.");
            if (res < 0) Environment.Exit(-1);

            // Connecting to the database
            (res, msg) = TryWithMessageBox(
                () => db = new PhoneBookDatabase(sqlConnectionString), 
                "Failed to connect with the data base. The app will be closed.");
            if (res < 0)
            {
                logger.Log($"Connection to the database using the connection string '{sqlConnectionString}' failed due to the following exception: {msg}", "Database");
                Environment.Exit(-1);
            }
            else logger.Log($"Connection to the database using the connection string '{sqlConnectionString}' established successfully.", "Database");
            
            // Preparing controls
            RefreshContacts();
            ChangeToEdit(false);
        }

        /// <summary>
        /// Invoked on closing the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (logger != null)
            {
                logger.Log("Closing the application", "Main window");
                logger.Dispose();
            }
            if (db != null) db.Dispose();
        }
        
        /// <summary>
        /// Called on clicking <b>Add</b> option in the ListView's contex menu. Opens the editable mode of the form for adding a new contact.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contactList_MenuItemAdd_Click(object sender, EventArgs e)
        {
            // Deselect any selected contact - required for the proper work of CurrentContact property.
            contactList.SelectedIndex = -1;
            // Create an object for a new contact
            _newContact = new Contact();
            // Enter editable mode of the contact form
            ChangeToEdit(true);
        }

        /// <summary>
        /// Called on clicking <b>Edit</b> option in the ListView's contex menu. Opens the editable mode of the form for editing the selected contact.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contactList_MenuItemEdit_Click(object sender, EventArgs e)
        {
            // Goes into editable mode only if a contact to edit was selected
            if (contactList.SelectedIndex != -1) ChangeToEdit(true);
        }

        /// <summary>
        /// Called on clicking <b>Delete</b> option in the ListView's contex menu. Deletes a contact from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contactList_MenuItemDelete_Click(object sender, EventArgs e)
        {
            // Delete the contact from database and refresh the ListView only if the contact was selected.
            if (contactList.SelectedIndex != -1)
            {
                MessageBoxResult msgBoxResult = ConfirmMessageBox(
                    $"Are you sure you want to delete {CurrentContact.DisplayName}?",
                    "Delete the contact");
                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    (int res, string msg) = TryWithMessageBox(
                        () => db.DeleteContact(CurrentContact.Id),
                        "Program failed to delete the contact. Check log for the more details.",
                        "Contact has been deleted successfully!");

                    if (res == -2) logger.Log($"Deleting the contact failed due to the following exception {msg}", "Database");
                    else if (res == -1)
                    {
                        logger.Log($"Fatal error while deleting the contact: {msg}", "Database");
                        Environment.Exit(-1);
                    }
                    else logger.Log($"The contact {CurrentContact} has been deleted", "Database");
                    RefreshContacts();
                }
            }
        }

        /// <summary>
        /// Called on every change in the search TextBox. Allows a dynamic filtering the ListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshContacts();
        }

        /// <summary>
        /// Calledn on every change of selection in the ListView. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If there is no selection, import the selected contact and display it
            if (contactList.SelectedIndex != -1)
            {           
                try
                {
                    var contactInfo = db.GetContactInfo(CurrentContact.Id);
                    firsNameTextBox.Text = contactInfo["FirstName"];
                    lastNameTextBox.Text = contactInfo["LastName"];
                    phoneNumberTextBox.Text = contactInfo["PhoneNumber"];
                    emailAddressTextBox.Text = contactInfo["EmailAddress"];
                    companyTextBox.Text = contactInfo["Company"];
                    favouriteCheckBox.IsChecked = CurrentContact.Favourite;
                    displayIcon.Fill = CurrentContact.IconBrush;
                }
                catch (Exception ex)
                {
                    string log = $"Refreshing the contacts failed due to the following exception {ex}. ";
                    if (ex.InnerException == null)
                    {
                        log += $"Inner exception: {ex.InnerException}";
                    }
                    logger.Log(log, "Database");
                }
            }
            // Otherwise clean the contact form
            else
            {
                firsNameTextBox.Text = "";
                lastNameTextBox.Text = "";
                phoneNumberTextBox.Text = "";
                emailAddressTextBox.Text = "";
                companyTextBox.Text = "";
                favouriteCheckBox.IsChecked = false;
                displayIcon.Fill = Utils.ImageToImageBrush("images/default_icon.png");
            }
        }

        /// <summary>
        /// Called on clicking the <b>Accept</b> Button. Accepts editing the selected contact or adding a new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            // PhoneNumber field in DataBase can't be empyty
            if (phoneNumberTextBox.Text == "")
            {
                MessageBox.Show("The Phone Number field cannot be empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Selection -> updating the contact with new details
            if (contactList.SelectedIndex != -1)
            {
                
                MessageBoxResult msgBoxResult = ConfirmMessageBox(
                    $"Are you sure you want to edit {CurrentContact.DisplayName}?",
                    "Edit the contact");
                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    (int res, string msg) = TryWithMessageBox(
                    () => db.UpdateContact(
                        CurrentContact.Id,
                        firsNameTextBox.Text,
                        lastNameTextBox.Text,
                        phoneNumberTextBox.Text,
                        emailAddressTextBox.Text,
                        companyTextBox.Text,
                        CurrentContact.Icon,
                        CurrentContact.Favourite),
                    "Failed to update the contact. Check log for more details.",
                    "Contact has been updated successfully!");

                    if (res == -2) logger.Log($"Updating the contact failed due to the following exception {msg}", "Database");
                    else if (res == -1)
                    {
                        logger.Log($"Fatal error while udpating the contact: {msg}", "Database");
                        Environment.Exit(-1);
                    }
                    else logger.Log($"Contact {CurrentContact} has been updated", "Database");
                }
            }
            // No selection -> Adding a new contact
            else
            {
                int id = -1;
                (int res, string msg) = TryWithMessageBox(
                () => id = db.AddContact(
                        firsNameTextBox.Text,
                        lastNameTextBox.Text,
                        phoneNumberTextBox.Text,
                        emailAddressTextBox.Text,
                        companyTextBox.Text,
                        CurrentContact.Icon,
                        favouriteCheckBox.IsChecked ?? false),
                "Failed to add a new contact. Check Log for more details.",
                "Contact has been added successfully!");
                CurrentContact.Id = id;

                if (res == -2) logger.Log($"Adding a new contact failed due to the following exception {msg}", "Database");
                else if (res == -1)
                {
                    logger.Log($"Fatal error while adding a contact: {msg}", "Database");
                    Environment.Exit(-1);
                }
                else logger.Log($"A new contact {CurrentContact} has been added", "Database");

            }
            // Exit the editable mode and refresh ListView
            ChangeToEdit(false);
            RefreshContacts(contactList.SelectedIndex);
        }

        /// <summary>
        /// Called on clicking the <b>Cancel</b> Button. Discards editing or adding new contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgBoxResult = ConfirmMessageBox(
                "Are you sure you want to discard the changes?", 
                "Cancel the changes");
            if (msgBoxResult == MessageBoxResult.Yes)
            {
                ChangeToEdit(false);
                RefreshContacts(contactList.SelectedIndex);
            }
        }

        /// <summary>
        /// Called on clicking the edit icon button. Opens the FileDialog and allows selecting an image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Select an image
            OpenFileDialog fileDialog = new()
            {
                Multiselect = false,
                AddExtension = true,
                InitialDirectory = Environment.SpecialFolder.MyPictures.ToString(),
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG",
                FilterIndex = 0
            };
            if (fileDialog.ShowDialog() ?? false)
            {
                (int res, string msg) = TryWithMessageBox(
                    () =>
                    {
                        CurrentContact.SetIcon(fileDialog.FileName); // Add the icon to Contact info
                        displayIcon.Fill = CurrentContact.IconBrush; // Display the icon
                    },
                    "Failed to load the icon.");
                if (res < 0) logger.Log($"Failed to load the icon from {fileDialog.FileName} due to the following exception: {msg}", "FileDialog");
            }
        }

        /// <summary>
        /// Called on clicking the remove icon button. Displays default icon and removes it from the Contact info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentContact.Icon != null)
            {
                CurrentContact.RemoveIcon();
                displayIcon.Fill = Utils.ImageToImageBrush("images/default_icon.png");
            }
        }


        // Additional methods used in the interactive part
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Queries the database for current list of contacts and displays them in ListView.
        /// </summary>
        /// <param name="index">Index of the contact to be selected after refrreshing. Default - no selection.</param>
        private void RefreshContacts(int index = -1)
        {
            try
            {
                contacts = db.ExtractContacts(searchTextBox.Text);
                contactList.ItemsSource = contacts;
                contactList.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                string log = $"Refreshing the contacts failed due to the following exception {ex}. ";
                if (ex.InnerException == null) {
                    log += $"Inner exception: {ex.InnerException}";
                }
                logger.Log(log, "Database");
            }          
        }

        /// <summary>
        /// Changes the mode of contact form between editing and displaying.
        /// </summary>
        /// <param name="value">The form enters editing mode if true and displaying mode if false.</param>
        private void ChangeToEdit(bool value)
        {
            Visibility visibility = value ? Visibility.Visible : Visibility.Hidden;
            firsNameTextBox.IsReadOnly = !value;
            lastNameTextBox.IsReadOnly = !value;
            phoneNumberTextBox.IsReadOnly = !value;
            emailAddressTextBox.IsReadOnly = !value;
            companyTextBox.IsReadOnly = !value;
            favouriteCheckBox.IsEnabled = value;
            contactList.IsEnabled = !value;
            acceptButton.Visibility = visibility;
            cancelButton.Visibility = visibility;
            editIcon.Visibility = visibility;
            removeIcon.Visibility = visibility;
        }

        /// <summary>
        /// Tries to invoke the <c>action</c> and informs about the result with MessageBox.
        /// </summary>
        /// <param name="action">Action to be tried</param>
        /// <param name="messageIfFailed">Message in the MessageBox if any exception will be caught.</param>
        /// <param name="messageIfSuccess">Message in the MessageBox if no exception will be caught. If <c>null</c>, no MessageBox is displayed.</param>
        /// <returns>Returns a tuple of integer and string. The integer corresponds to the result of the trial:
        /// 0 - no exception
        /// -1 - fatal exception, the app should be closed
        /// -2 - regular exception 
        /// The string will be the exception's text</returns>
        public (int, string) TryWithMessageBox(Action action, string messageIfFailed, string messageIfSuccess = null)
        {
            try
            {
                action.Invoke();
                if (messageIfSuccess != null) MessageBox.Show(messageIfSuccess, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return (0, "");
            }
            catch (ClosingException ex)
            {
                string exceptionMessage = (ex.InnerException ?? ex).ToString();
                MessageBox.Show(messageIfFailed + "\n" + exceptionMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return (-1, exceptionMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(messageIfFailed, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return (-2, ex.ToString());
            }
        }

        /// <summary>
        /// Displays a confirmation MessageBox with YesNo buttons..
        /// </summary>
        /// <param name="question">Question in the MessageBox</param>
        /// <param name="caption">Caption of the MessageBox</param>
        /// <returns></returns>
        public MessageBoxResult ConfirmMessageBox(string question, string caption)
        {
            return MessageBox.Show(question, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}

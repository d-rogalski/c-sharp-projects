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

namespace PhoneBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBaseHandler db = new DataBaseHandler(@"Data Source=(localdb)\MSSQLLocalDB;
                                        Initial Catalog=master;
                                        Integrated Security=True;
                                        Connect Timeout=30;
                                        Encrypt=False;
                                        Trust Server Certificate=False;
                                        Application Intent=ReadWrite;
                                        Multi Subnet Failover=False;
                                        Initial Catalog=PhoneBook");
        public List<Contact> contacts = new();
        private Contact CurrentContact { 
            get
            {
                if (contactList.SelectedIndex != -1) return contacts[contactList.SelectedIndex];
                else return _newContact; 
            } }
        private Contact _newContact;
        private void ChangeToEdit(bool value)
        {
            firsNameTextBox.IsReadOnly = !value;
            lastNameTextBox.IsReadOnly = !value;
            phoneNumberTextBox.IsReadOnly = !value;
            emailAddressTextBox.IsReadOnly = !value;
            companyTextBox.IsReadOnly = !value;
            favouriteCheckBox.IsEnabled = value;
            contactList.IsEnabled = !value;
            acceptButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            cancelButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            editIcon.Visibility = value ? Visibility.Visible : Visibility.Hidden;

            _editable = value;
        }
        private bool _editable;
        public MainWindow()
        {
            InitializeComponent();

            RefreshContacts();
            contactList.ItemsSource = contacts;
            ChangeToEdit(false);
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {      
        }

        private void contactList_MenuItemAdd_Click(object sender, EventArgs e)
        {
            contactList.SelectedIndex = -1;
            _newContact = new Contact();
            ChangeToEdit(true);
        }
        private void contactList_MenuItemEdit_Click(object sender, EventArgs e)
        {
            if (contactList.SelectedIndex != -1) ChangeToEdit(true);
        }
        private void contactList_MenuItemDelete_Click(object sender, EventArgs e)
        {
            if (contactList.SelectedIndex != -1)
            {
                db.DeleteContact(CurrentContact.Id);
                RefreshContacts();
            }
        }

        private void RefreshContacts(int index=-1)
        {
            contacts = db.ExtractContacts(searchTextBox.Text);
            contactList.ItemsSource = contacts;
            contactList.SelectedIndex = index;
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshContacts();
        }

        private void contactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contactList.SelectedIndex != -1)
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

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (contactList.SelectedIndex != -1)
            {
                db.UpdateContact(
                    CurrentContact.Id,
                    firsNameTextBox.Text,
                    lastNameTextBox.Text,
                    phoneNumberTextBox.Text,
                    emailAddressTextBox.Text,
                    companyTextBox.Text,
                    CurrentContact.Icon,
                    CurrentContact.Favourite
                    );
            }
            else
            {
                db.AddContact(
                    firsNameTextBox.Text,
                    lastNameTextBox.Text,
                    phoneNumberTextBox.Text,
                    emailAddressTextBox.Text,
                    companyTextBox.Text,
                    CurrentContact.Icon,
                    favouriteCheckBox.IsChecked ?? false
                    );
            }
            ChangeToEdit(false);
            RefreshContacts(contactList.SelectedIndex);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToEdit(false);
            RefreshContacts();
        }

        private void editIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                AddExtension = true,
                InitialDirectory = Environment.SpecialFolder.MyPictures.ToString(),
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG",
                FilterIndex = 0
            };
            if (fileDialog.ShowDialog() ?? false)
            {
                Debug.WriteLine(fileDialog.FileName);
                CurrentContact.SetIcon(fileDialog.FileName);
                displayIcon.Fill = CurrentContact.IconBrush;
            }
        }
    }
}

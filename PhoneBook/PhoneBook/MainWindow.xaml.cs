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
            firsNameTextBox.IsEnabled = value;
            lastNameTextBox.IsEnabled = value;
            phoneNumberTextBox.IsEnabled = value;
            emailAddressTextBox.IsEnabled = value;
            companyTextBox.IsEnabled = value;
            favouriteCheckBox.IsEnabled = value;
            displayIcon.IsEnabled = value;
            contactList.IsEnabled = !value;
            acceptButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;

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

        }

        private void RefreshContacts()
        {
            contacts = db.ExtractContacts(searchTextBox.Text);
            contactList.ItemsSource = contacts;
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
                if (CurrentContact.Icon != null) displayIcon.Fill = Utils.ImageToImageBrush(CurrentContact.Icon);
                else displayIcon.Fill = Utils.ImageToImageBrush("images/default_icon.png");
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
            if (contactList.SelectedIndex != -1) db.UpdateContact(
                CurrentContact.Id,
                firsNameTextBox.Text,
                lastNameTextBox.Text,
                phoneNumberTextBox.Text,
                emailAddressTextBox.Text,
                companyTextBox.Text,
                CurrentContact.Icon,
                CurrentContact.Favourite);
            else db.AddContact(
                firsNameTextBox.Text,
                lastNameTextBox.Text,
                phoneNumberTextBox.Text,
                emailAddressTextBox.Text,
                companyTextBox.Text,
                CurrentContact.Icon,
                favouriteCheckBox.IsChecked ?? false);

            ChangeToEdit(false);
        }
    }
}

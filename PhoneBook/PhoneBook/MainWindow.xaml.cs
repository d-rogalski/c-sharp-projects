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
        private void ChangeToEdit(bool value)
        {
            firsNameTextBox.IsEnabled = value;
            lastNameTextBox.IsEnabled = value;
            phoneNumberTextBox.IsEnabled = value;
            emailAddressTextBox.IsEnabled = value;
            companyTextBox.IsEnabled = value;
            favouriteCheckBox.IsEnabled = value;
            displayIcon.IsEnabled = value;
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
            ChangeToEdit(true);
        }
        private void contactList_MenuItemEdit_Click(object sender, EventArgs e)
        {
            ChangeToEdit(true);
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
            var currentContact = contacts[contactList.SelectedIndex];
            var contactInfo = db.GetContactInfo(currentContact.Id);
            firsNameTextBox.Text = contactInfo["FirstName"];
            lastNameTextBox.Text = contactInfo["LastName"];
            phoneNumberTextBox.Text = contactInfo["PhoneNumber"];
            emailAddressTextBox.Text = contactInfo["EmailAddress"];
            companyTextBox.Text = contactInfo["Company"];
            favouriteCheckBox.IsChecked = currentContact.Favourite;
            if (currentContact.Icon != null) displayIcon.Fill = Utils.ConvertImage(currentContact.Icon);
            else displayIcon.Fill = Utils.ConvertImage("images/default_icon.png");
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToEdit(false);

        }
    }
}

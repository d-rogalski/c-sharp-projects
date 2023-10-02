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
        public List<Contact> contacts;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshContacts();
            contactList.ItemsSource = contacts;
        }

        private void contactList_MenuItemAdd_Click(object sender, EventArgs e)
        {

        }
        private void contactList_MenuItemEdit_Click(object sender, EventArgs e)
        {

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
            if (currentContact.Icon != null) displayIcon.Fill = Utils.Convert(currentContact.Icon);          
        }
    }
}

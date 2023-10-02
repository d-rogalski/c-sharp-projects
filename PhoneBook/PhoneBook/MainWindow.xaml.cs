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
        public List<Contact> allContacts = new();
        public List<Contact> contacts;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            allContacts = ModelView.ExtractContacts(db.ExecuteQuery("SELECT * FROM Contact;"));
            ModelView.SearchUpdate("", allContacts, out contacts);
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
            contacts.Sort(Contact.SortDescending());
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ModelView.SearchUpdate(searchTextBox.Text, allContacts, out contacts);
            RefreshContacts();
        }
    }
}

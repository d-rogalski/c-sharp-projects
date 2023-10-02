using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace PhoneBook
{
    internal static class ModelView
    {
        public static void SearchUpdate(string searchTerm, List<Contact> inList, out List<Contact> outList)
        {
            outList = new();
            foreach (var item in inList)
            {
                if (item.DisplayName.Contains(searchTerm)) outList.Add(item);
            }
        }
        public static List<Contact> ExtractContacts(SqlDataReader reader)
        {
            List<Contact> list = new();
            while (reader.Read())
            {
                object[] record = new object[reader.FieldCount];
                reader.GetValues(record);
                list.Add(new Contact(record));
            }
            return list;
        }
    }
}

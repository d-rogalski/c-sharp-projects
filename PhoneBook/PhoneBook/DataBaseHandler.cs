using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace PhoneBook
{
    internal class DataBaseHandler
    {
        private SqlConnection cnn;
        public DataBaseHandler(string connectionString)
        {
            cnn = new SqlConnection(connectionString);
            cnn.Open();
        }
        ~DataBaseHandler()
        {
            cnn.Close();
        }
        public SqlDataReader ExecuteQuery(string query)
        {
            var cmd = new SqlCommand(query, cnn);
            return cmd.ExecuteReader();
        }
        public List<Contact> ExtractContacts(string? searchFor="")
        {
            string query = "SELECT * " +
                "FROM (SELECT Id, CONCAT(FirstName, ' ', LastName) AS DisplayName, Icon, Favourite FROM Contact) AS sub " +
                $"WHERE sub.DisplayName LIKE '%{searchFor}%';";
            using var reader = ExecuteQuery(query);
            
            List<Contact> list = new();
            while (reader.Read())
            {
                object[] record = new object[reader.FieldCount];
                reader.GetValues(record);
                list.Add(new Contact(record));
            }
            return list;
        }
        public Dictionary<string, string> GetContactInfo(int id)
        {
            var fieldNames = new string[] { "FirstName", "LastName", "PhoneNumber", "EmailAddress", "Company" };
            string query = $"SELECT {string.Join(", ", fieldNames)} FROM Contact WHERE Id = {id}";
            using var reader = ExecuteQuery(query);

            Dictionary<string, string> result = new();
            while (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);

                for (int i = 0; i < reader.FieldCount - 2; i++)
                {
                    Utils.LoadSafely(values[i], out string? tmp);
                    result[fieldNames[i]] = tmp ?? "";
                }
            }
            return result;
        }
    }
}
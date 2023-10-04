using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using System.Security.Cryptography;

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
        private SqlDataReader ExecuteQuery(string query)
        {
            Debug.Write(query);
            var cmd = new SqlCommand(query, cnn);
            return cmd.ExecuteReader();
        }
        public List<Contact> ExtractContacts(string? searchFor="")
        {
            string query = "SELECT * " +
                "FROM (SELECT Id, CONCAT(FirstName, ' ', LastName) AS DisplayName, Icon, Favourite FROM Contact) AS sub " +
                $"WHERE sub.DisplayName LIKE '%{searchFor}%' " +
                $"ORDER BY Favourite DESC, DisplayName;";
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
            if (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Utils.LoadSafely(values[i], out string? tmp);
                    result[fieldNames[i]] = tmp ?? "";
                }
            }
            return result;
        }

        public void UpdateContact(int id, string firstName, string lastName, string phoneNumber, string emailAddress, string company, Image icon, bool favourite)
        {
            string query = $"UPDATE Contact " +
                $"SET FirstName = '{firstName}', " +
                $"LastName = '{lastName}', " +
                $"PhoneNumber = '{phoneNumber}', " +
                $"EmailAddress = '{emailAddress}', " +
                $"Company = '{company}', " +
                $"Icon = {(icon == null ? "NULL" : Utils.ImageToBytes(icon))}, " +
                $"Favourite = {(favourite ? 1 : 0)} " +
                $"WHERE Id = {id};";
            using var reader = ExecuteQuery(query);
        }
        public void AddContact(string firstName, string lastName, string phoneNumber, string emailAddress, string company, Image? icon, bool favourite)
        {
            string query = $"INSERT INTO Contact (FirstName, LastName, PhoneNumber, EmailAddress, Company, Icon, Favourite) " +
                $"VALUES ('{firstName}', '{lastName}', '{phoneNumber}', '{emailAddress}', '{company}', {(icon == null ? "NULL" : Utils.ImageToBytes(icon))}, {(favourite ? 1 : 0)});";
            using var reader = ExecuteQuery(query);
        }
    }
}
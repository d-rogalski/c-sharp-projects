using System;
using System.Data;
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
        private string[] fieldNames = new string[] { "Id", "FirstName", "LastName", "PhoneNumber", "EmailAddress", "Company", "Icon", "Favourite" };
        //private SqlDbType[] fieldTypes = new SqlDbType[] { SqlDbType.Int, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Image, SqlDbType.Bit };
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
            using var cmd = new SqlCommand(query, cnn);
            return cmd.ExecuteReader();
        }
        private object ExecuteProcedure(string procedureName, SqlParameter[] parameters, SqlDbType? returnType = SqlDbType.Int)
        {
            using var cmd = new SqlCommand(procedureName, cnn) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.AddRange(parameters);

            cmd.Parameters.Add(new SqlParameter("@returnValue", returnType));
            cmd.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

            cmd.ExecuteNonQuery();
            return cmd.Parameters["@returnValue"].Value;
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
            var stringFields = fieldNames.SubArray(1, 5);
            string query = $"SELECT {string.Join(", ", stringFields)} FROM Contact WHERE Id = {id}";
            using var reader = ExecuteQuery(query);

            Dictionary<string, string> result = new();
            if (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Utils.LoadSafely(values[i], out string? tmp);
                    result[stringFields[i]] = tmp ?? "";
                }
            }
            return result;
        }

        public (int, int) UpdateContact(int id, string firstName, string lastName, string phoneNumber, string emailAddress, string company, Image icon, bool favourite)
        {
            SqlParameter[] parameters;
            
            parameters = new SqlParameter[fieldNames.Length - 1];
            parameters[0] = new SqlParameter("@id", SqlDbType.Int);
            parameters[0].Value = id;
            parameters[1] = new SqlParameter("@fn", SqlDbType.VarChar);
            parameters[1].Value = firstName;
            parameters[2] = new SqlParameter("@ln", SqlDbType.VarChar);
            parameters[2].Value = lastName;
            parameters[3] = new SqlParameter("@pn", SqlDbType.VarChar);
            parameters[3].Value = phoneNumber;
            parameters[4] = new SqlParameter("@ea", SqlDbType.VarChar);
            parameters[4].Value = emailAddress;
            parameters[5] = new SqlParameter("@c", SqlDbType.VarChar);
            parameters[5].Value = company;
            parameters[6] = new SqlParameter("@f", SqlDbType.Bit);
            parameters[6].Value = favourite ? 1 : 0;

            int res1 = (int)ExecuteProcedure("UpdateContact", parameters, SqlDbType.Int);

            parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@id", SqlDbType.Int);
            parameters[0].Value = id;
            parameters[1] = new SqlParameter("@i", SqlDbType.Image);
            parameters[1].Value = icon == null ? DBNull.Value : Utils.ImageToBytes(icon);

            int res2 = (int)ExecuteProcedure("UpdateContactIcon", parameters);

            return (res1, res2);
        }
        public int AddContact(string firstName, string lastName, string phoneNumber, string emailAddress, string company, Image? icon, bool favourite)
        {
            SqlParameter[] parameters = new SqlParameter[fieldNames.Length - 1];
            parameters[0] = new SqlParameter("@fn", SqlDbType.VarChar);
            parameters[0].Value = firstName;
            parameters[1] = new SqlParameter("@ln", SqlDbType.VarChar);
            parameters[1].Value = lastName;
            parameters[2] = new SqlParameter("@pn", SqlDbType.VarChar);
            parameters[2].Value = phoneNumber;
            parameters[3] = new SqlParameter("@ea", SqlDbType.VarChar);
            parameters[3].Value = emailAddress;
            parameters[4] = new SqlParameter("@c", SqlDbType.VarChar);
            parameters[4].Value = company;
            parameters[5] = new SqlParameter("@i", SqlDbType.Image);
            parameters[5].Value = icon == null ? DBNull.Value : Utils.ImageToBytes(icon);
            parameters[6] = new SqlParameter("@f", SqlDbType.Bit);
            parameters[6].Value = favourite ? 1 : 0;

            return (int)ExecuteProcedure("AddContact", parameters, SqlDbType.Int);
        }
        public void DeleteContact(int id)
        {
            string query = $"DELETE FROM Contact WHERE Id = {id};";
            ExecuteQuery(query);
        }
    }
}
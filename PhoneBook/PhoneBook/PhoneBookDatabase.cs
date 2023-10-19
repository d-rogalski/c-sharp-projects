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
using System.Collections;

namespace PhoneBook
{
    /// <summary>
    /// A class used to connect with the PhoneBook database, use simple queries and procedures needed in the application.
    /// </summary>
    internal class PhoneBookDatabase : IDisposable
    {
        // Fields
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Connection to the database
        /// </summary>
        private SqlConnection _cnn;

        /// <summary>
        /// Names of the fields in the Contact table from the PhoneBook database
        /// </summary>
        private string[] _fieldNames = new string[] { "Id", "FirstName", "LastName", "PhoneNumber", "EmailAddress", "Company", "Icon", "Favourite" };


        // Methods
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor of the class. Opens the connection using the given connection string.
        /// </summary>
        /// <param name="connectionString">Connections string used to connect to the database.</param>
        public PhoneBookDatabase(string connectionString)
        {
            _cnn = new SqlConnection(connectionString);
            _cnn.Open();
        }

        /// <summary>
        /// Method disposing of the object. Closes the connection.
        /// </summary>
        public void Dispose()
        {
            _cnn.Dispose();
            _cnn.Close();
        }

        /// <summary>
        /// Executes a given SQL query in the connected database.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <returns><c>SQLDataReader</c> storing the results of the query.</returns>
        private SqlDataReader ExecuteQuery(string query)
        {
            try
            {
                using var cmd = new SqlCommand(query, _cnn);
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception("An exception caught during executing a query.", ex);
            }
        }
        
        /// <summary>
        /// Executes a SQL stored procedure with the given name and the list of parameters.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">List of parameters for the procedure.</param>
        /// <param name="returnType">Type of the procedure's return value. Default: int.</param>
        /// <returns>Return value of the stored procedure with the specified data type.</returns>
        private object ExecuteProcedure(string procedureName, SqlParameter[] parameters, SqlDbType? returnType = SqlDbType.Int)
        {
            using var cmd = new SqlCommand(procedureName, _cnn) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.AddRange(parameters);

            cmd.Parameters.Add(new SqlParameter("@returnValue", returnType));
            cmd.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

            try
            {
                cmd.ExecuteNonQuery();
                return cmd.Parameters["@returnValue"].Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"An exception caught during executing the following procedure '{procedureName}'.", ex);
            }          
        }

        /// <summary>
        /// Extracts basic details of contacts from the database whose display name (first and last name) contains the specified text.
        /// </summary>
        /// <param name="searchFor">Text to search for. Default: empty string</param>
        /// <returns>A list of <c>Contact</c> objects created from the results of the query.</returns>
        public List<Contact> ExtractContacts(string? searchFor="")
        {
            // Executing the query
            string query = "SELECT * " +
                "FROM (" +
                    "SELECT Id, CONCAT(FirstName, ' ', LastName) AS DisplayName, Icon, Favourite " +
                    "FROM Contact) AS sub " +
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

        /// <summary>
        /// Extracts full details of a specified contact from the database.
        /// </summary>
        /// <param name="id">Id of the contact to extract.</param>
        /// <returns>A dictionary of (field's name, value) pairs describing the contact.</returns>
        public Dictionary<string, string> GetContactInfo(int id)
        {
            var stringFields = _fieldNames.SubArray(1, 5);

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

        /// <summary>
        /// Updates a contact with the given Id using given details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="emailAddress"></param>
        /// <param name="company"></param>
        /// <param name="icon"></param>
        /// <param name="favourite"></param>
        /// <returns>Tuple of results from executing <c>UpdateContact</c> and <c>UpdateContactIcon</c> procedures.</returns>
        public (int, int) UpdateContact(int id, string firstName, string lastName, string phoneNumber, string emailAddress, string company, byte[]? icon, bool favourite)
        {
            // Update text fields
            SqlParameter[] parameters;
            
            parameters = new SqlParameter[_fieldNames.Length - 1];
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

            // Update icon 
            parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@id", SqlDbType.Int);
            parameters[0].Value = id;
            parameters[1] = new SqlParameter("@i", SqlDbType.Image);
            parameters[1].Value = icon == null ? DBNull.Value : icon;

            int res2 = (int)ExecuteProcedure("UpdateContactIcon", parameters);

            return (res1, res2);
        }

        /// <summary>
        /// Add a new contact to the table with the given details.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="emailAddress"></param>
        /// <param name="company"></param>
        /// <param name="icon"></param>
        /// <param name="favourite"></param>
        /// <returns>A result of executing the <c>AddContact procedure</c>.</returns>
        public int AddContact(string firstName, string lastName, string phoneNumber, string emailAddress, string company, byte[]? icon, bool favourite)
        {
            SqlParameter[] parameters = new SqlParameter[_fieldNames.Length - 1];
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
            parameters[5].Value = icon == null ? DBNull.Value : icon;
            parameters[6] = new SqlParameter("@f", SqlDbType.Bit);
            parameters[6].Value = favourite ? 1 : 0;

            return (int)ExecuteProcedure("AddContact", parameters, SqlDbType.Int);
        }

        /// <summary>
        /// Deletes a contact with the given Id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteContact(int id)
        {
            string query = $"DELETE FROM Contact WHERE Id = {id};";
            using var reader = ExecuteQuery(query);
        }
    }
}
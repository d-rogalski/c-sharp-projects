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
    }
}
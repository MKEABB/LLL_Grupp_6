using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace LLL_Grupp_6
{
    class DatabaseConnection
    {
        private SqlConnection connection;

        public DatabaseConnection()
        {
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=Ninja-Astronauts-DB; Integrated Security=true; TrustServerCertificate=true;";
            connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error opening connection: " + e.Message);
            }
        }   

        public void CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error closing connection: " + e.Message);
            }
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }
    }
}

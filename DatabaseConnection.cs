using System;
using System.Data.SqlClient;

namespace LLL_Grupp_6
{
    class DatabaseConnection
    {
        private SqlConnection connection; // Connection to the database
        private readonly string connectionString; // Connection string
        private readonly string dbName = "Ninja-Astronauts-DB"; // Database name

        public DatabaseConnection() // Constructor for the DatabaseConnection class
        {
            connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=Ninja-Astronauts-DB; Integrated Security=true; TrustServerCertificate=true;";
            connection = new SqlConnection(connectionString);

            InitializeDatabase(); // Call this method to ensure the database and tables are set up when the connection is instantiated
        }

        private void InitializeDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Corrected way to check if the database exists
                string checkDbQuery = $"SELECT db_id('{dbName}')";
                using (SqlCommand checkDbCommand = new SqlCommand(checkDbQuery, conn))
                {
                    if (checkDbCommand.ExecuteScalar() == DBNull.Value)
                    {
                        // Switch to the 'master' database to create a new database
                        conn.ChangeDatabase("master");

                        // Create database
                        string createDbQuery = $"CREATE DATABASE [{dbName}]";
                        using (SqlCommand createDbCommand = new SqlCommand(createDbQuery, conn))
                        {
                            createDbCommand.ExecuteNonQuery();
                        }

                        // Create tables
                        conn.ChangeDatabase(dbName);
                        CreateTables(conn);
                    }
                }
            }
        }

        private void CreateTables(SqlConnection conn) // Method for creating the tables in the database
        {
            // SQL command to create Pallet table
            string createPalletTableQuery = @"CREATE TABLE Pallet (
                                                  PalletID INT PRIMARY KEY NOT NULL,
                                                  PalletType NVARCHAR(10) NOT NULL,
                                                  ArrivalTime DATETIME NOT NULL
                                              );";
            SqlCommand createPalletTableCommand = new SqlCommand(createPalletTableQuery, conn);
            createPalletTableCommand.ExecuteNonQuery();

            // SQL command to create Storage table
            string createStorageTableQuery = @"CREATE TABLE Storage (
                                                   StorageID INT PRIMARY KEY NOT NULL,
                                                   ShelfID1 INT,
                                                   FOREIGN KEY (ShelfID1) REFERENCES Pallet(PalletID),
                                                   ShelfID2 INT,
                                                   FOREIGN KEY (ShelfID2) REFERENCES Pallet(PalletID)
                                               );";
            SqlCommand createStorageTableCommand = new SqlCommand(createStorageTableQuery, conn);
            createStorageTableCommand.ExecuteNonQuery();
        }

        public void OpenConnection() // Method for opening the connection to the database
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed) // Check if connection is closed before opening it
                {
                    connection.Open();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error opening connection: " + e.Message);
            }
        }
        public void CloseConnection() // Method for closing the connection to the database
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open) // Check if connection is open before closing it
                {
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error closing connection: " + e.Message);
            }
        }
        public SqlConnection GetConnection() // Method for getting the connection to the database
        {
            return connection;
        }
    }
}

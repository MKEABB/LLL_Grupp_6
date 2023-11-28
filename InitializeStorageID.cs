using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    public class InitializeStorageID
    {
        private DatabaseConnection dbConnection;

        // Initialize the DatabaseConnection object
        public InitializeStorageID()
        {
            dbConnection = new DatabaseConnection();
        }
        public void initializeStorageID()
        {
            // Open the database connection
            dbConnection.OpenConnection();

            // SQL query to check the existence of records in the "Storage" table
            string checkExistenceQuery = "SELECT COUNT(*) FROM Storage";

            // Execute the checkExistenceQuery
            using (SqlCommand checkExistenceCommand = new SqlCommand(checkExistenceQuery, dbConnection.GetConnection()))
            {
                // Get the count of records in the "Storage" table
                int rowCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar());

                // If there are no records, initialize them
                if (rowCount == 0)
                {
                   // Loop through storage IDs from 1 to 20 for initialization
                    for (int storageId = 1; storageId <= 20; storageId++)
                    {
                        // SQL query to insert records into the "Storage" table
                        string insertQuery = "INSERT INTO Storage (StorageID, ShelfID1, ShelfID2) VALUES (@StorageID, NULL, NULL)";

                        // Execute the insertQuery
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnection.GetConnection()))
                        {
                            // Add parameter for StorageID
                            insertCommand.Parameters.AddWithValue("@StorageID", storageId);
                            // Execute the insertion
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    // Print success message
                    Console.WriteLine("StorageID initialized successfully.");
            }
                else
            {
                    // Handle SQL exceptions and print an error message
                    Console.WriteLine("StorageID already exist. No need for initialization.");
            }
        }
        }
    }
}

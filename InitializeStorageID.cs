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

        public InitializeStorageID()
        {
            dbConnection = new DatabaseConnection();
        }
        public void initializeStorageID()
        {
            dbConnection.OpenConnection();

            string checkExistenceQuery = "SELECT COUNT(*) FROM Storage";

            using (SqlCommand checkExistenceCommand = new SqlCommand(checkExistenceQuery, dbConnection.GetConnection()))
            {
                int rowCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar());

                // If there are no records, initialize them
                if (rowCount == 0)
                {
                    for (int storageId = 1; storageId <= 20; storageId++)
                    {
                        string insertQuery = "INSERT INTO Storage (StorageID, ShelfID1, ShelfID2) VALUES (@StorageID, NULL, NULL)";

                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnection.GetConnection()))
                        {
                            insertCommand.Parameters.AddWithValue("@StorageID", storageId);

                            insertCommand.ExecuteNonQuery();
                        }
                    }

                Console.WriteLine("StorageID initialized successfully.");
            }
                else
            {
                Console.WriteLine("StorageID already exist. No need for initialization.");
            }
        }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6   // Hani Haj Hamdo 
{
    public class InitializeStorageID
    {
        private DatabaseConnection dbConnection;

        // Initiera DatabaseConnection-objektet
        public InitializeStorageID()
        {
            dbConnection = new DatabaseConnection();
        }
        public void initializeStorageID()
        {
            // Öppna databasanslutningen
            dbConnection.OpenConnection();

            // SQL-fråga för att kontrollera förekomsten av poster i "Storage"-tabellen
            string checkExistenceQuery = "SELECT COUNT(*) FROM Storage";

            // Kör checkExistenceQuery
            using (SqlCommand checkExistenceCommand = new SqlCommand(checkExistenceQuery, dbConnection.GetConnection()))
            {
                // Få antalet poster i "Storage"-tabellen
                int rowCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar());

                // Om det inte finns några poster, initiera dem
                if (rowCount == 0)
                {
                    // Gå igenom StorageID:n från 1 till 20 för initiering
                    for (int storageId = 1; storageId <= 20; storageId++)
                    {

                        // SQL-fråga för att infoga poster i "Storage"-tabellen
                        string insertQuery = "INSERT INTO Storage (StorageID, Size) VALUES (@StorageID, @Size)";

                        // Kör insertQuery
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnection.GetConnection()))
                        {
                            // Lägg till parameter för StorageID
                            insertCommand.Parameters.AddWithValue("@StorageID", storageId);
                            insertCommand.Parameters.AddWithValue("@Size", 100);
                            // Utför infogningen
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    // Skriv ut framgångsmeddelande
                    Console.WriteLine("StorageID initialized successfully.");
            }
                else
            {
                    // Hantera SQL-undantag och skriv ut ett felmeddelande
                    Console.WriteLine("StorageID already exist. No need for initialization.");
            }
        }
        }
    }
}

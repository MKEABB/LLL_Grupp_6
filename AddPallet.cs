using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6    //Hani Haj Hamdo 
{

    // Klass som representerar information om lagring
    public class Storage
    {
        public int StorageID { get; set; }
        public int Size { get; set; }
    }


    // Klass ansvarig för att lägga till pallar till lager
    public class AddPallet
    {
        private DatabaseConnection dbConnection;

        public AddPallet()
        {
            dbConnection = new DatabaseConnection();
        }

        // Metod för att lägga till en pall till lagring
        public void addPallet(int palletID ,int palletSize , DateTime arrivalTime)
        {
            try
            {
                dbConnection.OpenConnection();

                // Hitta en ledig lagringsplats
                Storage availableStorage = FindAvailableStorage(palletSize);

                // Om en lämplig plats hittas, sätt in pallen
                if (availableStorage != null)
                {
                    // Infoga pallinformation
                    InsertPallet(palletID, palletSize ,arrivalTime);

                    // Uppdatera lagringsplatser
                    UpdateStorageContent(palletID, availableStorage.StorageID);

                    // Uppdatera lagringsstorlek
                    UpdateStorageSize(availableStorage.StorageID, palletSize);

                    // Visa framgångsmeddelande
                    Console.WriteLine($"The pallet {palletID} has been added to the storage location {availableStorage.StorageID}.");
                }
                else
                {
                    // Visa ett meddelande när ingen ledig plats för pallen hittas
                    Console.WriteLine("No available place for the pallet.");
                }
            }
            catch (SqlException e)
            {
                // Hantera SQL-undantag och visa ett felmeddelande
                Console.WriteLine("Error during pallet insertion: " + e.Message);
            }
            finally
            {
                // Stäng databasanslutningen i finalblocket
                dbConnection.CloseConnection();
            }
        }

        // Metod för att hitta en ledig lagringsplats för en pall av given storlek
        private Storage FindAvailableStorage(int palletSize)
        {
            for (int storageId = 1; storageId <= 20; storageId++)
            {
                if (IsStorageAvailable(storageId, palletSize))
                {
                    // Returnera ett nytt Storage-objekt som representerar den tillgängliga lagringsplatsen
                    return new Storage
                    {
                        StorageID = storageId,
                        Size = palletSize
                    };
                }
            }
            // Returnera null när ingen ledig plats hittas
            return null; 
        }

        // Metod för att kontrollera om en lagringsplats finns tillgänglig för en pall av given storlek
        private bool IsStorageAvailable(int storageId, int palletSize)
        {
            string query = "SELECT 1 FROM Storage WHERE StorageID = @StorageID AND Size >= @Size";

            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                // Ställ in parametrar för SQL-frågan
                command.Parameters.AddWithValue("@StorageID", storageId);
                command.Parameters.AddWithValue("@Size", palletSize);

                // Kör frågan och kontrollera resultatet
                object result = command.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        // Metod för att uppdatera lagringsinnehållstabellen med den nya pallen
        private void UpdateStorageContent(int palletID, int storageID)
        {
            string insertQuery = "INSERT INTO StorageContent (PalletID, StorageID) VALUES (@PalletID, @StorageID)";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnection.GetConnection()))
            {
                // Ställ in parametrar för SQL-frågan
                insertCommand.Parameters.AddWithValue("@PalletID", palletID);
                insertCommand.Parameters.AddWithValue("@StorageID", storageID);

                // Kör frågan för att infoga den nya pallen i StorageContent-tabellen
                insertCommand.ExecuteNonQuery();
            }
        }
        // Metod för att uppdatera storleken på lagret efter att ha lagt till en pall
        private void UpdateStorageSize(int storageID, int palletSize)
        {
            string updateQuery = "UPDATE Storage SET Size = Size - @Size WHERE StorageID = @StorageID";

            using (SqlCommand updateCommand = new SqlCommand(updateQuery, dbConnection.GetConnection()))
            {
                updateCommand.Parameters.AddWithValue("@Size", palletSize);
                updateCommand.Parameters.AddWithValue("@StorageID", storageID);

                // Kör frågan för att uppdatera storleken på lagringen
                updateCommand.ExecuteNonQuery();
            }
        }

        // Metod för att infoga pallinformation i palltabellen
        private void InsertPallet(int palletID, int palletSize , DateTime arrivalTime)
        {
            string insertQuery = "INSERT INTO Pallet (PalletID, PalletSize, ArrivalTime) VALUES (@PalletID, @PalletSize, @ArrivalTime)";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, dbConnection.GetConnection()))
            {
                insertCommand.Parameters.AddWithValue("@PalletID", palletID);
                insertCommand.Parameters.AddWithValue("@PalletSize", palletSize);
                insertCommand.Parameters.AddWithValue("@ArrivalTime", arrivalTime);

                // Execute the query to insert the pallet information into the Pallet table
                insertCommand.ExecuteNonQuery();
            }
        }
    }
}

    

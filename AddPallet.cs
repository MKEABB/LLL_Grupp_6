using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{

    public class Pallet
    {
        public int PalletID { get; set; }
        public string PalletType { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
    public class Storage
    {
        public int StorageID { get; set; }
        public int? ShelfID1 { get; set; }
        public int? ShelfID2 { get; set; }
    }
    public class AddPallet
    {
        private DatabaseConnection dbConnection;

        public AddPallet()
        {
            dbConnection = new DatabaseConnection();
        }

        public void addPallet(int palletID, string palletType)
        {
            try
            {
                dbConnection.OpenConnection();

                // Find an available storage place
                Storage AvailableStorage = FindAvailableStorage(dbConnection.GetConnection(), palletType);


                // If a suitable place is found, insert the pallet
                if (AvailableStorage != null)
                {

                    // Insert pallet information
                    InsertPallet(dbConnection.GetConnection(), palletID, palletType);
                    // Update storage places
                    UpdateStorage(dbConnection.GetConnection(), AvailableStorage.StorageID, palletID, palletType);



                    Console.WriteLine($"The pallet has been added to the storage location {AvailableStorage.StorageID}.");
                }
                else
                {
                    Console.WriteLine("No available place for the pallet.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error during pallet insertion: " + e.Message);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        private static Storage FindAvailableStorage(SqlConnection connection, string palletType)
        {

            for (int storageId = 1; storageId <= 20; storageId++)
            {
                // Check if the storage place is available based on the provided palletType
                if (IsStorageAvailable(connection, storageId, palletType))
                {
                    Storage availableStorage = new Storage
                    {
                        StorageID = storageId,
                        ShelfID1 = null,
                        ShelfID2 = null
                    };

                    return availableStorage;
                }
            }

            return null; // No available place found
        }

        private static bool IsStorageAvailable(SqlConnection connection, int storageId, string palletType)
        {
            string query = "SELECT 1 FROM Storage WHERE StorageID = @StorageID " +
                           "AND ((@PalletType = 'Hell' AND ShelfID1 IS NULL AND ShelfID2 IS NULL) OR " +
                           "(@PalletType = 'Halv' AND (ShelfID1 IS NULL OR ShelfID2 IS NULL)))";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StorageID", storageId);
                command.Parameters.AddWithValue("@PalletType", palletType);

                object result = command.ExecuteScalar();


                return result != null && Convert.ToInt32(result) == 1;

            }
        }

        private void UpdateStorage(SqlConnection connection, int StorageID, int palletID, string palletType)
        {
            string updateQuery = "UPDATE Storage SET ";

            if (palletType == "Hell")
            {
                updateQuery += "ShelfID1 = @PalletID, ShelfID2 = @PalletID ";
            }
            else if (palletType == "Halv")
            {
                // Choose either ShelfID1 or ShelfID2 based on availability
                updateQuery += "ShelfID1 = CASE WHEN ShelfID1 IS NULL THEN @PalletID ELSE ShelfID1 END, " +
                               "ShelfID2 = CASE WHEN ShelfID1 IS NOT NULL THEN @PalletID ELSE ShelfID2 END ";
            }

            updateQuery += "WHERE StorageID = @StorageID";


            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@PalletID", palletID);
                updateCommand.Parameters.AddWithValue("@PalletType", palletType);
                updateCommand.Parameters.AddWithValue("@StorageID", StorageID);

                updateCommand.ExecuteNonQuery();
            }
        }

        private void InsertPallet(SqlConnection connection, int palletID, string palletType)
        {
            
            string insertQuery = "INSERT INTO Pallet ( PalletID,PalletType, ArrivalTime) " +
                                 "VALUES (@PalletID,@PalletType, GETDATE())";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@PalletID", palletID);
                insertCommand.Parameters.AddWithValue("@PalletType", palletType);

                insertCommand.ExecuteNonQuery();
            }
        }
        


    }
}

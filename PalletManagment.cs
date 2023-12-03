using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data.SqlTypes;

namespace LLL_Grupp_6
{
    public class PalletManagment
    {
        private DatabaseConnection dbConnection;

        public PalletManagment()
        {
            dbConnection = new DatabaseConnection();
        }
        public void RetrievePallet(int palletId)                    // Fetches and displays the information of a specific pallet based on its ID.
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "select p.*, s.Size, s.StorageID FROM pallet p " +
                               "JOIN " +
                               "StorageContent sc ON p.PalletID = sc.PalletID " +
                               "JOIN " +
                               "Storage s ON sc.StorageID = s.StorageID " +
                               "WHERE p.PalletID = @PalletID";

                using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    command.Parameters.AddWithValue("@PalletID", palletId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Pallet ID: " + reader["PalletID"]);  //Skriver ut pallinfon
                            Console.WriteLine("Pallet Size: " + reader["PalletSize"]);
                            Console.WriteLine("Arrival Time: " + reader["ArrivalTime"]);
                            Console.WriteLine("StorageID: " + reader["StorageID"]);
                            Console.WriteLine("StorageSize: " + reader["Size"]);
                        }
                        else
                        {
                            Console.WriteLine("No pallet with ID {0} exists.", palletId);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error retrieving pallet: " + e.Message);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        public void DeletePallet(int palletId)
        {
            try
            {
                dbConnection.OpenConnection();

                // Retrieve the pallet and storage info for cost calculation and updating size
                string retrieveQuery = @"
                SELECT p.PalletSize, p.ArrivalTime, sc.StorageID 
                FROM Pallet p 
                INNER JOIN StorageContent sc ON p.PalletID = sc.PalletID 
                WHERE p.PalletID = @PalletID";
                SqlCommand retrieveCommand = new SqlCommand(retrieveQuery, dbConnection.GetConnection());
                retrieveCommand.Parameters.AddWithValue("@PalletID", palletId);

                int storageId = 0;
                int palletSize = 0;
                DateTime arrivalTime = DateTime.MinValue;

                using (SqlDataReader reader = retrieveCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        palletSize = Convert.ToInt32(reader["PalletSize"]);
                        arrivalTime = Convert.ToDateTime(reader["ArrivalTime"]);
                        storageId = Convert.ToInt32(reader["StorageID"]);
                    }
                    else
                    {
                        Console.WriteLine("Pallet not found.");
                        return; // Exit the method if the pallet is not found
                    }
                }

                // Calculate the cost
                TimeSpan duration = DateTime.Now - arrivalTime;
                int rate = palletSize == 100 ? 80 : 40; // 80 kr/h for full pallet, 40 kr/h for half pallet
                double cost = duration.TotalHours * rate;
                Console.WriteLine($"Total cost for PalletID {palletId}: {cost} kr");

                // Update the size in the Storage table
                string updateStorageQuery = "UPDATE Storage SET Size = Size + @PalletSize WHERE StorageID = @StorageID";
                SqlCommand updateStorageCommand = new SqlCommand(updateStorageQuery, dbConnection.GetConnection());
                updateStorageCommand.Parameters.AddWithValue("@PalletSize", palletSize);
                updateStorageCommand.Parameters.AddWithValue("@StorageID", storageId);
                updateStorageCommand.ExecuteNonQuery();

                // Delete the entry from StorageContent table
                string deleteStorageContentQuery = "DELETE FROM StorageContent WHERE PalletID = @PalletID";
                SqlCommand deleteStorageContentCommand = new SqlCommand(deleteStorageContentQuery, dbConnection.GetConnection());
                deleteStorageContentCommand.Parameters.AddWithValue("@PalletID", palletId);
                deleteStorageContentCommand.ExecuteNonQuery();

                // Delete the pallet from the Pallet table
                string deletePalletQuery = "DELETE FROM Pallet WHERE PalletID = @PalletID";
                SqlCommand deletePalletCommand = new SqlCommand(deletePalletQuery, dbConnection.GetConnection());
                deletePalletCommand.Parameters.AddWithValue("@PalletID", palletId);
                deletePalletCommand.ExecuteNonQuery();

                Console.WriteLine("Pallet deleted successfully.");
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error deleting pallet: " + e.Message);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }
    }
}


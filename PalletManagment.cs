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

                // Retrieve the pallet info (including ArrivalTime and PalletSize) for cost calculation
                string retrieveQuery = "SELECT PalletSize, ArrivalTime FROM Pallet WHERE PalletID = @PalletID";
                SqlCommand retrieveCommand = new SqlCommand(retrieveQuery, dbConnection.GetConnection());
                retrieveCommand.Parameters.AddWithValue("@PalletID", palletId);

                using (SqlDataReader reader = retrieveCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int palletSize = Convert.ToInt32(reader["PalletSize"]);
                        DateTime arrivalTime = Convert.ToDateTime(reader["ArrivalTime"]);

                        // Calculate the cost
                        TimeSpan duration = DateTime.Now - arrivalTime;
                        int rate = palletSize == 100 ? 80 : 40; // 80 kr/h for full pallet, 40 kr/h for half pallet
                        double cost = duration.TotalHours * rate;

                        // Output the cost
                        Console.WriteLine($"Total cost for PalletID {palletId}: {cost} kr");
                    }
                    else
                    {
                        Console.WriteLine("Pallet not found.");
                        return; // Exit the method if the pallet is not found
                    }
                }

                // Update the StorageContent table to remove the reference to the pallet
                string updateStorageContentQuery = "DELETE FROM StorageContent WHERE PalletID = @PalletID";
                SqlCommand updateStorageContentCommand = new SqlCommand(updateStorageContentQuery, dbConnection.GetConnection());
                updateStorageContentCommand.Parameters.AddWithValue("@PalletID", palletId);
                updateStorageContentCommand.ExecuteNonQuery();

                // Delete the pallet from the Pallet table
                string deleteQuery = "DELETE FROM Pallet WHERE PalletID = @PalletID";
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, dbConnection.GetConnection());
                deleteCommand.Parameters.AddWithValue("@PalletID", palletId);
                deleteCommand.ExecuteNonQuery();

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


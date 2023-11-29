using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;

namespace LLL_Grupp_6
{
    public class PalletManagment
    {
        private DatabaseConnection dbConnection;
        private const int WholePalletRate = 80; // The rate for a whole pallet is 80 SEK per hour.
        private const int HalfPalletRate = 40; // The rate for a half pallet is 40 SEK per hour.
        public PalletManagment()
        {
            dbConnection = new DatabaseConnection();
        }
        public Tuple<int, string, DateTime, int> RetrievePallet(int palletId)  // Fetches and displays the information of a specific pallet based on its ID.
        {
            Tuple<int, string, DateTime, int> palletTuple = null;
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT p.*, s.StorageID FROM Pallet p\r\n" +
                               "JOIN Storage s ON s.ShelfID1 = p.PalletID OR s.ShelfID2 = p.PalletID\r\n" +
                               "WHERE p.PalletID = @PalletID;";

                using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    command.Parameters.AddWithValue("@PalletID", palletId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Pallet ID: " + reader["PalletID"]);
                            Console.WriteLine("Pallet Type: " + reader["PalletType"]);
                            Console.WriteLine("Arrival Time: " + reader["ArrivalTime"]);
                            Console.WriteLine("StorageID: " + reader["StorageID"]);

                            int tPalletID = Convert.ToInt32(reader["PalletID"]);
                            string tPalletType = reader["PalletType"].ToString();
                            DateTime tDateTime = reader.GetDateTime(reader.GetOrdinal("ArrivalTime"));
                            int tStorageID = Convert.ToInt32(reader["StorageID"]);

                            palletTuple = new Tuple<int, string, DateTime, int>(
                                                    tPalletID, tPalletType, tDateTime, tStorageID);
                        }
                        else
                        {
                            Console.WriteLine("No pallet with ID {0} exists.", palletId);
                            return null;
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
            return palletTuple;
        } 

        public void DeletePallet(int palletId)
        {
            try
            {
                dbConnection.OpenConnection();

                // First, retrieve the pallet info (including ArrivalTime and PalletType) for calculating cost.
                string retrieveQuery = "SELECT PalletType, ArrivalTime FROM Pallet WHERE PalletID = @PalletID";
                SqlCommand retrieveCommand = new SqlCommand(retrieveQuery, dbConnection.GetConnection());
                retrieveCommand.Parameters.AddWithValue("@PalletID", palletId);
                SqlDataReader reader = retrieveCommand.ExecuteReader();

                if (reader.Read())
                {
                    string palletType = reader["PalletType"].ToString();
                    DateTime arrivalTime = Convert.ToDateTime(reader["ArrivalTime"]);
                    reader.Close();

                    // Calculate the cost.
                    TimeSpan timeSpan = DateTime.Now - arrivalTime;
                    int rate = (palletType == "Whole") ? WholePalletRate : HalfPalletRate;
                    double cost = duration.TotalHours * rate;

                    // Display the cost.
                    Console.WriteLine($"Total cost for PalletID {palletId}: {cost} kr");

                    // Delete the pallet from the database.
                    string deleteQuery = "DELETE FROM Pallet WHERE PalletID = @PalletID";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, dbConnection.GetConnection());
                    deleteCommand.Parameters.AddWithValue("@PalletID", palletId);
                    deleteCommand.ExecuteNonQuery();

                    // Update the storage.
                    string updateStorageQuery = "UPDATE Storage SET ShelfID1 = NULL WHERE ShelfID1 = @PalletID; UPDATE Storage SET ShelfID2 = NULL WHERE ShelfID2 = @PalletID";
                    SqlCommand updateStorageCommand = new SqlCommand(updateStorageQuery, dbConnection.GetConnection());
                    updateStorageCommand.Parameters.AddWithValue("@PalletID", palletId);
                    updateStorageCommand.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("Pallet not found.");
                }
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

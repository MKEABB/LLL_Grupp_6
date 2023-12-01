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
        private const int WholePalletRate = 80; // kr/h
        private const int HalfPalletRate = 40; // kr/h

        public PalletManagment()
        {
            dbConnection = new DatabaseConnection();
        }
        public Pallet RetrievePallet(int palletId)                    // Fetches and displays the information of a specific pallet based on its ID.
        {
            Pallet searchedPallet = new Pallet();
            
            try
            {
                dbConnection.OpenConnection();
                string query = "select p.*, s.Size AS StorageSize from pallet p " +
                               "JOIN" +
                               "StorageContent sc ON p.PalletID = sc.PalletID " +
                               "JOIN" +
                               "Storage s ON sc.StorageID = s.StorageID" +
                               "where p.PalletID = @PalletID";

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


                            searchedPallet.PalletID = Convert.ToInt32(reader["PalletID"]);    
                            searchedPallet.PalletID = Convert.ToInt32(reader["PalletType"]);
                            searchedPallet.ArrivalTime = reader.GetDateTime(reader.GetOrdinal("ArrivalTime"));
                            //int tStorageID = Convert.ToInt32(reader["StorageID"]);

                           
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
            return searchedPallet;
        }

        public void DeletePallet(int palletId)
        {
            try
            {
                dbConnection.OpenConnection();

                // Retrieve the pallet info (including ArrivalTime and PalletType) for cost calculation
                string retrieveQuery = "SELECT PalletType, ArrivalTime FROM Pallet WHERE PalletID = @PalletID";
                SqlCommand retrieveCommand = new SqlCommand(retrieveQuery, dbConnection.GetConnection());
                retrieveCommand.Parameters.AddWithValue("@PalletID", palletId);
                using (SqlDataReader reader = retrieveCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string palletType = reader["PalletType"].ToString();
                        DateTime arrivalTime = Convert.ToDateTime(reader["ArrivalTime"]);

                        // Calculate the cost
                        TimeSpan duration = DateTime.Now - arrivalTime;
                        int rate = (palletType == "Whole") ? WholePalletRate : HalfPalletRate;
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

                // Update the Storage table to remove references to the pallet
                string updateStorageQuery = "UPDATE Storage SET ShelfID1 = NULL WHERE ShelfID1 = @PalletID; " +
                                            "UPDATE Storage SET ShelfID2 = NULL WHERE ShelfID2 = @PalletID";
                SqlCommand updateStorageCommand = new SqlCommand(updateStorageQuery, dbConnection.GetConnection());
                updateStorageCommand.Parameters.AddWithValue("@PalletID", palletId);
                updateStorageCommand.ExecuteNonQuery();

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
        public void PrintStorage()  // Hani Ha Hamdo 
        {
            try
            {
                // Open the database connection
                dbConnection.OpenConnection();

                // SQL query to retrieve storage details
                string query = @"
                    SELECT
                        s.StorageID,
                        s.ShelfID1,
                        p1.PalletType,
                        COALESCE(CONVERT(VARCHAR(20), p1.ArrivalTime, 120), '') AS ArrivalTime_ShelfID1,
                        s.ShelfID2,
                        p2.PalletType,
                        COALESCE(CONVERT(VARCHAR(20), p2.ArrivalTime, 120), '') AS ArrivalTime_ShelfID2
                    FROM Storage s
                    LEFT JOIN Pallet p1 ON s.ShelfID1 = p1.PalletID
                    LEFT JOIN Pallet p2 ON s.ShelfID2 = p2.PalletID";

                // Execute the SQL query
                using (var command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Storage Details:");

                        // Loop through each row in the result set
                        while (reader.Read())
                        {
                            // Extract values from the current row
                            int storageId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            int shelfId1 = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            string palletType1 = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            string arrivalTimeShelfID1 = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            int shelfId2 = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                            string palletType2 = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            string arrivalTimeShelfID2 = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

                            // Set color directly based on PalletType for Shelf 1
                            Console.ForegroundColor = palletType1.ToLower() == "whole" ? ConsoleColor.Green :
                                              palletType1.ToLower() == "half" ? ConsoleColor.Yellow : ConsoleColor.Gray;
                            Console.Write($"StorageID:{storageId}, Shelf 1: {shelfId1}, Type: {palletType1}, ArrivalTime: {arrivalTimeShelfID1}");
                            Console.ResetColor(); // Reset color to default
                            Console.Write(", ");

                            // Set color directly based on PalletType for Shelf 2
                            Console.ForegroundColor = palletType2.ToLower() == "half" ? ConsoleColor.Yellow :
                                              palletType2.ToLower() == "whole" ? ConsoleColor.Green : ConsoleColor.Gray;
                            Console.WriteLine($"Shelf 2: {shelfId2}, Type: {palletType2}, ArrivalTime: {arrivalTimeShelfID2}");
                            Console.ResetColor(); // Reset color to default
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                // Handle SQL exceptions
                Console.WriteLine("Error retrieving storage details: " + e.Message);
            }
            finally
            {
                // Close the database connection in the finally block
                dbConnection.CloseConnection();
            }
        }


       
    }
}


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

        //public void AddPallet(int palletId,string palletType) // Adds a pallet to the database with the specified ID and type.
        //{
        //    try
        //    {


        //        dbConnection.OpenConnection();

        //        int storageID = AvailableStorage();

        //        string query = "INSERT INTO Pallet (PalletType, ArrivalTime) VALUES (@PalletType, GETDATE())";

        //        using (var command = new SqlCommand(query, dbConnection.GetConnection()))
        //        {
        //            command.Parameters.AddWithValue("@PalletID", palletId);
        //            command.Parameters.AddWithValue("@PalletType", palletType);
        //            command.ExecuteNonQuery();
        //        }

        //    }
        //    catch (SqlException e)
        //    {
        //        Console.WriteLine("Error adding pallet: " + e.Message);
        //    }
        //    finally
        //    {
        //        dbConnection.CloseConnection();
        //    }
        //}
        public Tuple<int, string, DateTime, int> RetrievePallet(int palletId)                    // Fetches and displays the information of a specific pallet based on its ID.
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

        public void DeletePallet(int palletId) // Deletes a pallet based on its ID.
        {
            try
            {
                dbConnection.OpenConnection();

               


                string query = "DELETE FROM Pallet WHERE PalletID = @PalletID";

                using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    command.Parameters.AddWithValue("@PalletID", palletId);
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Pallet deleted successfully.");
                    }
                    else if (rowsAffected == 0)
                    {
                        Console.WriteLine("No pallet with that ID exists: " + palletId);
                    }
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
        public void PrintStorage()
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
                            Console.ForegroundColor = palletType1.ToLower() == "hell" ? ConsoleColor.Green :
                                              palletType1.ToLower() == "halv" ? ConsoleColor.Yellow : ConsoleColor.Gray;
                            Console.Write($"StorageID:{storageId}, Shelf 1: {shelfId1}, Type: {palletType1}, ArrivalTime: {arrivalTimeShelfID1}");
                            Console.ResetColor(); // Reset color to default
                            Console.Write(", ");

                            // Set color directly based on PalletType for Shelf 2
                            Console.ForegroundColor = palletType2.ToLower() == "halv" ? ConsoleColor.Yellow :
                                              palletType2.ToLower() == "hell" ? ConsoleColor.Green : ConsoleColor.Gray;
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

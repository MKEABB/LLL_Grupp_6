using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace LLL_Grupp_6

{
    public class PalletManagment
    {
        private static DatabaseConnection dbConnection;

        public PalletManagment()
        {
            dbConnection = new DatabaseConnection();
        }

        public void AddPallet(int palletId, string palletType) // Adds a pallet to the database with the specified ID and type.
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT INTO Pallet (PalletID, PalletType, ArrivalTime) VALUES (@PalletID, @PalletType, GETDATE())";

                using (var command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    command.Parameters.AddWithValue("@PalletID", palletId);
                    command.Parameters.AddWithValue("@PalletType", palletType);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error adding pallet: " + e.Message);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }
        public static Tuple<int, string, DateTime, int> RetrievePallet(int palletId)                    // Fetches and displays the information of a specific pallet based on its ID.
        {
            Tuple <int, string, DateTime, int> palletTuple = null;
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
    }
}

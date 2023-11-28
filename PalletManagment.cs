using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace LLL_Grupp_6
{
    public class PalletManagment
    {
        private DatabaseConnection dbConnection;

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
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class PrintStorage
    {
        private DatabaseConnection dbConnection;

        // Konstruktor för att initiera DatabaseConnection-objektet
        public PrintStorage()
        {
            dbConnection = new DatabaseConnection();
        }

        // Metod för att skriva ut lagringsdetaljer
        public void printStorage()  // Hani Ha Hamdo 
        {

            try
            {
                // Öppna databasanslutningen
                dbConnection.OpenConnection();

                // SQL-fråga för att hämta lagringsdetaljer med joins
                string query = @"SELECT s.StorageID, s.Size, p.PalletID, p.PalletSize, p.ArrivalTime 
                            FROM Storage s 
                            LEFT JOIN StorageContent sc ON s.StorageID = sc.StorageID 
                            LEFT JOIN Pallet p ON sc.PalletID = p.PalletID";

                // Använda SqlCommand för att köra frågan
                using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
                {
                    // Använda SqlDataReader för att läsa resultaten
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Utskriftshuvud för lagringsdetaljerna
                        Console.WriteLine("Storage Details:");
                        Console.WriteLine("--------------------------------------------------------------------");
                        Console.WriteLine("StorageID | Size Available  | PalletID | PalletSize | ArrivalTime");
                        Console.WriteLine("--------------------------------------------------------------------");

                        // Gå igenom resultatuppsättningen och skriv ut varje rad
                        while (reader.Read())
                        {
                            // Extrahera värden från läsaren
                            int storageID = reader.GetInt32(reader.GetOrdinal("StorageID"));
                            int size = reader.GetInt32(reader.GetOrdinal("Size"));
                            int? palletID = reader.IsDBNull(reader.GetOrdinal("PalletID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PalletID"));
                            int? palletSize = reader.IsDBNull(reader.GetOrdinal("PalletSize")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PalletSize"));
                            DateTime? arrivalTime = reader.IsDBNull(reader.GetOrdinal("ArrivalTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ArrivalTime"));

                            // Skriver ut formaterad rad
                            Console.WriteLine($"{storageID,-9} | {size,-15} | {palletID,-8} | {palletSize,-11} | {arrivalTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                // Hanterar SQL-undantag och skriver ut ett felmeddelande
                Console.WriteLine("Error retrieving storage details: " + e.Message);
            }
            finally
            {
                // Se till att databasanslutningen är stängd, oavsett framgång eller misslyckande
                dbConnection.CloseConnection();
            }
        }
    }
}

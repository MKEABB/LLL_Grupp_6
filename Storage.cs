using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class StorageCheck
    {
        public static bool GotHalfPalletCapacity(int newStorageID)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=LundsNyaLånglager; Integrated Security=true; TrustServerCertificate=true;";

            // Returnerar en ledig plats om det finns en
            string halfPalletQuery = "SELECT TOP 1 StorageID FROM Storage WHERE StorageID = @StorageID " +
                                     "AND (ShelfID1 IS NULL OR ShelfID2 IS NULL)"; //Med stringbuilder skulle man kunna ha ett condition
                                                                                   //som gör att man skulle kunna använda samma metod för
                                                                                   //kolla både hel och halvpallsplatser

            // CONNECTION
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // QSTRING + CONNECTION = COMMAND
                SqlCommand command = new SqlCommand(halfPalletQuery, connection);

                command.Parameters.AddWithValue("@StorageID", newStorageID);

                // ÖPPNA CONNECTION
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return reader.HasRows;

                }
            }
        }
        public static bool GotFullPalletCapacity(int newStorageID)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=LundsNyaLånglager; Integrated Security=true; TrustServerCertificate=true;";

            // Returnerar en ledig plats om det finns en
            string halfPalletQuery = "SELECT TOP 1 StorageID FROM Storage WHERE StorageID = @StorageID " +
                                     "AND (ShelfID1 IS NULL AND ShelfID2 IS NULL)";

            // CONNECTION
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // QSTRING + CONNECTION = COMMAND
                SqlCommand command = new SqlCommand(halfPalletQuery, connection);

                command.Parameters.AddWithValue("@StorageID", newStorageID);

                // ÖPPNA CONNECTION
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return reader.HasRows;

                }
            }
        }
    }
}

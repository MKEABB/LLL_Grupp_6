using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class Storage
    {
        public static int GotHalfPalletCapacity()
        {
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=LundsNyaLånglager; Integrated Security=true; TrustServerCertificate=true;";

            //Returnerar en ledig plats om det finns en
            string halfPalletQuery = "SELECT TOP 1 StorageID FROM Storage" +        
                                         "WHERE ShelfID1 IS NULL OR ShelfID2 IS NULL";

            //CONNECTION
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //QSTRING + CONNECTION = COMMAND
                SqlCommand command = new SqlCommand(halfPalletQuery, connection);

                //ÖPPNA CONNECTION
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int storageID = (int)reader["StorageID"];
                    return storageID;
                }
            }
        }
    }
}

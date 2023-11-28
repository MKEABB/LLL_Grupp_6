using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class Pallet
    {

        public static void Move()
        {
            //vvHur kallar jag på dataconnection och bara får stringen. 
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=LundsNyaLånglager; Integrated Security=true; TrustServerCertificate=true;";

            int newStorageID = -1;
            var searchedTuple = PalletManagment.RetrievePallet(112);
            Console.WriteLine("TEST: Item 1:{0}, item 2:{1}, item 3:{2}, item 4:{3}",
                              searchedTuple.Item1, searchedTuple.Item2, searchedTuple.Item3, searchedTuple.Item4);

            while (newStorageID > 20 || newStorageID < 0)                                       //Om inte den nya pallplatsen är inom 0-20 tar den in ny indata
            {
                UserInput.Int($"Move pallet from {searchedTuple.Item4} to ", ref newStorageID);
            }
            int firstAvalible = Storage.GotHalfPalletCapacity();

            if (searchedTuple.Item2 == "HALV" && firstAvalible >= 0) //Palltypen är halv och första tillgängliga platsen är större eller likamed noll
            {
                string moveInsertQuery = "INSERT INTO Pallet (PalletID, PalletType, ArrivalTime)\r\n" +
                                         "VALUES (@PalletID, @PalletType, @ArrivalTime);" +
                                         "INSERT INTO Storage (StorageID, ShelfID1, ShelfID2)\r\n" +
                                         "VALUES (" +
                                         "CASE WHEN ShelfID1 IS NULL THEN @ShelfID ELSE ShelfID1 END, " +
                                         "CASE WHEN ShelfID1 IS NOT NULL THEN @ShelfID ELSE ShelfID2 END)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //QSTRING + CONNECTION = COMMAND
                    SqlCommand command = new SqlCommand(moveInsertQuery, connection);

                    //ÖPPNA CONNECTION
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    command.Parameters.AddWithValue("@PalletID", searchedTuple.Item1);
                    command.Parameters.AddWithValue("@PalletType", searchedTuple.Item2);
                    command.Parameters.AddWithValue("@ArrivalTime", searchedTuple.Item3);
                    command.Parameters.AddWithValue("@StorageID", firstAvalible);
                    command.Parameters.AddWithValue("@ShelfID1"), 
                    command.Parameters.AddWithValue("@ShelfID2"), 
                    




                }



                // skriv över med null på den gamla platsen 
            }
            else if (searchedTuple.Item2 == "HEL")
            {
                string fullPalletQuery = "SELECT TOP 1 StorageID FROM Storage" +
                                         "WHERE ShelfID1 IS NULL AND ShelfID2 IS NULL";
                //Lediga helpallsplatser visas
                // informationen blir inserted
                // skriv över med null på de gamla platserna 
            }




        }
    }
}

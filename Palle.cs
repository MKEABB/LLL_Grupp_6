using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class Palle
    {

        public static void Move(Tuple<int, string, DateTime, int> retrievedPallet)
        {
            //Ändra
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=Ninja-Astronauts-DB; Integrated Security=true; TrustServerCertificate=true;";

            bool gotCapacity = false;
            int newStorageID = -1;


            while (newStorageID > 20 || newStorageID < 0)                                       //Om inte den nya pallplatsen är inom 0-20 tar den in ny indata
            {
                UserInput.Int($"Move pallet from {retrievedPallet.Item4} to ", ref newStorageID);
            }


            if (retrievedPallet.Item2 == "Half")                                                //Palltypen är halv 
            {
                gotCapacity = StorageCheck.GotHalfPalletCapacity(newStorageID);                 //Kollar om sökt StorageID har minst en ledig halvplats.

                if (gotCapacity == true)
                {
                    string deleteUpdateQuery = "UPDATE Storage " +  
                                               "SET ShelfID1 = NULL, ShelfID2 = NULL " +
                                               "WHERE ShelfID1 = @PreviousPID OR ShelfID2 = @PreviousPID;";

                    string moveInsertQuery = "UPDATE Storage " +                                //Ändrar shelfID 1/2 till nytt id beroende på om den är null eller inte. 
                                             "SET ShelfID1 = CASE WHEN ShelfID1 IS NULL THEN @PalletID ELSE ShelfID1 END, " + //Om den inte är null är värdet kvar 
                                             "ShelfID2 = CASE WHEN ShelfID1 IS NOT NULL THEN @PalletID ELSE ShelfID2 END " +  //Alltså väljs vilken av de två halvpalls-
                                             "WHERE StorageID = @StorageID;";                                                 //platserna den ska vara på automatiskt.

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Skriver över med null på tidigare plats
                        using (SqlCommand deleteCommand = new SqlCommand(deleteUpdateQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@PreviousPID", retrievedPallet.Item1);
                            deleteCommand.ExecuteNonQuery();
                        }

                        // Nya värden
                        using (SqlCommand moveCommand = new SqlCommand(moveInsertQuery, connection))
                        {
                            moveCommand.Parameters.AddWithValue("@StorageID", newStorageID);
                            moveCommand.Parameters.AddWithValue("@PalletID", retrievedPallet.Item1);
                            moveCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine("Pallet moved successfully");

                    }
                }
            }
            else if (retrievedPallet.Item2 == "Whole")                                          //Inmatad pall är en helpall
            {
                gotCapacity = StorageCheck.GotFullPalletCapacity(newStorageID);                 //Metoden för att se om de önskade platserna är lediga.

                if (gotCapacity == true)
                {
                    string deleteUpdateQuery = "UPDATE Storage " +
                                               "SET ShelfID1 = NULL, ShelfID2 = NULL " +
                                               "WHERE ShelfID1 = @PreviousPID OR ShelfID2 = @PreviousPID;";

                    string moveInsertQuery = "UPDATE Storage " +                                //Om ShelfID är null ges ShelfID värdet av den sökta pallen
                                             "SET ShelfID1 = COALESCE(ShelfID1, @PalletID)," +
                                             "ShelfID2 = COALESCE(ShelfID2, @PalletID)" +
                                             "WHERE StorageID = @StorageID; ";


                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        connection.Open();

                        // Skriver över med null på tidigare platser
                        using (SqlCommand deleteCommand = new SqlCommand(deleteUpdateQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@PreviousPID", retrievedPallet.Item1);
                            deleteCommand.ExecuteNonQuery();
                        }
                        //Nya värden
                        SqlCommand command = new SqlCommand(moveInsertQuery, connection);
                        command.Parameters.AddWithValue("@StorageID", newStorageID);
                        command.Parameters.AddWithValue("@PalletID", retrievedPallet.Item1);
                        SqlDataReader reader = command.ExecuteReader();
                    }
                }
            }
            else
            {

                //Lediga helpallsplatser visas
                // informationen blir inserted
                // skriv över med null på de gamla platserna 
            }
        }
    }
}

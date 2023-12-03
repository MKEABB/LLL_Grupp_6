using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class Palle
    {


        public static void Move(int palletId)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=Ninja-Astronauts-DB; Integrated Security=true; TrustServerCertificate=true;";

            int newStorageID = -1;

            int scPalletSize = 0;                                                                                           //Pallvariabler
            int scStorageID = 0;
            int storageSize = 0;

                                                                                                                            //Hämtar infon om pallen i fråga
            string palletPlacementQ = @"SELECT p.PalletSize, sc.StorageID AS [SC StorageID], sc.PalletID AS[SC PalletID],
                                    s.Size AS StorageSize
                                    FROM Pallet p
                                    JOIN StorageContent sc ON p.PalletID = sc.PalletID
                                    JOIN Storage s ON sc.StorageID = s.StorageID
                                    WHERE p.PalletID = @PalletID";

            while (newStorageID < 1 || newStorageID > 20)                                                                   //Ser till att inmatat värde inte är
            {                                                                                                               //utanför 20
                Console.WriteLine("New storageID: ");
                newStorageID = int.Parse(Console.ReadLine());
            }
                                                                                                                            //Första update ser till att Size
                                                                                                                            //för den gamla platsen får rätt
                                                                                                                            //värde och att den nya platsens
                                                                                                                            //size blir uppdaterad
            string restoreStorageSizeQ = @"UPDATE Storage                                                                   
                                         SET Size = Size + @SCPalletSize
                                         WHERE StorageID = @PreviousStorageID;
                                         UPDATE Storage
                                         SET Size = Size - @SCPalletSize
                                         WHERE StorageID = @NewStorageID;";
                                                                                                                            //Till sist uppdateras storageContent
            string updateStorageContentQ = @"UPDATE StorageContent
                                          SET StorageID = @NewStorageID
                                          WHERE PalletID = @PalletID;";


            using (SqlConnection connection = new SqlConnection(connectionString))                                          //Main connection
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())                                          //Transaction så att inget uppdateras om inte allt går igenom
                {
                    try
                    {

                        // (palletPlacementQ) Pallinfo
                        using (SqlCommand placementCommand = new SqlCommand(palletPlacementQ, connection, transaction))
                        {
                            placementCommand.Parameters.AddWithValue("@PalletID", palletId);

                            using (SqlDataReader reader = placementCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    scPalletSize = Convert.ToInt32(reader["PalletSize"]);
                                    scStorageID = Convert.ToInt32(reader["SC StorageID"]);
                                    storageSize = Convert.ToInt32(reader["StorageSize"]);
                                }
                            }
                        }
                                                                                                                            //Kollar så att den valda storageplatsen
                        if (IsAvailable(scStorageID, scPalletSize) == false)                                                //har plats åt den nya pallen
                        {
                            throw new Exception("StorageID not available");
                        }

                        // (restoreStorageSizeQ) Återställa Storage>Size                                                    
                        using (SqlCommand restoreStorageSizeCommand = new SqlCommand(restoreStorageSizeQ, connection, transaction))
                        {
                            restoreStorageSizeCommand.Parameters.AddWithValue("@PalletID", palletId);
                            restoreStorageSizeCommand.Parameters.AddWithValue("@SCPalletSize", scPalletSize);
                            restoreStorageSizeCommand.Parameters.AddWithValue("@PreviousStorageID", scStorageID);
                            restoreStorageSizeCommand.Parameters.AddWithValue("@CurrentStorageSize", storageSize);
                            restoreStorageSizeCommand.Parameters.AddWithValue("@NewStorageID", newStorageID);
                            restoreStorageSizeCommand.ExecuteNonQuery();
                        }

                        // (updateStorageContentQ) uppdatera * StorageContent 
                        using (SqlCommand updateStorageContentCommand = new SqlCommand(updateStorageContentQ, connection, transaction))
                        {
                            updateStorageContentCommand.Parameters.AddWithValue("@PalletID", palletId);
                            updateStorageContentCommand.Parameters.AddWithValue("@NewStorageID", newStorageID);
                            updateStorageContentCommand.ExecuteNonQuery();
                        }
                        Console.WriteLine("PalletID {0} moved from StorageID {1} to {2}", palletId, scStorageID, newStorageID);
                        transaction.Commit();
                    }
                    catch (Exception x)
                    {
                        transaction.Rollback();
                        Console.WriteLine("ROLLBACK: {0}", x);
                    }
                }
            }
        }
        private static bool IsAvailable(int newStorageID, int palletSize)//!!!Lånad från Hani men jag har modifierat den lite. 
        {                                                                                                                   //(AKA jag har gjort den lite sämre)
            string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=Ninja-Astronauts-DB; Integrated Security=true; TrustServerCertificate=true;";
            string query = "SELECT 1 FROM Storage WHERE StorageID = @StorageID AND Size >= @Size";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand queryCommand = new SqlCommand(query, connection))
                {
                    queryCommand.Parameters.AddWithValue("@StorageID", newStorageID);
                    queryCommand.Parameters.AddWithValue("@Size", palletSize);

                    using (SqlDataReader reader = queryCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
    }
}

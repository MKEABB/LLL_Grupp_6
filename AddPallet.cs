﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6    //Hani Haj Hamdo 
{
    // Pallet class to represent pallet information
    public class Pallet
    {
        public int PalletID { get; set; }
        public string PalletType { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
    // Storage class to represent storage information
    public class Storage
    {
        public int StorageID { get; set; }
        public int? ShelfID1 { get; set; }
        public int? ShelfID2 { get; set; }
    }
    // Class responsible for adding pallets to storage
    public class AddPallet
    {
        private DatabaseConnection dbConnection;

        public AddPallet()
        {
            dbConnection = new DatabaseConnection();
        }

        // Method to add a pallet to storage
        public void addPallet(int palletID, string palletType, DateTime ArrivalTime)
        {
            try
            {
                dbConnection.OpenConnection();

                // Find an available storage place
                Storage AvailableStorage = FindAvailableStorage(dbConnection.GetConnection(), palletType);


                // If a suitable place is found, insert the pallet
                if (AvailableStorage != null)
                {

                    // Insert pallet information
                    InsertPallet(dbConnection.GetConnection(), palletID, palletType, ArrivalTime);
                    // Update storage places
                    UpdateStorage(dbConnection.GetConnection(), AvailableStorage.StorageID, palletID, palletType);



                    Console.WriteLine($"The pallet {palletID} has been added to the storage location {AvailableStorage.StorageID}.");
                }
                else
                {
                    Console.WriteLine("No available place for the pallet.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error during pallet insertion: " + e.Message);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }
        // Method to find an available storage place based on pallet type
        private static Storage FindAvailableStorage(SqlConnection connection, string palletType)
        {
            for (int storageId = 1; storageId <= 20; storageId++)
            {
                string query = "SELECT 1 FROM Storage WHERE StorageID = @StorageID " +
                               "AND ((@PalletType = 'Whole' AND ShelfID1 IS NULL AND ShelfID2 IS NULL) OR " +
                               "(@PalletType = 'Half' AND (ShelfID1 IS NULL OR ShelfID2 IS NULL)))";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StorageID", storageId);
                    command.Parameters.AddWithValue("@PalletType", palletType);

                    object result = command.ExecuteScalar();

                    if (result != null && Convert.ToInt32(result) == 1)
                    {
                        Storage availableStorage = new Storage
                        {
                            StorageID = storageId,
                            ShelfID1 = null,
                            ShelfID2 = null
                        };

                        return availableStorage;
                    }
                }
            }
            // No available place found
            return null; 
        }

        // Method to update storage information after adding a pallet
        private void UpdateStorage(SqlConnection connection, int StorageID, int palletID, string palletType)
        {
            string updateQuery = "UPDATE Storage SET ";

            if (palletType == "Whole")
            {
                updateQuery += "ShelfID1 = @PalletID, ShelfID2 = @PalletID ";
            }
            else if (palletType == "Half")
            {
                // Choose either ShelfID1 or ShelfID2 based on availability
                updateQuery += "ShelfID1 = CASE WHEN ShelfID1 IS NULL THEN @PalletID ELSE ShelfID1 END, " +
                               "ShelfID2 = CASE WHEN ShelfID1 IS NOT NULL THEN @PalletID ELSE ShelfID2 END ";
            }

            updateQuery += "WHERE StorageID = @StorageID";


            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@PalletID", palletID);
                updateCommand.Parameters.AddWithValue("@PalletType", palletType);
                updateCommand.Parameters.AddWithValue("@StorageID", StorageID);

                updateCommand.ExecuteNonQuery();
            }
        }
        // Method to insert pallet information into the database
        private void InsertPallet(SqlConnection connection, int palletID, string palletType, DateTime ArrivalTime)
        {
            



            string insertQuery = "INSERT INTO Pallet ( PalletID,PalletType, ArrivalTime) " +
                                 "VALUES (@PalletID,@PalletType, @ArrivalTime)";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@PalletID", palletID);
                insertCommand.Parameters.AddWithValue("@PalletType", palletType);
                insertCommand.Parameters.AddWithValue("@ArrivalTime", ArrivalTime);

                insertCommand.ExecuteNonQuery();
            }
        }
        


    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6    // Hani Haj Hamdo
{
    public class InitializePall
    {

        private DatabaseConnection dbConnection;

        public InitializePall()
        {
            dbConnection = new DatabaseConnection();
        }
        public void initializePall()
        {
            dbConnection.OpenConnection();

            AddPallet addPallet = new AddPallet();

            // Get the current count of "Half" pallets
            int currentHalfPalletCount = PalletExist("Half");

            // Calculate the number of "Half" pallets to add
            int halfPalletsToAdd = Math.Min(3 - currentHalfPalletCount, 3);

            // Initialize "Half" pallets
            for (int i = 1; i <= halfPalletsToAdd; i++)
            {
                InitializePallet(addPallet, currentHalfPalletCount + i, "Half");
            }

            // Get the current count of "Whole" pallets
            int currentWholePalletCount = PalletExist("Whole");

            // Calculate the number of "Whole" pallets to add
            int wholePalletsToAdd = Math.Min(3 - currentWholePalletCount, 3);

            // Initialize "Whole" pallets
            for (int i = 4; i <= 3 + wholePalletsToAdd; i++)
            {
                InitializePallet(addPallet, currentWholePalletCount + i, "Whole");
            }


        }
        void InitializePallet(AddPallet addPallet, int palletID, string palletType)
        {
            while (PalletIDExists(palletID))
            {
                palletID++; // Increment the palletID until a unique one is found
            }
            Random random = new Random();

            // Define a range for the random time (adjust as needed)
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = DateTime.Now;

            // Generate a random time within the specified range
            DateTime randomTime = startDate + TimeSpan.FromTicks((long)(random.NextDouble() * (endDate - startDate).Ticks));


            // Call the addPallet method to add the pallet to storage
            addPallet.addPallet(palletID, palletType, randomTime);

        }
        // Method to check the count of pallets with a specific type
        int PalletExist(string palletType)
        {
            int palletCount = 0;
            // SQL query to count pallets of a specific type
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletType = @PalletType";


            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                // Adding parameter to the SQL command
                command.Parameters.AddWithValue("@PalletType", palletType);

                // Executing the query and retrieving the count
                palletCount = (int)command.ExecuteScalar();
            }

            // Returning the count of pallets with the specified type
            return palletCount;
        }
        // Method to check if a pallet with a specific ID exists
        bool PalletIDExists(int palletID)
        {
            // SQL query to count pallets with a specific ID
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletID = @PalletID";

            // Using statement ensures proper resource disposal
            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                // Adding parameter to the SQL command
                command.Parameters.AddWithValue("@PalletID", palletID);

                // Executing the query and retrieving the count
                int palletCount = (int)command.ExecuteScalar();

                // Returning whether a pallet with the specified ID exists
                return palletCount > 0;
            }
        }

    }
}

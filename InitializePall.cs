using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
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

        int PalletExist(string palletType)
        {
            int palletCount = 0;
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletType = @PalletType";


            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@PalletType", palletType);
                palletCount = (int)command.ExecuteScalar();
            }


            return palletCount;
        }
        bool PalletIDExists(int palletID)
        {
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletID = @PalletID";

            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@PalletID", palletID);
                int palletCount = (int)command.ExecuteScalar();

                return palletCount > 0;
            }
        }

    }
}

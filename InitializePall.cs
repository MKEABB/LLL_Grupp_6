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



            // Initialize three "Hell" pallets
            for (int i = 1; i <= 3; i++)
            {
                InitializePallet(addPallet, i, "Whole");
            }

            // Initialize three "Halv" pallets
            for (int i = 4; i <= 6; i++)
            {
                InitializePallet(addPallet, i, "Half");
            }
        }
        void InitializePallet(AddPallet addPallet, int palletID, string palletType)
        {
            Random random = new Random();

            // Define a range for the random time (adjust as needed)
            DateTime startDate = new DateTime(2023, 1, 1);
            DateTime endDate = DateTime.Now;

            // Generate a random time within the specified range
            DateTime randomTime = startDate + TimeSpan.FromTicks((long)(random.NextDouble() * (endDate - startDate).Ticks));

            // Count the number of pallets with the specified ID
            int palletCount = PalletExist();

            // Check if there are already three pallets of the specified type
            if (palletCount < 6)
            {
                // Call the addPallet method to add the pallet to storage
                addPallet.addPallet(palletID, palletType, randomTime);
            }
            else
            {
                // Console.WriteLine($"Pallets already exist. No need for initialization .");
            }
        }

        int PalletExist()
        {
            int palletCount = 0;
            string query = "SELECT COUNT(*) FROM Pallet ";


            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                
                palletCount = (int)command.ExecuteScalar();
            }


            return palletCount;
        }

    }
}

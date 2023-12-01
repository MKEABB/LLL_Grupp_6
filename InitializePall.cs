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

            // Få det aktuella antalet "halva" pallar
            int currentHalfPalletCount = PalletExist(50);

            // Beräkna antalet "halva" pallar att lägga till
            int halfPalletsToAdd = Math.Min(3 - currentHalfPalletCount, 3);

            // Initiera "Halva" pallar
            for (int i = 1; i <= halfPalletsToAdd; i++)
            {
                InitializePallet(addPallet, currentHalfPalletCount + i, 50);
            }

            // Få det aktuella antalet "Hela" pallar
            int currentWholePalletCount = PalletExist(100);

            // Beräkna antalet "Hela" pallar att lägga till
            int wholePalletsToAdd = Math.Min(3 - currentWholePalletCount, 3);

            // Initiera "Hela" pallar
            for (int i = 4; i <= 3 + wholePalletsToAdd; i++)
            {
                InitializePallet(addPallet, currentWholePalletCount + i, 100);
            }


        }
        void InitializePallet(AddPallet addPallet, int palletID, int palletType)
        {
            while (PalletIDExists(palletID))
            {
                palletID++; // Öka pall-ID tills ett unikt hittas
            }
            Random random = new Random();

            // Definiera ett intervall för den slumpmässiga tiden 
            DateTime startDate = new DateTime(2023, 5, 1);
            DateTime endDate = DateTime.Now;

            // Generera en slumpmässig tid inom det angivna intervallet
            DateTime randomTime = startDate + TimeSpan.FromTicks((long)(random.NextDouble() * (endDate - startDate).Ticks));


            // Anropa addPallet-metoden för att lägga till pallen till lagring
            addPallet.addPallet(palletID, palletType, randomTime);

        }
        // Metod för att kontrollera antalet pallar med en specifik typ
        int PalletExist(int palletSize)
        {
            int palletCount = 0;
            // SQL-fråga för att räkna pallar av en specifik typ
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletSize = @PalletSize";


            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                // Lägger till parameter till SQL-kommandot
                command.Parameters.AddWithValue("@PalletSize", palletSize);

                // Utför frågan och hämtar räkningen
                palletCount = (int)command.ExecuteScalar();
            }

            // Returnerar antalet pallar med angiven typ
            return palletCount;
        }
        // Metod för att kontrollera om en pall med ett specifikt ID finns
        bool PalletIDExists(int palletID)
        {
            // SQL-fråga för att räkna pallar med ett specifikt ID
            string query = "SELECT COUNT(*) FROM Pallet WHERE PalletID = @PalletID";

            // Att använda uttalandet säkerställer korrekt resursförfogande
            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                // Lägger till parameter till SQL-kommandot
                command.Parameters.AddWithValue("@PalletID", palletID);

                // Utför frågan och hämtar räkningen
                int palletCount = (int)command.ExecuteScalar();

                // Returnerar om en pall med angivet ID finns
                return palletCount > 0;
            }
        }

    }
}

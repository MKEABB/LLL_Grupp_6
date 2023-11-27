using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class AddPalletTesting
    {
        public static string connectionString =
               "Data Source=DESKTOP-O4PFI5T\\SQLEXPRESS;Database=LundsLånglager2;"
               + "Integrated Security=true; TrustServerCertificate=true;";
        public static void New()
        {
            //INPUT
            Console.WriteLine("PalletID (int) ");
            int palletId = int.Parse(Console.ReadLine());

            Console.WriteLine("PalletType (int) ");
            int palletType = int.Parse(Console.ReadLine());

            //CONNECTION
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //ÖPPNA CONNECTION
                connection.Open();

                //QUERYSTRING MED PARAMETER
                string addPalletQuery = "INSERT INTO Pallet (PalletID, PalletType, ArrivalTime) " +
                                        "VALUES (@PalletID, @PalletType, GETDATE());";

                using (SqlCommand palletCommand = new SqlCommand(addPalletQuery, connection)) //Första SQLQuery för order körs
                {
                    palletCommand.Parameters.AddWithValue("@PalletID", palletId);
                    //int newOrderID = Convert.ToInt32(palletCommand.ExecuteScalar()); //Returnerar den första raden från orderCommand(OrderID)

                    string storageAddQuery = "INSERT INTO Storage (PalletID)" +
                                              "VALUES(@PalletID);";

                    using (SqlCommand orderDetailsCommand = new SqlCommand(storageAddQuery, connection))
                    {
                        orderDetailsCommand.Parameters.AddWithValue("@PalletID", palletId);
                        orderDetailsCommand.Parameters.AddWithValue("@PalletType", palletType);

                        orderDetailsCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine("Pallet added");
                    Console.ReadLine();
                }
            }
        }
    }
}


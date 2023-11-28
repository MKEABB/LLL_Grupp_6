﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PalletManagment palletManagment = new PalletManagment();
            bool menu = true;

            while (menu)
            {
                //my menu shows what we can do in the Storage
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("** Stroage menu **");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("******************************************");
                Console.WriteLine("1) Submission of pallets ");
                Console.WriteLine("2) Handing out of pallets ");
                Console.WriteLine("3) Moving of pallet ");
                Console.WriteLine("4) Search for pallets");
                Console.WriteLine("5) Printout of storage ");
                Console.WriteLine("6) Close the program ");
                Console.WriteLine("******************************************");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Choose one of the options, Enter a number:");

                // switch statement checks which option the user chose
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Enter Pallet ID: ");
                        int palletId = Convert.ToInt32(Console.ReadLine());

                        string palletType = PalletType();

                        palletManagment.AddPallet(palletId, palletType);
                        break;
                    case "2":
                        Console.Clear();

                        break;
                    case "3":
                        Console.Clear();

                        break;
                    case "4":
                        Console.Clear();
                        PalletManagment.RetrievePallet(211);
                        break;
                    case "5":
                        Console.Clear();
                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("***************************************************************************************************");
                        Console.WriteLine(" Thank you very much for using my program, which I spent 20 hours on, and I learned incredibly much.");
                        Console.WriteLine("*************************************************************************************************** \n\n\n\n\n\n");
                        menu = false;
                        break;
                    default:        //default statement if the user enters an invalid choice 
                        Console.Clear();
                        Console.WriteLine("Invalid choice. Try again.");
                        continue;
                }

            }
        }
        private static string PalletType()
        {
            Console.WriteLine("Choose a Pallet Type:");
            Console.WriteLine("1) Whole (Hel) pallet");
            Console.WriteLine("2) Half (Halv) pallet");

            string palletTypeChoice = Console.ReadLine();

            switch (palletTypeChoice)
            {
                case "1":
                    return "Hel";

                case "2":
                    return "Halv";

                default:
                    Console.WriteLine("Invalid choice for pallet type. Defaulting to Hel (Whole) pallet.");
                    return "Hel";
            }
        }
    }
}
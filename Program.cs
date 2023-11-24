using System;
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
            bool menu = true;

            while (menu)
            {
                //min meny vissar vad kan vi göra på lagert 
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("** Stroage menu **");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("******************************************");
                Console.WriteLine("1) Inlämning av pallar ");
                Console.WriteLine("2) Utlämning av pallar ");
                Console.WriteLine("3) Flyttning av pall ");
                Console.WriteLine("4) Sökning efter pallar");
                Console.WriteLine("5) Utskrift av lagerplatser ");
                Console.WriteLine("6) Avsluta programmet ");

                Console.WriteLine("******************************************");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Välj ett av alternativen , Ange ett nummer :");

                // switch sats kontrolerar vilken alternativ användaren valde 
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();

                        break;
                    case "2":
                        Console.Clear();

                        break;
                    case "3":
                        Console.Clear();

                        break;
                    case "4":
                        Console.Clear();

                        break;
                    case "5":
                        Console.Clear();

                        break;

                    case "6":
                        Console.Clear();
                        Console.WriteLine("*********************************************************************************************************");
                        Console.WriteLine(" Tack så mycket för att använd min program som jag slöaste 20 timmar på och fick lära mig otroligt mycket");
                        Console.WriteLine("********************************************************************************************************* \n\n\n\n\n\n");
                        menu = false;
                        break;
                    default:        //default sats om användaren skiv in ogiltigt val 
                        Console.Clear();
                        Console.WriteLine("Ogiltigt val. Försök igen.");
                        continue;
                }
            }
        }
    }
}
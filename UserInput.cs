using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLL_Grupp_6
{
    internal class UserInput
    {
        public static string String(string outMessage, ref string stringVariable)
        {
            try
            {
                Console.WriteLine(outMessage);
                stringVariable = (Console.ReadLine().ToUpper());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return stringVariable;
        }
        public static int Int(string outMessage, ref int intVariable)
        {
            try
            {
                Console.WriteLine(outMessage);
                intVariable = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return intVariable;
        }
    }
}

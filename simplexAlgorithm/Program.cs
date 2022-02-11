using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of variables");
            int variableNum = int.Parse(Console.ReadLine());

            for (int i = 0; i < variableNum; i++)
            {
                Console.WriteLine("Enter the start of a constraint in the form [int]x + [int]y + int[z]");
                string constraint = Console.ReadLine();
                Console.WriteLine("Is the constraint <= or >= the constant? Enter 0 for <= and 1 for >=");
                bool lessThanOrGreater = Console.ReadLine() == "0" ? false : true;
                Console.WriteLine("Enter the constant term");
                int constant = int.Parse(Console.ReadLine());
            }
        }
    }
}

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
            Console.WriteLine("Enter the number of constraints");
            int constraintsNum = int.Parse(Console.ReadLine());

            int height = constraintsNum + 1;
            int width = constraintsNum + variableNum + 1;

            decimal[,] initialTableau = new decimal[height, width];

            for (int i = 0; i < constraintsNum; i++)
            {
                Console.WriteLine("Enter the start of a constraint in the form x,y,z");
                string constraint = Console.ReadLine();

                string[] constraintArray = constraint.Split(',');
                for (int j = 0; j < variableNum; j++)
                {
                    initialTableau[i, j] = int.Parse(constraintArray[j]);
                }
                

                // I have not yet implemented >= constraints
                Console.WriteLine("Is the constraint <= or >= the constant? Enter 0 for <= and 1 for >=");               
                bool lessThanOrGreater = Console.ReadLine() == "0" ? false : true;
                initialTableau[i, i + variableNum + 1] = 1; 

                Console.WriteLine("Enter the constant term");
                int constant = int.Parse(Console.ReadLine());
                initialTableau[i, constraintsNum] = constant;

            }

            Console.WriteLine("Enter the objective function in the form (p = ) x,y,z,c where c is the constant term");
            string objective = Console.ReadLine();
            Console.WriteLine("Should it be maximized [max] or minimized [min]?");
            bool maximized = Console.ReadLine() == "max" ? true : false;

            string[] objectiveArray = objective.Split(',');          

            for (int i = 0; i < height; i++)
            {
                initialTableau[height - 1, i] = decimal.Parse(objectiveArray[i]);
                if (maximized)
                {
                    // We have the negative already, so if it is maximised we invert it.
                    initialTableau[height - 1, i] *= -1;
                }
            }

            initialTableau[height - 1, constraintsNum] *= -1;
            // Setting the value of the profit row to it's negative, this is equivalent to rearranging the P = x + y + z equation

            DisplayTable(initialTableau);

            // Initial tableau complete!

            // Select pivot column (most negative value in objective row)
            int pivotCol = -1;
            decimal min = 0;
            for (int i = 0; i < width; i++)
            {
                if (initialTableau[height - 1 , i] < min)
                {
                    min = initialTableau[height - 1, i];
                    pivotCol = i;
                }
            }

            // Select pivot row (using least positive theta values)
            int pivotRow = -1;
            decimal min2 = int.MaxValue;
            // Do not check the objective row
            for (int i = 0; i < height - 1; i++)
            {
                decimal thetaVal = initialTableau[i, constraintsNum] / initialTableau[i, pivotCol];
                if (thetaVal < min2 && thetaVal > 0)
                {
                    min2 = thetaVal;
                    pivotRow = i;
                }
            }

            decimal pivot = initialTableau[pivotCol, pivotRow];
            Console.WriteLine($"pivot = {pivot}");

            // Create another tableau using the pivot
            decimal[,] newTableau = new decimal[height, width];

            // Divide the pivot row by the pivot
            for (int i = 0; i < width; i++)
            {
                newTableau[pivotRow, i] = initialTableau[pivotRow, i] / pivot;
            }

            // Subtract some multiple of the pivot row from each other row so that the value in the pivot column is 0.
            for (int i = 0; i < height; i++)
            {
                if (i != pivotRow)
                {
                    decimal mult = initialTableau[pivotCol, i];
                    for (int j = 0; j < width; j++)
                    {
                        newTableau[i, j] = initialTableau[i, j] - mult * newTableau[pivotRow, j];
                    }
                }
            }

            Console.WriteLine();
            DisplayTable(newTableau);

            Console.ReadLine();
        }

        private static void DisplayTable(decimal[,] table)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                string line = "";
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    line += table[i, j] + "|";
                }
                Console.WriteLine(line);
            }
        }
    }
}

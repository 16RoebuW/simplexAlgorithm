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
            RegularSimplex();
            Console.ReadLine();
        }

        private static void DisplayTable(decimal[,] table)
        {
            // Find out the widest number in each column
            int[] widths = new int[table.GetLength(1)];
            for (int i = 0; i < table.GetLength(1); i++)
            {
                int max = 0;
                for (int j = 0; j < table.GetLength(0); j++)
                {
                    if (table[j, i].ToString().Length > max)
                    {
                        max = Math.Round(table[j, i], 2).ToString().Length;
                    }
                }
                widths[i] = max;
            }


            // Display the table
            for (int i = 0; i < table.GetLength(0); i++)
            {
                string line = "";
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    int previousLength = line.Length;
                    line += Math.Round(table[i, j], 2);
                    while (line.Length < previousLength + widths[j])
                    {
                        line += " ";
                    }
                    line += "|";
                }
                Console.WriteLine(line);
            }
        }

        private static void RegularSimplex()
        {
            Console.WriteLine("Enter the number of variables");
            int variableNum = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the number of constraints");
            int constraintsNum = int.Parse(Console.ReadLine());

            char[] variables = new char[variableNum + constraintsNum];
            for (int i = 0; i < variableNum; i++)
            {
                variables[i] = (char)(120 + i);
                // Add a letter (starting from x) to the array for each variable
            }
            for (int i = 0; i < constraintsNum; i++)
            {
                variables[i + variableNum] = (char)(115 + i);
                // Add a letter (starting from s) to the array for each slack variable/constraint
            }

            char[] basicVariables = new char[constraintsNum];

            int height = constraintsNum + 1;
            int width = constraintsNum + variableNum + 1;

            decimal[,] initialTableau = new decimal[height, width];


            Console.WriteLine("Constraints must be entered in their simplest forms");

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
                //Console.WriteLine("Is the constraint <= or >= the constant? Enter 0 for <= and 1 for >=");
                //bool lessThanOrGreater = Console.ReadLine() == "0" ? false : true;
                initialTableau[i, i + variableNum + 1] = 1;

                Console.WriteLine("Enter the constant term");
                int constant = int.Parse(Console.ReadLine());
                initialTableau[i, variableNum] = constant;

                basicVariables[i] = (char)(115 + i);
                // starts from s
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

            initialTableau[height - 1, variableNum] *= -1;
            // Setting the value of the profit row to it's negative, this is equivalent to rearranging the P = x + y + z equation

            Console.WriteLine();
            DisplayTable(initialTableau);

            // Initial tableau complete!

            bool solutionFound = false;

            while (!solutionFound)
            {
                // Select pivot column (most negative value in objective row)
                int pivotCol = -1;
                decimal min = 0;
                for (int i = 0; i < width; i++)
                {
                    if (initialTableau[height - 1, i] < min)
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
                    decimal thetaVal = -1;
                    if (initialTableau[i, pivotCol] != 0)
                    {
                        thetaVal = initialTableau[i, variableNum] / initialTableau[i, pivotCol];
                    }
                    if (thetaVal < min2 && thetaVal > 0)
                    {
                        min2 = thetaVal;
                        pivotRow = i;
                    }
                }

                decimal pivot = initialTableau[pivotRow, pivotCol];
                Console.WriteLine($"pivot = {pivot}");

                basicVariables[pivotRow] = variables[pivotCol];

                // Create another tableau using the pivot
                //decimal[,] newTableau = new decimal[height, width];

                // Divide the pivot row by the pivot
                for (int i = 0; i < width; i++)
                {
                    initialTableau[pivotRow, i] = initialTableau[pivotRow, i] / pivot;
                }

                // Subtract some multiple of the pivot row from each other row so that the value in the pivot column is 0.
                for (int i = 0; i < height; i++)
                {
                    if (i != pivotRow)
                    {
                        decimal mult = initialTableau[i, pivotCol];
                        for (int j = 0; j < width; j++)
                        {
                            initialTableau[i, j] = initialTableau[i, j] - mult * initialTableau[pivotRow, j];
                        }
                    }
                }

                Console.WriteLine();
                DisplayTable(initialTableau);

                // Check if there are negative values in the objective row
                solutionFound = true;
                for (int i = 0; i < width; i++)
                {
                    if (initialTableau[height - 1, i] < 0)
                    {
                        solutionFound = false;
                    }
                }

                // If there are, repeat
            }

            // Output
            Console.WriteLine();
            for (int i = 0; i < variableNum; i++)
            {
                if (basicVariables.Contains(variables[i]))
                {
                    Console.WriteLine($"{variables[i]} = {initialTableau[Array.IndexOf(basicVariables, variables[i]), variableNum]}");
                }
                else
                {
                    Console.WriteLine($"{variables[i]} = 0");
                }
            }
            Console.WriteLine($"Objective value = {initialTableau[height - 1, variableNum]}");
        }

        private static void BigM()
        {
            int lessThanConstraints = 0;
            int greaterThanConstraints = 0;
            int variableNum;

            Console.WriteLine("Enter the number of variables");
            variableNum = int.Parse(Console.ReadLine());

            List<string> unparsedConstraints = new List<string>();
            bool entryDone = false;

            while (!entryDone)
            {
                Console.WriteLine("Enter the start of a constraint in the form x,y,z");
                unparsedConstraints.Add(Console.ReadLine());

                Console.WriteLine("Is the constraint <= or >= the constant? Enter 0 for <= and 1 for >=");
                unparsedConstraints.Add(Console.ReadLine());
                if (Console.ReadLine() == "1")
                {
                    greaterThanConstraints++; 
                }
                else
                {
                    lessThanConstraints++;
                }

                //bool lessThanOrGreater = Console.ReadLine() == "0" ? false : true;

                Console.WriteLine("Enter the constant term");
                unparsedConstraints.Add(Console.ReadLine());

                Console.WriteLine("Have all of the constraints been entered? Y/N");
                entryDone = Console.ReadLine() == "Y" ? true : false;
            }

            Console.WriteLine("Enter the objective function in the form (p = ) x,y,z,c where c is the constant term");
            string objective = Console.ReadLine();

            int height = lessThanConstraints + greaterThanConstraints + 1;
            int width = variableNum + lessThanConstraints + greaterThanConstraints + greaterThanConstraints + 1;
        }
    }
}

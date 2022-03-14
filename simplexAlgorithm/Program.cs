using System;
using System.Collections.Generic;
using System.Linq;

namespace simplexAlgorithm
{
    class Program
    {
        static int precision = 4;
        // The precision in decimal places of outputs

        static void Main(string[] args)
        {
            BigM();
            //RegularSimplex();
            Console.ReadLine();
        }

        private static void DisplayTable(decimal[,] table)
        {
            // Find the widest number in each column
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
                    // Do not check the value column
                    if (initialTableau[height - 1, i] < min && i != variableNum)
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
                Console.WriteLine($"pivot = {Math.Round(pivot, precision)}");

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
                    if (i != variableNum && initialTableau[height - 1, i] < 0)
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
                    Console.WriteLine($"{variables[i]} = {Math.Round(initialTableau[Array.IndexOf(basicVariables, variables[i]), variableNum], precision)}");
                }
                else
                {
                    Console.WriteLine($"{variables[i]} = 0");
                }
            }
            if (maximized)
            {
                Console.WriteLine($"Objective value = {Math.Round(initialTableau[height - 1, variableNum], precision)}");
            }
            else
            {
                Console.WriteLine($"Objective value = {-Math.Round(initialTableau[height - 1, variableNum], precision)}");
            }
        }

        private static void BigM()
        {
            int lessThanConstraints = 0;
            int greaterThanConstraints = 0;
            int variableNum;

            List<string> variables = new List<string>();
            List<string> basicVariables = new List<string>();

            Console.WriteLine("Enter the number of variables");
            variableNum = int.Parse(Console.ReadLine());
            for (int i = 0; i < variableNum; i++)
            {
                variables.Add(((char)(120 + i)).ToString());
                // Add a letter (starting from x) to the array for each variable
            }
            variables.Add("value");

            List<string> unparsedConstraints = new List<string>();
            bool entryDone = false;

            while (!entryDone)
            {
                Console.WriteLine("Enter the start of a constraint in the form x,y,z");
                unparsedConstraints.Add(Console.ReadLine());

                Console.WriteLine("Is the constraint <= or >= the constant? Enter 0 for <= and 1 for >=");
                unparsedConstraints.Add(Console.ReadLine());
                if (unparsedConstraints.Last() == "1")
                {
                    // Surplus variables are represented as s0,s1,s2...
                    variables.Add("s" + greaterThanConstraints);
                    basicVariables.Add("s" + greaterThanConstraints);

                    // Artificial variables are represented as a0,a1,a2...
                    variables.Add("a" + greaterThanConstraints);

                    greaterThanConstraints++;
                }
                else
                {
                    variables.Add(((char)(115 + lessThanConstraints)).ToString());
                    basicVariables.Add(((char)(115 + lessThanConstraints)).ToString());
                    // Add a letter (starting from s) to the array for each slack variable/constraint

                    lessThanConstraints++;
                }

                //bool lessThanOrGreater = Console.ReadLine() == "0" ? false : true;

                Console.WriteLine("Enter the constant term");
                unparsedConstraints.Add(Console.ReadLine());

                Console.WriteLine("Have all of the constraints been entered? Y/N");
                entryDone = Console.ReadLine() == "Y" ? true : false;
            }


            int height = lessThanConstraints + greaterThanConstraints + 1;
            int width = variableNum + lessThanConstraints + greaterThanConstraints + greaterThanConstraints + 1;

            Console.WriteLine("Enter the objective function in the form (p = ) x,y,z,c where c is the constant term");
            string objective = Console.ReadLine();
            Console.WriteLine("Should it be maximized [max] or minimized [min]?");
            bool maximized = Console.ReadLine() == "max" ? true : false;

            decimal[,] initialTableau = new decimal[height, width];

            decimal[,] artificialVariables = new decimal[greaterThanConstraints, width];
            int artiVIndex = 0;

            for (int i = 0; i < unparsedConstraints.Count / 3; i++)
            {
                string[] constraintArray = unparsedConstraints[3 * i].Split(',');
                for (int j = 0; j < variableNum; j++)
                {
                    initialTableau[i, j] = int.Parse(constraintArray[j]);
                }
                initialTableau[i, variableNum] = int.Parse(unparsedConstraints[(3 * i) + 2]);

                if (unparsedConstraints[(3 * i) + 1] == "0")
                {
                    initialTableau[i, i + variableNum + artiVIndex + 1] = 1;
                }
                else
                {
                    // This will add the values for surplus and artificial variables to the table, however columns will be in the order they were entered.
                    // E.g x, y, z, Value, s, t, s1, a1, u, v...
                    initialTableau[i, i + variableNum + artiVIndex + 1] = -1;
                    initialTableau[i, i + variableNum + artiVIndex + 2] = 1;

                    // Rearrange the artificial variables
                    for (int j = 0; j < constraintArray.Length; j++)
                    {
                        artificialVariables[artiVIndex, j] = -Convert.ToDecimal(constraintArray[j]);
                    }
                    // Add the value to the artificial variable
                    artificialVariables[artiVIndex, variableNum] = Convert.ToDecimal(unparsedConstraints[(3 * i) + 2]);

                    // Add the surplus variable to the artificial variable
                    artificialVariables[artiVIndex, i + variableNum + artiVIndex + 1] = 1;

                    artiVIndex++;
                }
            }

            string[] objectiveArray = objective.Split(',');

            for (int i = 0; i < variableNum + 1; i++)
            {
                initialTableau[height - 1, i] = decimal.Parse(objectiveArray[i]);
            }

            // Subtracting M(sum of artificial variables) from the objective row
            // M is int.Max, 2^31 - 1
            // Decimals can store values up to 2^96 (kinda)

            for (int i = 0; i < greaterThanConstraints; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (maximized)
                    {
                        initialTableau[height - 1, j] -= int.MaxValue * artificialVariables[i, j];
                    }
                    else
                    {
                        initialTableau[height - 1, j] += int.MaxValue * artificialVariables[i, j];
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                if (maximized)
                {
                    // We have the negative already, so if it is maximised we invert it.
                    initialTableau[height - 1, i] *= -1;
                }
            }
            initialTableau[height - 1, variableNum] *= -1;
            // Setting the value of the profit row to it's negative, this is equivalent to rearranging the P = x + y + z equation


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
                    // Do not check the value column
                    if (initialTableau[height - 1, i] < min && i != variableNum)
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
                Console.WriteLine($"pivot = {Math.Round(pivot, precision)}");

                basicVariables[pivotRow] = variables[pivotCol];

                // Create another tableau using the pivot

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
                    if ((i != variableNum && initialTableau[height - 1, i] < 0) || basicVariables.Contains("a" + i))
                    {
                        solutionFound = false;
                    }
                }

                // If there are, repeat
            }

            // Output
            for (int i = 0; i < variableNum; i++)
            {
                if (basicVariables.Contains(variables[i]))
                {
                    Console.WriteLine($"{variables[i]} = {Math.Round(initialTableau[basicVariables.IndexOf(variables[i]), variableNum], precision)}");
                }
                else
                {
                    Console.WriteLine($"{variables[i]} = 0");
                }
            }
            if (maximized)
            {
                Console.WriteLine($"Objective value = {Math.Round(initialTableau[height - 1, variableNum], precision)}");
            }
            else
            {
                Console.WriteLine($"Objective value = {-Math.Round(initialTableau[height - 1, variableNum], precision)}");
            }
        }
    }
}
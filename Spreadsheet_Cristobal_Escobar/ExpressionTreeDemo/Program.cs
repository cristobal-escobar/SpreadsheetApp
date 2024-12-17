namespace ExpressionTreeCodeDemo
{
    using System;
    using SpreadsheetEngine;

    public class Program
    {
        public static void Main()
        {
            // init tree and current expresion and other vars
            string curExpr = "A1+B1+C1", choice, newVarName, newVarValue;
            int varValue;

            // ExpressionTree tree = new ExpressionTree(curExpr);
            while (true)
            {
                // write menu options
                Console.WriteLine("Menu (current expression = \"" + curExpr + "\")");
                Console.WriteLine("1. Enter a new expression");
                Console.WriteLine("2. Set a variable value");
                Console.WriteLine("3. Evaluate tree");
                Console.WriteLine("4. Quit");

                // get choice number
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // updates expression

                        Console.WriteLine("Enter new expression");
                        curExpr = Console.ReadLine();

                        // tree = new ExpressionTree(curExpr);
                        break;
                    case "2": // Adds a variable name and value to the tree dictionary

                        Console.WriteLine("Enter a variable name: ");
                        newVarName = Console.ReadLine();

                        Console.WriteLine("Enter a variable value: ");
                        newVarValue = Console.ReadLine();
                        varValue = int.Parse(newVarValue);

                        // tree.SetVariable(newVarName, varValue);
                        break;
                    case "3": // Evaluates current expression

                        // Console.WriteLine(tree.Evaluate());
                        break;
                    case "4": // exits

                        Console.WriteLine("Done");
                        return;
                }
            }
        }
    }
}
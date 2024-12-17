namespace SpreadsheetEngine
{
    using System.Text;
    using System.Xml.Linq;

    internal class ExpressionTree
    {
#pragma warning disable SA1401 // Fields should be private
        internal NodeFactory NodeCreator;
#pragma warning restore SA1401 // Fields should be private

        private Node root;
        private string expr;

        // private Dictionary<char, Type> operators;
        private Dictionary<char, int> operators;

        public ExpressionTree(string expr)
        {
            this.operators = new Dictionary<char, int>() { { '+', 1 }, { '-', 1 }, { '*', 2 }, { '/', 2 } };

            // this.operators = new Dictionary<char, Type>();
            this.NodeCreator = new NodeFactory();
            this.expr = this.ShuntingYard(expr);
            this.root = this.ConstructTree(this.expr);
        }

        /// <summary>
        /// given a variable and a value push variable as key and value as value in variables dict
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="variableValue"></param>
        public void SetVariable(string variableName, double variableValue)
        {
            if (variableName != string.Empty)
            {
                this.NodeCreator.SetVariable(variableName, variableValue);
            }
        }

        /// <summary>
        /// calls node factory method to remove variable
        /// </summary>
        /// <param name="name"></param>
        public void RemoveVariable(string name)
        {
            this.NodeCreator.RemoveVariable(name);
        }

        /// <summary>
        /// given a variable name retuns a VariableNode type containing variable if exists
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public Node GetNode(string variableName)
        {
            if (variableName != string.Empty)
            {
                return this.NodeCreator.GetVariable(variableName);
            }

            return null;
        }

        /// <summary>
        /// evaluates expr
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            return this.Evaluate(this.root);
        }

        public void ChangeExpr(string newExpr)
        {
            this.expr = this.ShuntingYard(newExpr);
            this.root = this.ConstructTree(this.expr);
        }

        public string GetShuntingYard(string newExpr)
        {
            return this.ShuntingYard(newExpr);
        }

        /// <summary>
        /// given and expression returns a postfix expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private string ShuntingYard(string str)
        {
            // adding spaces before and after operators or parenthesis
            string newStr = str;
            for (int i = 0; i < str.Length; i++)
            {
                if (this.operators.ContainsKey(str[i]))
                {
                    newStr = newStr.Replace($"{str[i]}", $" {str[i]} ");
                }
            }

            newStr = newStr.Replace("(", " ( ");
            newStr = newStr.Replace(")", " ) ");

            // creates a list of strings and inits the stack and a string builder
            string[] expr = newStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Stack<string> stack = new Stack<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < expr.Length; i++) // go trough each symbol in expr
            {
                if (expr[i] == "(") // if left parenthesis push it to stack
                {
                    stack.Push(expr[i]);
                }
                else if (expr[i] == ")") // if right parenthesis pop and print all symbols til right parenthesis
                {
                    while (true)
                    {
                        string symbol = stack.Pop();
                        if (symbol == "(")
                        {
                            break;
                        }

                        // Console.WriteLine(symbol);
                        sb.Append(symbol);
                        sb.Append(" ");
                        try
                        {
                            if (stack.Count() == 0)
                            {
                                throw new Exception("Invalid parenthesis!");
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Error: {exception.Message}");
                            return " ";
                        }
                    }
                }
                else if (this.operators.ContainsKey(expr[i][0])) // if operator
                {
                    if (stack.Count() != 0) // if not stack not empty
                    {
                        string top = stack.Pop();
                        if (top == "(") // if stack.pop() == '(' push operator
                        {
                            stack.Push(top);
                            stack.Push(expr[i]);
                        }
                        else
                        {
                            if (this.operators[expr[i][0]] >= this.operators[top[0]]) // if str[i] >= top is right associative
                            {
                                stack.Push(top);
                                stack.Push(expr[i]);
                            }
                            else if (this.operators[expr[i][0]] <= this.operators[top[0]]) // if str[i] <= top is left associative
                            {
                                while (true)
                                {
                                    // Console.WriteLine(top);
                                    sb.Append(top);
                                    sb.Append(" ");
                                    if (stack.Count() == 0 || this.operators[expr[i][0]] > this.operators[top[0]])
                                    {
                                        stack.Push(expr[i]);
                                        break;
                                    }

                                    top = stack.Pop();
                                }
                            }
                        }
                    }
                    else
                    {
                        stack.Push(expr[i]);
                    }
                }
                else // if operand
                {
                    // Console.WriteLine(expr[i]);
                    sb.Append(expr[i]);
                    sb.Append(" ");
                }
            }

            try
            {
                if (stack.Contains("(")) // if closing parenthesis was not found
                {
                    throw new Exception("Invalid parenthesis!");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
                return " ";
            }

            while (stack.Count() > 0) // pop all operators in stack
            {
                string top = stack.Pop();

                // Console.WriteLine(top);
                sb.Append(top);
                sb.Append(" ");
            }

            return sb.ToString().TrimEnd(); // return string
        }

        /// <summary>
        /// given a postfix string construct the tree and returns root node
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private Node ConstructTree(string expr)
        {
            if (expr == string.Empty)
            {
                return null;
            }

            Stack<Node> stack = new Stack<Node>();
            string[] str = expr.Split(' ');
            double constValue;
            for (int i = 0; i < str.Length; i++) // iterates trough each symbols in string
            {
                if (double.TryParse(str[i], out constValue))
                { // if operand push node to the stack
                    Node node = this.NodeCreator.CreateConstant(constValue);
                    stack.Push(node);
                }
                else if (this.operators.ContainsKey(str[i][0])) // if operator pop last 2 nodes from stack
                {
                    if (stack.Count() >= 2)
                    {
                        Node node = this.NodeCreator.CreateOperator(str[i][0], stack.Pop(), stack.Pop());
                        stack.Push(node);
                    }
                    else
                    {
                        return null;
                    }
                }
                else // if variable push it to the stack
                {
                    // check if variable name is valid
                    int row;
                    try
                    {
                        if (int.TryParse(str[i].Substring(1), out row) && (row >= 1 && row <= 50))
                        {
                            try
                            {
                                if (char.IsUpper(str[i][0]))
                                {
                                    Node node = this.NodeCreator.CreateVariable(str[i]);
                                    stack.Push(node);
                                }
                                else
                                {
                                    throw new Exception($"Invalid column name {str[i][0]}!");
                                }
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine($"Error: {exception.Message}");
                                return null;
                            }
                        }
                        else
                        {
                            throw new Exception($"Invalid row number {str[i].Substring(1)}!");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Error: {exception.Message}");
                        return null;
                    }
                }
            }

            return stack.Pop();
        }

        /// <summary>
        /// evaluates given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private double Evaluate(Node node)
        {
            if (node != null)
            {
                return node.Evaluate();
            }

            return 0.0;
        }
    }
}

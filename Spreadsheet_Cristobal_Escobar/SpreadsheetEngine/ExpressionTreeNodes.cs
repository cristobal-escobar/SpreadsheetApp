namespace SpreadsheetEngine
{
    using System.Reflection;

    public abstract class Node
    {
        /// <summary>
        /// evaluates node contents
        /// </summary>
        /// <returns></returns>
        public virtual double Evaluate()
        {
            return -1;
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class NodeFactory
#pragma warning restore SA1402 // File may only contain a single type
    {
        private static Dictionary<string, double> variables = new Dictionary<string, double>();
        private Dictionary<char, Type> operators;

        public NodeFactory()
        {
            // contructor
            this.operators = new Dictionary<char, Type>();
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// Creates an operator node and returns it
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public OperatorNode CreateOperatorNode(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                object operatorNodeObject = System.Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }
            }

            return null;
        }

        /// <summary>
        /// creates a ConstantNode and returns it
        /// </summary>
        /// <returns></returns>
        public Node CreateConstant(double newValue)
        {
            return new ConstantNode(newValue);
        }

        /// <summary>
        /// creates a VariableNode and returns it
        /// </summary>
        /// <returns></returns>
        public Node CreateVariable(string newVariable)
        {
            return new VariableNode(newVariable);
        }

        /// <summary>
        /// creates an OperatorNode and returns it
        /// </summary>
        /// <returns></returns>
        public Node CreateOperator(char c, Node right, Node left)
        {
            OperatorNode node = this.CreateOperatorNode(c);
            node.Left = left;
            node.Right = right;
            return node;
        }

        /// <summary>
        ///  Returns variableNode containing variable as name if exists
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Node GetVariable(string variable)
        {
            if (variable != null)
            {
                try
                {
                    if (variables.ContainsKey(variable))
                    {
                        return new VariableNode(variable);
                    }
                    else
                    {
                        throw new Exception($"Variable {variable} was not found!");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error: {exception.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// given a variable and a value sets variable as key and value as value in dict variables
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="varValue"></param>
        public void SetVariable(string variable, double varValue)
        {
            if (variable != null)
            {
                variables[variable] = varValue;
            }
        }

        /// <summary>
        /// removes a variable if it exits
        /// </summary>
        /// <param name="name"></param>
        public void RemoveVariable(string name)
        {
            if (variables.ContainsKey(name))
            {
                variables.Remove(name);
            }
        }

        // function to init delegate
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            Type operatorNodeType = typeof(OperatorNode);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> operatorTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                foreach (var type in operatorTypes)
                {
                    PropertyInfo operatorField = type.GetProperty("Operator");
                    if (operatorField != null)
                    {
                        object value = operatorField.GetValue(Activator.CreateInstance(type));

                        if (value is char)
                        {
                            char operatorSymbol = (char)value;
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }

        public class ConstantNode : Node
        {
            public ConstantNode(double value)
            {
                this.Value = value;
            }

            public double Value { get; set; }

            /// <summary>
            /// returns ConstantNode value
            /// </summary>
            /// <returns></returns>
            public override double Evaluate()
            {
                return this.Value;
            }
        }

        public class VariableNode : Node
        {
            public VariableNode(string name)
            {
                if (name != null)
                {
                    this.Name = name;
                }
            }

            public string Name { get; set; }

            /// <summary>
            /// evaluates variable
            /// </summary>
            /// <returns></returns>
            public override double Evaluate()
            {
                if (this.Name != null)
                {
                    try
                    {
                        if (variables.ContainsKey(this.Name))
                        {
                            return variables[this.Name];
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Variable {this.Name} was not found!");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Error: {exception.Message}");
                    }
                }

                return 0;
            }
        }

        public abstract class OperatorNode : Node
        {
            public OperatorNode()
            {
                this.Left = null;
                this.Right = null;
            }

            public char Operator { get; set; }

            public int Precedence { get; set; }

            public string Associativity { get; set; }

            public Node Left { get; set; }

            public Node Right { get; set; }

            public void AddChilds(Node left, Node right)
            {
                this.Left = left;
                this.Right = right;
            }
        }

        public class SumOperator : OperatorNode
        {
            public SumOperator()
            {
                this.Operator = '+';
                this.Precedence = 2;
                this.Associativity = "left";
            }

            public override double Evaluate()
            {
                if (this.Left != null && this.Right != null)
                {
                    return this.Left.Evaluate() + this.Right.Evaluate();
                }

                return 0;
            }
        }

        private class SubOperator : OperatorNode
        {
            public SubOperator()
            {
                this.Operator = '-';
                this.Precedence = 2;
                this.Associativity = "left";
            }

            public override double Evaluate()
            {
                if (this.Left != null && this.Right != null)
                {
                    return this.Left.Evaluate() - this.Right.Evaluate();
                }

                return 0;
            }
        }

        private class MulOperator : OperatorNode
        {
            public MulOperator()
            {
                this.Operator = '*';
                this.Precedence = 1;
                this.Associativity = "left";
            }

            public override double Evaluate()
            {
                if (this.Left != null && this.Right != null)
                {
                    return this.Left.Evaluate() * this.Right.Evaluate();
                }

                return 0;
            }
        }

        private class DivOperator : OperatorNode
        {
            public DivOperator()
            {
                this.Operator = '/';
                this.Precedence = 1;
                this.Associativity = "left";
            }

            public override double Evaluate()
            {
                if (this.Left != null && this.Right != null)
                {
                    try
                    {
                        double denominator = this.Right.Evaluate();
                        if (denominator == 0)
                        {
                            throw new Exception("Division by 0");
                        }

                        return this.Left.Evaluate() / denominator;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Error: {exception.Message}");
                    }
                }

                return 0;
            }
        }
    }
}

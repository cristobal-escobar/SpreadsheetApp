namespace SpreadsheetTests
{
    using SpreadsheetEngine;

    public class ExpressionTreeNodesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// normal case for method evaluate() in ConstantNode class and CreateConstant in NodeCreator
        /// </summary>
        [Test]
        public void TestConstantNodeEvaluateMethodNormalCase()
        {
            // init some vars
            ExpressionTree tree = new ExpressionTree("A2+3");
            Node node = tree.NodeCreator.CreateConstant(9);

            // test if results are the expected
            Assert.AreEqual(9, node.Evaluate());
        }

        /// <summary>
        /// normal case for method evaluate() in VariableNode class and CreateVariable in NodeCreator
        /// </summary>
        [Test]
        public void TestVariableNodeEvaluateMethodNormalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            Node node = expr.NodeCreator.CreateVariable("A2");

            expr.SetVariable("A2", 7);

            // test if results are the expected
            Assert.AreEqual(7, node.Evaluate());
        }

        /// <summary>
        /// edge case for method evaluate() in VariableNode class and CreateVariable in NodeCreator
        /// </summary>
        [Test]
        public void TestVariableNodeEvaluateMethodEdgeCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            Node node = expr.NodeCreator.CreateVariable(string.Empty);
            expr.SetVariable("A2", 7);

            // test if results are the expected
            Assert.AreEqual(0, node.Evaluate());
        }

        /// <summary>
        /// execptional case for method evaluate() in VariableNode class and CreateVariable in NodeCreator
        /// </summary>
        [Test]
        public void TestVariableNodeEvaluateMethodExecptionalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            Node node = expr.NodeCreator.CreateVariable(null);
            expr.SetVariable("A2", 7);

            // test if results are the expected
            Assert.AreEqual(0, node.Evaluate());
        }

        /// <summary>
        /// normal case for method evaluate() in OperatorNode class and CreateOperator in NodeCreator
        /// </summary>
        [Test]
        public void TestOperationNodeEvaluateMethodNormalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");

            Node right = expr.NodeCreator.CreateConstant(3), left = expr.NodeCreator.CreateConstant(5);
            Node node = expr.NodeCreator.CreateOperator('*', right, left);

            // test if results are the expected
            Assert.AreEqual(15, node.Evaluate());
        }

        /// <summary>
        /// edge case for method evaluate() in OperatorNode class and CreateOperator in NodeCreator
        /// </summary>
        [Test]
        public void TestOperationNodeEvaluateMethodEdgeCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            expr.SetVariable("A2", 7);
            Node node = expr.NodeCreator.CreateOperator('+', null, null);

            // test if results are the expected
            Assert.AreEqual(0, node.Evaluate());
        }

        /// <summary>
        /// execptional case for method evaluate() in OperatorNode class and CreateOperator in NodeCreator
        /// </summary>
        [Test]
        public void TestOperationNodeEvaluateMethodExecptionalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            expr.SetVariable("A2", 7);
            Node right = expr.NodeCreator.CreateConstant(3);
            Node node = expr.NodeCreator.CreateOperator('/', right, null);

            // test if results are the expected
            Assert.AreEqual(0, node.Evaluate());
        }
    }
}
namespace SpreadsheetTests
{
    using System.Reflection;
    using SpreadsheetEngine;

    public class ExpressionTreeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// normal case for method SetVariable() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestSetvariableMethodNormalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            expr.SetVariable("A2", 7);

            // test if results are the expected
            Assert.NotNull(expr.GetNode("A2"));
        }

        /// <summary>
        /// edge case for method SetVariable() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestSetvariableMethodEdgeCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            expr.SetVariable(string.Empty, 7);

            // test if results are the expected
            Assert.AreEqual(null, expr.GetNode(string.Empty));
        }

        /// <summary>
        /// normal case for method GetVariable() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestGetvariableMethodNormalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");
            expr.SetVariable("C2", 10);

            // test if results are the expected
            Assert.AreEqual(10, expr.GetNode("C2").Evaluate());
        }

        /// <summary>
        /// edge case for method GetVariable() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestGetvariableMethodEdgeCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("A2+3");

            // test if results are the expected
            Assert.AreEqual(null, expr.GetNode("C3"));
        }

        /// <summary>
        /// normal case for method Evaluate() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestEvaluateMethodNormalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("7+3");

            // test if results are the expected
            Assert.AreEqual(10, expr.Evaluate());
        }

        /// <summary>
        /// edge case for method Evaluate() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestEvaluateMethodEdgeCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("8+");

            // test if results are the expected
            Assert.AreEqual(0.0, expr.Evaluate());
        }

        /// <summary>
        /// exceptional case for method Evaluate() in ExpressionTree class
        /// </summary>
        [Test]
        public void TestEvaluateMethodExceptionaalCase()
        {
            // init some vars
            ExpressionTree expr = new ExpressionTree("/");

            // test if results are the expected
            Assert.AreEqual(0.0, expr.Evaluate());
        }

        public class TestPrivateEvaluateMethod
        {
            private ExpressionTree objectUnderTest = new ExpressionTree("9+2");

            private MethodInfo GetMethod(string methodName)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    Assert.Fail(string.Empty);
                }

                var method = this.objectUnderTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                {
                    Assert.Fail(string.Format("{0} method not found", methodName));
                }

                return method;
            }

#pragma warning disable SA1202 // Elements should be ordered by access
            public void TestPrivateInstanceMethod()
#pragma warning restore SA1202 // Elements should be ordered by access
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("Evaluate");
                ExpressionTree tree = new ExpressionTree("A2+3");
                Node right = tree.NodeCreator.CreateConstant(9), left = tree.NodeCreator.CreateConstant(2);
                Node node = tree.NodeCreator.CreateOperator('+', right, left);

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual(11, methodInfo.Invoke(this.objectUnderTest, new object[] { node }));
            }
        }

        /// <summary>
        /// normal case for private method Evaluate() in ExpressionTree class
        /// </summary>
        [Test]
#pragma warning disable SA1201 // Elements should appear in the correct order
        public void TestPrivateEvaluateMethodNormalCase()
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
            TestPrivateEvaluateMethod test = new TestPrivateEvaluateMethod();
            test.TestPrivateInstanceMethod();
        }

        public class TestPrivateShuntingYardMethod
        {
            private ExpressionTree objectUnderTest = new ExpressionTree("A2*(B2+C2*D2)+E2");

            private MethodInfo GetMethod(string methodName)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    Assert.Fail(string.Empty);
                }

                var method = this.objectUnderTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                {
                    Assert.Fail(string.Format("{0} method not found", methodName));
                }

                return method;
            }

            // Still in class ClassToDemoTestingNonPublicTest
#pragma warning disable SA1202 // Elements should be ordered by access
            public void TestMethodNormalCase()
#pragma warning restore SA1202 // Elements should be ordered by access
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ShuntingYard");

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual("A2 B2 C2 D2 * + * E2 +", methodInfo.Invoke(this.objectUnderTest, new object[] { "A2*(B2+C2*D2)+E2" }));
            }

            public void TestMethodEdgeCase()
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ShuntingYard");

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual(" ", methodInfo.Invoke(this.objectUnderTest, new object[] { "A2*(B2+C2*D2" }));
            }

            public void TestMethodExceptionalCase()
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ShuntingYard");

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual(string.Empty, methodInfo.Invoke(this.objectUnderTest, new object[] { string.Empty }));
            }
        }

        [Test]
#pragma warning disable SA1201 // Elements should appear in the correct order
        public void TestParserNormalCase()
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
            TestPrivateShuntingYardMethod test = new TestPrivateShuntingYardMethod();
            test.TestMethodNormalCase();
        }

        [Test]
        public void TestParserEdgeCase()
        {
            TestPrivateShuntingYardMethod test = new TestPrivateShuntingYardMethod();
            test.TestMethodEdgeCase();
        }

        [Test]
        public void TestParserExceptionalCase()
        {
            TestPrivateShuntingYardMethod test = new TestPrivateShuntingYardMethod();
            test.TestMethodExceptionalCase();
        }

        public class TestPrivateConstructTreeMethod
        {
            private ExpressionTree objectUnderTest = new ExpressionTree("A2*(B2+C2*D2)+E2");

            private MethodInfo GetMethod(string methodName)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    Assert.Fail(string.Empty);
                }

                var method = this.objectUnderTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                {
                    Assert.Fail(string.Format("{0} method not found", methodName));
                }

                return method;
            }

            // Still in class ClassToDemoTestingNonPublicTest
#pragma warning disable SA1202 // Elements should be ordered by access
            public void TestMethodNormalCase()
#pragma warning restore SA1202 // Elements should be ordered by access
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ConstructTree");
                ExpressionTree tree = new ExpressionTree("A2+3");

                Node result = (Node)methodInfo.Invoke(this.objectUnderTest, new object[] { "1 4 7 2 * + * 9 +" });

                Assert.AreEqual(27, result.Evaluate());
            }

            public void TestMethodEdgeCase()
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ConstructTree");

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual(null, methodInfo.Invoke(this.objectUnderTest, new object[] { "A2 * ( B2 + C2 * D2" }));
            }

            public void TestMethodExceptionalCase()
            {
                // Retrieve the method that we want to test using reflection
                MethodInfo methodInfo = this.GetMethod("ConstructTree");

                // Test the method by calling the MethodBase.Invoke method
                Assert.AreEqual(null, methodInfo.Invoke(this.objectUnderTest, new object[] { string.Empty }));
            }
        }

        [Test]
#pragma warning disable SA1201 // Elements should appear in the correct order
        public void TestTreeNormalCase()
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
            TestPrivateConstructTreeMethod test = new TestPrivateConstructTreeMethod();
            test.TestMethodNormalCase();
        }

        [Test]
        public void TestTreeEdgeCase()
        {
            TestPrivateConstructTreeMethod test = new TestPrivateConstructTreeMethod();
            test.TestMethodEdgeCase();
        }

        [Test]
        public void TestTreeExceptionalCase()
        {
            TestPrivateConstructTreeMethod test = new TestPrivateConstructTreeMethod();
            test.TestMethodExceptionalCase();
        }

        /// <summary>
        /// normal case test for ChangeExpr() method
        /// </summary>
        [Test]
        public void TestChangeExprMethodNormalCase()
        {
            ExpressionTree tree = new ExpressionTree("2+3*(10/5)");
            tree.ChangeExpr("(4-2)*3");

            Assert.AreEqual(6, tree.Evaluate());
        }

        /// <summary>
        /// edge case test for ChangeExpr() method
        /// </summary>
        [Test]
        public void TestChangeExprMethodEdgeCase()
        {
            ExpressionTree tree = new ExpressionTree("2+3*(10/5)");
            tree.ChangeExpr("0");

            Assert.AreEqual(0, tree.Evaluate());
        }

        /// <summary>
        /// exceptional case test for ChangeExpr() method
        /// </summary>
        [Test]
        public void TestChangeExprMethodExceptionalCase()
        {
            ExpressionTree tree = new ExpressionTree("2+3*(10/5)");
            tree.ChangeExpr(string.Empty);

            Assert.AreEqual(0, tree.Evaluate());
        }
    }
}
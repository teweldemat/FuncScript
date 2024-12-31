using funcscript.core;
using funcscript.model;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static funcscript.core.FuncScriptParser;

namespace funcscript.test
{
    internal class ParseTreeTests
    {
        public static void AssertNoOverlappingSpans(ParseNode node)
        {
            int nodeStart = node.Pos;
            int nodeEnd = node.Pos + node.Length;

            // Make sure this node's children are within its own span
            foreach (var child in node.Children)
            {
                int childStart = child.Pos;
                int childEnd = child.Pos + child.Length;
                Assert.That(childStart >= nodeStart, 
                    $"Child start {childStart} is before the parent start {nodeStart}");
                Assert.That(childEnd <= nodeEnd, 
                    $"Child end {childEnd} is after the parent end {nodeEnd}");
            }

            // Make sure children don't overlap each other
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c1 = node.Children[i];
                int c1Start = c1.Pos;
                int c1End = c1.Pos + c1.Length;

                for (int j = i + 1; j < node.Children.Count; j++)
                {
                    var c2 = node.Children[j];
                    int c2Start = c2.Pos;
                    int c2End = c2.Pos + c2.Length;

                    Assert.That(!Overlaps(c1Start, c1End, c2Start, c2End),
                        $"ParseNode children overlap: [{c1Start},{c1End}) overlaps [{c2Start},{c2End})");
                }
            }

            // Recurse into children
            foreach (var child in node.Children)
            {
                AssertNoOverlappingSpans(child);
            }
        }

        private static bool Overlaps(int start1, int end1, int start2, int end2)
        {
            return (start1 < end2) && (start2 < end1);
        }
        static ParseNode Flatten(ParseNode node)
        {
            if (node.Children.Count == 1)
            {
                return node.Children[0];
            }

            if (node.Children.Count > 1)
            {
                for (int i = 0; i < node.Children.Count; i++)
                    node.Children[i] = Flatten(node.Children[i]);
            }

            return node;
        }

        [Test]
        public void PasreKvcTest()
        {
            var g = new DefaultFsDataProvider();
            var expText = "{a,b,c}";
            var list = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new ParseContext(g, expText, list);
            var (exp, node, _) = funcscript.core.FuncScriptParser.Parse(context);
            Assert.IsNotNull(exp);
            Assert.IsNotNull(node);
            node = Flatten(node);
            Assert.AreEqual(Tuple.Create(ParseNodeType.KeyValueCollection, 0, expText.Length),
                Tuple.Create(node.NodeType, node.Pos, node.Length));

        }

        [Test]
        public void SimpleInfixTokensParsedCorrectly()
        {
            // Arrange
            var expression = "5+6";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            // Act
            var (exp, parseNode, _) = funcscript.core.FuncScriptParser.Parse(context);

            // Assert
            Assert.AreEqual(0, syntaxErrors.Count, "Expected no syntax errors.");

            Assert.IsNotNull(parseNode, "ParseNode should not be null.");
            Assert.AreEqual(ParseNodeType.InfixExpression, parseNode.NodeType,
                "Expected NodeType to be InfixExpression.");
            Assert.AreEqual(3, parseNode.Children?.Count ?? 0, "Expected 3 children in the parse node.");
            Assert.AreEqual(0, parseNode.Pos, "Expected position to be 0.");
            Assert.AreEqual(3, parseNode.Length, "Expected length to be 3.");

            var oper1 = parseNode.Children[0];
            var op = parseNode.Children[1];
            var oper2 = parseNode.Children[2];

            Assert.AreEqual(ParseNodeType.LiteralInteger, oper1.NodeType,
                "Expected first child NodeType to be LiteralInteger.");
            Assert.AreEqual(0, oper1.Children?.Count ?? 0, "Expected first child to have no children.");
            Assert.AreEqual(0, oper1.Pos, "Expected position of first child to be 0.");
            Assert.AreEqual(1, oper1.Length, "Expected length of first child to be 1.");

            Assert.AreEqual(ParseNodeType.Operator, op.NodeType, "Expected second child NodeType to be Operator.");
            Assert.AreEqual(0, op.Children?.Count ?? 0, "Expected second child to have no children.");
            Assert.AreEqual(1, op.Pos, "Expected position of second child to be 1.");
            Assert.AreEqual(1, op.Length, "Expected length of second child to be 1.");

            Assert.AreEqual(ParseNodeType.LiteralInteger, oper2.NodeType,
                "Expected third child NodeType to be LiteralInteger.");
            Assert.AreEqual(0, oper2.Children?.Count ?? 0, "Expected third child to have no children.");
            Assert.AreEqual(2, oper2.Pos, "Expected position of third child to be 2.");
            Assert.AreEqual(1, oper2.Length, "Expected length of third child to be 1.");
        }

        [Test]
        public void NegOperator()
        {
            // Arrange
            var expression = "{return -x}";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            // Act
            var (exp, parseNode, _) = funcscript.core.FuncScriptParser.Parse(context);


            Assert.IsNotNull(parseNode, "ParseNode should not be null.");
            Assert.AreEqual(ParseNodeType.KeyValueCollection, parseNode.NodeType);
            Assert.AreEqual(1, parseNode.Children?.Count ?? 0);
            
            var ret = parseNode!.Children[0];
            Assert.AreEqual(ParseNodeType.ReturnExpression, ret.NodeType);
            Assert.AreEqual(2, ret.Children?.Count ?? 0);

            var retKey = ret.Children[0];
            Assert.AreEqual(ParseNodeType.KeyWord, retKey.NodeType);
            Assert.AreEqual(0, retKey.Children?.Count ?? 0);
            

            var retExp = ret.Children[1];
            Assert.AreEqual(ParseNodeType.PrefixOperatorExpression, retExp.NodeType);
            Assert.AreEqual(2, retExp.Children?.Count ?? 0);

            Assert.AreEqual(ParseNodeType.Operator, retExp.Children[0].NodeType);
            Assert.AreEqual(0, retExp.Children[0].Children?.Count ?? 0);

            Assert.AreEqual(ParseNodeType.Identifier, retExp.Children[1].NodeType);
            Assert.AreEqual(0, retExp.Children[1].Children?.Count ?? 0);

        }
    }
}

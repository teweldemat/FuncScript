using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test
{
    public class NotExpressionParseTest
    {
        [Test]
        public void ParseNotTrueCorrectly()
        {
            var expression = "!true";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            var (exp, parseNode, _) = FuncScriptParser.Parse(context);

            Assert.NotNull(parseNode);
            Assert.AreEqual(FuncScriptParser.ParseNodeType.PrefixOperatorExpression, parseNode.NodeType);

            Assert.AreEqual(0, parseNode.Pos);
            Assert.AreEqual(5, parseNode.Length);
            Assert.That(parseNode.Children.Count, Is.EqualTo(2));

            var operatorNode = parseNode.Children[0];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Operator, operatorNode.NodeType);
            Assert.AreEqual(0, operatorNode.Pos);
            Assert.AreEqual(1, operatorNode.Length);
            
            var operandNode = parseNode.Children[1];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyWord, operandNode.NodeType);
            Assert.AreEqual(1, operandNode.Pos);
            Assert.AreEqual(4, operandNode.Length);

        }
    }
}
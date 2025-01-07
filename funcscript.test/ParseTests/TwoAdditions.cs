using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test
{
    public class AdditionExpressionParse
    {
        [Test]
        public void AdditionExpressionParseTest()
        {
            var expression = "4+5+8";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            var (exp, parseNode, _) = FuncScriptParser.Parse(context);

            Assert.IsNotNull(parseNode);
            Assert.AreEqual(FuncScriptParser.ParseNodeType.InfixExpression, parseNode.NodeType);

            Assert.AreEqual(5, parseNode.Children.Count);

            var firstNum = parseNode.Children[0];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, firstNum.NodeType);
            Assert.AreEqual(0, firstNum.Pos);
            Assert.AreEqual(1, firstNum.Length);

            var firstOp = parseNode.Children[1];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Operator, firstOp.NodeType);
            Assert.AreEqual(1, firstOp.Pos);
            Assert.AreEqual(1, firstOp.Length);

            var secondNum = parseNode.Children[2];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, secondNum.NodeType);
            Assert.AreEqual(2, secondNum.Pos);
            Assert.AreEqual(1, secondNum.Length);

            var secondOp = parseNode.Children[3];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Operator, secondOp.NodeType);
            Assert.AreEqual(3, secondOp.Pos);
            Assert.AreEqual(1, secondOp.Length);

            var thirdNum = parseNode.Children[4];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, thirdNum.NodeType);
            Assert.AreEqual(4, thirdNum.Pos);
            Assert.AreEqual(1, thirdNum.Length);
        }
    }
}

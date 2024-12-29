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
        static ParseNode Flatten(ParseNode node)
        {
            if(node.Children.Count==1)
            {
                return node.Children[0];
            }
            
            if(node.Children.Count>1)
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
            var (exp,node,_)= funcscript.core.FuncScriptParser.Parse(context);
            Assert.IsNotNull(exp);
            Assert.IsNotNull(node);
            node = Flatten(node);
            Assert.AreEqual(Tuple.Create(ParseNodeType.KeyValueCollection,0,expText.Length), Tuple.Create(node.NodeType,node.Pos,node.Length));

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
    Assert.AreEqual(ParseNodeType.InfixExpression, parseNode.NodeType, "Expected NodeType to be InfixExpression.");
    Assert.AreEqual(3, parseNode.Children?.Count ?? 0, "Expected 3 children in the parse node.");
    Assert.AreEqual(0, parseNode.Pos, "Expected position to be 0.");
    Assert.AreEqual(3, parseNode.Length, "Expected length to be 3.");

    var oper1 = parseNode.Children[0];
    var op = parseNode.Children[1];
    var oper2 = parseNode.Children[2];

    Assert.AreEqual(ParseNodeType.LiteralInteger, oper1.NodeType, "Expected first child NodeType to be LiteralInteger.");
    Assert.AreEqual(0, oper1.Children?.Count ?? 0, "Expected first child to have no children.");
    Assert.AreEqual(0, oper1.Pos, "Expected position of first child to be 0.");
    Assert.AreEqual(1, oper1.Length, "Expected length of first child to be 1.");

    Assert.AreEqual(ParseNodeType.Operator, op.NodeType, "Expected second child NodeType to be Operator.");
    Assert.AreEqual(0, op.Children?.Count ?? 0, "Expected second child to have no children.");
    Assert.AreEqual(1, op.Pos, "Expected position of second child to be 1.");
    Assert.AreEqual(1, op.Length, "Expected length of second child to be 1.");

    Assert.AreEqual(ParseNodeType.LiteralInteger, oper2.NodeType, "Expected third child NodeType to be LiteralInteger.");
    Assert.AreEqual(0, oper2.Children?.Count ?? 0, "Expected third child to have no children.");
    Assert.AreEqual(2, oper2.Pos, "Expected position of third child to be 2.");
    Assert.AreEqual(1, oper2.Length, "Expected length of third child to be 1.");
}
    }
}

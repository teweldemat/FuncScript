using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test
{
    public class SelectorExpressionParseTest
    {
        [Test]
        public void SelectorExpressionParsedCorrectly()
        {
            var expression = "{a:5,b:3}{a}";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            var (exp, parseNode, _) = FuncScriptParser.Parse(context);

            Assert.IsNotNull(parseNode);
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Selection, parseNode.NodeType);

            Assert.AreEqual(2, parseNode.Children.Count);

            var firstKvc = parseNode.Children[0];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, firstKvc.NodeType);
            Assert.AreEqual(2, firstKvc.Children.Count);

            // KeyValuePair #1
            var kvp1 = firstKvc.Children[0];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValuePair, kvp1.NodeType);

            // KeyValuePair #2
            var kvp2 = firstKvc.Children[1];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValuePair, kvp2.NodeType);

            var secondKvc = parseNode.Children[1];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, secondKvc.NodeType);
            Assert.AreEqual(1, secondKvc.Children.Count);

            // Identifier
            var identifierNode = secondKvc.Children[0];
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, identifierNode.NodeType);
        }
    }
}

using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test.ParseTests;

public class ReturnExpressionParseTest
{
    [Test]
    public void ReturnExpressionParsedCorrectly()
    {
        var expression = "{return 2}";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, parseNode.NodeType);

        Assert.AreEqual(1, parseNode.Children.Count);

        var returnNode = parseNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.ReturnExpression, returnNode.NodeType);
        Assert.AreEqual(1, returnNode.Pos);    
        Assert.AreEqual(8, returnNode.Length); 

        Assert.That(returnNode.Children.Count, Is.EqualTo(2));

        var keyWordNode = returnNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyWord, keyWordNode.NodeType);
        Assert.AreEqual(1, keyWordNode.Pos);   
        Assert.AreEqual(6, keyWordNode.Length); 

        
        var literalNode = returnNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, literalNode.NodeType);
        Assert.AreEqual(8, literalNode.Pos);   
        Assert.AreEqual(1, literalNode.Length); 
    }
}

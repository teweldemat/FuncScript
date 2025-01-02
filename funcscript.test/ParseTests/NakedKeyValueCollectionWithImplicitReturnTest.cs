using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test;

public class NakedKeyValueCollectionWithImplicitReturnTest
{
    [Test]
    public void NakedKeyValueCollectionWithImplicitReturParsedCorrectly()
    {
        var expression = "x:12;x*2;";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, parseNode.NodeType);
        Assert.That(parseNode.Children.Count, Is.EqualTo(2));

        var keyValuePairNode = parseNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValuePair, keyValuePairNode.NodeType);

        var infixExpressionNode = parseNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.InfixExpression, infixExpressionNode.NodeType);
    }
}
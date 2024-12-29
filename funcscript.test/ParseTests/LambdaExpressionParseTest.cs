using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test;

public class LambdaExpressionParseTest
{
    [Test]
public void MapLambdaExpressionParsedCorrectly()
{
    var expression = "x map (a)=>b";
    var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
    var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

    var (exp, parseNode, _) = FuncScriptParser.Parse(context);

    Assert.IsNotNull(parseNode);
    Assert.AreEqual(FuncScriptParser.ParseNodeType.GeneralInfixExpression, parseNode.NodeType);

    Assert.AreEqual(3, parseNode.Children.Count);

    var xNode = parseNode.Children[0];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, xNode.NodeType);
    Assert.AreEqual(0, xNode.Pos);    // 'x' starts at index 0
    Assert.AreEqual(1, xNode.Length); // 'x' is 1 character long

    var mapNode = parseNode.Children[1];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, mapNode.NodeType);
    Assert.AreEqual(2, mapNode.Pos);  // 'map' starts at index 2
    Assert.AreEqual(3, mapNode.Length);

    var lambdaNode = parseNode.Children[2];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.LambdaExpression, lambdaNode.NodeType);
    Assert.AreEqual(6, lambdaNode.Pos);    // "(a)=>b" starts at index 6
    Assert.AreEqual(6, lambdaNode.Length); // 6 characters: ( a ) = > b

    Assert.That(lambdaNode.Children.Count, Is.EqualTo(2));

    var paramListNode = lambdaNode.Children[0];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.IdentifierList, paramListNode.NodeType);
    Assert.AreEqual(6, paramListNode.Pos);    // "(a)" starts at index 6
    Assert.AreEqual(3, paramListNode.Length); // (a) is 3 characters

    Assert.That(paramListNode.Children.Count, Is.EqualTo(1));
    var aNode = paramListNode.Children[0];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, aNode.NodeType);
    Assert.AreEqual(7, aNode.Pos);    // 'a' is at index 7
    Assert.AreEqual(1, aNode.Length); // 'a' is 1 character

    var bodyNode = lambdaNode.Children[1];
    Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, bodyNode.NodeType);
    Assert.AreEqual(11, bodyNode.Pos);    // 'b' is at index 11
    Assert.AreEqual(1, bodyNode.Length);  // 'b' is 1 character
}
}
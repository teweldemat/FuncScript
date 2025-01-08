using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test.ParseTests;

public class MemberAccessParseTest
{
    [Test]
    public void MemberAccessParsedCorrectly()
    {
        var expression = "5.a";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.MemberAccess, parseNode.NodeType);

        Assert.AreEqual(2, parseNode.Children.Count);

        var integerNode = parseNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, integerNode.NodeType);
        Assert.AreEqual(0, integerNode.Pos);    // '5' starts at index 0
        Assert.AreEqual(1, integerNode.Length);  // '5' is 1 character long

        var identifierNode = parseNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, identifierNode.NodeType);
        Assert.AreEqual(2, identifierNode.Pos);    // 'a' starts at index 2
        Assert.AreEqual(1, identifierNode.Length);  // 'a' is 1 character long
    }
}
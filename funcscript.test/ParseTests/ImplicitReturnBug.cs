using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test.ParseTests;

public class ImplicitReturnBug
{
    [Test]
    public void XColon12SemiXParsedCorrectly()
    {
        var expression = "x:12; x";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, parseNode.NodeType);
        Assert.AreEqual(2, parseNode.Children.Count);

        var kvpNode = parseNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValuePair, kvpNode.NodeType);

        var idNode = parseNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, idNode.NodeType);
    }
}

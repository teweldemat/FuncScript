using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test;

public class ParseMemberAccess
{
    [Test]
    public void MemberAccessParsedCorrectly()
    {
        var expression = "x.y";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode, "ParseNode should not be null.");
        Assert.AreEqual(FuncScriptParser.ParseNodeType.MemberAccess, parseNode.NodeType, "Expected NodeType to be MemberAccess.");

        Assert.AreEqual(2, parseNode.Children.Count, "Expected 2 children for MemberAccess.");
        var leftChild = parseNode.Children[0];
        var rightChild = parseNode.Children[1];
        Assert.NotNull(leftChild);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, leftChild.NodeType, "Expected left child NodeType to be Identifier.");
        Assert.AreEqual(0, leftChild.Pos, "Expected left child position to be 0.");
        Assert.AreEqual(1, leftChild.Length, "Expected left child length to be 1.");

        Assert.NotNull(rightChild);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, rightChild.NodeType, "Expected right child NodeType to be Identifier.");
        Assert.AreEqual(2, rightChild.Pos, "Expected right child position to be 2.");
        Assert.AreEqual(1, rightChild.Length, "Expected right child length to be 1.");
    }
}

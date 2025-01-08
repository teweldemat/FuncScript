using System.Collections.Generic;
using FuncScript.Core;
using FuncScript.Test.ParseTests;
using NUnit.Framework;

namespace FuncScript.Test.Bugs;

public class MapFilterBug
{
    //[1,2,3] map ((x)=>5) filter (y)=>y<3
    [Test]
    public void TestMapFilterBug()
    {
        const string expStr ="[1,2,3] map ((x)=>5) filter (y)=>y<3";
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expStr, errors);
        var (exp, node, _) = FuncScriptParser.Parse(context);
        Assert.NotNull(node);
        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.InfixExpression));
        Assert.That(node.Pos, Is.EqualTo(0));
        Assert.That(node.Length, Is.EqualTo(expStr.Length));
        Assert.That(node.Children.Count, Is.EqualTo(5));
        
        Assert.That(node.Children[0].NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.List));
        Assert.That(node.Children[1].NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.Identifier));
        Assert.That(node.Children[2].NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.ExpressionInBrace));
        Assert.That(node.Children[3].NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.Identifier));
        var lambda = node.Children[4];
        Assert.That(lambda.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.LambdaExpression));
        
        Assert.That(lambda.Children[1].Children[2].NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.LiteralInteger));
        
        ParseTreeTests.AssertNoOverlappingSpans(node);
        exp.SetReferenceProvider(context.ReferenceProvider);
        var res = exp.Evaluate();
        Assert.That(FuncScript.FormatToJson(res).Replace(" ", ""), Is.EqualTo("[]"));
    }
}
